
public class MultChangeInt_MAX : MultChangeValue<int>
{
    public MultChangeInt_MAX() : base() { }
    public MultChangeInt_MAX(int baseValue)
        : base(baseValue)
    {
    }
    protected override void CalcuCurValue()
    {
        if (mValueChangeList.Count > 0)
        {
            var lastV = mValueChangeList[mValueChangeList.Count - 1].Value;
            if (lastV > mCurValue)
            {
                mCurValue = lastV;
            }
        }
        else
        { 
            mCurValue = mBaseValue;
        }
    }
}