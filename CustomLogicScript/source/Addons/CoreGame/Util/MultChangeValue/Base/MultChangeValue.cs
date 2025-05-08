using System.Collections.Generic;

//////////////////////////////////////////////////////////////////////////
//可能同时被多个独立功能模块修改的值, 会记录多个改动，根据定义的计算策略得出当前值
public abstract class MultChangeValue<T>
{
    //标记数值是哪一个功能模块修改的
    protected struct FlagedValue
    {
        public FlagedValue(int flag, T value)
        {
            Flag = flag;
            Value = value;
        }

        public int Flag;
        public T Value;
    }

    protected List<FlagedValue> mValueChangeList = new List<FlagedValue>();
    protected T mBaseValue;
    protected T mCurValue;

    public MultChangeValue()
    {
        mCurValue = mBaseValue;
    }

    public MultChangeValue(T baseValue)
    {
        mCurValue = mBaseValue = baseValue;
    }

    public T BaseValue
    {
        get { return mBaseValue; }
        set { mBaseValue = value; }
    }

    public T CurValue { get { return mCurValue; } }

    public void AddChange(T value, int flag = 0)
    {
        RemoveChange(flag);
        mValueChangeList.Add(new FlagedValue(flag, value));
        CalcuCurValue();
    }

    public void RemoveChange(int flag = 0)
    {
        for (int i = mValueChangeList.Count - 1; i >= 0; --i)
        {
            var item = mValueChangeList[i];
            if (item.Flag == flag)
            {
                mValueChangeList.RemoveAt(i);
                break;
            }
        }
        CalcuCurValue();
    }

    public void Clear()
    {
        mValueChangeList.Clear();
        CalcuCurValue();
    }

    protected abstract void CalcuCurValue();

    public static implicit operator T(MultChangeValue<T> n)
    {
        if (n == null)
            return default(T);
        return n.CurValue;
    }
}