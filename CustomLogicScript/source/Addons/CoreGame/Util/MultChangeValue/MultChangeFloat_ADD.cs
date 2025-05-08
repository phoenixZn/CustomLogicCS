using System.Collections;
using System.Collections.Generic;

public class MultChangeFloat_ADD : MultChangeValue<float>
{

	public MultChangeFloat_ADD()
    {
        mCurValue = mBaseValue = 0f;
    }

    public MultChangeFloat_ADD(float baseValue)
    {
        mCurValue = mBaseValue = baseValue;
        AddChange(baseValue);
    }

    protected override void CalcuCurValue()
    {
        mCurValue = 0f;
        foreach (var v in mValueChangeList)
        {
            mCurValue += v.Value;
        }
    }
}
