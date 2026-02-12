using System.IO;

namespace HotUpdate.CoreGame
{
    public class EntityCustomLogicGenInfo : ICustomLogicGenInfo
    {
    }
    

    public class LogicUnitTest
    {
        protected CustomLogicService svc = new CustomLogicService();
        protected float realTimeScale = 0f;
            
        public LogicUnitTest()
        {
            string resPath = "../../../source/Project.Test/CustomLogicConfig.xml";
            bool isExist = File.Exists(resPath);
            if (isExist)
            {
                svc.AddConfigContainer(new XmlLogicConfigContainer(LogicContainerKey.LogicConfig_UnitTest_Xml, resPath));
            }

            svc.AddConfigContainer(new LogicConfigs_UnitTest(LogicContainerKey.LogicConfig_UnitTest_CSharp));
            //realTimeScale = 1f;
            //realTimeScale = 4f;
        }
        
        public void RunTest()
        {
            TestCustomLogicContainer(LogicContainerKey.LogicConfig_UnitTest_CSharp);
            //TestCustomLogicContainer(LogicContainerKey.LogicConfig_UnitTest_Xml);
            
            // int ConfigBeg = 10000;
            // int ConfigEnd = 10003;
            // for (int configID = ConfigBeg; configID <= ConfigEnd; configID++)
            // {
            // }
        }
        
        public void TestCustomLogicContainer(string containerName)
        {
            int frame_cnt = 0;
            float dt = 0.5f;
            var cfgs = svc.GetConfigContainer(containerName);
            foreach (var configID in cfgs.GetConfigIDs())
            {
                LogWrapper.LogError($"...................Test_CustomLogic[{configID}]................... Beg");
                var varEnv = svc.NewVarEnv();
                var genInfo = svc.NewGenInfo<EntityCustomLogicGenInfo>();
                genInfo.LogicConfigID = configID;
                genInfo.ConfigContainerName = containerName;
                genInfo.PreEnv = varEnv;
                var logic = svc.CreateLogic(genInfo);
                for (int i = 0; i < 20; i++)
                {
                    CheckSleep(dt);
                    
                    LogWrapper.LogError($"...................:{i}, time:{i*dt}");
                    frame_cnt++;
                    
                    logic.Update(dt);
                    if (logic.CanStop())
                    {
                        svc.DestroyLogic(logic);
                        logic = null;
                        LogWrapper.LogError($"... : 逻辑销毁");
                        break;
                    }
                }
            }
        }

        private void CheckSleep(float dt)
        {
            if (realTimeScale > 0)
            {
                int ms = (int) (dt * 1000 / realTimeScale);
                System.Threading.Thread.Sleep(ms);
            }
        }
    }
}

