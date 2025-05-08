
public class MultChangeFloat_MAX : MultChangeValue<float>
{
    public MultChangeFloat_MAX() : base() { }
    public MultChangeFloat_MAX(float baseValue)
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