using System.Xml;

namespace CoreGame.Custom
{
    public static partial class NodeConfigTypeRegistry
    {
        static bool _TimeOutCndCfg = Register(typeof(TimeOutCndCfg), NodeCategory.Cnd);
    }
    //静态配置
    public class TimeOutCndCfg : ICustomNodeXmlCfg
    {
        public System.Type NodeType() { return typeof(TimeOutCnd); }

        public float TimeLimit;  //定时设置（秒）

        public bool ParseFromXml(XmlNode cndNode)
        {
            string time = XmlHelper.GetAttribute(cndNode, "time");
            if (CLHelper.Assert(!string.IsNullOrEmpty(time)))
            {
                float.TryParse(time, out TimeLimit);
            }
            return true;
        }

    }

    //////////////////////////////////////////////////////////////////////////
    // 时间条件，超时
    //////////////////////////////////////////////////////////////////////////
    public class TimeOutCnd : BaseCnd, INeedStopCheck
    {
        private TimeOutCndCfg mCfg;
        private float mTimeAcc = 0;

        public void Init(TimeOutCndCfg cfg)
        {
            mCfg = cfg;
            mTimeAcc = 0;
        }

        //////////////////////////////////////////////////////////////////////////
        // ICustomNode
        public override void InitializeNode(ICustomNodeCfg cfg, CustomNodeContext context)
        {
            base.InitializeNode(cfg, context);
            Init(cfg as TimeOutCndCfg);
        }

        public override void Destroy()
        {
            base.Destroy();
            mTimeAcc = 0;
            mCfg = null;
        }

        //////////////////////////////////////////////////////////////////////////
        // ICondition
        public override bool Update(float dt) 
        {
            if (mTimeAcc <= mCfg.TimeLimit)
            {
                mTimeAcc += dt;
            }
            return true;
        }

        public override bool IsConditionReached()
        {
            return mTimeAcc > mCfg.TimeLimit;
        }

        //////////////////////////////////////////////////////////////////////////
        // ICanReset
        public override void Reset() 
        {
            mTimeAcc = 0;
        }

        //////////////////////////////////////////////////////////////////////////
        // INeedStopCheck
        public bool CanStop()
        {
            return IsConditionReached();
        }

    }
}

