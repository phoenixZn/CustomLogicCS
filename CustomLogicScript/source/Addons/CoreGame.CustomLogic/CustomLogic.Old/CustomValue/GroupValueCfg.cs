using System;
using System.Collections.Generic;

/*
namespace CoreGame.Custom
{
    public abstract class GroupValueCfg<T> : IValueConfig<T>
    {
        protected T mValue;
        protected List<IValueConfig<T>> mCfgList = new List<IValueConfig<T>>();
        protected IValueConfig<T> mCurCfg = null;


        public static implicit operator T(GroupValueCfg<T> cv)
        {
            return cv.Value;
        }

        protected abstract void InitCfg();

        protected bool HasCfg()
        {
            return mCfgList != null && mCfgList.Count != 0;
        }

        public void AddCfg(IValueConfig<T> cfg)
        {
            mCfgList.Add(cfg);
        }

        /// IValueConfig<T>
        public T Value
        {
            get
            {
                if (mCurCfg != null)
                {
                    mValue = mCurCfg.Value;
                }
                return mValue;
            }
            set
            {
                mValue = value;
            }
        }

        public bool Parse(string s)
        {
            if (!HasCfg())
                InitCfg();
            for (int i = 0; mCfgList != null && i < mCfgList.Count; ++i)
            {
                if (!mCfgList[i].CanParse(s))
                    continue;
                bool success = mCfgList[i].Parse(s);
                if (success)
                {
                    mCurCfg = mCfgList[i];
                }
            }
            return false;
        }

        public static bool CanParse(string s) { return true; }

        public abstract IValueConfig<T> Copy();
    }
}
*/