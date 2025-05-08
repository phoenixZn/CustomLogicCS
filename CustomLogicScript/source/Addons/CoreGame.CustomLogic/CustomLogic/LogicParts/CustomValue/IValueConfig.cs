using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreGame.Custom
{
    public interface IValueConfig<T>
    {
        T Value { get; }
        bool Parse(string s);
    }

}
