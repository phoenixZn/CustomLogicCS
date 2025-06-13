namespace CoreGame.Custom
{
    public abstract class BehaviorNodeBase : CustomNode, INeedUpdate//IBehavior
    {
        //需要知道第一次Update的行为
        protected bool mHasUpdate = false;

        protected virtual void OnBegin()
        {
        }

        //返回消耗后剩余的时间片
        protected virtual float OnUpdate(float dt)
        {
            return dt;
        }

        //////////////////////////////////////////////////////////////////////////
        // IBehavior
        public override void Reset()
        {
            //运行前，内部状态的初始化放在这里。（主要用于可以重复多次执行的Behavior）
            mHasUpdate = false;
        }

        public virtual float Update(float dt)
        {
            if (!mHasUpdate)
            {
                mHasUpdate = true;
                OnBegin();
            }
            return OnUpdate(dt);
        }

        //////////////////////////////////////////////////////////////////////////
        // CustomNode
        public override void Destroy()
        {
            mHasUpdate = false;
        }

    }
}