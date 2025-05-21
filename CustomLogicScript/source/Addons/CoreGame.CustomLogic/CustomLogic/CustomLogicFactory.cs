/********************************************************************
	created:	2015/10/29
	purpose:	*  CustomLogicFactory 自定义逻辑工厂
                *　负责根据使用方初始数据、配置信息，构造各个条件、行为，装配出一个CustomLogic。
                *　每个CustomLogic配置有一个ID做标识

	change list:
*********************************************************************/
using System.Collections.Generic;


namespace CoreGame.Custom
{
    public class CustomLogicFactory
    {
        static CustomLogicFactory sInstance = null;
        public static CustomLogicFactory Instance()
        {
            if (sInstance == null)
            {
                sInstance = new CustomLogicFactory();
            }
            return sInstance;
        }
        
        private CustomLogicFactory()
        {
        }
        
        protected Dictionary<string, LogicConfigContainer> mConfigContainerDic = new();

        public Dictionary<string, LogicConfigContainer> ConfigContainer { get { return mConfigContainerDic; } }

        public virtual void InitConfigMng(string xmlPath)
        {
            var testContainer = new LogicConfigContainer();
            testContainer.ReadXml(xmlPath);
            mConfigContainerDic.Add("LogicUnitTest", testContainer);
        }

        //////////////////////////////////////////////////////////////////////////
        //缓存
        protected static ObjectPool<ICanRecycle> mObjectPool = new ObjectPool<ICanRecycle>();

        public static ObjectPool<ICanRecycle> ObjectPool()
        {
            return mObjectPool;
        }

        public virtual void DoCache()
        {
        }

        //////////////////////////////////////////////////////////////////////////
        //主方法：创建并装配一个自定义逻辑
        public CustomLogic CreateCustomLogic(ICustomLogicGenInfo genInfo)
        {
            if (!mConfigContainerDic.TryGetValue(genInfo.ConfigContainerName, out var cfgContainer))
            {
                return null;
            }
            if (cfgContainer == null)
            {
                CLHelper.Assert(false, "CreateCustomLogic mConfigMng = null");
                return null;
            }
            CustomLogicCfg config = cfgContainer.GetCustomLogicCfg(genInfo.LogicConfigID);
            if (config == null)
            {
                CLHelper.Assert(false, "CreateCustomLogic Cant Find Config : id = " + genInfo.LogicConfigID);
                return null;
            }

            CustomLogic customLogic = CreateCustomLogic(genInfo, config, cfgContainer);
            return customLogic;
        }

        public CustomLogic CreateCustomLogic(ICustomLogicGenInfo genInfo, CustomLogicCfg config, ILogicConfigContainer cfgContainer = null)
        {
            if (config == null)
            {
                CLHelper.Assert(false, "CreateCustomLogic config == null");
                return null;
            }
            System.Type logicType = config.NodeType();
            CustomLogic customLogic = mObjectPool.Create<CustomLogic>(logicType);
            if (genInfo == null)
            {
                genInfo = new ICustomLogicGenInfo();
            }
            genInfo.LogicConfigID = config.ID;

            //区别于CreateCustomNode
            CustomNodeContext context = new CustomNodeContext();
            context.GenInfo = genInfo;
            context.Logic = customLogic;  //RootNode
            context.TempleteConfigContainer = cfgContainer;
            context.NodeFactory = this;
            context.Blackboard = genInfo.PreBlackboard ?? new KVBlackBoard();
            customLogic.InitializeNode(config, context);

            return customLogic;
        }

        public static void DestroyCustomLogic(CustomLogic cl)
        {
            if (cl != null)
            {
                mObjectPool.Destroy(cl);
            }
            cl = null;
        }

        public static CustomNode CreateCustomNode(ICustomNodeCfg cfg, CustomNodeContext context)
        {
            //初始化行为
            System.Type nodeType = cfg.NodeType();
            var theNode = ObjectPool().Create<CustomNode>(nodeType);
            theNode.InitializeNode(cfg, context);
            return theNode;
        }
    }
}