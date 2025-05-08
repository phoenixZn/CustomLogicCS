namespace CoreGame.Custom
{
    public abstract class HasBeginBhv : CustomNode, IBehavior
    {
        //需要知道第一次Update的行为
        private bool mHasUpdate = false;

        protected virtual void OnBegin()
        {
        }

        protected virtual void OnUpdate(float dt)
        {
        }

        //////////////////////////////////////////////////////////////////////////
        // IBehavior
        public virtual void Reset()
        {
            //运行前，内部状态的初始化放在这里。（主要用于可以重复多次执行的Behavior）
            mHasUpdate = false;
        }

        public virtual bool Update(float dt)
        {
            if (!mHasUpdate)
            {
                mHasUpdate = true;
                OnBegin();
            }
            OnUpdate(dt);
            return true;
        }

        //////////////////////////////////////////////////////////////////////////
        // CustomNode
        public override void Destroy()
        {
            mHasUpdate = false;
        }

    }
}