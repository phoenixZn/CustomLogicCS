
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace CoreGame.Custom
{
    public static partial class NodeConfigTypeRegistry
    {
        static bool _DelayBhvCfg = Register(typeof(FTDelayBhvCfg), NodeCategory.Bhv);
    }
    public class FTDelayBhvCfg : ICustomNodeXmlCfg
    {
        public FloatCfg TimeLen { get; protected set; }     //延迟时间

        public System.Type NodeType() { return typeof(FTDelayBhv); }

        public FTDelayBhvCfg()
        {
            TimeLen = new FloatCfg(0f);
        }

        public FTDelayBhvCfg(float timeLen)
        {
            TimeLen = new FloatCfg(timeLen);
        }
        
        public FTDelayBhvCfg(string varID)
        {
            TimeLen = new FloatCfg(0f);
            TimeLen.SetVarID(varID);
        }
        
        public bool ParseFromXml(XmlNode xmlNode)
        {
            var str = XmlHelper.GetAttribute(xmlNode, "TimeLen");
            CLHelper.Assert(!string.IsNullOrEmpty(str));
            TimeLen = new FloatCfg(0);
            return TimeLen.ParseByFormatString(str);
        }

    }

    //////////////////////////////////////////////////////////////////////////
    // 运行时节点:  时间延迟
    //////////////////////////////////////////////////////////////////////////
    public class FTDelayBhv : FiniteTimeBhv
    {
        private FTDelayBhvCfg mCfg;

        //////////////////////////////////////////////////////////////////////////
        // ICustomNode
        public override void InitializeNode(ICustomNodeCfg cfg, CustomNodeContext context)
        {
            base.InitializeNode(cfg, context);
            mCfg = cfg as FTDelayBhvCfg;
            var timeLen = mCfg.TimeLen.GetValue(this);
            InitDuration(timeLen);
        }

        public override void Destroy()
        {
            base.Destroy();
        }

        public override void Reset()
        {
            base.Reset();
            InitDuration(mCfg.TimeLen.GetValue(this));
        }

        protected override void OnBegin()
        {
            LogWrapper.LogInfo($"FTDelayBhv Time={mCfg.TimeLen.GetValue(this)}");
        }
    }
}