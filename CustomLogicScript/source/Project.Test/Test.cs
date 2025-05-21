using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CoreGame.Custom
{

    public class TestCustomLogic
    {
        public int testLogicID = 10004;
        CustomLogic logic;

        public TestCustomLogic()
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
                ConfigContainerName = "LogicUnitTest",
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

