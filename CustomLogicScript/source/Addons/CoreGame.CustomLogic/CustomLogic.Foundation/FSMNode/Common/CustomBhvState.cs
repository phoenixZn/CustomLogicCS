using System.Collections.Generic;
using System.Xml;

namespace CoreGame.Custom
{
    public static partial class NodeConfigTypeRegistry
    {
        static bool CustomBhvStateCfg = Register(typeof(CustomBhvStateCfg), NodeCategory.State);
    }

    
    //静态配置
    public class CustomBhvStateCfg : StateNodeCfg
    {
        public ICustomNodeCfg Bhv { get; protected set; }    
        public ICustomNodeCfg ExitBhv { get; protected set; }  
        
        public override System.Type NodeType() { return typeof(CustomBhvState); }
        
        public override bool ParseFromXml(XmlNode xmlNode)
        {
            Bhv = ICustomNodeXmlCfg.CreateNodeCfg(xmlNode.SelectSingleNode("Bhv"));
            ExitBhv = ICustomNodeXmlCfg.CreateNodeCfg(xmlNode.SelectSingleNode("ExitBhv"));

            CLHelper.AssertNodeCfgCategory(Bhv, NodeCategory.Bhv, false);
            CLHelper.AssertNodeCfgCategory(ExitBhv, NodeCategory.Bhv, false);

            return base.ParseFromXml(xmlNode);
        }
    }

    
    //////////////////////////////////////////////////////////////////////////
    // 运行时节点: 基类状态
    //////////////////////////////////////////////////////////////////////////
    public class CustomBhvState : StateNode
    {
        protected BehaviorNodeBase mBhv = null;  
        protected BehaviorNodeBase mExitBhv = null;
        protected bool CS_IsStateBhvEnd = false;
        
        //////////////////////////////////////////////////////////////////////////
        // CustomNode
        public override void InitializeNode(ICustomNodeCfg cfg, CustomNodeContext context)
        {
            base.InitializeNode(cfg, context);
            var theCfg = cfg as CustomBhvStateCfg;

            if (theCfg.Bhv != null)
            {
                mBhv = mContext.NodeFactory.CreateCustomNode(theCfg.Bhv, context) as BehaviorNodeBase;
                mBhv.Deactivate();  
            }
            if (theCfg.ExitBhv != null)
            {
                mExitBhv = mContext.NodeFactory.CreateCustomNode(theCfg.ExitBhv, context) as BehaviorNodeBase;
                mExitBhv.Deactivate();  
            }
        }

        public override void Destroy()
        {
            base.Destroy();
            
            mContext.NodeFactory.DestroyCustomNode(mBhv);
            mBhv = null;
            
            mContext.NodeFactory.DestroyCustomNode(mExitBhv);
            mExitBhv = null;

            CS_IsStateBhvEnd = false;
        }

        public override void Activate()
        {
            base.Activate();
            mBhv?.Activate();
        }

        public override void Deactivate()
        {
            base.Deactivate();
            mBhv?.Deactivate();
        }
        
        public override void CollectInterfaceInChildren<T>(ref List<T> interfaceList)
        {
            base.CollectInterfaceInChildren(ref interfaceList);
            if (mBhv != null)
            {
                TraverseCollectInterface<T>(ref interfaceList, mBhv);    
            }
            // if (mExitBhv != null)    //结束行为，激活状态只在瞬间。无需处理外部输入
            // {
            //     TraverseCollectInterface<T>(ref interfaceList, mExitBhv);    
            // }
        }
 
        //////////////////////////////////////////////////////////////////////////
        //INeedUpdate
        public override float Update(float dt)
        {
            base.Update(dt);
            if (mBhv != null)
            {
                mBhv.Update(dt);
                if (!CS_IsStateBhvEnd && mBhv.IsNodeCanStop())
                {
                    CS_IsStateBhvEnd = true;
                    OnNodeLogicEnd();
                }
            }
            return dt;
        }

        //////////////////////////////////////////////////////////////////////////
        //as StateBase:
        public override void Enter()
        {
            base.Enter();

            if (mBhv != null)
            {
                mBhv.Reset();
                mBhv.Activate();
            }

            mExitBhv?.Reset();
            CS_IsStateBhvEnd = false;
        }

        public override void Exit()
        {
            if (mBhv != null)
            {
                mBhv.Deactivate();
            }
            
            if (mExitBhv != null)
            {
                //ExitNode中只能执行能够立即完成的节点，原则上不该有持续性节点
                mExitBhv.Activate();
                mExitBhv.Update(1000f);
                mExitBhv.Deactivate();
            }
            
            base.Exit();
        }

        public override string CheckTransitions()
        {
            if (!CS_IsStateBhvEnd)
            {
                return null;
            }
            var next_state = base.CheckTransitions();
            if (next_state != null)
            {
                return next_state;
            }
            return mDefaultNextStateID;
        }

        //////////////////////////////////////////////////////////////////////////
        /// this:
        protected virtual void OnNodeLogicEnd()
        {
        }
    }
}