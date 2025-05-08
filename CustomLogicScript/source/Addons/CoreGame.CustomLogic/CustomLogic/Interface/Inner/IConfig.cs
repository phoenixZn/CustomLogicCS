using System.Collections;
using System.Collections.Generic;

namespace CoreGame.Custom
{

    //////////////////////////////////////////////////////////////////////////
    //静态配置接口
    //////////////////////////////////////////////////////////////////////////

    //可由Xml节点初始化自身数据
    public interface IParseFromXml
    {
        bool ParseFromXml(System.Xml.XmlNode node);
    }

    public interface IParseWithConfigMng
    {
        void SetConfigMng(CustomLogicConfigMng mng);
    }

    //含有子节点配置
    public interface IHasSubCfgList
    {
        List<ICustomNodeCfg> GetNodeCfgList();
    }
    public interface IHasSubNodeCfg
    {
        ICustomNodeCfg GetSubNodeCfg();
    }

    public interface IUseVariablesLib
    {
        void BindVariablesLib(VariablesLib varLib);
        void UnBindVariablesLib();
    }

}


