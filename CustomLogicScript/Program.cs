using System;
using CoreGame.DSL;
using CoreGame.Custom;

namespace CustomLogicScript
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestDSL();
            TestCustomLogic();
        }

        static void TestDSL()
        {
            var varlib = new VariablesLib();
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

        static void TestCustomLogic()
        {
            var logic = new CoreGame.Custom.TestCustomLogic();
            while(true)
            {
                System.Threading.Thread.Sleep(30);
                logic.Update(0.03f);
            }
        }
    }
}