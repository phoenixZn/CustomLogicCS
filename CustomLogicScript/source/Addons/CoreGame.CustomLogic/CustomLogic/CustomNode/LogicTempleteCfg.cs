
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace CoreGame.Custom
{
    //////////////////////////////////////////////////////////////////////////
    // CustomLogic模板节点， 等价于在该LogicTempletNode处直接插入TempletLogic的全部节点
    //////////////////////////////////////////////////////////////////////////

    public static partial class NodeConfigTypeRegistry
    {
        static bool _LogicTempleteNodeCfg = Register(typeof(LogicTempleteCfg), NodeCategory.Mixture);
    }

    public class LogicTempleteCfg : ICustomNodeXmlCfg
    {
        public int LogicID;

        public System.Type NodeType() 
        {
            LogWrapper.LogError("ERROR : try to Initialize LogicTempleteNode!");
            return null; 
        }

        public bool ParseFromXml(XmlNode xmlNode)
        {
            string str = XmlHelper.GetAttribute(xmlNode, "LogicID");
            CLHelper.Assert(!string.IsNullOrEmpty(str));
            bool r = CLHelper.Assert(int.TryParse(str, out LogicID));
            return r;
        }

    }

}