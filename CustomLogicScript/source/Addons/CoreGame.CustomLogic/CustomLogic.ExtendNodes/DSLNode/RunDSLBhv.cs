using System;
using System.Xml;
using CoreGame.DSL;

namespace CoreGame.Custom
{
    public static partial class NodeConfigTypeRegistry
    {
        static bool _RunDSLBhvCfg = Register(typeof(RunDSLBhvCfg), NodeCategory.Bhv);
    }
    public class RunDSLBhvCfg : ICustomNodeXmlCfg
    {
        public DSLCode Code;

        public System.Type NodeType() { return typeof(RunDSLBhv); }

        public bool ParseFromXml(XmlNode bhvNode)
        {
            try
            {
                string str = bhvNode.InnerText;
                bhvNode.Assert(!string.IsNullOrEmpty(str), "Code IsNullOrEmpty");

                Code = new DSLCode();
                Code.Compile(str);
            }
            catch (Exception e)
            {
                LogWrapper.LogError(e);
            }
            return true;
        }

    }

    public class RunDSLBhv : FiniteTimeBhv
    {
        private RunDSLBhvCfg mCfg;

        //////////////////////////////////////////////////////////////////////////
        // ICustomNode
        public override void InitializeNode(ICustomNodeCfg cfg, CustomNodeContext context)
        {
            base.InitializeNode(cfg, context);
            mCfg = cfg as RunDSLBhvCfg;
        }

        public override void Destroy()
        {
            base.Destroy();
            mCfg = null;
        }

        protected override void OnBegin()
        {
            mCfg.Code.Execute(m_varLibRef);
        }
    }
}