using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace CoreGame.Custom
{
    public static partial class NodeConfigTypeRegistry
    {
        private static bool _LoopConditionBranchBhvCfg = Register(typeof(LoopConditionBranchBhvCfg), NodeCategory.Mixture);
    }

    //静态配置
    public class LoopConditionBranchBhvCfg : ICustomNodeXmlCfg
    {
        public ICustomNodeCfg mConditionCfg;   //条件配置
        public ICustomNodeCfg mBehaviorCfg;    //行为配置

        public System.Type NodeType()
        {
            return typeof(LoopConditionBranchBhv);
        }

        public bool ParseFromXml(XmlNode xmlNode)
        {
            mConditionCfg = ICustomNodeXmlCfg.CreateNodeCfg(xmlNode.SelectSingleNode("Condition"));
            mBehaviorCfg = ICustomNodeXmlCfg.CreateNodeCfg(xmlNode.SelectSingleNode("Bhv"));
            var categoryCnd = NodeConfigTypeRegistry.GetNodeCfgCategory(mConditionCfg.GetType());
            var categoryBhv = NodeConfigTypeRegistry.GetNodeCfgCategory(mBehaviorCfg.GetType());
            xmlNode.Assert(categoryCnd == NodeCategory.Cnd);
            xmlNode.Assert(categoryBhv == NodeCategory.Bhv);
            return true;
        }
    }

    //////////////////////////////////////////////////////////////////////////
    //  结构容器节点：不停Reset重新执行的 条件+行为 节点
    //////////////////////////////////////////////////////////////////////////
    public class LoopConditionBranchBhv : CustomNode, INeedUpdate
    {
        public ConditionNodeBase mCondition = null;      //激活条件
        public FiniteTimeBhv mBehavior = null;    //附带行为

        //////////////////////////////////////////////////////////////////////////
        // CustomNode
        public override void InitializeNode(ICustomNodeCfg cfg, CustomNodeContext context)
        {
            base.InitializeNode(cfg, context);
            ConditionBranchBhvCfg theCfg = cfg as ConditionBranchBhvCfg;
            mCondition = mContext.NodeFactory.CreateCustomNode(theCfg.CndCfg, context) as ConditionNodeBase;
            mBehavior = mContext.NodeFactory.CreateCustomNode(theCfg.TrueBhvCfg, context) as FiniteTimeBhv;
            CLHelper.Assert(mCondition != null);
            CLHelper.Assert(mBehavior != null);
        }

        public override void CollectInterfaceInChildren<T>(ref List<T> interfaceList)
        {
            TraverseCollectInterface<T>(ref interfaceList, mCondition);
            TraverseCollectInterface<T>(ref interfaceList, mBehavior);
        }

        public override void Destroy()
        {
            mContext.NodeFactory.DestroyCustomNode(mCondition as CustomNode);
            mContext.NodeFactory.DestroyCustomNode(mBehavior as CustomNode);
            mCondition = null;
            mBehavior = null;
        }


        //////////////////////////////////////////////////////////////////////////
        // INeedUpdate
        public virtual float Update(float dt)
        {
            if (dt == 0)
            {
                return dt;
            }

            if (mCondition is INeedUpdate updateCnd)
            {
                updateCnd.Update(dt);
            }
            //如果条件达成，则行为触发、开始Update
            if (mCondition.IsConditionReached())
            {
                mBehavior.Update(dt);
                if (mBehavior.IsDurationEnd())
                {
                    mCondition.Reset();
                    mBehavior.Reset();
                }
            }
            return dt;
        }
    }
}