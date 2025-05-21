/********************************************************************
	created:	2015/10/29
	purpose:	* CustomLogic中的节点概念，用于组织逻辑对象的结构关联

	change list:
*********************************************************************/

using System.Collections.Generic;

namespace CoreGame.Custom
{
    //////////////////////////////////////////////////////////////////////////
    // CustomNodeContext 
    // 逻辑内流通，接口性质的上下文结构，（不应被修改）
    // 如果项目确定因为特殊情况，需要扩展，只能添加一个 CustomNodeContext 的继承类
    //////////////////////////////////////////////////////////////////////////
    public class CustomNodeContext
    {
        public ICustomLogicGenInfo GenInfo = null;
        public CustomLogic Logic = null;
        public KVBlackBoard Blackboard = null;
        //模板配置库（静态配置get），支持通过ID复用一整个配置当模板
        public ILogicConfigContainer TempleteConfigContainer = null;    
        //逻辑节点工厂（运行时逻辑节点get）
        public CustomLogicFactory NodeFactory = null;   
    }

    //////////////////////////////////////////////////////////////////////////
    //  自定义节点:  条件节点、行为节点、结构容器节点 都继承自它
    //////////////////////////////////////////////////////////////////////////
    public class CustomNode : ICustomNode
    {
        private bool m_isActive = false;
        protected VariablesLib m_varLibRef;
        protected CustomNodeContext mContext;
        
        public VariablesLib VariablesLibRef => m_varLibRef;
        public KVBlackBoard BlackBoardRef => mContext.Blackboard;
        
        //////////////////////////////////////////////////////////////////////////
        // ICanRecycle
        public virtual void Destroy()
        {
            Deactivate();
            m_varLibRef = null;
        }

        //////////////////////////////////////////////////////////////////////////
        // ICustomNode
        public virtual void InitializeNode(ICustomNodeCfg cfg, CustomNodeContext context)
        {
            m_varLibRef = context.Logic.VarLib;
            Activate(); 
        }

        //Active概念比较纯粹， 只有IsActive的Node才能够响应外部的输入驱动、通知、查询
        public virtual void Activate()
        {
            m_isActive = true;
        }

        public virtual void Deactivate()
        {
            m_isActive = false;
        }

        public bool IsActive { get { return m_isActive; } }

        //////////////////////////////////////////////////////////////////////////
        //IInterfaceCollector
        public virtual void CollectInterface<T>(ref List<T> interfaceList) where T : class
        {
            T notify = this as T;
            if (notify != null)
            {
                interfaceList.Add(notify);
            }
        }

        public virtual void CollectInterfaceInChildren<T>(ref List<T> interfaceList) where T : class
        {
            //如果有子节点，重载实现这个方法
        }

        //////////////////////////////////////////////////////////////////////////
        //遍历收集所有interface
        protected static void TraverseCollectInterface<T>(ref List<T> interfaceList, object obj) where T : class
        {
            if (obj == null)
                return;
            ICustomNode node = obj as ICustomNode;
            if (node == null)
            {
                T notify = obj as T;
                if (notify != null)
                    interfaceList.Add(notify);
                return;
            }
            node.CollectInterface<T>(ref interfaceList);
            node.CollectInterfaceInChildren<T>(ref interfaceList);
        }
    }
}