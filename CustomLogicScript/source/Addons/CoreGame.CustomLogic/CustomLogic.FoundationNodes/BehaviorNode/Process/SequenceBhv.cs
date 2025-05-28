//
// using System.Collections;
// using System.Collections.Generic;
// using System.Xml;
//
// namespace CoreGame.Custom
// {
//     public static partial class NodeConfigTypeRegistry
//     {
//         static bool SequenceBhvCfg = Register(typeof(SequenceBhvCfg), NodeCategory.Bhv);
//     }
//
//     //静态配置
//     public class SequenceBhvCfg : ICustomNodeXmlCfg, IHasSubCfgList
//     {
//         private List<ICustomNodeCfg> mSubCfgList = null;
//         public List<ICustomNodeCfg> SubCfgList => mSubCfgList;
//
//         public int LoopCnt = 1;
//         public float LoopInterval = 0f;   
//         
//         public System.Type NodeType() { return typeof(SequenceBhv); }
//
//         public SequenceBhvCfg(){}
//
//         public SequenceBhvCfg(List<ICustomNodeCfg> nodeCfgList)
//         {
//             mSubCfgList = nodeCfgList;
//         }
//         public bool ParseFromXml(XmlNode xmlNode)
//         {
//             NodeCfgList cfglist = new();
//             mSubCfgList = cfglist;
//             return cfglist.ParseFromXml(xmlNode);
//         }
//         public List<ICustomNodeCfg> GetNodeCfgList() { return SubCfgList; }
//     }
//
//
//     //////////////////////////////////////////////////////////////////////////
//     // 顺序执行 行为队列包装 
//     //////////////////////////////////////////////////////////////////////////
//     public class SequenceBhv : BehaviorNodeBase, INeedStopCheck
//     {
//         private List<BehaviorNodeBase> mBehaviorSeq = new();
//         private int mCurBhvIndex = 0;
//         private int mLoopCnt = 1;
//         private int mRemainLoopCnt = 0;
//         private float mLoopInterval = 0;
//         private float mRemainTimeToNextLoop = -1f;
//         
//         public void Add(BehaviorNodeBase bhv)
//         {
//             if (bhv != null)
//             {
//                 bhv.Deactivate();
//                 mBehaviorSeq.Add(bhv);
//             }
//             else
//             {
//                 CLHelper.Assert(false, "SequenceBhv Add bhv == null");
//             }
//         }
//
//         //////////////////////////////////////////////////////////////////////////
//         // ICustomNode
//         public override void InitializeNode(ICustomNodeCfg cfg, CustomNodeContext context)
//         {
//             base.InitializeNode(cfg, context);
//             SequenceBhvCfg theCfg = cfg as SequenceBhvCfg;
//
//             mLoopCnt = theCfg.LoopCnt;
//             mLoopInterval = theCfg.LoopInterval;
//             
//             mRemainLoopCnt = mLoopCnt;
//             
//             mCurBhvIndex = 0;
//             mBehaviorSeq.Clear();
//
//
//             for (int i = 0; i < theCfg.SubCfgList.Count; ++i)
//             {
//                 ICustomNodeCfg bhvCfg = theCfg.SubCfgList[i];
//                 var subbhv = mContext.NodeFactory.CreateCustomNode(bhvCfg, context) as BehaviorNodeBase;
//                 if (!CLHelper.Assert(subbhv != null))
//                     continue;
//                 mBehaviorSeq.Add(subbhv);
//             }
//         }
//
//         public override void Activate()
//         {
//             base.Activate();
//             ActivateCurBhv();
//         }
//
//         public override void Deactivate()
//         {
//             base.Deactivate();
//             DeactivateCurBhv();
//         }
//
//         public override void Destroy()
//         {
//             base.Destroy();
//             for (int i = 0; i < mBehaviorSeq.Count; ++i)
//             {
//                 mContext.NodeFactory.DestroyCustomNode(mBehaviorSeq[i]);
//             }
//             mBehaviorSeq.Clear();
//         }
//
//         //////////////////////////////////////////////////////////////////////////
//         //FiniteTimeBhv
//         public override void Reset()
//         {
//             base.Reset();
//             mCurBhvIndex = 0;
//             mRemainLoopCnt = mLoopCnt;
//             mRemainTimeToNextLoop = -1f;
//             
//             for (int i = 0; i < mBehaviorSeq.Count; ++i)
//             {
//                 mBehaviorSeq[i].Reset();
//             }
//         }
//         
//
//         public override float Update(float dt)
//         {
//             base.Update(dt);
//
//             CLHelper.Assert(mBehaviorSeq != null);
//             if (mBehaviorSeq == null || mBehaviorSeq.Count == 0)
//                 return dt;
//
//             //尽量保证时间精确，过剩的时间片传入后续的更新
//             float dt_overplus = dt;
//             for (int i = 0;  dt_overplus > 0 && mCurBhvIndex < mBehaviorSeq.Count; ++i )
//             {
//                 if (i > mBehaviorSeq.Count) //设立极端中断条件，防止死循环
//                     break;
//
//                 float curDur = mBehaviorSeq[mCurBhvIndex].GetDuration();
//                 mBehaviorSeq[mCurBhvIndex].Update(dt_overplus);
//
//                 //下面这个看似多余的判断，是为了防止某个节点内部的逻辑，可能会干坏事Clear整个CustomLogic
//                 if (mCurBhvIndex >= mBehaviorSeq.Count)
//                 {
//                     //mCurBhvIndex = mBehaviorSeq.Count;
//                     break;
//                 }
//
//                 dt_overplus = dt_overplus - curDur;
//                 if (mBehaviorSeq[mCurBhvIndex].IsDurationEnd())
//                 {
//                     //进行下一个行为
//                     DeactivateCurBhv();
//                     ++mCurBhvIndex;
//                     ActivateCurBhv();
//                 }
//             }
//
//             return dt_overplus;
//         }
//
//         //////////////////////////////////////////////////////////////////////////
//         //ICustomNode
//         public override void CollectInterfaceInChildren<T>(ref List<T> interfaceList)
//         {
//             base.CollectInterfaceInChildren<T>(ref interfaceList);
//             if (mBehaviorSeq == null)
//                 return;
//             for (int i = 0; i < mBehaviorSeq.Count; ++i)
//             {
//                 CustomNode.TraverseCollectInterface(ref interfaceList, mBehaviorSeq[i]);
//             }
//         }
//         
//         //////////////////////////////////////////////////////////////////////////
//         //INeedStopCheck
//         public bool CanStop()
//         {
//             if (mBehaviorSeq.Count == 0)
//                 return true;
//             return mRemainLoopCnt <= 0;
//         }
//         
//         //////////////////////////////////////////////////////////////////////////
//         //self
//         private void ActivateCurBhv()
//         {
//             if (mCurBhvIndex >= 0 && mCurBhvIndex < mBehaviorSeq.Count)
//             {
//                 mBehaviorSeq[mCurBhvIndex].Activate();
//             }
//         }
//         private void DeactivateCurBhv()
//         {
//             if (mCurBhvIndex >= 0 && mCurBhvIndex < mBehaviorSeq.Count)
//             {
//                 mBehaviorSeq[mCurBhvIndex].Deactivate();
//             }
//         }
//
//     }
// }