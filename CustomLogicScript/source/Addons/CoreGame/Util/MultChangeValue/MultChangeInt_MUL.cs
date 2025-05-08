using System.Collections;
using System.Collections.Generic;

public class MultChangeInt_MUL : MultChangeValue<int>
{
	public MultChangeInt_MUL()
    {
        mCurValue = mBaseValue = 1;
    }

    public MultChangeInt_MUL(int baseValue)
    {
        mCurValue = mBaseValue = baseValue;
        AddChange(baseValue);
    }

    protected override void CalcuCurValue()
    {
        mCurValue = 1;
        foreach (var v in mValueChangeList)
        {
            mCurValue *= v.Value;
        }
    }
}
