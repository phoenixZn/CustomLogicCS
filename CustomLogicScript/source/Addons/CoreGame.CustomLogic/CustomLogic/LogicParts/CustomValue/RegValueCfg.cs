
using System.Collections;
using System.Collections.Generic;

namespace CoreGame.Custom
{
    public class VarValueCfg<T> : IValueConfig<T>, IUseVariablesLib
    {
        string mVarID;
        VariablesLib mLib;

        public T Value
        {
            get
            {
                if (mLib != null)
                {
                    T ret;
                    if (mLib.ReadVar<T>(mVarID, out ret))
                        return ret;
                }
                return default(T);
            }
        }

        public static bool CanParse(string s)
        {
            return s.StartsWith("RegInt[");
        }

        public bool Parse(string s)
        {
            string[] strID = s.Split(new char[] { '[', ']' });
            mVarID = strID[1];
            return false;
        }

        public void BindVariablesLib(VariablesLib varLib)
        {
            mLib = varLib;
        }
        public void UnBindVariablesLib()
        {
            mLib = null;
        }
    }


    //////////////////////////////////////////////////////////////////////////
    public class VarIntCfg : VarValueCfg<int>
    {
    }


    //////////////////////////////////////////////////////////////////////////
    public class VarFloatCfg : VarValueCfg<float>
    {
    }
}
