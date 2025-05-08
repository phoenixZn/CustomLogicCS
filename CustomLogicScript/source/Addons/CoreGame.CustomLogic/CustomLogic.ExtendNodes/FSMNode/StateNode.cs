using System.Xml;

namespace CoreGame.Custom
{
    //静态配置
    public class StateNodeCfg
    {
        public string StateName;
        public int StateID = -1;

        public virtual bool ParseFromXml(XmlNode xmlNode)
        {
            StateName = XmlHelper.GetAttribute(xmlNode, "StateName");

            string str = XmlHelper.GetAttribute(xmlNode, "StateID");
            if (!string.IsNullOrEmpty(str))
                StateID = int.Parse(str);

            return true;
        }
    }

    public class StateNode : CustomNode, INeedUpdate, INeedStopCheck
    {
        protected int mStateID = -1;
        public string StateName;

        //////////////////////////////////////////////////////////////////////////
        // CustomNode
        public override void InitializeNode(ICustomNodeCfg cfg, CustomNodeContext context)
        {
            base.InitializeNode(cfg, context);
            StateNodeCfg theCfg = cfg as StateNodeCfg;

            CLHelper.Assert(theCfg.StateID != -1, "theCfg.StateID == -1");
            if (mStateID == -1)
                mStateID = theCfg.StateID;
            StateName = theCfg.StateName;
        }

        public override void Destroy()
        {
            base.Destroy();
            mStateID = -1;
            StateName = null;
        }

        //////////////////////////////////////////////////////////////////////////
        //INeedUpdate
        public virtual bool Update(float dt)
        {
            return true;
        }

        //////////////////////////////////////////////////////////////////////////
        //StateBase
        public int StateID
        {
            get { return mStateID; }
        }

        public virtual void Enter()
        {
            this.Activate();
        }

        public virtual void Exit()
        {
            this.Deactivate();
        }

        public virtual int CheckTransitions()
        {
            return StateID;
        }

        ////////////////////////////////////////////////////////////////////////
        //INeedStopCheck
        public virtual bool CanStop()
        {
            return false;
        }
    }
}