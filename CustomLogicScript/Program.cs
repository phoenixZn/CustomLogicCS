using System;
using HotUpdate.CoreGame;

namespace CustomLogicScript
{
    class Program
    {
        static void Main(string[] args)
        {
            TestCustomLogic();
        }
        
        static void TestCustomLogic()
        {
            var logic = new HotUpdate.CoreGame.TestCustomLogic();
            while(true)
            {
                System.Threading.Thread.Sleep(30);
                logic.Update(0.03f);
            }
        }
    }
}