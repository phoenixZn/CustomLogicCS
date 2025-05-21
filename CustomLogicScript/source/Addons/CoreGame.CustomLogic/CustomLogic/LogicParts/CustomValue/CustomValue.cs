// using System.Collections;
// using System.Collections.Generic;
//
//
// namespace CoreGame.Custom
// {
//
//     public abstract class CustomValue<T> : IValueConfig<T>
//     {
//         protected T m_defaultValue;
//         protected IValueConfig<T> m_valueCfg;
//
//         public static implicit operator T(CustomValue<T> cv)
//         {
//             return cv.GetValue();
//         }
//
//         public T GetValue()
//         {
//             if (m_valueCfg != null)
//             {
//                 return m_valueCfg.GetValue();
//             }
//             return m_defaultValue;
//         }
//
//         public abstract bool ParseByFormatString(string s);
//         
//     }
//     
//     
//     //////////////////////////////////////////////////////////////////////////
//     // int
//     //////////////////////////////////////////////////////////////////////////
//     public class CustomInt : CustomValue<int>
//     {
//         public override bool ParseByFormatString(string s)
//         {
//             if (VarIntCfg.CheckFormatString(s))
//             {
//                 m_valueCfg = new VarIntCfg();
//                 m_valueCfg.ParseByFormatString(s);
//             }
//             else if (RandIntCfg.CanParse(s))
//             {
//                 m_valueCfg = new RandIntCfg();
//                 m_valueCfg.ParseByFormatString(s);
//             }
//
//             if (int.TryParse(s, out m_defaultValue))
//             {
//                 return true;
//             }
//             LogWrapper.LogError("CustomInt defaultValue Prase Error : " + s);
//             return false;
//         }
//     }
//
//     //////////////////////////////////////////////////////////////////////////
//     // float
//     //////////////////////////////////////////////////////////////////////////
//     public class CustomFloat : CustomValue<float>
//     {
//         public override bool ParseByFormatString(string s)
//         {
//             if (VarFloatCfg.CheckFormatString(s))
//             {
//                 m_valueCfg = new VarFloatCfg();
//                 m_valueCfg.ParseByFormatString(s);
//             }
//             else if (RandFloatCfg.CanParse(s))
//             {
//                 m_valueCfg = new RandFloatCfg();
//                 m_valueCfg.ParseByFormatString(s);
//             }
//
//             if (float.TryParse(s, out m_defaultValue))
//             {
//                 return true;
//             }
//             LogWrapper.LogError("CustomFloat defaultValue Prase Error : " + s);
//             return false;
//         }
//     }
//
//
// }
