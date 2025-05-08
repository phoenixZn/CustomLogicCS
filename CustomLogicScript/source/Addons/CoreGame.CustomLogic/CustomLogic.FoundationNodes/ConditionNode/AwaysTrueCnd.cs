using System.Xml;


namespace CoreGame.Custom
{
    public static partial class NodeConfigTypeRegistry
    {
        static bool _AwaysTrueCndCfg = Register(typeof(AwaysTrueCndCfg), NodeCategory.Cnd);
    }

    public class AwaysTrueCndCfg : ICustomNodeXmlCfg
    {
        public System.Type NodeType() { return typeof(AwaysTrueCnd); }

        public bool ParseFromXml(XmlNode cndNode)
        {
            return true;
        }
    }

    public class AwaysTrueCnd : CustomNode, ICondition
    {
        public bool IsConditionReached()
        {
            return true;
        }
    }
}

