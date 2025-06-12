using System.Diagnostics;

namespace CoreGame.Custom
{
    //////////////////////////////////////////////////////////////////////////
    /// 硬编码的特殊状态逻辑
    public static partial class NodeConfigTypeRegistry
    {
        static bool TestHeroStateCfg = Register(typeof(TestHeroStateCfg), NodeCategory.State);
    }
    public class TestHeroStateCfg : StateNodeCfg
    {
        public override System.Type NodeType()
        {
            switch (StateID)
            {
                case "HST_Idle":
                    return typeof(TestHeroStateIdle);
                case "HST_Run":
                    return typeof(TestHeroStateRun);
                case "HST_Skill":
                    return typeof(TestHeroStateSkill);
            }
            return typeof(TestHeroStateIdle);
        }
    }
    public class TestHeroStateIdle : StateNode
    {
        public override void Enter()
        {
            base.Enter();
            this.LogInfo("TestHeroStateIdle Enter");
            this.LogInfo("TestHeroStateIdle 调用了一堆特殊接口 ....");
        }

        public override void Exit()
        {
            this.LogInfo("TestHeroStateIdle Exit");
            base.Exit();
        }
    }
    
    public class TestHeroStateRun : StateNode
    {
        public override void Enter()
        {
            base.Enter();
            this.LogInfo("TestHeroStateRun Enter");
            this.LogInfo("TestHeroStateRun 调用了一堆特殊接口 ....");
        }

        public override void Exit()
        {
            this.LogInfo("TestHeroStateRun Exit");
            base.Exit();
        }
    }
    
    
    public class TestHeroStateSkill : StateNode
    {
        public override void Enter()
        {
            base.Enter();
            this.LogInfo("TestHeroStateSkill Enter");
            this.LogInfo("TestHeroStateSkill 调用了一堆特殊接口 ....");
        }

        public override void Exit()
        {
            this.LogInfo("TestHeroStateSkill Exit");
            base.Exit();
        }
    }
}