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
        public VarEnv VarEnvImp = null;
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
        private bool mIsActive = false;
        protected CustomNodeContext mContext;
        
        //运行时变量环境（黑板）
        public VarEnv VarEnvRef => mContext.VarEnvImp;
        //运行时初始数据
        public ICustomLogicGenInfo GenInfo => mContext.GenInfo;

        //////////////////////////////////////////////////////////////////////////
        // ICanRecycle
        public virtual void Destroy()
        {
            Deactivate();
            mContext = null;
        }

        //////////////////////////////////////////////////////////////////////////
        // ICustomNode
        public virtual void InitializeNode(ICustomNodeCfg cfg, CustomNodeContext context)
        {
            mContext = context;
            Activate(); 
        }

        //Active概念比较纯粹， 只有IsActive的Node才能够响应外部的输入驱动、通知、查询
        public virtual void Activate()
        {
            mIsActive = true;
        }

        public virtual void Deactivate()
        {
            mIsActive = false;
        }

        public bool IsActive { get { return mIsActive; } }
        
        //Reset的设计语义是：节点内部状态恢复到InitializeNode之后的样子（主要用于可以重复多次执行的Node）
        public virtual void Reset()
        {
        }


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

        //////////////////////////////////////////////////////////////////////////
        /// This
        protected void SetVar<T>(string key, T value)
        {
            VarEnvRef.WriteVar(key, value);
        }

        protected T GetVar<T>(string key)
        {
            if (VarEnvRef.ReadVar<T>(key, out var value))
                return value;
            return default;
        }
    }
}