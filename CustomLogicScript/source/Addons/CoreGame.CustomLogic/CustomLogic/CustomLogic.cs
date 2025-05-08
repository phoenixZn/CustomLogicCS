/********************************************************************
	created:	2015/10/29
	purpose:	* CustomLogic 自定义逻辑，是由一系列Node子逻辑的自由组合

                * CustomLogic 本身是一个逻辑根节点，拥有其他节点相同的性质。
                  特殊职责在于： 
                  作为外部输入的门面（如：update、接口方式的事件通知）
                  存放一块公用的数据黑板

                * 可以在CustomLogic的特化子类（如SkillLogic、BuffLogic），扩展对外部输入响应的种类。
                * 出于各种权衡，外部输入的方式 主要采用显视接口调用的形式，而不是Event、Signal
*********************************************************************/

using System.Collections.Generic;
using System.Xml;

namespace CoreGame.Custom
{
    public static partial class NodeConfigTypeRegistry
    {
        private static bool _CustomLogicConfig = Register(typeof(CustomLogicCfg), NodeCategory.Mixture);
    }

    //////////////////////////////////////////////////////////////////////////
    //自定义逻辑静态配置，装配信息
    //////////////////////////////////////////////////////////////////////////
    public class CustomLogicCfg : ICustomNodeXmlCfg, INodeCfgList
    {
        public List<ICustomNodeCfg> NodeCfgList
        {
            get { return m_nodeCfgList; }
        }

        public int ID = 0;

        private List<ICustomNodeCfg> m_nodeCfgList = new List<ICustomNodeCfg>();

        public virtual System.Type NodeType()
        {
            return typeof(CustomLogic);
        }

        public virtual bool ParseFromXml(System.Xml.XmlNode cfgNode)
        {
            int id = cfgNode.GetLogicConfigID();

            //逐个解析子节点
            XmlNodeList customNodeList = cfgNode.SelectNodes("Node");
            if (customNodeList == null)
                return false;

            this.ID = id;
            this.m_nodeCfgList.Clear();
            foreach (XmlNode customNode in customNodeList)
            {
                try
                {
                    ICustomNodeCfg nodeCfg = CustomLogicConfigMng.CreateNodeCfg(customNode);
                    this.m_nodeCfgList.Add(nodeCfg);
                }
                catch (System.Exception e)
                {
                    LogWrapper.LogError("FillCustomLogicCfg Failed, config ID [" + this.ID + "]");
                    LogWrapper.LogError(e);
                    return false;
                }
            }
            return true;
        }
    }

    //////////////////////////////////////////////////////////////////////////
    // 自定义逻辑运行时： 逻辑根节点，由多个子节点构成
    //////////////////////////////////////////////////////////////////////////
    public class CustomLogic : CustomNode, INeedUpdate, INeedStopCheck
    {
        //自定义的子节点
        protected List<ICustomNode> m_customNodes;
        //运行时初始数据
        protected ICustomLogicGenInfo m_genInfo = null;
        //黑板
        protected VariablesLib m_varLibImp;


        public CustomLogic()
        {
            m_varLibImp = new VariablesLib();
            m_customNodes = new List<ICustomNode>();
            m_genInfo = null;
        }

        public VariablesLib VarLib { get { return m_varLibImp; } }

        //////////////////////////////////////////////////////////////////////////
        // ICustomNode

        private HashSet<int> usedTempLogicSet = new HashSet<int>();
        public override void InitializeNode(ICustomNodeCfg cfg, CustomNodeContext context)
        {
            base.InitializeNode(cfg, context);

            m_genInfo = context.GenInfo;
            var logicCfg = cfg as CustomLogicCfg;
            usedTempLogicSet.Clear();

            _InitializeNodes(logicCfg, context, usedTempLogicSet);
        }

