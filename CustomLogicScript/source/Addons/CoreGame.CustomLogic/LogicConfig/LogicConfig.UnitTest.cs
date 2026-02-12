using System.Collections.Generic;

namespace HotUpdate.CoreGame
{
    using Nodes = List<ICustomNodeCfg>;
    // using Logic = CustomLogicCfg;
    // using Templete = LogicTempleteCfg;
    // using CustomState = CustomBhvStateCfg;
    // using FSM = FSMNodeCfg;
    // using State = StateNodeCfg;
    // using Bhv = NoneParamBhvCfg;
    // using Log = LogBhvCfg;
    // using Seq = SequenceBhvCfg;         //顺序
    // using Parallel = ParallelBhvCfg;    //并行
    // using Delay = FTDelayBhvCfg;
    
    public class LogicConfigs_UnitTest : LogicConfigContainerBase
    {
        public LogicConfigs_UnitTest(string name)
            : base(name, 20)
        {
            DefaultLogicType = typeof(CustomLogic);
            InitConfigs_Test();
        }

        private void InitConfigs_Test()
        {
            AddConfig(9990001, new Nodes()
            {
                Log("LogicConfig_UnitTest 9990001"),
                Seq(new Nodes
                {
                    Log("Step1"),
                    Delay(2.1f),
                    Log("Step2"),
                }),
            });
            
            AddConfig(9990002, new Nodes()
            {
                Log("LogicConfig_UnitTest 9990002"),
                BeginCall(node =>
                {
                    node.SetVar<int>("CV_TestV", 1314);
                }),
                Branch(
                    HasVar<int>("CV_TestV"), 
                    LogVar<int>("CV_TestV")
                ),
            }).DefaultVar(env =>
            {
            });
        }
        
    }
}