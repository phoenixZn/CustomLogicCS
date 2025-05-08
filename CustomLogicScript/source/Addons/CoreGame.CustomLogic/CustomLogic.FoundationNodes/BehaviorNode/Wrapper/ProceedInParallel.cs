using System.Collections.Generic;
using System.Xml;

/// <summary>
/// 逻辑节点: ProceedInParallel
/// 节点描述: 并行执行所包含的节点
/// 用例：
/// <Node type="ProceedInParallel">
///   <Node type="PlayEffect" PlayOn="Player" EffectID="21" SetEffectName="AttackChain" />
///   <Node type="PlayEffect" PlayOn="Enemy" EffectID="22" SetEffectName="Attacked" />
/// </Node>
/// </summary>
namespace CoreGame.Custom
{
    //静态配置
    public static partial class NodeConfigTypeRegistry
    {
        private static bool _ParallelBhvCfg = Register(typeof(ProceedInParallelCfg), NodeCategory.Bhv);
    }

    //静态配置
    public class ProceedInParallelCfg : ICustomNodeXmlCfg, IHasSubCfgList
    {
        public NodeCfgList SubCfgList = new NodeCfgList();

        public System.Type NodeType()
        {
            return typeof(ProceedInParallel);
        }

        public bool ParseFromXml(XmlNode xmlNode)
        {
            return SubCfgList.ParseFromXml(xmlNode);
        }

        public List<ICustomNodeCfg> GetNodeCfgList()
        {
            return SubCfgList;
        }
    }

    //////////////////////////////////////////////////////////////////////////
    // 并行执行 行为组包装
    //////////////////////////////////////////////////////////////////////////
    public class ProceedInParallel : CustomNode, IBehavior, INeedStopCheck
    {
        private ProceedInParallelCfg mCfg;
        private List<ICustomNode> mNodeList = new List<ICustomNode>();

        public override void InitializeNode(ICustomNodeCfg cfg, CustomNodeContext context)
        {
            base.InitializeNode(cfg, context);
            mCfg = cfg as ProceedInParallelCfg;
            mNodeList.Clear();

            var theCfg = cfg as ProceedInParallelCfg;
            for (int i = 0; i < theCfg.SubCfgList.Count; ++i)
            {
                ICustomNodeCfg bhvCfg = theCfg.SubCfgList[i];
                var subbhv = CustomLogicFactory.CreateCustomNode(bhvCfg, context);
                AddBhv(subbhv);
            }

            Reset();
        }

        public void AddBhv(ICustomNode node)
        {
            if (!CLHelper.Assert(node is IBehavior))
            {
                CLHelper.Assert(false, "Parallel Add bhv == null");
                return;
            }

            if (node != null)
                mNodeList.Add(node);
        }

        public override void Activate()
        {
            base.Activate();
            for (int i = 0; i < mNodeList.Count; ++i)
            {
                mNodeList[i].Activate();
            }
        }

        public override void Deactivate()
        {
            base.Deactivate();
            for (int i = 0; i < mNodeList.Count; ++i)
            {
                mNodeList[i].Deactivate();
            }
        }

        public override void Destroy()
        {
            if (mNodeList != null)
            {
                for (int i = 0; i < mNodeList.Count; ++i)
                {
                    ICanRecycle irc = mNodeList[i] as ICanRecycle;
                    CustomLogicFactory.ObjectPool().Destroy(irc);
                }
                mNodeList.Clear();
            }
        }

        //////////////////////////////////////////////////////////////////////////
        //IBehavior
        public virtual void Reset()
        {
            for (int i = 0; i < mNodeList.Count; ++i)
            {
                var bhv = mNodeList[i] as IBehavior;
                bhv.Reset();
            }
            Activate();
        }

        public bool Update(float dt)
        {
            CLHelper.Assert(mNodeList != null);
            if (mNodeList == null)
                return true;

            for (int i = 0; i < mNodeList.Count; ++i)
            {
                var node = mNodeList[i];
                if (!node.IsActive)
                    continue;

                var bhv = node as IBehavior;
                bhv.Update(dt);

                var canStopBhv = node as INeedStopCheck;
                if (canStopBhv != null && canStopBhv.CanStop())
                {
                    node.Deactivate();
                }
            }
            return true;
        }

        //////////////////////////////////////////////////////////////////////////
        //ICustomNode
        public override void CollectInterfaceInChildren<T>(ref List<T> interfaceList)
        {
            base.CollectInterfaceInChildren<T>(ref interfaceList);
            if (mNodeList == null)
                return;
            for (int i = 0; i < mNodeList.Count; ++i)
            {
                CustomNode.TraverseCollectInterface(ref interfaceList, mNodeList[i]);
            }
        }

        //////////////////////////////////////////////////////////////////////////
        // INeedStopCheck
        public bool CanStop()
        {
            for (int i = 0; i < mNodeList.Count; ++i)
            {
                INeedStopCheck bhvSC = mNodeList[i] as INeedStopCheck;
                if (bhvSC != null && !bhvSC.CanStop())
                {
                    return false;
                }
            }
            return true;
        }
    }
}