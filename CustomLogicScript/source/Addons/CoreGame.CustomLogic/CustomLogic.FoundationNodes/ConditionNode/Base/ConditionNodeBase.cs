using System.Collections.Generic;
using System.Xml;

namespace CoreGame.Custom
{
    public abstract class ConditionBaseCfg : ICustomNodeXmlCfg
    {
        public bool UseUnaryOperationNOT { get; protected set; } = false;
        public bool UseFixedResult { get; protected set; } = false;
        
        public abstract System.Type NodeType();

        public bool ParseFromXml(XmlNode node)
        {
            UseUnaryOperationNOT = XmlHelper.GetBool(node, "UseUnaryNOT");
            UseFixedResult = XmlHelper.GetBool(node, "UseFixedResult");
            return true;
        }
    }
    
    
    public abstract class ConditionNodeBase : CustomNode, ICondition, ICanReset
    {
        //是否对条件结果取反
        protected bool mUseUnaryOperationNOT = false;
        //是否判断一次之后，保存结果，使得返回值不再变化
        protected bool mUseFixedResult = false;
        //保存判断结果 for UseFixedResult
        protected bool? mFixedResult = null;
        
        
        //////////////////////////////////////////////////////////////////////////
        // ICustomNode
        public override void InitializeNode(ICustomNodeCfg cfg, CustomNodeContext context)
        {
            base.InitializeNode(cfg, context);
            var theCfg = cfg as ConditionBaseCfg;
            mUseUnaryOperationNOT = theCfg.UseUnaryOperationNOT;
            mUseFixedResult = theCfg.UseFixedResult;
            mFixedResult = null;
        }

        public override void Destroy()
        {
            mUseUnaryOperationNOT = false;
            mUseFixedResult = false;
            mFixedResult = null;
            base.Destroy();
        }
        
        //////////////////////////////////////////////////////////////////////////
        // ICondition
        public virtual bool IsConditionReached()
        {
            if (mFixedResult != null)
            {
                return (bool) mFixedResult;
            }
            var res = Inner_ConditionCheck();
            if (mUseUnaryOperationNOT)
            {
                res = !res;
            }
            if (mUseFixedResult)
            {
                mFixedResult = res;
            }
            return res;
        }
        
        public virtual void Reset()
        {
            mFixedResult = null;
        }
        
        ///////////////////////////////////////////////////////////////////////////
        // this
        protected virtual bool Inner_ConditionCheck()
        {
            return false;
        }
    }


    //静态配置
    public abstract class CndListCfg : ICustomNodeXmlCfg
    {
        public List<ICustomNodeCfg> CndCfgList;

        public abstract System.Type NodeType();

        public bool ParseFromXml(XmlNode cndNode)
        {
            CndCfgList = new List<ICustomNodeCfg>();

            XmlNodeList subNodeList = cndNode.SelectNodes("Condition");
            if (subNodeList == null)
                return false;
            foreach (XmlNode subNode in subNodeList)
            {
                ICustomNodeCfg cndCfg = ICustomNodeXmlCfg.CreateNodeCfg(subNode);
                CndCfgList.Add(cndCfg);
            }
            if (this.CndCfgList.Count == 0)
            {
                LogWrapper.LogError("GroupCndCfg.ParseFromXml() CndCfgList.Count == 0");
                CLHelper.AssertBreak();
                return false;
            }
            return true;
        }
    }

}
