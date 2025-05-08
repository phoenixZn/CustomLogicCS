using System.Xml;
using System;


namespace CoreGame.Custom
{
    public static partial class NodeConfigTypeRegistry
    {
        static bool _ExpressionCndCfg = Register(typeof(ExpressionCndCfg), NodeCategory.Cnd);
    }

    public class ExpressionCndCfg : ICustomNodeXmlCfg
    {
        public DSL.Expression Code;

        public System.Type NodeType()
        {
            return typeof(ExpressionCnd);
        }

        public bool ParseFromXml(XmlNode xmlNode)
        {
            try
            {
                string str = XmlHelper.GetAttribute(xmlNode, "Code");
                CLHelper.Assert(!string.IsNullOrEmpty(str));
                Code = new DSL.Expression();
                Code.Compile(str);
            }
            catch (Exception e)
            {
                LogWrapper.LogError(e);
            }
            return true;
        }
    }

    public class ExpressionCnd : CustomNode, ICondition
    {
        private ExpressionCndCfg m_cfg;

        //////////////////////////////////////////////////////////////////////////
        // ICustomNode
        public override void InitializeNode(ICustomNodeCfg cfg, CustomNodeContext context)
        {
            base.InitializeNode(cfg, context);
            m_cfg = cfg as ExpressionCndCfg;
        }

        public override void Destroy()
        {
            m_cfg = null;
            base.Destroy();
        }

        //////////////////////////////////////////////////////////////////////////
        // ICondition
        public bool IsConditionReached()
        {
            FixPoint v = m_cfg.Code.Evaluate(m_varLibRef);
            return (v != FixPoint.Zero);
        }

        ////////////////////////////////////////////////////////////////////////////
        //// ICanReset
        //public override void Reset()
        //{
        //    base.Reset();
        //}
    }
}

