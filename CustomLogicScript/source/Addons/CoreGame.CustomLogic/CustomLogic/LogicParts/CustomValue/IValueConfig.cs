using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreGame.Custom
{
    public interface IValueConfig<T>
    {
        T GetValue();
        bool ParseByFormatString(string s);
    }
    
}
