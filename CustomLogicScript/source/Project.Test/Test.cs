using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CoreGame.DSL;

namespace CoreGame.Custom
{

    public class UnitTest
    {
        private CustomLogic mLogic;
        public UnitTest()
        {
            InitTestXmlCfg();
        }
        
        public void RunTest()
        {
            //TestDSL();
            //TestCustomLogic();
            TestCustomLogic_RealTime();
        }

        public void TestCustomLogic()
        {
            int frame_cnt = 0;
            int ConfigBeg = 10000;
            int ConfigEnd = 10004;
            float dt = 0.5f;
            for (int ConfigID = ConfigBeg; ConfigID <= ConfigEnd; ConfigID++)
            {
                LogWrapper.LogError($"...................Test_CustomLogic[{ConfigID}]................... Beg");
                var genInfo = new ICustomLogicGenInfo()
                {
                    LogicConfigID = ConfigID,
                    ConfigContainerName = "LogicUnitTest",
                    PreEnv = null,
                };
                var logic = CustomLogicFactory.Instance().CreateCustomLogic(genInfo);
                for (int i = 0; i < 20; i++)
                {
                    LogWrapper.LogError($"...................:{i}, time:{i*dt}");
                    frame_cnt++;
                    logic.Update(dt);
                    if (logic.CanStop())
                    {
                        CustomLogicFactory.Instance().DestroyCustomLogic(ref logic);
                        LogWrapper.LogError($"... : 逻辑销毁");
                        break;
                    }
                }
            }
        }
        
        public void TestCustomLogic_RealTime()
        {
            TestXmlCfg(10005);
            //TestCodeCfg_Seq();

            int i = 0;
            while(true)
            {
                float dt = 0.5f;
                int ms = (int) (dt * 1000);
                System.Threading.Thread.Sleep(ms);
                
                if (mLogic == null)
                    continue;
                LogWrapper.LogError($"...................:{i}, time:{i*dt}");
                mLogic.Update(dt);
                if (mLogic.CanStop())
                {
                    LogWrapper.LogError($"DestroyCustomLogic LogicConfigID={mLogic.GenInfo.LogicConfigID}");
                    CustomLogicFactory.Instance().DestroyCustomLogic(ref mLogic);
                }
                i++;
            }
        }
        
        public void TestDSL()
        {
            var varlib = new VarEnv();
            varlib.AddVarType<int>(typeof(int));
            varlib.AddVarType<float>(typeof(float));
            varlib.AddVarType<FixPoint>(typeof(FixPoint));

            varlib.WriteVar<int>("BaseAtk", 9);
            varlib.WriteVar<int>("Def", 4);
            varlib.WriteVar<float>("MulFactor", 1.5f);

            float c = 1.5f;
            FixPoint f = (FixPoint)c;

            DSLCode dsl = new DSLCode();
            dsl.Compile("FixPoint:Atk = BaseAtk * MulFactor \r\n float:CalcuHp = (BaseAtk - 4) * (MulFactor + 1) \r\n float:sub = Atk - CalcuHp");
            dsl.Execute(varlib);

            FixPoint fxv;
            varlib.ReadVar<FixPoint>("Atk", out fxv);
            float fv;
            varlib.ReadVar<float>("CalcuHp", out fv);

            Expression formula = new Expression();
            var b = formula.Compile("float:Atk = BaseAtk * MulFactor");
            var v = formula.Evaluate(varlib);

            varlib.ReadVar<float>("Atk", out fv);

            formula.Reset();
            b = formula.Compile("       ");
            v = formula.Evaluate(varlib);

            formula.Reset();
            b = formula.Compile("3 #AND 0");
            v = formula.Evaluate(varlib);

            formula.Reset();
            b = formula.Compile("1 + 3 * 4");
            v = formula.Evaluate(varlib);

            formula.Reset();
            b = formula.Compile("(2 * 3 <= -2 + 8) & (3 < 4)");
            v = formula.Evaluate(varlib);

            formula.Reset();
            b = formula.Compile("VarA == VarB.B");
            v = formula.Evaluate(varlib);

            formula.Reset();
            b = formula.Compile("-1 + -2 * -3");
            v = formula.Evaluate(varlib);

            formula.Reset();
            b = formula.Compile("(-1) + (-2)");
            v = formula.Evaluate(varlib);

            formula.Reset();
            b = formula.Compile("VarA * Sqrt(9)");
            v = formula.Evaluate(varlib);
        }
        

        //////////////////////////////////////////////////////////////////////////
        private void TestXmlCfg(int testLogicID)
        {
            var genInfo = new ICustomLogicGenInfo()
            {
                LogicConfigID = testLogicID,
                ConfigContainerName = "LogicUnitTest",
                PreEnv = null,
            };
            mLogic = CustomLogicFactory.Instance().CreateCustomLogic(genInfo);
        }

        private static void InitTestXmlCfg()
        {
            // 方法1：通过入口程序集获取（通用）
            var entryAssemblyPath = Assembly.GetEntryAssembly()?.Location;
            var directoryFromAssembly = entryAssemblyPath != null
                ? Path.GetDirectoryName(entryAssemblyPath)
                : "无法获取入口程序集路径";

            // // 方法2：通过进程路径获取（.NET Core 2.1+）
            // var processPath = Environment.ProcessPath;
            // var directoryFromProcess = processPath != null 
            //     ? Path.GetDirectoryName(processPath) 
            //     : "无法获取进程路径";

            // 方法3：通过应用程序域基目录（可能包含子目录）
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // 输出结果
            Console.WriteLine("=== 可执行文件所在目录 ===");
            Console.WriteLine($"方法1（入口程序集）: {directoryFromAssembly}");
            //Console.WriteLine($"方法2（进程路径）: {directoryFromProcess}");
            Console.WriteLine($"方法3（应用程序域基目录）: {baseDirectory}");

            string resPath = $"{directoryFromAssembly}/CustomLogicConfig.xml";
            bool isExist = File.Exists(resPath);
            if (!isExist)
            {
                LogWrapper.LogError($"找不到 resPath={resPath}");
                resPath = "../../../source/Project.Test/CustomLogicConfig.xml";
                isExist = File.Exists(resPath);
            }

            CustomLogicFactory.Instance().InitConfigMng(resPath);
        }

        private void TestCodeCfg_Seq()
        {
            VarEnv varEnv = new VarEnv();
            varEnv.WriteVar("CV_DelayTime1", 0.65f);
            varEnv.WriteVar("CV_LogInfo1", "LogVar1");
            var genInfo = new ICustomLogicGenInfo()
            {
                LogicConfigID = -1,
                ConfigContainerName = "LogicUnitTest",
                PreEnv = varEnv,
            };
            var cfg = new CustomLogicCfg(-1, new List<ICustomNodeCfg>()
            {
                new ConditionBranchBhvCfg(
                    new SimpleFunctionCndCfg("CoreGame.Custom.SimpleFunctionCnd", "TestInvokeStatic1", 1234),
                    new LogBhvCfg("Test ConditionBranchBhvCfg")
                ),
                new SequenceBhvCfg(new List<ICustomNodeCfg>()
                {
                    //FTSequenceBhvCfg
                    new FTDelayBhvCfg(0.2f),
                    new LogBhvCfg("Test1"),
                    new FTDelayBhvCfg("CV_DelayTime1"),
                    new LogBhvCfg("Test2"),
                }, 3, 3.5f),
            });
            mLogic = CustomLogicFactory.Instance().CreateCustomLogic(genInfo, cfg);
        }


    }
}

