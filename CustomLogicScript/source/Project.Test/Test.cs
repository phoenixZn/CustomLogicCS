using System.IO;

namespace CoreGame.Custom
{
    public struct EntityCustomLogicGenInfo : ICustomLogicGenInfo
    {
        int m_configID;
        public int ConfigID
        {
            get { return m_configID; }
            set { m_configID = value; }
        }
    }

    public class TestLogicFactory : CustomLogicFactory
    {
        //单例
        static TestLogicFactory sInstance = null;
        public static TestLogicFactory Instance()
        {
            if (sInstance == null)
            {
                sInstance = new TestLogicFactory();
            }
            return sInstance;
        }

        private TestLogicFactory() { }


        public override void DoCache()
        {
            //cache一些常用的
            //mObjectPool.Cache<CustomLogic>(40);
            //mObjectPool.Cache<CndBhvNode>(60);
            //mObjectPool.Cache<TimeOutCnd>(120);
            //mObjectPool.Cache<FTRepeatBhv>(5);
            //mObjectPool.Cache<LogicANDCnd>(5);
            //mObjectPool.Cache<DelayBhv>(20);
        }
    }



    public class TestCustomLogic
    {
        public int testLogicID = 10004;
        CustomLogic logic;

        public TestCustomLogic()
        {
            string resPath = "../../../source/Project.Test/CustomLogicConfig.xml";
            bool isExist = File.Exists(resPath);

            TestLogicFactory.Instance().InitConfigMng(resPath);

            var genInfo = new EntityCustomLogicGenInfo();
            genInfo.ConfigID = testLogicID;
            logic = TestLogicFactory.Instance().CreateCustomLogic(genInfo);
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

