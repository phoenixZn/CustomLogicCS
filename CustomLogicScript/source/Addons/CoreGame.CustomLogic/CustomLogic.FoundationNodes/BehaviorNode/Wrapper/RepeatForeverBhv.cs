/*

using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace CoreGame.Custom
{
    public struct RepeatForeverBhvCfg : ICustomNodeXmlCfg, IHasSubBhvCfg
    {
        public BehaviorType NodeType() { return BehaviorType.Bhv_RepeatForever; }

        public ICustomNodeCfg BhvCfg;
        public float Interval;  //无时间消耗的Bhv，设定执行间隔
        public bool ParseFromXml(XmlNode bhvNode)
        {
            string str = XmlHelper.GetAttribute(bhvNode, "Interval");
            if (!string.IsNullOrEmpty(str))
            {
                Interval = float.Parse(str);
                CLHelper.Assert(Interval > 0, "RepeatForeverBhvCfg Interval <= 0");
            }
            if (Interval <= 0f)
            {
                Interval = 1f / 30f/ *LogicFPS* /;
            }

            XmlNode subNode = bhvNode.SelectSingleNode("Behavior");
            CLHelper.Assert(subNode != null);
            BhvCfg = CustomLogicConfigMng.PraseBehavior(subNode);
            return true;
        }

        //IHasSubBhvCfg
        public ICustomNodeCfg GetSubBhvCfg()
        {
            return BhvCfg;
        }
    }

    //////////////////////////////////////////////////////////////////////////
    //无限循环包装行为，它不是有限时间能结束的行为
    public class RepeatForeverBhv : HasBeginBhv
    {
        private RepeatForeverBhvCfg mCfg;
        private FiniteTimeBhv mBhv;
        private float mInterval = 0;

        public void Init(RepeatForeverBhvCfg cfg, FiniteTimeBhv bhv)
        {
            CLHelper.Assert(bhv != null);
            mCfg = cfg;
            mBhv = bhv;
        }

        public override void Destroy()
        {
            base.Destroy();
            mInterval = 0;
            CustomLogicFactory.ObjectPool().Destroy(mBhv);
            mBhv = null;
        }

        //////////////////////////////////////////////////////////////////////////
        //IBehavior
        public override void Update(float dt)
        {
            base.Update(dt);

            if (mBhv == null)
                return;

            //尽量保证时间精确，过剩的时间片传入后续的更新
            float dt_overplus = dt;     
            for (int i = 0; dt_overplus > 0; ++i)
            {
                float timeSub = mBhv.GetDuration();
                mBhv.Update(dt_overplus);
                if (timeSub <= 0)
                {
                    timeSub = mCfg.Interval;  //如果一个Bhv不耗时间，就设定执行间隔
                }

                dt_overplus = dt_overplus - timeSub;
                if (mBhv.IsDurationEnd())
                {
                    //重新开始
                    mBhv.Reset();
                }
            }
        }


        //////////////////////////////////////////////////////////////////////////
        //ICustomNode
        public override void GetInterfaceInSubNode<T>(ref List<T> interfaceList)
        {
            base.GetInterfaceInSubNode<T>(ref interfaceList);
            CustomNode.CustomObjectGetInterface(ref interfaceList, mBhv);
        }

    }
}*/