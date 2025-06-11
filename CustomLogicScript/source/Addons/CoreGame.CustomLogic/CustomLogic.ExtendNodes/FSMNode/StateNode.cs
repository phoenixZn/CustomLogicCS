using System.Collections.Generic;
using System.Xml;

namespace CoreGame.Custom
{
    //静态配置
    public class StateNodeCfg : ICustomNodeXmlCfg
    {
        //命名的状态ID
        public string StateID { get; protected set; } 
        
        //自动进入的下一个状态（可以不配）
        public string NextStateID { get; protected set; } 
        
        //跳转逻辑（可以不配）
        public NodeCfgList Transitions { get; protected set; } = null;  
        
        public virtual System.Type NodeType() { return typeof(StateNode); }
        
        public virtual bool ParseFromXml(XmlNode xmlNode)
        {
            StateID = XmlHelper.GetAttribute(xmlNode, "StateID");
            if (!CLHelper.Assert(StateID != null, "StateNodeCfg StateID == null"))
            {
                return false;
            }
            NextStateID = XmlHelper.GetAttribute(xmlNode, "NextStateID");

            var xmlTransitions = xmlNode.SelectSingleNode("Transitions");
            if (xmlTransitions != null)
            {
                Transitions = new NodeCfgList();
                if (!Transitions.ParseFromXml(xmlTransitions))
                {
                    return false;
                }
            }
            return true;
        }
    }

    
    //////////////////////////////////////////////////////////////////////////
    // 运行时节点: 基类状态
    //////////////////////////////////////////////////////////////////////////
    public class StateNode : CustomNode, INeedUpdate
    {
        protected string mStateID;
        protected string mDefaultNextStateID;
        protected List<StateTransitionNode> mTransitions = new();
        //////////////////////////////////////////////////////////////////////////
        // CustomNode
        public override void InitializeNode(ICustomNodeCfg cfg, CustomNodeContext context)
        {
            base.InitializeNode(cfg, context);
            StateNodeCfg theCfg = cfg as StateNodeCfg;

            mStateID = theCfg.StateID;
            mDefaultNextStateID = theCfg.NextStateID;

            if (theCfg.Transitions != null)
            {
                for (int i = 0; i < theCfg.Transitions.Count; ++i)
                {
                    ICustomNodeCfg bhvCfg = theCfg.Transitions[i];
                    var transNode = mContext.NodeFactory.CreateCustomNode(bhvCfg, context) as StateTransitionNode;
                    if (!CLHelper.Assert(transNode != null))
                        continue;
                    mTransitions.Add(transNode);
                }
            }

            if (string.IsNullOrEmpty(mStateID))
            {
                this.LogError($"StateNode string.IsNullOrEmpty(mStateID)");
            }
        }

        public override void Destroy()
        {
            base.Destroy();
            mStateID = null;
            mDefaultNextStateID = null;
            for (int i = 0; i < mTransitions.Count; ++i)
            {
                mContext.NodeFactory.DestroyCustomNode(mTransitions[i]);
            }
            mTransitions.Clear();
        }

        //////////////////////////////////////////////////////////////////////////
        //INeedUpdate
        public virtual float Update(float dt)
        {
            foreach (var transNode in mTransitions)
            {
                transNode.Update(dt);
            }
            return dt;
        }

        //////////////////////////////////////////////////////////////////////////
        //StateBase
        public string StateID
        {
            get { return mStateID; }
        }

        public virtual void Enter()
        {
            this.Activate();
            foreach (var transNode in mTransitions)
            {
                transNode.Reset();
                transNode.Activate();
            }
        }

        public virtual void Exit()
        {
            this.Deactivate();
            foreach (var transNode in mTransitions)
            {
                transNode.Deactivate();
            }
        }

        public virtual string CheckTransitions()
        {
            foreach (var transNode in mTransitions)
            {
                var goal_state = transNode.CheckTransitions();
                if (goal_state != null)
                {
                    return goal_state;
                }
            }
            return mDefaultNextStateID;
        }
        
        //////////////////////////////////////////////////////////////////////////
        //ICustomNode
        public override void CollectInterfaceInChildren<T>(ref List<T> interfaceList)
        {
            base.CollectInterfaceInChildren<T>(ref interfaceList);
            
            if (mTransitions == null)
                return;
            for (int i = 0; i < mTransitions.Count; ++i)
            {
                TraverseCollectInterface(ref interfaceList, mTransitions[i]);
            }
        }
    }
}