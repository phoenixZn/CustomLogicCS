using System.Collections;
using System.Collections.Generic;
using System;
namespace CoreGame.DSL
{
    public class DSLCode
    {
        List<Expression> m_expressions = new List<Expression>();

        public void Reset()
        {
            for (int i = 0; i < m_expressions.Count; ++i)
            {
                RecyclableObject.Recycle(m_expressions[i]);
            }
            m_expressions.Clear();
        }

        public void CopyFrom(DSLCode rhs)
        {
            if (rhs == null)
                return;

            for (int i = 0; i < rhs.m_expressions.Count; ++i)
            {
                var expression = RecyclableObject.Create<Expression>();
                expression.CopyFrom(rhs.m_expressions[i]);
                m_expressions.Add(expression);
            }
        }

        public bool Compile(string rawStr)
        {
            string[] codeStr = rawStr.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            for (int line = 0; line < codeStr.Length; ++line)
            {
                string sub = codeStr[line];
                if (string.IsNullOrWhiteSpace(sub))
                    continue;

                var expression = RecyclableObject.Create<Expression>();
                if (expression.Compile(sub))
                {
                    m_expressions.Add(expression);
                }
                else
                {
                    DSLHelper.LogError("DSL Compile ERROR  line:" + line);
                }
            }
            return true;
        }

        public void Execute(IVariableEnv variable_env)
        {
            for (int i = 0; i < m_expressions.Count; ++i)
            {
                m_expressions[i].Evaluate(variable_env);
            }
        }
    }

}
