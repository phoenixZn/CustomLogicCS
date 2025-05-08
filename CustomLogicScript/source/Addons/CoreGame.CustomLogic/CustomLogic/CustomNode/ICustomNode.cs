using System.Collections.Generic;

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
