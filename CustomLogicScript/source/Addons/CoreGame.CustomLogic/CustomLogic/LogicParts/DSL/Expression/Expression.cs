using System.Collections;
using System.Collections.Generic;
namespace CoreGame.DSL
{
    public class Expression : IRecyclable
    {
        FixPoint m_constant = FixPoint.Zero;
        ExpressionCompiler m_program;

        public void Reset()
        {
            m_constant = FixPoint.Zero;
            if (m_program != null)
            {
                RecyclableObject.Recycle(m_program);
                m_program = null;
            }
        }

        public void CopyFrom(Expression rhs)
        {
            m_constant = rhs.m_constant;
            if (rhs.m_program != null)
            {
                m_program = RecyclableObject.Create<ExpressionCompiler>();
                m_program.CopyFrom(rhs.m_program);
            }
        }

        public List<Variable> GetAllVariables()
        {
            if (m_program != null)
                return m_program.GetAllVariables();
            else
                return null;
        }

        public bool Compile(string expression)
        {
            ExpressionCompiler program = RecyclableObject.Create<ExpressionCompiler>();
            if (!program.Compile(expression))
                return false;

            m_program = program;
            if (program.IsConstant())
            {
                m_constant = Evaluate(null);
                RecyclableObject.Recycle(m_program);
                m_program = null;
            }

            return true;
        }

        public FixPoint Evaluate(IVariableEnv variable_env)
        {
            if (m_program == null)
                return m_constant;

            var instructions = m_program.Instructions;
            var variables = m_program.Variables;

            Stack<FixPoint> stack = new Stack<FixPoint>();
            FixPoint var1, var2, var3;
            int index = 0;
            int total_count = instructions.Count;
            while (index < total_count)
            {
                OperationCode op_code = (OperationCode)instructions[index];
                ++index;
                switch (op_code)
                {
                    case OperationCode.PUSH_NUMBER:
                        stack.Push(FixPoint.CreateFromRaw(instructions[index]));
                        ++index;
                        break;
                    case OperationCode.PUSH_VARIABLE:
                        if (variable_env != null)
                        {
                            var1 = variable_env.GetFixPoint(variables[(int)instructions[index]]);
                            stack.Push(var1);
                        }
                        else
                        {
                            stack.Push(FixPoint.Zero);
                        }
                        ++index;
                        break;
                    case OperationCode.SET_VARIABLE:
                        stack.Push((FixPoint)instructions[index]);
                        ++index;
                        break;
                    case OperationCode.NEGATE:
                        var1 = stack.Pop();
                        stack.Push(-var1);
                        break;
                    case OperationCode.NOT:
                        var1 = stack.Pop();
                        if (var1 == FixPoint.Zero)
                            stack.Push(FixPoint.One);
                        else
                            stack.Push(FixPoint.Zero);
                        break;
                    case OperationCode.SET:
                        var2 = stack.Pop();
                        var1 = stack.Pop();
                        Variable dslVar = variables[(int)var1];
                        if (dslVar.m_varType == VariableType.Int)
                            variable_env.SetVariable<int>(dslVar, (int)var2);
                        else if(dslVar.m_varType == VariableType.Float)
                            variable_env.SetVariable<float>(dslVar, (float)var2);
                        else 
                            variable_env.SetVariable<FixPoint>(dslVar, var2);
                        stack.Push(var2);
                        break;
                    case OperationCode.ADD:
                        var2 = stack.Pop();
                        var1 = stack.Pop();
                        stack.Push(var1 + var2);
                        break;
                    case OperationCode.SUBTRACT:
                        var2 = stack.Pop();
                        var1 = stack.Pop();
                        stack.Push(var1 - var2);
                        break;
                    case OperationCode.MULTIPLY:
                        var2 = stack.Pop();
                        var1 = stack.Pop();
                        stack.Push(var1 * var2);
                        break;
                    case OperationCode.GREATER_THAN:
                        var2 = stack.Pop();
                        var1 = stack.Pop();
                        if (var1 > var2)
                            stack.Push(FixPoint.One);
                        else
                            stack.Push(FixPoint.Zero);
                        break;
                    case OperationCode.LESS_THAN:
                        var2 = stack.Pop();
                        var1 = stack.Pop();
                        if (var1 < var2)
                            stack.Push(FixPoint.One);
                        else
                            stack.Push(FixPoint.Zero);
                        break;
                    case OperationCode.GREATER_EQUAL:
                        var2 = stack.Pop();
                        var1 = stack.Pop();
                        if (var1 >= var2)
                            stack.Push(FixPoint.One);
                        else
                            stack.Push(FixPoint.Zero);
                        break;
                    case OperationCode.LESS_EQUAL:
                        var2 = stack.Pop();
                        var1 = stack.Pop();
                        if (var1 <= var2)
                            stack.Push(FixPoint.One);
                        else
                            stack.Push(FixPoint.Zero);
                        break;
                    case OperationCode.EQUAL:
                        var2 = stack.Pop();
                        var1 = stack.Pop();
                        if (var1 == var2)
                            stack.Push(FixPoint.One);
                        else
                            stack.Push(FixPoint.Zero);
                        break;
                    case OperationCode.NOT_EQUAL:
                        var2 = stack.Pop();
                        var1 = stack.Pop();
                        if (var1 != var2)
                            stack.Push(FixPoint.One);
                        else
                            stack.Push(FixPoint.Zero);
                        break;
                    case OperationCode.AND:
                        var2 = stack.Pop();
                        var1 = stack.Pop();
                        if (var1 == FixPoint.Zero || var2 == FixPoint.Zero)
                            stack.Push(FixPoint.Zero);
                        else
                            stack.Push(FixPoint.One);
                        break;
                    case OperationCode.OR:
                        var2 = stack.Pop();
                        var1 = stack.Pop();
                        if (var1 != FixPoint.Zero && var2 != FixPoint.Zero)
                            stack.Push(FixPoint.One);
                        else
                            stack.Push(FixPoint.Zero);
                        break;
                    case OperationCode.DIVIDE:
                        var2 = stack.Pop();
                        var1 = stack.Pop();
                        stack.Push(var1 / var2);
                        break;
                    case OperationCode.SIN:
                        var1 = stack.Pop();
                        stack.Push(FixPoint.Sin(var1));
                        break;
                    case OperationCode.COS:
                        var1 = stack.Pop();
                        stack.Push(FixPoint.Cos(var1));
                        break;
                    case OperationCode.TAN:
                        var1 = stack.Pop();
                        stack.Push(FixPoint.Tan(var1));
                        break;
                    case OperationCode.SQRT:
                        var1 = stack.Pop();
                        stack.Push(FixPoint.Sqrt(var1));
                        break;
                    case OperationCode.MIN:
                        var2 = stack.Pop();
                        var1 = stack.Pop();
                        stack.Push(FixPoint.Min(var1, var2));
                        break;
                    case OperationCode.MAX:
                        var2 = stack.Pop();
                        var1 = stack.Pop();
                        stack.Push(FixPoint.Max(var1, var2));
                        break;
                    case OperationCode.CLAMP:
                        var3 = stack.Pop();
                        var2 = stack.Pop();
                        var1 = stack.Pop();
                        stack.Push(FixPoint.Clamp(var1, var2, var3));
                        break;
                    default:
                        break;
                }
            }

            if (stack.Count > 0)
                return stack.Pop();
            else
                return FixPoint.Zero;
        }

        public bool IsConst()
        {
            return m_program == null;
        }

    }
}