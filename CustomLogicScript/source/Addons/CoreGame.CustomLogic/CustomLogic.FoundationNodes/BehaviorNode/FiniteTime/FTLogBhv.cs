using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace CoreGame.Custom
{
    public static partial class NodeConfigTypeRegistry
    {
        static bool _FTLogBhvCfg = Register(typeof(FTLogBhvCfg), NodeCategory.Bhv);
    }
    public class FTLogBhvCfg : ICustomNodeXmlCfg
    {
        public string LogStr;

        public System.Type NodeType() { return typeof(FTLogBhv); }

        public FTLogBhvCfg(){}

        public FTLogBhvCfg(string str)
        {
            LogStr = str;
        }
        
        public bool ParseFromXml(XmlNode xmlNode)
        {
            LogStr = XmlHelper.GetAttribute(xmlNode, "LogStr");
            CLHelper.Assert(!string.IsNullOrEmpty(LogStr));
            return true;
        }
    }

    public class FTLogBhv : FiniteTimeBhv
    {
        private string m_LogStr;
        private int m_logicID;

        //////////////////////////////////////////////////////////////////////////
        // ICustomNode
        public override void InitializeNode(ICustomNodeCfg cfg, CustomNodeContext context)
        {
            base.InitializeNode(cfg, context);
            var theCfg = cfg as FTLogBhvCfg;
            CLHelper.Assert(theCfg != null);
            m_LogStr = theCfg.LogStr;
            m_logicID = context.GenInfo.LogicConfigID;
        }

        public override void Destroy()
        {
            m_logicID = 0;
            m_LogStr = null;
            base.Destroy();
        }

        protected override void OnBegin()
        {
            if (m_LogStr == null)
                return;
            LogWrapper.LogInfo(string.Format("Logic[{0}] : {1} ", m_logicID, m_LogStr));
        }
    }
}