using System.Collections;
using System.Collections.Generic;

namespace CoreGame.DSL
{
    public class ExpressionCompiler : IRecyclable
    {
        protected bool m_error_occurred = false;
        protected List<Variable> m_variables = new List<Variable>();
        protected List<long> m_instructions = new List<long>();

        #region Compile的临时变量，Reset不用关心
        protected Tokenizer m_tokenizer;
        protected Token m_token;
        protected TokenType m_token_type;
        protected List<string> m_raw_variable = new List<string>();
        #endregion

        public List<Variable> Variables { get { return m_variables; } }
        public List<long> Instructions { get { return m_instructions; } }

        public virtual void Reset()
        {
            m_error_occurred = false;
            RecycleVariable();
            m_instructions.Clear();
        }

        public void CopyFrom(ExpressionCompiler rhs)
        {
            m_error_occurred = rhs.m_error_occurred;
            RecycleVariable();
            for (int i = 0; i < rhs.m_variables.Count; ++i)
            {
                Variable variable = RecyclableObject.Create<Variable>();
                variable.CopyFrom(rhs.m_variables[i]);
                m_variables.Add(variable);
            }
            m_instructions.Clear();
            for (int i = 0; i < rhs.m_instructions.Count; ++i)
                m_instructions.Add(rhs.m_instructions[i]);
        }

        void RecycleVariable()
        {
            for (int i = 0; i < m_variables.Count; ++i)
                RecyclableObject.Recycle(m_variables[i]);
            m_variables.Clear();
        }

        public virtual bool Compile(string expression_string)
        {
            m_error_occurred = false;
            m_tokenizer = RecyclableObject.Create<Tokenizer>();
            m_tokenizer.Construct(expression_string);
            m_token = null;
            m_token_type = TokenType.ERROR;
            GetToken();
            ParseExpression();
            RecyclableObject.Recycle(m_tokenizer);
            m_tokenizer = null;
            m_token = null;
            m_token_type = TokenType.ERROR;
            m_raw_variable.Clear();
            return !m_error_occurred;
        }

        public List<Variable> GetAllVariables()
        {
            return m_variables;
        }

        public bool IsConstant()
        {
            return m_variables.Count == 0;
        }

        void GetToken()
        {
            m_token = m_tokenizer.GetNextToken();
            m_token_type = m_token.Type;
            if (m_token_type == TokenType.ERROR)
                m_error_occurred = true;
        }

        void ParseExpression()
        {
            ParseBinaryOperator(OperationSetting.MinBinaryPriority);
        }

        void ParseBinaryOperator(int symbolPriority)
        {
            ParseHighPriorityBinary(symbolPriority);

            while (OperationSetting.GetBinaryOPTokenPriority(m_token_type) == symbolPriority)
            {
                OperationCode op_code = OperationSetting.GetBinaryOperationCode(m_token_type);
                GetToken();

                ParseHighPriorityBinary(symbolPriority);

                AppendOperation(op_code);
            }
        }

        private void ParseHighPriorityBinary(int symbolPriority)
        {
            if (OperationSetting.isMaxBinaryPriority(symbolPriority))
            {
                OperationCode unary_op = OperationCode.END;
                if (m_token_type == TokenType.MINUS)
                {
                    unary_op = OperationCode.NEGATE;
                    GetToken();
                }
                else if (m_token_type == TokenType.NOT)
                {
                    unary_op = OperationCode.NOT;
                    GetToken();
                }

                ParseFactor();

                if (unary_op != OperationCode.END)
                    AppendOperation(unary_op);
            }
            else
            {
                ParseBinaryOperator(symbolPriority + 1);
            }
        }

