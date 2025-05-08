using System.Collections.Generic;
using System.Xml;

namespace CoreGame.Custom
{
    public static partial class NodeConfigTypeRegistry
    {
        private static bool _CndBhvNodeCfg = Register(typeof(CndBhvNodeCfg), NodeCategory.Mixture);
    }

    //静态配置
    public class CndBhvNodeCfg : ICustomNodeXmlCfg
    {
        public ICustomNodeCfg mConditionCfg;   //触发条件配置
        public ICustomNodeCfg mBehaviorCfg;    //行为配置

        public System.Type NodeType()
        {
            return typeof(CndBhvNode);
        }

        public bool ParseFromXml(XmlNode xmlNode)
        {
            mConditionCfg = CustomLogicConfigMng.CreateNodeCfg(xmlNode.SelectSingleNode("Condition"));
            mBehaviorCfg = CustomLogicConfigMng.CreateNodeCfg(xmlNode.SelectSingleNode("Bhv"));

            var categoryCnd = NodeConfigTypeRegistry.GetNodeCfgCategory(mConditionCfg.GetType());
            var categoryBhv = NodeConfigTypeRegistry.GetNodeCfgCategory(mBehaviorCfg.GetType());
            CLHelper.Assert(categoryCnd == NodeCategory.Cnd);
            CLHelper.Assert(categoryBhv == NodeCategory.Bhv);
            return true;
        }
    }

    //////////////////////////////////////////////////////////////////////////
    //  结构容器节点：条件 + 行为
    //////////////////////////////////////////////////////////////////////////
    public class CndBhvNode : CustomNode, INeedUpdate, INeedStopCheck
    {
        public ICondition mCondition = null;  //激活条件
        public IBehavior mBehavior = null;    //附带行为
        public CustomNode mBhvNode = null;
        public CustomNode mCndNode = null;
        public bool m_isConditionReached = false;

        //////////////////////////////////////////////////////////////////////////
        // CustomNode
        public override void InitializeNode(ICustomNodeCfg cfg, CustomNodeContext context)
        {
            base.InitializeNode(cfg, context);
            CndBhvNodeCfg theCfg = cfg as CndBhvNodeCfg;

            mCndNode = CustomLogicFactory.CreateCustomNode(theCfg.mConditionCfg, context);
            mBhvNode = CustomLogicFactory.CreateCustomNode(theCfg.mBehaviorCfg, context);
            CLHelper.Assert(mCndNode != null);
            CLHelper.Assert(mBhvNode != null);
            mBhvNode.Deactivate();  //行为一开始处于非激活状态

            mCondition = mCndNode as ICondition;
            mBehavior = mBhvNode as IBehavior;
            CLHelper.Assert(mCondition != null);
            CLHelper.Assert(mBehavior != null);
        }

        public override void Destroy()
        {
            CustomLogicFactory.ObjectPool().Destroy(mCondition as ICanRecycle);
            CustomLogicFactory.ObjectPool().Destroy(mBehavior as ICanRecycle);
            mCondition = null;
            mBehavior = null;
            mCndNode = null;
            mBhvNode = null;
            m_isConditionReached = false;
        }

        public override void CollectInterfaceInChildren<T>(ref List<T> interfaceList)
        {
            TraverseCollectInterface<T>(ref interfaceList, mCondition);
            TraverseCollectInterface<T>(ref interfaceList, mBehavior);
        }

        public override void Activate()
        {
            base.Activate();

            if (mCndNode != null)
                mCndNode.Activate();
            if (mCondition != null && mCondition.IsConditionReached())
            {
                if (mBhvNode != null)
                    mBhvNode.Activate();
            }
        }

        public override void Deactivate()
        {
            base.Deactivate();
            if (mCndNode != null)
                mCndNode.Deactivate();
            if (mBhvNode != null)
                mBhvNode.Deactivate();
        }

        //////////////////////////////////////////////////////////////////////////
        //INeedStopCheck
        public bool CanStop()
        {
            // 1. 条件检查是否能被停止
            INeedStopCheck cndNF = mCondition as INeedStopCheck;
            if (cndNF != null && !cndNF.CanStop())
            {
                return false;
            }

            // 2. 条件达成后，行为是否能被停止
            if (m_isConditionReached)
            {
                INeedStopCheck bhvNSC = mBehavior as INeedStopCheck;
                if (bhvNSC != null && !bhvNSC.CanStop())
                {
                    return false;
                }
            }
            return true;
        }

        //////////////////////////////////////////////////////////////////////////
        // INeedUpdate
        public virtual bool Update(float dt)
        {
            if (dt == 0)
                return true;

            var updateCnd = mCondition as INeedUpdate;
            if (updateCnd != null)
            {
                updateCnd.Update(dt);
            }

            //条件达成时 BhvNode 才是激活状态
            if (m_isConditionReached ^ mCondition.IsConditionReached())
            {
                m_isConditionReached = mCondition.IsConditionReached();
                if (m_isConditionReached)
                {
                    mBhvNode.Activate();
                }
                else
                {
                    mBhvNode.Deactivate();
                }
            }

            //如果条件达成，则行为触发、开始Update
            if (m_isConditionReached)
            {
                mBehavior.Update(dt);
            }
            return true;
        }
    }
}