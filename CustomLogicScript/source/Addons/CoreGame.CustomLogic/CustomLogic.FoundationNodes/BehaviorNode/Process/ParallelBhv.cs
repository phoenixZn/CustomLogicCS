using System.Collections.Generic;
using System.Xml;

/// <summary>
/// 逻辑节点: ParallelBhv
/// 节点描述: 并行执行所包含的节点
/// 用例：
/// <Node type="ParallelBhv">
///   <Node type="PlayEffect" PlayOn="Player" EffectID="21" SetEffectName="AttackChain" />
///   <Node type="PlayEffect" PlayOn="Enemy" EffectID="22" SetEffectName="Attacked" />
/// </Node>
/// </summary>
namespace CoreGame.Custom
{
    //静态配置
    public static partial class NodeConfigTypeRegistry
    {
        private static bool _ParallelBhvCfg = Register(typeof(ParallelBhvCfg), NodeCategory.Bhv);
    }

    //静态配置ProceedInParallelCfg
    public class ParallelBhvCfg : ICustomNodeXmlCfg, IHasSubCfgList
    {
        public NodeCfgList SubCfgList = new NodeCfgList();

        public System.Type NodeType()
        {
            return typeof(ParallelBhv);
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
    public class ParallelBhv : BehaviorNodeBase, INeedStopCheck
    {
        private List<BehaviorNodeBase> mNodeList = new();

        public override void InitializeNode(ICustomNodeCfg cfg, CustomNodeContext context)
        {
            base.InitializeNode(cfg, context);
            mNodeList.Clear();

            var theCfg = cfg as ParallelBhvCfg;
            for (int i = 0; i < theCfg.SubCfgList.Count; ++i)
            {
                ICustomNodeCfg bhvCfg = theCfg.SubCfgList[i];
                var subbhv = mContext.NodeFactory.CreateCustomNode(bhvCfg, context) as BehaviorNodeBase;
                AddBhv(subbhv);
            }

            Reset();
        }

        public void AddBhv(BehaviorNodeBase node)
        {
            if (node == null)
            {
                this.LogError("ParallelBhv Add bhv == null");
                return;
            }
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
                    mContext.NodeFactory.DestroyCustomNode(mNodeList[i]);
                }
                mNodeList.Clear();
            }
            base.Destroy();
        }

        //////////////////////////////////////////////////////////////////////////
        //IBehavior
        public virtual void Reset()
        {
            base.Reset();
            for (int i = 0; i < mNodeList.Count; ++i)
            {
                mNodeList[i].Reset();
            }
        }

        protected override float OnUpdate(float dt)
        {
            CLHelper.Assert(mNodeList != null);
            if (mNodeList == null)
                return dt;

            var dt_remain = dt;
            for (int i = 0; i < mNodeList.Count; ++i)
            {
                var bhv = mNodeList[i];
                if (!bhv.IsActive)
                    continue;
                
                var sub_dt_remain = bhv.Update(dt);
                if (sub_dt_remain < dt_remain)
                {
                    dt_remain = sub_dt_remain;
                }
                
                var canStopBhv = bhv as INeedStopCheck;
                if (canStopBhv != null && canStopBhv.CanStop())
                {
                    bhv.Deactivate();
                }
            }
            return dt_remain;
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