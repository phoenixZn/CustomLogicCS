namespace CoreGame.DSL
{
    public static class DSLHelper
    {
        static int ErrorCnt;
        public static void LogError(string logstr)
        {
            ErrorCnt++;
            //LogWrapper.LogError(logstr);
        }
        public static void LogError(params object[] data)
        {
            ErrorCnt++;
            //LogWrapper.LogError(data);
        }
    }
}
