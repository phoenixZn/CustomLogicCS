using System.Collections;
using System.Collections.Generic;


namespace CoreGame.Custom
{
    //////////////////////////////////////////////////////////////////////////
    // int
    //////////////////////////////////////////////////////////////////////////
    public class CustomInt : CustomValue<int>
    {
        public override bool Parse(string s)
        {
            if (VarIntCfg.CanParse(s))
            {
                m_valueCfg = new VarIntCfg();
                m_valueCfg.Parse(s);
            }
            else if (RandIntCfg.CanParse(s))
            {
                m_valueCfg = new RandIntCfg();
                m_valueCfg.Parse(s);
            }

            if (int.TryParse(s, out m_defaultValue))
            {
                return true;
            }
            LogWrapper.LogError("CustomInt defaultValue Prase Error : " + s);
            return false;
        }
    }

    //////////////////////////////////////////////////////////////////////////
    // float
    //////////////////////////////////////////////////////////////////////////
    public class CustomFloat : CustomValue<float>
    {
        public override bool Parse(string s)
        {
            if (VarFloatCfg.CanParse(s))
            {
                m_valueCfg = new VarFloatCfg();
                m_valueCfg.Parse(s);
            }
            else if (RandFloatCfg.CanParse(s))
            {
                m_valueCfg = new RandFloatCfg();
                m_valueCfg.Parse(s);
            }

            if (float.TryParse(s, out m_defaultValue))
            {
                return true;
            }
            LogWrapper.LogError("CustomFloat defaultValue Prase Error : " + s);
            return false;
        }
    }



    public abstract class CustomValue<T> : IUseVariablesLib
    {
        protected T m_defaultValue;
        protected IValueConfig<T> m_valueCfg;

        public static implicit operator T(CustomValue<T> cv)
        {
            return cv.Value;
        }

        public T Value
        {
            get
            {
                if (m_valueCfg != null)
                {
                    return m_valueCfg.Value;
                }
                return m_defaultValue;
            }
            set
            {
                m_defaultValue = value;
            }
        }

        public abstract bool Parse(string s);

        public void BindVariablesLib(VariablesLib varLib)
        {
            if (m_valueCfg == null)
                return;
            var varLibUser = m_valueCfg as IUseVariablesLib;
            if (varLibUser != null)
                varLibUser.BindVariablesLib(varLib);
        }

        public void UnBindVariablesLib()
        {
            if (m_valueCfg == null)
                return;
            var varLibUser = m_valueCfg as IUseVariablesLib;
            if (varLibUser != null)
                varLibUser.UnBindVariablesLib();
        }
    }
}
