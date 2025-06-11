
using System.Collections;
using System.Collections.Generic;

namespace CoreGame.Custom
{
    //兼容常量、黑板变量 等多种格式化配置方式
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
            VarEnv varLib = node.VarEnvRef;
            if (varLib != null && !string.IsNullOrEmpty(mVarID))
            {
                if (varLib.ReadVar<T>(mVarID, out var ret))
                    return ret;
            }
            return mDefaultValue;
        }

        //常量解析
        public abstract bool ParseByString(string str);

        //格式化解析
        public bool ParseByFormatString(string str)
        {
            //判断是否变量
            if (str.StartsWith("BB#"))
            {
                mVarID = str.Substring(3);
                return true;
            }
            //使用常量
            return ParseByString(str);
        }

        public void SetVarID(string varID)
        {
            mVarID = varID;
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
