using System.Collections.Generic;
using System.IO;

namespace CoreGame.Custom
{

    public class TestCustomLogic
    {
        public int testLogicID = 10004;
        CustomLogic logic;

        public TestCustomLogic()
        {
            string resPath = "../../../source/Project.Test/CustomLogicConfig.xml";
            bool isExist = File.Exists(resPath);

            CustomLogicFactory.Instance().InitConfigMng(resPath);

            // var genInfo = new ICustomLogicGenInfo()
            // {
            //     LogicConfigID = testLogicID,
            //     ConfigGroupName = "LogicUnitTest",
            //     PreBlackboard = null,
            // };
            // logic = CustomLogicFactory.Instance().CreateCustomLogic(genInfo);
            
            var genInfo = new ICustomLogicGenInfo()
            {
                LogicConfigID = -1,
                ConfigGroupName = "LogicUnitTest",
                PreBlackboard = null,
            };
            logic = CustomLogicFactory.Instance().CreateCustomLogic(genInfo, 
                new CustomLogicCfg(-1, new List<ICustomNodeCfg>() 
                {
                    new FTSequenceBhvCfg(new List<ICustomNodeCfg>()
                    {
                        new FTDelayBhvCfg(2f),
                        new FTLogBhvCfg("Test1"),
                        new FTDelayBhvCfg(3f),
                        new FTLogBhvCfg("Test2"),
                    }),
                })
            );
        }

        public void Update(float dt)
        {
            if (logic == null)
                return;

            logic.Update(dt);

            if (logic.CanStop())
            {
                CustomLogicFactory.DestroyCustomLogic(logic);
                logic = null;
            }
        }
    }
}

