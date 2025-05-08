using System.Collections.Generic;
using System.Xml;

namespace CoreGame.Custom
{
    //静态配置
    public class LogicORCndCfg : CndListCfg
    {
        public override System.Type NodeType() { return typeof(LogicORCnd); }

    }

    //////////////////////////////////////////////////////////////////////////
    // 条件组合： 布尔逻辑 或（OR）
    //////////////////////////////////////////////////////////////////////////
    public class LogicORCnd : BaseCnd, INeedStopCheck
    {
        private List<ICondition> mCndList = new List<ICondition>();

        //////////////////////////////////////////////////////////////////////////
        // ICustomNode
        public override void InitializeNode(ICustomNodeCfg cfg, CustomNodeContext context)
        {
            base.InitializeNode(cfg, context);

            mCndList.Clear();
            LogicORCndCfg cndcfg = (LogicORCndCfg)cfg;
            for (int i = 0; i < cndcfg.CndCfgList.Count; ++i)
            {
                ICustomNodeCfg subcndCfg = cndcfg.CndCfgList[i];
                ICondition subcnd = CustomLogicFactory.CreateCustomNode(subcndCfg, context) as ICondition;
                Add(subcnd);
            }
        }

        public void Add(ICondition cnd)
        {
            mCndList.Add(cnd);
        }

        public override void Destroy()
        {
            base.Destroy();
            if (mCndList == null)
                return;
            for (int i = 0; i < mCndList.Count; ++i)
            {
                ICanRecycle icr = mCndList[i] as ICanRecycle;
                CustomLogicFactory.ObjectPool().Destroy(icr);
            }
            mCndList.Clear();
        }

        //////////////////////////////////////////////////////////////////////////
        // ICondition
        public override bool Update(float dt)
        {
            CLHelper.Assert(mCndList != null);
            if (mCndList == null)
                return true;
            for (int i = 0; i < mCndList.Count; ++i)
            {
                var updateCnd = mCndList[i] as INeedUpdate;
                if (updateCnd != null)
                {
                    updateCnd.Update(dt);
                }
            }
            return true;
        }

        public override bool IsConditionReached()
        {
            for (int i = 0; i < mCndList.Count; ++i)
            {
                if (mCndList[i].IsConditionReached())
                {
                    return true;
                }
            }
            return false;
        }

        //////////////////////////////////////////////////////////////////////////
        //ICustomNode
        public override void CollectInterfaceInChildren<T>(ref List<T> interfaceList)
        {
            base.CollectInterfaceInChildren<T>(ref interfaceList);
            if (mCndList == null)
                return;
            for (int i = 0; i < mCndList.Count; ++i)
            {
                CustomNode.TraverseCollectInterface(ref interfaceList, mCndList[i]);
            }
        }

        //////////////////////////////////////////////////////////////////////////
        // ICanReset
        public override void Reset()
        {
            for (int i = 0; i < mCndList.Count; ++i)
            {
                ICanReset icr = mCndList[i] as ICanReset;
                if (icr != null)
                {
                    icr.Reset();
                }
            }
        }


        //////////////////////////////////////////////////////////////////////////
        // INeedStopCheck
        public bool CanStop()
        {
            for (int i = 0; i < mCndList.Count; ++i)
            {
                INeedStopCheck insc = mCndList[i] as INeedStopCheck;
                if (insc != null && !insc.CanStop())
                {
                    return false;
                }
            }
            return true;
        }
    }
}
