// using System.Collections;
// using System.Collections.Generic;
//
// namespace CoreGame.Custom
// {
//     public interface INeedReset
//     {
//         void Reset();
//     }
//     //////////////////////////////////////////////////////////////////////////
//     // 支持xml中数值可以使用， 类似 timeLen = "Rand[ 1.5 ~ 3.0 ]" 的方便随机数配置
//     //////////////////////////////////////////////////////////////////////////
//     public abstract class RandValueCfg<T> : IValueConfig<T>, INeedReset
//     {
//         T mRandValue;
//         bool mHasSetRand;
//         T mValueMin;
//         T mValueMax;
//
//         public T GetValue()
//         {
//             if (!mHasSetRand)
//             {
//                 mRandValue = GetRnad(mValueMin, mValueMax);
//                 mHasSetRand = true;
//             }
//             return mRandValue;
//         }
//
//
//         public static bool CanParse(string s) 
//         {
//             return s.StartsWith("Rand[");
//         }
//
//         public bool ParseByFormatString(string s)
//         {
//             Reset();
//             string[] randInfo = s.Trim(" Rand[]".ToCharArray()).Split('~');
//             if (randInfo.Length == 2)
//             {
//                 if (!SetValue(randInfo[0], out mValueMin))
//                     return false;
//                 if (!SetValue(randInfo[1], out mValueMax))
//                     return false;
//
//                 return true;
//             }
//             LogWrapper.LogError("RandValueCfg Prase Error : " + s);
//             return false;
//         }
//
//         public void Reset()
//         {
//             mHasSetRand = false;
//         }
//
//         protected abstract bool SetValue(string vstr, out T value);
//
//         public abstract T GetRnad(T min, T max);
//     }
//
//     //////////////////////////////////////////////////////////////////////////
//     public class RandIntCfg : RandValueCfg<int>
//     {
//         protected override bool SetValue(string vstr, out int value)
//         {
//             if (int.TryParse(vstr, out value))
//                 return true;
//             return false;
//         }
//
//         public override int GetRnad(int min, int max)
//         {
//             return UnityEngine.Random.Range(min, max);
//         }
//     }
//
//     //////////////////////////////////////////////////////////////////////////
//     public class RandFloatCfg : RandValueCfg<float>
//     {
//         protected override bool SetValue(string vstr, out float value)
//         {
//             if (float.TryParse(vstr, out value))
//                 return true;
//             return false;
//         }
//
//         public override float GetRnad(float min, float max)
//         {
//             return UnityEngine.Random.Range(min, max);
//         }
//     }
//
//
//
// }
