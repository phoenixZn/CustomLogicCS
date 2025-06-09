using System.Collections.Generic;
using System.Xml;

namespace CoreGame.Custom
{
    public static partial class NodeConfigTypeRegistry
    {
        private static bool _ConditionBranchBhvCfg = Register(typeof(ConditionBranchBhvCfg), NodeCategory.Mixture);
    }

    //静态配置
    public class ConditionBranchBhvCfg : ICustomNodeXmlCfg
    {
        public ICustomNodeCfg mConditionCfg { get; protected set; }   //判断条件配置
        public ICustomNodeCfg mTrueBhvCfg { get; protected set; }    //条件达成行为配置
        public ICustomNodeCfg mFalseBhvCfg { get; protected set; }   //条件不达成行为配置
        public bool CheckOnTick { get; protected set; } = false;

        public System.Type NodeType()
        {
            return typeof(ConditionBranchBhv);
        }

        public ConditionBranchBhvCfg(ICustomNodeCfg cndCfg, ICustomNodeCfg trueCfg = null, ICustomNodeCfg falseCfg = null)
        {
            mConditionCfg = cndCfg;
            mTrueBhvCfg = trueCfg;
            mFalseBhvCfg = falseCfg;
        }

        public bool ParseFromXml(XmlNode xmlNode)
        {
            mConditionCfg = ICustomNodeXmlCfg.CreateNodeCfg(xmlNode.SelectSingleNode("Condition"));
            mTrueBhvCfg = ICustomNodeXmlCfg.CreateNodeCfg(xmlNode.SelectSingleNode("TrueBhv"));
            mFalseBhvCfg = ICustomNodeXmlCfg.CreateNodeCfg(xmlNode.SelectSingleNode("FalseBhv"));

            if (CLHelper.Assert(mTrueBhvCfg != null || mFalseBhvCfg != null))
            {
                return false;
            }
            
            var categoryCnd = NodeConfigTypeRegistry.GetNodeCfgCategory(mConditionCfg.GetType());
            CLHelper.Assert(categoryCnd == NodeCategory.Cnd);
            
            if (mTrueBhvCfg != null)
            {
                var categoryBhv1 = NodeConfigTypeRegistry.GetNodeCfgCategory(mTrueBhvCfg.GetType());
                CLHelper.Assert(categoryBhv1 == NodeCategory.Bhv);
            }
            if (mFalseBhvCfg != null)
            {
                var categoryBhv2 = NodeConfigTypeRegistry.GetNodeCfgCategory(mFalseBhvCfg.GetType());
                CLHelper.Assert(categoryBhv2 == NodeCategory.Bhv);
            }
            
            CheckOnTick = XmlHelper.GetBool(xmlNode, "CheckOnTick");
            
            return true;
        }
    }

    //////////////////////////////////////////////////////////////////////////
    //  结构容器节点：条件 + 行为
    //////////////////////////////////////////////////////////////////////////
    public class ConditionBranchBhv : BehaviorNodeBase, INeedStopCheck
    {
        protected ConditionNodeBase mCondition = null;  //激活条件
        protected BehaviorNodeBase mTrueBhv = null;  //附带行为
        protected BehaviorNodeBase mFalseBhv = null;  //附带行为
        
        protected bool? mIsConditionReached = null;
        protected bool mCheckOnTick = false;

        //////////////////////////////////////////////////////////////////////////
        // CustomNode
        public override void InitializeNode(ICustomNodeCfg cfg, CustomNodeContext context)
        {
            base.InitializeNode(cfg, context);
            ConditionBranchBhvCfg theCfg = cfg as ConditionBranchBhvCfg;

            mCondition = mContext.NodeFactory.CreateCustomNode(theCfg.mConditionCfg, context) as ConditionNodeBase;
            
            //行为一开始处于非激活状态
            if (theCfg.mTrueBhvCfg != null)
            {
                mTrueBhv = mContext.NodeFactory.CreateCustomNode(theCfg.mTrueBhvCfg, context) as BehaviorNodeBase;
                mTrueBhv.Deactivate();  
            }
            if (theCfg.mFalseBhvCfg != null)
            {
                mFalseBhv = mContext.NodeFactory.CreateCustomNode(theCfg.mFalseBhvCfg, context) as BehaviorNodeBase;
                mFalseBhv.Deactivate();  
            }
            
            CLHelper.Assert(mCondition != null);
            CLHelper.Assert(mTrueBhv != null || mFalseBhv != null);

            mIsConditionReached = null;
            mCheckOnTick = theCfg.CheckOnTick;
        }

        public override void Destroy()
        {
            mContext.NodeFactory.DestroyCustomNode(mCondition);
            mCondition = null;
            
            mContext.NodeFactory.DestroyCustomNode(mTrueBhv);
            mTrueBhv = null;
            
            mContext.NodeFactory.DestroyCustomNode(mFalseBhv);
            mFalseBhv = null;
            
            mIsConditionReached = null;
            mCheckOnTick = false;
        }

        public override void CollectInterfaceInChildren<T>(ref List<T> interfaceList)
        {
            TraverseCollectInterface<T>(ref interfaceList, mCondition);
            if (mTrueBhv != null)
            {
                TraverseCollectInterface<T>(ref interfaceList, mTrueBhv);    
            }
            if (mFalseBhv != null)
            {
                TraverseCollectInterface<T>(ref interfaceList, mFalseBhv);    
            }
        }

        public override void Activate()
        {
            base.Activate();
            mCondition?.Activate();
        }

        public override void Deactivate()
        {
            base.Deactivate();
            mCondition?.Deactivate();
            mTrueBhv?.Deactivate();
            mFalseBhv?.Deactivate();
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
            var bhv = mFalseBhv;
            if (mIsConditionReached == true)
            {
                bhv = mTrueBhv;
            }
            if (bhv != null && bhv is INeedStopCheck bhvNSC)
            {
                return bhvNSC.CanStop();
            }
            return true;
        }

        //////////////////////////////////////////////////////////////////////////
        // BehaviorNodeBase
        protected override void OnBegin()
        {
            Inner_CheckConditionReached();
        }
        
        protected override float OnUpdate(float dt)
        {
            if (mCheckOnTick)
            {
                Inner_CheckConditionReached();
            }
            
            if (mIsConditionReached == true)
            {
                if (mTrueBhv != null)
                {
                    dt = mTrueBhv.Update(dt);    
                }
            }
            else
            {
                if (mTrueBhv != null)
                {
                    dt = mFalseBhv.Update(dt);    
                }
            }
            return dt;
        }
        
        //////////////////////////////////////////////////////////////////////////
        // this
        protected void Inner_CheckConditionReached()
        {
            var isReached = mCondition.IsConditionReached();
            bool hasChange = mIsConditionReached != isReached;
            mIsConditionReached = isReached;
            if (!hasChange)
            {
                return;
            }
            if (mIsConditionReached == true)
            {
                mTrueBhv?.Activate();
                mFalseBhv?.Deactivate();
            }
            else
            {
                mTrueBhv?.Deactivate();
                mFalseBhv?.Activate();
            }
        }
    }
}