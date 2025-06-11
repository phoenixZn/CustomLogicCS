
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace CoreGame.Custom
{
    public static partial class NodeConfigTypeRegistry
    {
        static bool SequenceBhvCfg = Register(typeof(SequenceBhvCfg), NodeCategory.Bhv);
    }

    //静态配置
    public class SequenceBhvCfg : ICustomNodeXmlCfg, IHasSubCfgList
    {
        public List<ICustomNodeCfg> SubCfgList { get; protected set; } = null;
        public int LoopCnt { get; protected set; } = 1;
        public float LoopInterval { get; protected set; } = 0f; 
        
        public System.Type NodeType() { return typeof(SequenceBhv); }

        public SequenceBhvCfg(){}

        public SequenceBhvCfg(List<ICustomNodeCfg> nodeCfgList, int loopCnt = 1, float loopInterval = 0)
        {
            SubCfgList = nodeCfgList;
            LoopCnt = loopCnt;
            LoopInterval = loopInterval;
        }
        
        public bool ParseFromXml(XmlNode xmlNode)
        {
            string strLoopCnt = XmlHelper.GetAttribute(xmlNode, "LoopCnt");
            if (!string.IsNullOrEmpty(strLoopCnt))
            {
                LoopCnt = int.Parse(strLoopCnt);
            }
            string strLoopInterval = XmlHelper.GetAttribute(xmlNode, "LoopInterval");
            if (!string.IsNullOrEmpty(strLoopInterval))
            {
                LoopInterval = float.Parse(strLoopInterval);
            }
            
            NodeCfgList cfglist = new();
            SubCfgList = cfglist;
            return cfglist.ParseFromXml(xmlNode);
        }
        public List<ICustomNodeCfg> GetNodeCfgList() { return SubCfgList; }
    }


    //////////////////////////////////////////////////////////////////////////
    // 顺序执行 行为队列包装 
    //////////////////////////////////////////////////////////////////////////
    public class SequenceBhv : BehaviorNodeBase, INeedStopCheck
    {
        private List<BehaviorNodeBase> mBehaviorSeq = new();
        private int mCfgLoopCnt = 1;
        private float mCfgLoopInterval = 0;
        
        private int mCurBhvIndex = 0;
        private int mRemainLoopCnt = 0;
        private float mRemainTimeToNextLoop = -1f;
        

        //////////////////////////////////////////////////////////////////////////
        // ICustomNode
        public override void InitializeNode(ICustomNodeCfg cfg, CustomNodeContext context)
        {
            base.InitializeNode(cfg, context);
            SequenceBhvCfg theCfg = cfg as SequenceBhvCfg;

            mCfgLoopCnt = theCfg.LoopCnt;
            mCfgLoopInterval = theCfg.LoopInterval;
            
            mRemainLoopCnt = mCfgLoopCnt;
            
            mCurBhvIndex = 0;
            mBehaviorSeq.Clear();

            if (theCfg.SubCfgList == null)
            {
                this.LogError("SequenceBhv:InitializeNode theCfg.SubCfgList == null");
                return;
            }

            for (int i = 0; i < theCfg.SubCfgList.Count; ++i)
            {
                ICustomNodeCfg bhvCfg = theCfg.SubCfgList[i];
                var subbhv = mContext.NodeFactory.CreateCustomNode(bhvCfg, context) as BehaviorNodeBase;
                if (!CLHelper.Assert(subbhv != null))
                    continue;
                mBehaviorSeq.Add(subbhv);
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
            mCurBhvIndex = 0;
            mRemainLoopCnt = 1;
            mCfgLoopInterval = 0;
            mRemainTimeToNextLoop = -1f;
            for (int i = 0; i < mBehaviorSeq.Count; ++i)
            {
                mContext.NodeFactory.DestroyCustomNode(mBehaviorSeq[i]);
            }
            mBehaviorSeq.Clear();
            
            base.Destroy();
        }

        //////////////////////////////////////////////////////////////////////////
        //FiniteTimeBhv
        public override void Reset()
        {
            base.Reset();
            mCurBhvIndex = 0;
            mRemainLoopCnt = mCfgLoopCnt;
            mRemainTimeToNextLoop = -1f;
            
            for (int i = 0; i < mBehaviorSeq.Count; ++i)
            {
                mBehaviorSeq[i].Reset();
            }
        }
        
        protected override float OnUpdate(float dt)
        {
            if (mBehaviorSeq == null)
                return dt;
            var nodesSize = mBehaviorSeq.Count;
            if (nodesSize == 0)
                return dt;

            var remainLoopCnt = mRemainLoopCnt;
            var totalIndexCnt = nodesSize * remainLoopCnt;
            //尽量保证时间精确，过剩的时间片传入后续的更新
            float dt_remain = dt;
            for (int i = 0;  i < totalIndexCnt; ++i )
            {
                var curIndex = mCurBhvIndex;
                //---------------------- 处理 Loop Interval Beg ----------------------
                var RemainTimeToNextLoop = mRemainTimeToNextLoop;
                if (RemainTimeToNextLoop > 0)
                {
                    if (dt_remain >= RemainTimeToNextLoop)
                    {
                        dt_remain = dt_remain - RemainTimeToNextLoop;
                        mRemainTimeToNextLoop = -1;
                        //开启新的循环
                        curIndex = 0;
                        //所有节点Reset
                        foreach (var bhv in mBehaviorSeq)
                        {
                            bhv.Reset();
                        }
                        mCurBhvIndex = curIndex;
                        ActivateCurBhv();
                    }
                    else
                    {
                        mRemainTimeToNextLoop = RemainTimeToNextLoop - dt_remain;
                        dt_remain = 0;
                    }
                }
                //---------------------- 处理 Loop Interval End ----------------------
                if (dt_remain <= 0)
                {
                    break;
                }

                if (curIndex >= nodesSize )
                {
                    this.LogError("SequenceBhv:BN_OnUpdate curIndex >= nodesSize");
                    break;
                }

                var curBhv = mBehaviorSeq[mCurBhvIndex];
                //过剩的时间片传入后续的更新
                dt_remain = curBhv.Update(dt_remain);

                //内部的节点如果有不推荐的暴力行为, Update后可能会从内部销毁整个逻辑, 作为通用节点，需要对此防御一手
                if (!IsActive)
                {
                    break;
                }

                if (IsCurBhvEnd(curBhv))
                {
                    //进行下一个行为
                    DeactivateCurBhv();
                    curIndex++;
                    
                    //处理多次循环
                    if (curIndex >= nodesSize )
                    {
                        remainLoopCnt--;
                        mRemainLoopCnt = remainLoopCnt;
                        if (remainLoopCnt > 0)
                        {
                            //设置interval
                            mRemainTimeToNextLoop = mCfgLoopInterval;
                        }
                    }
                    else
                    {
                        mCurBhvIndex = curIndex;
                        ActivateCurBhv();
                    }
                }
                else
                {
                    return 0f;
                }
            }

            return dt_remain;
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
        //INeedStopCheck
        public bool CanStop()
        {
            if (mBehaviorSeq.Count == 0)
                return true;
            return mRemainLoopCnt <= 0;
        }
        
        //////////////////////////////////////////////////////////////////////////
        //self
        private bool IsCurBhvEnd(BehaviorNodeBase curBhv)
        {
            if (curBhv is INeedStopCheck theBhv)
            {
                return theBhv.CanStop();
            }

            return true;
        }
        
        private BehaviorNodeBase GetCurBhv()
        {
            if (mCurBhvIndex < 0 || mCurBhvIndex >= mBehaviorSeq.Count)
            {
                return null;
            }
            return mBehaviorSeq[mCurBhvIndex];
        }
        
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