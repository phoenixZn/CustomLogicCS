using System.Collections.Generic;
using CoreGame.DSL;

namespace CoreGame.Custom
{
    public interface IVariables
    {
        void Clear();
    }

    public class VariablesImp<T> : IVariables
    {
        protected Dictionary<string, T> mVarDic = new Dictionary<string, T>();

        public void WriteVar(string ID, T value)
        {
            mVarDic[ID] = value;
        }

        public bool ReadVar(string ID, out T getV)
        {
            if (mVarDic.ContainsKey(ID))
            {
                getV = mVarDic[ID];
                return true;
            }
            getV = default(T);
            return false;
        }

        public bool HasVar(string ID)
        {
            return mVarDic.ContainsKey(ID);
        }

        public void Clear()
        {
            mVarDic.Clear();
        }
    }

    //Variables Envionment
    public class VarEnv : IVariableEnv
    {
        private Dictionary<System.Type, IVariables> mVarTypeDic;

        public VarEnv()
        {
            mVarTypeDic = new Dictionary<System.Type, IVariables>();
        }

        public void AddVarType<T>(System.Type type)
        {
            if (!mVarTypeDic.ContainsKey(type))
                mVarTypeDic.Add(type, new VariablesImp<T>());
        }

        private VariablesImp<T> GetVariables<T>()
        {
            var type = typeof(T);
            if (mVarTypeDic.ContainsKey(type))
            {
                return mVarTypeDic[type] as VariablesImp<T>;
            }
            DSLHelper.LogError("GetVarType<T> == null  " + typeof(T));
            return null;
        }

        public bool ReadVar<T>(string key, out T value)
        {
            var variables = GetVariables<T>();
            if (variables != null && variables.ReadVar(key, out value))
            {
                return true;
            }
            DSLHelper.LogError("ReadVar<T> is null, " + typeof(T) + ", key=" + key);
            value = default(T);
            return false;
        }

        public void WriteVar<T>(string key, T value)
        {
            VariablesImp<T> variables = GetVariables<T>();
            if (variables == null)
            {
                variables = new VariablesImp<T>();
                mVarTypeDic.Add(typeof(T), variables);
            }
            variables.WriteVar(key, value);
        }

        public bool HasVar<T>(string key)
        {
            var varType = GetVariables<T>();
            if (varType != null)
            {
                return varType.HasVar(key);
            }
            return false;
        }

        public void Clear()
        {
            foreach (var v in mVarTypeDic.Values)
            {
                v.Clear();
            }
        }

        /////////////////////////////////////////////////////////////////////////////
        //IVariableProvider
        public FixPoint GetFixPoint(Variable variable)
        {
            FixPoint fpv;
            if (ReadVar<FixPoint>(variable[0], out fpv))
                return fpv;
            float fv;
            if (ReadVar<float>(variable[0], out fv))
                return FixPoint.CreateFromFloat(fv);
            int iv;
            if (ReadVar<int>(variable[0], out iv))
                return (FixPoint)iv;
            return FixPoint.Zero;
        }

        public bool SetVariable<T>(Variable variable, T value)
        {
            if (variable.MaxIndex == 0)
            {
                WriteVar<T>(variable[0], value);
                return true;
            }
            return false;
        }
    }
}