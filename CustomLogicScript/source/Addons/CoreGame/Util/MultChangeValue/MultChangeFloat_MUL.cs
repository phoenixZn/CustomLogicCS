using System.Collections;
using System.Collections.Generic;

public class MultChangeFloat_MUL : MultChangeValue<float>
{
	public MultChangeFloat_MUL()
    {
        mCurValue = mBaseValue = 1f;
    }

    public MultChangeFloat_MUL(float baseValue)
    {
        mCurValue = mBaseValue = baseValue;
        AddChange(baseValue);
    }

    protected override void CalcuCurValue()
    {
        mCurValue = 1f;
        foreach (var v in mValueChangeList)
        {
            mCurValue *= v.Value;
        }
    }
}
