
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace CoreGame.Custom
{
    public static partial class NodeConfigTypeRegistry
    {
        static bool _LogicReferenceNodeCfg = Register(typeof(LogicReferenceCfg), NodeCategory.Mixture);
    }

    public class LogicReferenceCfg : ICustomNodeXmlCfg
    {
        public int LogicID;

        public virtual System.Type NodeType() { return typeof(LogicReference); }

        public bool ParseFromXml(XmlNode xmlNode)
        {
            string str = XmlHelper.GetAttribute(xmlNode, "LogicID");
            xmlNode.Assert(!string.IsNullOrEmpty(str));
            bool r = CLHelper.Assert(int.TryParse(str, out LogicID));
            return r;
        }
    }

    //////////////////////////////////////////////////////////////////////////
    // CustomLogic引用节点， 不同于模板，它是独立完整的子逻辑
    //////////////////////////////////////////////////////////////////////////
    public class LogicReference : CustomLogic
    {

        public override void InitializeNode(ICustomNodeCfg cfg, CustomNodeContext context)
        {
            LogicReference.SwitchReferenceCfg(this, ref cfg, ref context);
            base.InitializeNode(cfg, context);
        }

        public static void SwitchReferenceCfg(CustomLogic logic, ref ICustomNodeCfg cfg, ref CustomNodeContext context)
        {
            var theCfg = cfg as LogicReferenceCfg;
            CustomLogicCfg newCfg = context.TempleteConfigContainer.GetCustomLogicCfg(theCfg.LogicID);
            if (newCfg == null)
            {
                LogWrapper.LogError("LogicReferenceNode Cant Find Config : id = " + theCfg.LogicID);
            }

            CustomNodeContext newContext = new CustomNodeContext();
            newContext.GenInfo = context.GenInfo;
            newContext.Logic = logic;    //独立的黑板
            newContext.TempleteConfigContainer = context.TempleteConfigContainer;

            cfg = newCfg;
            context = newContext;
        }


    }
}