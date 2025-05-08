using System.Xml;

namespace CoreGame.Custom
{
    public static partial class NodeConfigTypeRegistry
    {
        static bool _RandomChanceCndCfg = Register(typeof(RandomChanceCndCfg), NodeCategory.Cnd);
    }
    //静态配置
    public class RandomChanceCndCfg : ICustomNodeXmlCfg
    {
        public float ProbPercent;   //百分比概率

        public System.Type NodeType() { return typeof(RandomChanceCnd); }

        public bool ParseFromXml(XmlNode cndNode)
        {
            string str = XmlHelper.GetAttribute(cndNode, "ProbPercent");
            CLHelper.Assert(!string.IsNullOrEmpty(str));
            ProbPercent = float.Parse(str);
            return true;
        }
    }

    //////////////////////////////////////////////////////////////////////////
    // 随机概率条件
    //////////////////////////////////////////////////////////////////////////
    public class RandomChanceCnd : CustomNode, ICondition
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
            mRandNum = 0f;
        }

        //////////////////////////////////////////////////////////////////////////
        // ICondition
        public bool IsConditionReached()
        {
            return mRandNum < mCfg.ProbPercent;
        }
    }
}

