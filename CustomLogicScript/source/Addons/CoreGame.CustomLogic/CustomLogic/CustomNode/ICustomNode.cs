using System.Collections.Generic;
using System.Xml;

namespace CoreGame.Custom
{
    //////////////////////////////////////////////////////////////////////////
    //配置接口
    public interface ICustomNodeCfg
    {
        System.Type NodeType();     //节点静态配置对应的运行时节点类
    }


    public interface ICustomNodeXmlCfg : ICustomNodeCfg, IParseFromXml
    {
        public static ICustomNodeCfg CreateNodeCfg(XmlNode node)
        {
            XmlElement cusNode = node as XmlElement;
            if (cusNode == null)
            {
                CLHelper.Assert(false, "CustomLogicConfig PraseNodeCfg ParseError  cusNode == null");
                return null;
            }

            string nodeTypeStr = string.Format("{0}{1}", cusNode.GetAttribute("type"), "Cfg");
            ICustomNodeCfg nodeCfg = NodeConfigTypeRegistry.CreateCustomNodeCfg(nodeTypeStr);
            if (nodeCfg == null)
            {
                CLHelper.Assert(false, "NodeConfigTypeRegistry.CreateCustomNodeCfg == null  nodeTypeStr = " + nodeTypeStr);
                return null;
            }
            var xmlNodeCfg = nodeCfg as IParseFromXml;
            if (xmlNodeCfg != null)
            {
                if (!xmlNodeCfg.ParseFromXml(node))
                {
                    node.LogError(nodeTypeStr);
                }
            }

            CLHelper.Assert(nodeCfg != null);
            return nodeCfg;
        }
    }

    public interface INodeCfgList
    {
        List<ICustomNodeCfg> NodeCfgList { get; }
    }

    //自定义节点
    public interface ICustomNode : ICanRecycle, IInterfaceCollector
    {
        void InitializeNode(ICustomNodeCfg cfg, CustomNodeContext context);

        void Activate();

        void Deactivate();

        bool IsActive { get; }
    }
}
