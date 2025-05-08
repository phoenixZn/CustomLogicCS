using System.Collections;
using System.Collections.Generic;

public class MultChangeInt_ADD : MultChangeValue<int>
{

	public MultChangeInt_ADD()
    {
        mCurValue = mBaseValue = 0;
    }

    public MultChangeInt_ADD(int baseValue)
    {
        mCurValue = mBaseValue = baseValue;
        AddChange(baseValue);
    }

    protected override void CalcuCurValue()
    {
        mCurValue = 0;
        foreach (var v in mValueChangeList)
        {
            mCurValue += v.Value;
        }
    }
}
