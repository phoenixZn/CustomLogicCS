using System.Collections;
using System.Collections.Generic;


namespace CoreGame.DSL
{

    public interface IVariableEnv
    {
        FixPoint GetFixPoint(Variable variable);
        bool SetVariable<T>(Variable variable, T value);
    }
}