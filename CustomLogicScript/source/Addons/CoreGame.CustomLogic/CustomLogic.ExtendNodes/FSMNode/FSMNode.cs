using System.Collections.Generic;
using System.Xml;

namespace CoreGame.Custom
{
    public static partial class NodeConfigTypeRegistry
    {
        private static bool _FSMNodeCfg = Register(typeof(FSMNodeCfg), NodeCategory.State);
    }

    //静态配置
    public class FSMNodeCfg : ICustomNodeXmlCfg
    {
        public List<ICustomNodeCfg> StateList;
        public int MaxTransitionInOneFrame = 1;  //同一帧内最多能发生多少次状态切换

        public virtual System.Type NodeType()
        {
            return typeof(FSMNode);
        }

        public virtual bool ParseFromXml(XmlNode xmlNode)
        {
            XmlNodeList subNodeList = xmlNode.SelectNodes("State");
            if (subNodeList == null)
                return false;

            StateList = new List<ICustomNodeCfg>();
            foreach (XmlNode subNode in subNodeList)
            {
                ICustomNodeCfg cfg = ICustomNodeXmlCfg.CreateNodeCfg(subNode);
                StateList.Add(cfg);
            }
            if (this.StateList.Count == 0)
            {
                LogWrapper.LogError("FSMNodeCfg.ParseFromXml() CndCfgList.Count == 0");
                CLHelper.AssertBreak();
                return false;
            }

            MaxTransitionInOneFrame = 1;
            string str = XmlHelper.GetAttribute(xmlNode, "MaxTransitionInOneFrame");
            if (!string.IsNullOrEmpty(str))
            {
                MaxTransitionInOneFrame = int.Parse(str);
                if (MaxTransitionInOneFrame <= 0)
                    MaxTransitionInOneFrame = 1;
            }

            return true;
        }
    }

    public class FSMNode : CustomNode, INeedUpdate, ICanReset, INeedStopCheck
    {
        FSMNodeCfg mCfg;
        protected List<StateNode> mStates = new List<StateNode>();
        private int mDefaultStateID = -1;
        private StateNode mCurrentState = null;

        //////////////////////////////////////////////////////////////////////////
        // CustomNode
        public override void InitializeNode(ICustomNodeCfg cfg, CustomNodeContext context)
        {
            base.InitializeNode(cfg, context);

            mCfg = cfg as FSMNodeCfg;

            mStates.Clear();
            for (int i = 0; i < mCfg.StateList.Count; ++i)
            {
                ICustomNodeCfg subcndCfg = mCfg.StateList[i];
                StateNode subcnd = CustomLogicFactory.CreateCustomNode(subcndCfg, context) as StateNode;
                subcnd.Deactivate();
                mStates.Add(subcnd);
            }

            mCurrentState = null;
            CLHelper.Assert(mStates.Count > 0);
            mDefaultStateID = mStates[0].StateID;
        }

        public override void CollectInterfaceInChildren<T>(ref List<T> interfaceList)
        {
            base.CollectInterfaceInChildren<T>(ref interfaceList);
            if (mStates == null)
                return;
            for (int i = 0; i < mStates.Count; ++i)
            {
                CustomNode.TraverseCollectInterface(ref interfaceList, mStates[i]);
            }
        }

        public override void Activate()
        {
            base.Activate();
            if (mCurrentState != null)
            {
                mCurrentState.Activate();
            }
        }

        public override void Deactivate()
        {
            base.Deactivate();
            if (mCurrentState != null && mCurrentState.IsActive)
            {
                mCurrentState.Deactivate();
            }
        }

        public override void Destroy()
        {
            base.Destroy();

            if (mCurrentState != null)
            {
                mCurrentState.Exit();
            }

            if (mStates != null)
            {
                for (int i = 0; i < mStates.Count; ++i)
                {
                    ICanRecycle icr = mStates[i] as ICanRecycle;
                    CustomLogicFactory.ObjectPool().Destroy(icr);
                }
                mStates.Clear();
            }

            mCurrentState = null;
            mDefaultStateID = -1;
            mCfg = null;
        }

        ////////////////////////////////////////////////////////////////////////
        //INeedStopCheck
        public virtual bool CanStop()
        {
            if (mCurrentState == null)
                return false;

            return mCurrentState.CanStop();
        }

        ////////////////////////////////////////////////////////////////////////
        //INeedUpdate
        public virtual bool Update(float dt)
        {
            if (mStates.Count == 0)
                return true;

            if (mCurrentState == null)
            {
                mCurrentState = FindState(mDefaultStateID);
                if (mCurrentState == null)
                    return true;
                mCurrentState.Enter();
            }

            //支持一帧内连续切换 MaxTransitionInOneFrame 个状态
            for (int i = 0; i < mCfg.MaxTransitionInOneFrame; i++)
            {
                mCurrentState.Update(dt);

                //检查状态转移
                int oldStateID = mCurrentState.StateID;
                var goalStateID = mCurrentState.CheckTransitions();

                if (goalStateID == oldStateID)
                {
                    break; //如果没有状态转移
                }
                var mGoalState = FindState(goalStateID);
                if (mGoalState != null)
                {
                    mCurrentState.Exit();
                    mCurrentState = mGoalState;
                    mCurrentState.Enter();
                }
            }

            return true;
        }

        //////////////////////////////////////////////////////////////////////////
        public StateNode CurrentState
        {
            get { return mCurrentState; }
        }

        public int CurrentStateType
        {
            get
            {
                if (mCurrentState == null)
                    return 0;
                return mCurrentState.StateID;
            }
        }

        //////////////////////////////////////////////////////////////////////////
        // Init
        public FSMNode AddState(StateNode state)
        {
            mStates.Add(state);
            return this;
        }

        private StateNode FindState(int stateID)
        {
            if (mStates.Count == 0)
                return null;

            for (int i = 0; i < mStates.Count; i++)
            {
                if (mStates[i].StateID == stateID)
                {
                    return mStates[i];
                }
            }
            return null;
        }

        public virtual void Reset()
        {
            if (mCurrentState != null)
                mCurrentState.Exit();

            mCurrentState = FindState(mDefaultStateID);

            if (mCurrentState != null)
                mCurrentState.Enter();
        }
    }
}