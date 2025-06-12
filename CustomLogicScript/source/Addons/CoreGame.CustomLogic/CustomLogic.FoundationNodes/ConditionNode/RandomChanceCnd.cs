using System.Xml;

namespace CoreGame.Custom
{
    public static partial class NodeConfigTypeRegistry
    {
        static bool _RandomChanceCndCfg = Register(typeof(RandomChanceCndCfg), NodeCategory.Cnd);
    }
    //静态配置
    public class RandomChanceCndCfg : ConditionBaseCfg
    {
        public float ProbPercent { get; protected set; } = 0f;//百分比概率

        public override System.Type NodeType() { return typeof(RandomChanceCnd); }

        public override bool ParseFromXml(XmlNode cndNode)
        {
            string str = XmlHelper.GetAttribute(cndNode, "ProbPercent");
            CLHelper.Assert(!string.IsNullOrEmpty(str));
            ProbPercent = float.Parse(str);
            return base.ParseFromXml(cndNode);
        }
    }

    //////////////////////////////////////////////////////////////////////////
    // 随机概率条件
    //////////////////////////////////////////////////////////////////////////
    public class RandomChanceCnd : ConditionNodeBase
    {
        private RandomChanceCndCfg mCfg;
        private float mRandNum;

        //////////////////////////////////////////////////////////////////////////
        // ICustomNode
        public override void InitializeNode(ICustomNodeCfg cfg, CustomNodeContext context)
        {
            base.InitializeNode(cfg, context);
            mCfg = cfg as RandomChanceCndCfg;
            mRandNum = UnityEngine.Random.Range(0f, 100f);
        }

        public override void Destroy()
        {
            base.Destroy();
            mRandNum = 0f;
            mCfg = null;
        }

        //////////////////////////////////////////////////////////////////////////
        // ConditionNodeBase
        protected override bool Inner_ConditionCheck()
        {
            return mRandNum < mCfg.ProbPercent;
        }
    }
}

