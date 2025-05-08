//#define USE_CRC_KEY 

using System.Collections;
using System.Collections.Generic;


namespace CoreGame.DSL
{
    public enum VariableType
    {
        Int,
        Float,
        FixPoint,
    }


    public class Variable : IRecyclable
    {
#if USE_CRC_KEY
        public List<int> m_variable = new List<int>();
#else
        public List<string> m_variable = new List<string>();
#endif

        public VariableType m_varType = VariableType.FixPoint;

        public void Construct(List<string> raw_variable)
        {
            int count = raw_variable.Count;
            for (int i = 0; i < count; ++i)
            {
                var str = raw_variable[i];
                if (count == 2 && i == 0)
                {
                    if (str == "int")
                        m_varType = VariableType.Int;
                    else if (str == "float")
                        m_varType = VariableType.Float;
                    continue;
                }

#if USE_CRC_KEY
                m_variable.Add((int)CRC.Calculate(raw_variable[i]));
#else
                m_variable.Add(raw_variable[i]);
#endif
            }
        }

        public void Reset()
        {
            m_variable.Clear();
            m_varType = VariableType.FixPoint;
        }

        public void CopyFrom(Variable rhs)
        {
            m_variable.Clear();
            for (int i = 0; i < rhs.m_variable.Count; ++i)
                m_variable.Add(rhs.m_variable[i]);
        }

#if USE_CRC_KEY
        public int this[int index]
#else
        public string this[int index]
#endif
        {
            get
            {
                if (index >= 0 && index < m_variable.Count)
                    return m_variable[index];
#if USE_CRC_KEY
                return 0;
#else
                return "";
#endif

            }
        }

        public int MaxIndex
        {
            get { return m_variable.Count - 1; }
        }
    }

}