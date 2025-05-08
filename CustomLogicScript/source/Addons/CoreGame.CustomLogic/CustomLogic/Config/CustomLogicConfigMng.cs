using System.Collections.Generic;
using System.Xml;

namespace CoreGame.Custom
{
    public interface ICustomLogicConfigMng
    {
        CustomLogicCfg GetCustomLogicCfg(int id);
    }


    //////////////////////////////////////////////////////////////////////////
    //自定义逻辑配置管理器
    //////////////////////////////////////////////////////////////////////////
    public class CustomLogicConfigMng : ICustomLogicConfigMng
    {
        //////////////////////////////////////////////////////////////////////////
        //ICustomLogicConfigMng
        public CustomLogicCfg GetCustomLogicCfg(int id)
        {
            if (mCustomCfgMap.ContainsKey(id))
            {
                return mCustomCfgMap[id];
            }
            return null;
        }

        #region 临时读配置

        //<自定义逻辑ID, 配置信息>
        private Dictionary<int, CustomLogicCfg> mCustomCfgMap = new Dictionary<int, CustomLogicCfg>();

        //读取Xml配置
        public bool ReadXml(string xmlPath)
        {
            XmlDocument xml = XmlHelper.LoadXmlDocFromResPath(xmlPath);
            if (xml == null)
            {
                CLHelper.Assert(false, "CustomLogicConfigMng  Cant Find Xml File: " + xmlPath);
                return false;
            }

            //逐个解析 各个配置
            XmlNode root = xml.SelectSingleNode("ConfigSystem");
            CLHelper.Assert(root != null);
            root = root.SelectSingleNode("CustomLogicConfig");
            CLHelper.Assert(root != null);
            XmlNodeList cfgNodeList = root.SelectNodes("ConfigItem");
            foreach (XmlElement cfgNode in cfgNodeList)
            {
                CustomLogicCfg cfg = CreateNodeCfg(cfgNode) as CustomLogicCfg;
                if (cfg == null)
                {
                    continue;
                }
                if (mCustomCfgMap.ContainsKey(cfg.ID))
                {
                    LogWrapper.LogError(xmlPath + "中出现重复的CustomLogicCfg ID :" + cfg.ID);
                    continue;
                }
                mCustomCfgMap[cfg.ID] = cfg;
            }
            return true;
        }

        #endregion 临时读配置

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
}