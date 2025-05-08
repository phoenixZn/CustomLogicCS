using System.Collections.Generic;
using System.Xml;

namespace CoreGame.Custom
{
    public static partial class NodeConfigTypeRegistry
    {
        static bool _LogicNotCndCfg = Register(typeof(LogicNotCndCfg), NodeCategory.Cnd);
    }

    public struct LogicNotCndCfg : ICustomNodeXmlCfg
    {
        public ICustomNodeCfg mCndCfg;
        public System.Type NodeType() { return typeof(LogicNotCnd); }

        public bool ParseFromXml(XmlNode cndNode)
        {
            XmlNode subNode = cndNode.SelectSingleNode("Condition");
            if (subNode == null)
            {
                LogWrapper.LogError("LogicNotCndCfg.ParseFromXml() SubCondition == 0");
                CLHelper.AssertBreak();
                return false;
            }
            mCndCfg = CustomLogicConfigMng.CreateNodeCfg(subNode);
            return true;
        }
    }

    //////////////////////////////////////////////////////////////////////////
    // 条件布尔逻辑 非（NOT）
    //////////////////////////////////////////////////////////////////////////
    public class LogicNotCnd : BaseCnd
    {
        private ICondition mCnd;

        //////////////////////////////////////////////////////////////////////////
        // ICustomNode
        public override void InitializeNode(ICustomNodeCfg cfg, CustomNodeContext context)
        {
            base.InitializeNode(cfg, context);
            LogicNotCndCfg cndcfg = (LogicNotCndCfg)cfg;
            mCnd = CustomLogicFactory.CreateCustomNode(cndcfg.mCndCfg, context) as ICondition;
        }

        public override void Destroy()
        {
            base.Destroy();
            ICanRecycle icr = mCnd as ICanRecycle;
            CustomLogicFactory.ObjectPool().Destroy(icr);
            mCnd = null;
        }

        //////////////////////////////////////////////////////////////////////////
        // ICondition
        public override bool Update(float dt)
        {
            var updateCnd = mCnd as INeedUpdate;
            if (updateCnd != null)
            {
                updateCnd.Update(dt);
            }
            return true;
        }

        public override bool IsConditionReached()
        {
            return !mCnd.IsConditionReached();
        }

        //////////////////////////////////////////////////////////////////////////
        // ICanReset
        public override void Reset()
        {
            ICanReset icr = mCnd as ICanReset;
            if (icr != null)
            {
                icr.Reset();
            }
        }

        //////////////////////////////////////////////////////////////////////////
        //ICustomNode
        public override void CollectInterfaceInChildren<T>(ref List<T> interfaceList)
        {
            base.CollectInterfaceInChildren<T>(ref interfaceList);
            if (mCnd == null)
                return;
            CustomNode.TraverseCollectInterface(ref interfaceList, mCnd);
        }

    }
}