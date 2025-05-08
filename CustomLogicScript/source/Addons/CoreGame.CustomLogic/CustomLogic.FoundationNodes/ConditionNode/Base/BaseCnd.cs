using System.Collections.Generic;
using System.Xml;

namespace CoreGame.Custom
{
    public abstract class BaseCnd : CustomNode, ICondition, ICanReset, INeedUpdate
    {
        //////////////////////////////////////////////////////////////////////////
        // ICondition
        public abstract bool IsConditionReached();
        public virtual bool Update(float dt) { return true; }
        public virtual void Reset() { }
    }


    //静态配置
    public abstract class CndListCfg : ICustomNodeXmlCfg
    {
        public List<ICustomNodeCfg> CndCfgList;

        public abstract System.Type NodeType();

        public bool ParseFromXml(XmlNode cndNode)
        {
            CndCfgList = new List<ICustomNodeCfg>();

            XmlNodeList subNodeList = cndNode.SelectNodes("Condition");
            if (subNodeList == null)
                return false;
            foreach (XmlNode subNode in subNodeList)
            {
                ICustomNodeCfg cndCfg = CustomLogicConfigMng.CreateNodeCfg(subNode);
                CndCfgList.Add(cndCfg);
            }
            if (this.CndCfgList.Count == 0)
            {
                LogWrapper.LogError("GroupCndCfg.ParseFromXml() CndCfgList.Count == 0");
                CLHelper.AssertBreak();
                return false;
            }
            return true;
        }
    }

}