        void ParseFactor()
        {
            switch (m_token_type)
            {
                case TokenType.NUMBER:
                    AppendNumber(m_token.GetNumber());
                    GetToken();
                    break;
                case TokenType.LEFT_PAREN:
                    GetToken();
                    ParseExpression();
                    if (m_token_type != TokenType.RIGHT_PAREN)
                    {
                        m_error_occurred = true;
                        DSLHelper.LogError("Expression: ParseFactor(), ')' expected");
                        return;
                    }
                    GetToken();
                    break;
                case TokenType.IDENTIFIER:
                    m_raw_variable.Clear();
                    m_raw_variable.Add(m_token.GetRawString());
                    GetToken();
                    //左值被赋值时可以指定类型  eg: float:Atk = BaseAtk * MultFactor;
                    if (m_token_type == TokenType.COLON)
                    {
                        GetToken();
                        if (m_token_type != TokenType.IDENTIFIER)
                        {
                            m_error_occurred = true;
                            DSLHelper.LogError("Expression: ParseFactor(), COLON TokenType.Identifier expected");
                            return;
                        }
                        m_raw_variable.Add(m_token.GetRawString());
                        GetToken();
                        SetVariable(m_raw_variable);
                        m_raw_variable.Clear();
                        break;
                    }
                    //支持 Attacker.Attr.atk 这类的调用
                    while (m_token_type == TokenType.PERIOD)
                    {
                        GetToken();
                        if (m_token_type != TokenType.IDENTIFIER)
                        {
                            m_error_occurred = true;
                            DSLHelper.LogError("Expression: ParseFactor(), TokenType.Identifier expected");
                            return;
                        }
                        m_raw_variable.Add(m_token.GetRawString());
                        GetToken();
                    }
                    AppendVariable(m_raw_variable);
                    m_raw_variable.Clear();
                    break;
                case TokenType.SINE:
                    ParseBuildInFunction(OperationCode.SIN, 1);
                    break;
                case TokenType.COSINE:
                    ParseBuildInFunction(OperationCode.COS, 1);
                    break;
                case TokenType.TANGENT:
                    ParseBuildInFunction(OperationCode.TAN, 1);
                    break;
                case TokenType.SQUARE_ROOT:
                    ParseBuildInFunction(OperationCode.SQRT, 1);
                    break;
                case TokenType.MINIMUM:
                    ParseBuildInFunction(OperationCode.MIN, 2);
                    break;
                case TokenType.MAXIMUM:
                    ParseBuildInFunction(OperationCode.MAX, 2);
                    break;
                case TokenType.CLAMP:
                    ParseBuildInFunction(OperationCode.CLAMP, 3);
                    break;
                default:
                    break;
            }
        }

        void ParseBuildInFunction(OperationCode opcode, int param_count)
        {
            GetToken();
            if (m_token_type != TokenType.LEFT_PAREN)
                DSLHelper.LogError("Expression: ParseBuildInFunction, '(' expected");
            if (param_count > 0)
            {
                GetToken();
                ParseExpression();
                --param_count;
                while (param_count > 0)
                {
                    if (m_token_type != TokenType.COMMA)
                    {
                        m_error_occurred = true;
                        DSLHelper.LogError("Expression: ParseBuildInFunction, ',' expected");
                        return;
                    }
                    GetToken();
                    ParseExpression();
                    --param_count;
                }
            }
            if (m_token_type != TokenType.RIGHT_PAREN)
            {
                m_error_occurred = true;
                DSLHelper.LogError("Expression: ParseBuildInFunction, ')' expected");
                return;
            }
            AppendOperation(opcode);
            GetToken();
        }

        void AppendOperation(OperationCode op_code)
        {
            m_instructions.Add((long)op_code);
        }
        void AppendOperation(long op_code)
        {
            m_instructions.Add(op_code);
        }

        void AppendNumber(FixPoint number)
        {
            AppendOperation(OperationCode.PUSH_NUMBER);
            m_instructions.Add(number.RawValue);
        }

        void AppendVariable(List<string> variable)
        {
            AppendOperation(OperationCode.PUSH_VARIABLE);
            int index = AddVariable(variable);
            m_instructions.Add(index);
        }

        void SetVariable(List<string> variable)
        {
            AppendOperation(OperationCode.SET_VARIABLE);
            int index = AddVariable(variable);
            m_instructions.Add(index);
        }

        int AddVariable(List<string> raw_variable)
        {
            Variable variable = RecyclableObject.Create<Variable>();
            variable.Construct(raw_variable);
            int index = m_variables.Count;
            m_variables.Add(variable);
            return index;
        }

    }
}