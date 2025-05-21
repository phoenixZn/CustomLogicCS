
using System.Collections;
using System.Collections.Generic;

namespace CoreGame.Custom
{
    public abstract class FormatValueCfg<T> : IValueConfig<T>
    {
        protected string mVarID = null;
        protected T mDefaultValue;
        
        public FormatValueCfg(T defaultValue)
        {
            mDefaultValue = defaultValue;
        }
        
        //////////////////////////////////////////////////////////////////////////
        //as: IValueConfig<T>
        public T GetValue()
        {
            return mDefaultValue;
        }

        public T GetValue(CustomNode node)
        {
            VariablesLib varLib = node.VariablesLibRef;
            if (varLib != null && !string.IsNullOrEmpty(mVarID))
            {
                if (varLib.ReadVar<T>(mVarID, out var ret))
                    return ret;
            }
            return mDefaultValue;
        }

        public abstract bool ParseByString(string str);

        public bool ParseByFormatString(string str)
        {
            if (str.StartsWith("BB#"))
            {
                mVarID = str.Substring(3);
            }
            return ParseByString(str);
        }
        
    }


    //////////////////////////////////////////////////////////////////////////
    public class IntCfg : FormatValueCfg<int>
    {
        public IntCfg(int defaultValue) : base(defaultValue)
        {
        }

        public override bool ParseByString(string str)
        {
             if (int.TryParse(str, out mDefaultValue))
             {
                 return true;
             }
             return false;
        }
    }


    //////////////////////////////////////////////////////////////////////////
    public class FloatCfg : FormatValueCfg<float>
    {
        public FloatCfg(float defaultValue) : base(defaultValue)
        {
        }
        
        public override bool ParseByString(string str)
        {
            if (float.TryParse(str, out mDefaultValue))
            {
                return true;
            }
            return false;
        }
    }
    
    //////////////////////////////////////////////////////////////////////////
    public class StringCfg : FormatValueCfg<string>
    {
        public StringCfg(string defaultValue) : base(defaultValue)
        {
        }
        
        public override bool ParseByString(string str)
        {
            mDefaultValue = str;
            return true;
        }
    }
}
