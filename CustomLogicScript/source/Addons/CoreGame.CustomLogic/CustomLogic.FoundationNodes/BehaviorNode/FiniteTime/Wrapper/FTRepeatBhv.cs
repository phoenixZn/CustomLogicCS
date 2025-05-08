
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace CoreGame.Custom
{
    public static partial class NodeConfigTypeRegistry
    {
        static bool FTRepeatBhv = Register(typeof(FTRepeatBhvCfg), NodeCategory.Bhv);
    }

    public class FTRepeatBhvCfg : ICustomNodeXmlCfg, IHasSubNodeCfg
    {
        public ICustomNodeCfg BhvCfg;
        public int Times;  //重复次数

        public System.Type NodeType() { return typeof(FTRepeatBhv); }

        public bool ParseFromXml(XmlNode xmlNode)
        {
            string str = XmlHelper.GetAttribute(xmlNode, "Times");
            CLHelper.Assert(!string.IsNullOrEmpty(str));
            Times = int.Parse(str);

            XmlNode subNode = xmlNode.SelectSingleNode("Node");
            CLHelper.Assert(subNode != null);
            BhvCfg = CustomLogicConfigMng.CreateNodeCfg(subNode);

            //节点类别限定
            var category = NodeConfigTypeRegistry.GetNodeCfgCategory(BhvCfg.GetType());
            CLHelper.Assert(category == NodeCategory.Bhv);
            return true;
        }

        //IHasSubBhvCfg
        public ICustomNodeCfg GetSubNodeCfg()
        {
            return BhvCfg;
        }
    }

    //////////////////////////////////////////////////////////////////////////
    //有限次数的循环包装行为，它是FiniteTimeBhv
    public class FTRepeatBhv : FiniteTimeBhv
    {
        private FTRepeatBhvCfg mCfg;
        private FiniteTimeBhv mBhv;
        private int mRemainTimes = 0;


        //////////////////////////////////////////////////////////////////////////
        // ICustomNode
        public override void InitializeNode(ICustomNodeCfg cfg, CustomNodeContext context)
        {
            base.InitializeNode(cfg, context);

            mCfg = cfg as FTRepeatBhvCfg;
            mBhv = CustomLogicFactory.CreateCustomNode(mCfg.BhvCfg, context) as FiniteTimeBhv;
            CLHelper.Assert(mBhv != null);

            Reset();
        }

        public override void Activate()
        {
            base.Activate();
            if (mBhv != null)
            {
                mBhv.Activate();
            }
        }

        public override void Deactivate()
        {
            base.Deactivate();
            if (mBhv != null)
            {
                mBhv.Deactivate();
            }
        }

        public override void Destroy()
        {
            base.Destroy();
            mRemainTimes = 0;
            CustomLogicFactory.ObjectPool().Destroy(mBhv);
            mBhv = null;
        }

        public override void Reset()
        {
            base.Reset();
            InitDuration(mBhv.GetDuration() * mCfg.Times);
            mRemainTimes = mCfg.Times;
            mBhv.Reset();
        }

        public override bool IsDurationEnd()
        {
            if (mBhv == null)
                return true;
            if (mRemainTimes <= 0)
            {
                return true;
            }
            return false;
        }

        //////////////////////////////////////////////////////////////////////////
        //FiniteTimeBhv
        public override bool Update(float dt)
        {
            base.Update(dt);

            if (mBhv == null)
                return true;

            if (mRemainTimes <= 0)
                return true;

            //尽量保证时间精确，过剩的时间片传入后续的更新
            float dt_overplus = dt;     
            for (int i = 0; dt_overplus > 0 && mRemainTimes > 0; ++i)
            {
                if (i > mCfg.Times) //设立极端中断条件，防止死循环
                    break;

                float curDur = mBhv.GetDuration();
                mBhv.Update(dt_overplus);

                dt_overplus = dt_overplus - curDur;
                if (mBhv.IsDurationEnd())
                {
                    --mRemainTimes;
                    //重新开始
                    mBhv.Reset();
                }
            }

            return true;
        }

        //////////////////////////////////////////////////////////////////////////
        //ICustomNode
        public override void CollectInterfaceInChildren<T>(ref List<T> interfaceList)
        {
            base.CollectInterfaceInChildren<T>(ref interfaceList);
            CustomNode.TraverseCollectInterface(ref interfaceList, mBhv);
        }
    }
}