        private void _InitializeNodes(CustomLogicCfg logicCfg, CustomNodeContext context, HashSet<int> usedTempLogicSet)
        {
            //装配节点
            for (int i = 0; i < logicCfg.NodeCfgList.Count; i++)
            {
                var nodeCfg = logicCfg.NodeCfgList[i];
                try
                {
                    //////////////////////////// 处理模板引用 Begin ////////////////////////////
                    var templeteCfg = nodeCfg as LogicTempleteCfg;
                    if (templeteCfg != null)
                    {
                        int templeteID = templeteCfg.LogicID;
                        if (usedTempLogicSet.Contains(templeteID))
                        {
                            LogWrapper.LogError("ERROR: 循环引用CustomLogic模板!  RootLogicID=" + m_genInfo.ConfigID + ", templeteID=" + templeteID);
                            continue;
                        }
                        usedTempLogicSet.Add(templeteID);
                        var logiccfg = context.ConfigMng.GetCustomLogicCfg(templeteID) as CustomLogicCfg;
                        if (logiccfg != null)
                        {
                            //插入模板CustomLogic所配置的各个节点
                            _InitializeNodes(logiccfg, context, usedTempLogicSet);
                        }
                        else
                        {
                            LogWrapper.LogError("ERROR: CustomLogic模板找不到 RootLogicID=" + m_genInfo.ConfigID + ", templeteID=" + templeteID);
                        }
                        continue;
                    }
                    ////////////////////////////// 处理模板引用 End ////////////////////////////

                    CustomNode theNode = CustomLogicFactory.CreateCustomNode(nodeCfg, context);
                    this.AddCustomNode(theNode);
                }
                catch (System.Exception e)
                {
                    LogWrapper.LogError("CreateCustomLogic Failed,  ID = " + logicCfg.ID + " ErrorNodeIndex" + i);
                    LogWrapper.LogError(e);
                }
            }
        }

        public override void Destroy()
        {
            //节点
            for (int i = 0; i < m_customNodes.Count; ++i)
            {
                CustomLogicFactory.ObjectPool().Destroy(m_customNodes[i]);
            }
            m_customNodes.Clear();
            ClearInterfaceCache();

            //黑板
            m_varLibRef.Clear();
            m_genInfo = null;

            base.Destroy();
        }

        public override void CollectInterface<T>(ref List<T> interfaceList)
        {
            base.CollectInterface(ref interfaceList);
        }

        public override void CollectInterfaceInChildren<T>(ref List<T> interfaceList)
        {
            for (int i = 0; i < m_customNodes.Count; ++i)
            {
                CustomNode.TraverseCollectInterface(ref interfaceList, m_customNodes[i]);
            }
        }

        //////////////////////////////////////////////////////////////////////////
        //self
        internal virtual void AddCustomNode(CustomNode node)
        {
            m_customNodes.Add(node);
            CacheInterface(node);
        }

        //////////////////////////////////////////////////////////////////////////
        //CustomLogic系统外界信息的通知, 将通知传播给各个CustomNode和其子Node
        private List<INeedUpdate> mNeedUpdateList = new List<INeedUpdate>();
        private List<INeedStopCheck> mNeedStopCheckList = new List<INeedStopCheck>();

        protected virtual void ClearInterfaceCache()
        {
            mNeedUpdateList.Clear();
            mNeedStopCheckList.Clear();
        }

        protected virtual void CacheInterface(CustomNode node)
        {
            //Update作为Node默认的标准驱动方式具有一定特殊性， 父节点去管理子节点的Update
            var updateNode = node as INeedUpdate;
            if (updateNode != null)
            {
                mNeedUpdateList.Add(node as INeedUpdate);
            }

            CustomNode.TraverseCollectInterface(ref mNeedStopCheckList, node);
        }

        //////////////////////////////////////////////////////////////////////////
        //INeedUpdate
        public bool Update(float dt)
        {
            for (int i = 0; i < mNeedUpdateList.Count; ++i)
            {
                var iupdate = mNeedUpdateList[i];
                var node = iupdate as ICustomNode;
                if (node != null && node.IsActive)
                {
                    iupdate.Update(dt);
                }
            }

            return true;
        }

        //////////////////////////////////////////////////////////////////////////
        //INeedStopCheck
        //CustomLogic逻辑的生存周期： 默认是刚创建就可以被销毁
        //除非某些Node通过NeedStopCheck, 表达自己当前不能被销毁，如果被打断可能会出问题
        public virtual bool CanStop()
        {
            for (int i = 0; i < mNeedStopCheckList.Count; ++i)
            {
                var stopcheck = mNeedStopCheckList[i];
                var node = stopcheck as ICustomNode;
                if (node != null && node.IsActive)
                {
                    if (!stopcheck.CanStop())
                        return false;
                }
            }
            return true;
        }
    }
}