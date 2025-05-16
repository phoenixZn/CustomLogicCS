using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace CoreGame.Custom
{
    public static partial class NodeConfigTypeRegistry
    {
        private static bool _LoopCndBhvNodeCfg = Register(typeof(LoopCndBhvNodeCfg), NodeCategory.Mixture);
    }

    //静态配置
    public class LoopCndBhvNodeCfg : ICustomNodeXmlCfg
    {
        public ICustomNodeCfg mConditionCfg;   //条件配置
        public ICustomNodeCfg mBehaviorCfg;    //行为配置

        public System.Type NodeType()
        {
            return typeof(LoopCndBhvNode);
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
    public class LoopCndBhvNode : CustomNode, INeedUpdate
    {
        public BaseCnd mCondition = null;      //激活条件
        public FiniteTimeBhv mBehavior = null;    //附带行为

        //////////////////////////////////////////////////////////////////////////
        // CustomNode
        public override void InitializeNode(ICustomNodeCfg cfg, CustomNodeContext context)
        {
            base.InitializeNode(cfg, context);
            CndBhvNodeCfg theCfg = cfg as CndBhvNodeCfg;
            mCondition = CustomLogicFactory.CreateCustomNode(theCfg.mConditionCfg, context) as BaseCnd;
            mBehavior = CustomLogicFactory.CreateCustomNode(theCfg.mBehaviorCfg, context) as FiniteTimeBhv;
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
            CustomLogicFactory.ObjectPool().Destroy(mCondition as ICanRecycle);
            CustomLogicFactory.ObjectPool().Destroy(mBehavior as ICanRecycle);
            mCondition = null;
            mBehavior = null;
        }


        //////////////////////////////////////////////////////////////////////////
        // INeedUpdate
        public virtual bool Update(float dt)
        {
            if (dt == 0)
                return true;
            mCondition.Update(dt);
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
            return true;
        }
    }
}