/*

using System.Collections;
using System.Collections.Generic;

namespace CoreGame.Custom
{
    //////////////////////////////////////////////////////////////////////////
    public struct FixedIntCfg : IValueConfig<int>
    {
        int mFixedValue;
        public int Value
        {
            get { return mFixedValue; }
        }

        public static bool CanParse(string s)
        {
            int v;
            return int.TryParse(s, out v);
        }

        public bool Parse(string s)
        {
            if (int.TryParse(s, out mFixedValue))
            {
                return true;
            }
            LogWrapper.LogError("FixedIntCfg Prase Error : " + s);
            return false;
        }
    }

    //////////////////////////////////////////////////////////////////////////
    public struct FixedFloatCfg : IValueConfig<float>
    {
        float mFixedValue;
        public float Value
        {
            get { return mFixedValue; }
        }

        public static bool CanParse(string s)
        {
            float v;
            return float.TryParse(s, out v);
        }

        public bool Parse(string s)
        {
            if (float.TryParse(s, out mFixedValue))
            {
                return true;
            }
            LogWrapper.LogError("FixedFloatCfg Prase Error : " + s);
            return false;
        }
    }
}
*/
