
namespace CoreGame.Custom
{

    //////////////////////////////////////////////////////////////////////////
    //能在有限固定时间内结束的行为
    //////////////////////////////////////////////////////////////////////////
    public abstract class FiniteTimeBhv : BehaviorNodeBase, INeedStopCheck
    {
        //行为的持续时间
        private float mDuration = 0;
        private bool mIsEnd = false;

        public virtual bool IsDurationEnd()
        {
            //这里不用 <= 0, 保证会运行一帧
            return mDuration < 0;         
        }   

        public float GetDuration() 
        {
            if (mDuration < 0)
                return 0;
            return mDuration; 
        }

        protected void InitDuration(float duration) 
        { 
            mDuration = duration;
        }

        protected virtual void OnDurationEnd() { }

        //////////////////////////////////////////////////////////////////////////
        // IBehavior
        public override void Reset()
        {
            base.Reset();
            mDuration = 0;
            mIsEnd = false;
        } 

        public override bool Update(float dt)
        {
            if (!IsDurationEnd())
            {
                if (mDuration > 0 && dt > mDuration)
                    base.Update(mDuration);
                else
                    base.Update(dt);

                mDuration -= dt;
            }

            if (IsDurationEnd() && !mIsEnd)
            {
                mIsEnd = true;
                OnDurationEnd();
            }
            return true;
        }

        public override void Destroy()
        {
            base.Destroy();
            mDuration = 0;
            mIsEnd = false;
        }

        //////////////////////////////////////////////////////////////////////////
        // INeedStopCheck
        public virtual bool CanStop()
        {
            return mIsEnd;
        }
    }

}