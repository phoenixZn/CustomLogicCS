using System.Collections.Generic;
using System.Xml;

namespace CoreGame.Custom
{
    public interface ILogicConfigContainer
    {
        CustomLogicCfg GetCustomLogicCfg(int id);
    }


    //////////////////////////////////////////////////////////////////////////
    //自定义逻辑配置管理器
    //////////////////////////////////////////////////////////////////////////
    public class LogicConfigContainer : ILogicConfigContainer
    {
        //<自定义逻辑ID, 配置信息>
        private Dictionary<int, CustomLogicCfg> mCustomCfgMap = new();
        
        //////////////////////////////////////////////////////////////////////////
        //ILogicConfigContainer
        public CustomLogicCfg GetCustomLogicCfg(int id)
        {
            if (mCustomCfgMap.ContainsKey(id))
            {
                return mCustomCfgMap[id];
            }
            return null;
        }
        
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
                CustomLogicCfg cfg = ICustomNodeXmlCfg.CreateNodeCfg(cfgNode) as CustomLogicCfg;
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

    }
}