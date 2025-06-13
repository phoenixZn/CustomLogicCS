using System.Collections.Generic;
using System.Xml;

namespace CoreGame.Custom
{
    public class NodeCfgList : List<ICustomNodeCfg>
    {
        public bool ParseFromXml(XmlNode xmlNode, string xmlNodeName = "Node")
        {
            Clear();
            XmlNodeList subNodeList = xmlNode.SelectNodes(xmlNodeName);
            if (subNodeList == null)
                return false;
            foreach (XmlNode subNode in subNodeList)
            {
                ICustomNodeCfg nodeCfg = ICustomNodeXmlCfg.CreateNodeCfg(subNode);
                CLHelper.Assert(nodeCfg != null);
                this.Add(nodeCfg);
            }
            if (this.Count == 0)
            {
                LogWrapper.LogError("NodeCfgList.ParseFromXml() CfgList.Count == 0");
                CLHelper.AssertBreak();
                return false;
            }
            return true;
        }
    }

}