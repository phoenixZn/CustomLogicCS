
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace CoreGame.Custom
{
    public static partial class NodeConfigTypeRegistry
    {
        static bool _FTSequenceBhvCfg = Register(typeof(FTSequenceBhvCfg), NodeCategory.Bhv);
    }

    //静态配置
    public class FTSequenceBhvCfg : ICustomNodeXmlCfg, IHasSubCfgList
    {
        public NodeCfgList SubCfgList = new NodeCfgList();

        public System.Type NodeType() { return typeof(FTSequenceBhv); }

        public bool ParseFromXml(XmlNode xmlNode)
        {
            return SubCfgList.ParseFromXml(xmlNode);
        }
        public List<ICustomNodeCfg> GetNodeCfgList() { return SubCfgList; }
    }


    //////////////////////////////////////////////////////////////////////////
    // 顺序执行 行为队列包装 
    //////////////////////////////////////////////////////////////////////////
    public class FTSequenceBhv : FiniteTimeBhv
    {
        private List<FiniteTimeBhv> mBehaviorSeq = new List<FiniteTimeBhv>();
        private int mCurBhvIndex = 0;

        public void Add(FiniteTimeBhv bhv)
        {
            if (bhv != null)
                mBehaviorSeq.Add(bhv);
            else
                CLHelper.Assert(false, "FTSequenceBhv Add bhv == null");
        }

        //////////////////////////////////////////////////////////////////////////
        // ICustomNode
        public override void InitializeNode(ICustomNodeCfg cfg, CustomNodeContext context)
        {
            base.InitializeNode(cfg, context);

            mCurBhvIndex = 0;
            mBehaviorSeq.Clear();

            FTSequenceBhvCfg theCfg = cfg as FTSequenceBhvCfg;
            for (int i = 0; i < theCfg.SubCfgList.Count; ++i)
            {
                ICustomNodeCfg bhvCfg = theCfg.SubCfgList[i];
                FiniteTimeBhv subbhv = CustomLogicFactory.CreateCustomNode(bhvCfg, context) as FiniteTimeBhv;
                if (!CLHelper.Assert(subbhv != null))
                    continue;
                if (i == 0)
                    subbhv.Activate();
                else
                    subbhv.Deactivate();
                Add(subbhv);
            }
        }

        public override void Activate()
        {
            base.Activate();
            ActivateCurBhv();
        }

        public override void Deactivate()
        {
            base.Deactivate();
            DeactivateCurBhv();
        }

        public override void Destroy()
        {
            base.Destroy();
            for (int i = 0; i < mBehaviorSeq.Count; ++i)
            {
                CustomLogicFactory.ObjectPool().Destroy(mBehaviorSeq[i]);
            }
            mBehaviorSeq.Clear();
        }

        //////////////////////////////////////////////////////////////////////////
        //FiniteTimeBhv
        public override void Reset()
        {
            base.Reset();
            mCurBhvIndex = 0;

            float totalDuration = 0;
            for (int i = 0; i < mBehaviorSeq.Count; ++i)
            {
                mBehaviorSeq[i].Reset();
                totalDuration += mBehaviorSeq[i].GetDuration();
            }
            InitDuration(totalDuration);
        }

        public override bool IsDurationEnd()
        {
            if (mBehaviorSeq.Count == 0)
                return true;
            //最后一个行为结束，判定为全部结束
            if (mCurBhvIndex >= mBehaviorSeq.Count)
            {
                return true;
            }
            return false;
        }

        public override bool Update(float dt)
        {
            base.Update(dt);

            CLHelper.Assert(mBehaviorSeq != null);
            if (mBehaviorSeq == null || mBehaviorSeq.Count == 0)
                return true;

            //尽量保证时间精确，过剩的时间片传入后续的更新
            float dt_overplus = dt;
            for (int i = 0;  dt_overplus > 0 && mCurBhvIndex < mBehaviorSeq.Count; ++i )
            {
                if (i > mBehaviorSeq.Count) //设立极端中断条件，防止死循环
                    break;

                float curDur = mBehaviorSeq[mCurBhvIndex].GetDuration();
                mBehaviorSeq[mCurBhvIndex].Update(dt_overplus);

                //下面这个看似多余的判断，是为了防止某个节点内部的逻辑，可能会干坏事Clear整个CustomLogic
                if (mCurBhvIndex >= mBehaviorSeq.Count)
                {
                    //mCurBhvIndex = mBehaviorSeq.Count;
                    break;
                }

                dt_overplus = dt_overplus - curDur;
                if (mBehaviorSeq[mCurBhvIndex].IsDurationEnd())
                {
                    //进行下一个行为
                    DeactivateCurBhv();
                    ++mCurBhvIndex;
                    ActivateCurBhv();
                }
            }

            return true;
        }

        //////////////////////////////////////////////////////////////////////////
        //ICustomNode
        public override void CollectInterfaceInChildren<T>(ref List<T> interfaceList)
        {
            base.CollectInterfaceInChildren<T>(ref interfaceList);
            if (mBehaviorSeq == null)
                return;
            for (int i = 0; i < mBehaviorSeq.Count; ++i)
            {
                CustomNode.TraverseCollectInterface(ref interfaceList, mBehaviorSeq[i]);
            }
        }

        //////////////////////////////////////////////////////////////////////////
        //self
        private void ActivateCurBhv()
        {
            if (mCurBhvIndex >= 0 && mCurBhvIndex < mBehaviorSeq.Count)
            {
                mBehaviorSeq[mCurBhvIndex].Activate();
            }
        }
        private void DeactivateCurBhv()
        {
            if (mCurBhvIndex >= 0 && mCurBhvIndex < mBehaviorSeq.Count)
            {
                mBehaviorSeq[mCurBhvIndex].Deactivate();
            }
        }

    }
}