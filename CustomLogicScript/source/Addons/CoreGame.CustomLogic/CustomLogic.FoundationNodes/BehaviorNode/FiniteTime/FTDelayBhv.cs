
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
        public float TimeLen { get { return mTimeLen; } }           //延迟时间
        float mTimeLen;

        public System.Type NodeType() { return typeof(FTDelayBhv); }

        public FTDelayBhvCfg(){}

        public FTDelayBhvCfg(float timeLen)
        {
            mTimeLen = timeLen;
        }
        
        public bool ParseFromXml(XmlNode xmlNode)
        {
            mTimeLen = 0f;
            string str = XmlHelper.GetAttribute(xmlNode, "TimeLen");
            if (!string.IsNullOrEmpty(str))
            {
                float.TryParse(str, out mTimeLen);
            }
            return true;
        }

    }

    public class FTDelayBhv : FiniteTimeBhv
    {
        private FTDelayBhvCfg mCfg;

        //////////////////////////////////////////////////////////////////////////
        // ICustomNode
        public override void InitializeNode(ICustomNodeCfg cfg, CustomNodeContext context)
        {
            base.InitializeNode(cfg, context);
            mCfg = cfg as FTDelayBhvCfg;
            InitDuration(mCfg.TimeLen);
        }

        public override void Destroy()
        {
            base.Destroy();
        }

        public override void Reset()
        {
            base.Reset();
            InitDuration(mCfg.TimeLen);
        }

        protected override void OnBegin()
        {
            LogWrapper.LogInfo("FTDelayBhv " + mCfg.TimeLen);
        }
    }
}