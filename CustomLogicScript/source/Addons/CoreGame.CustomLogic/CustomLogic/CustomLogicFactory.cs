/********************************************************************
	created:	2015/10/29
	purpose:	*  CustomLogicFactory 自定义逻辑工厂
                *　负责根据使用方初始数据、配置信息，构造各个条件、行为，装配出一个CustomLogic。
                *　每个CustomLogic配置有一个ID做标识

	change list:
*********************************************************************/

namespace CoreGame.Custom
{
    public class CustomLogicFactory
    {
        protected CustomLogicConfigMng mConfigMng = null;

        public CustomLogicConfigMng ConfigMng { get { return mConfigMng; } }

        public virtual void InitConfigMng(string xmlPath)
        {
            mConfigMng = new CustomLogicConfigMng();
            mConfigMng.ReadXml(xmlPath);
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
            if (mConfigMng == null)
            {
                CLHelper.Assert(false, "CreateCustomLogic mConfigMng = null");
                return null;
            }
            CustomLogicCfg config = mConfigMng.GetCustomLogicCfg(genInfo.ConfigID);
            if (config == null)
            {
                CLHelper.Assert(false, "CreateCustomLogic Cant Find Config : id = " + genInfo.ConfigID);
                return null;
            }

            System.Type logicType = config.NodeType();
            CustomLogic customLogic = mObjectPool.Create<CustomLogic>(logicType);

            //区别于CreateCustomNode
            CustomNodeContext context = new CustomNodeContext();
            context.GenInfo = genInfo;
            context.Logic = customLogic;  //RootNode
            context.ConfigMng = ConfigMng;

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