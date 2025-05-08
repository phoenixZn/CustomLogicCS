using System.Collections;
using System.Collections.Generic;

//////////////////////////////////////////////////////////////////////////
// Skill对象面向特殊游戏的 外部信息接口扩展
//////////////////////////////////////////////////////////////////////////

namespace CoreGame.Custom
{
    public interface ISkillInterface : ISkillButtonStatusNotify
    {
    }


    //////////////////////////////////////////////////////////////////////////
    ////当前是否禁止移动控制 （供查询）
    //public interface IActorLockMove
    //{
    //    bool IsLockMove();
    //}

    ////技能动作结束通知
    //public interface ISkillActionEndNotify
    //{
    //    void OnSkillActionEnd(int actionIndex/*当前结束的动作索引*/, int actionCount/*动作总数*/);
    //}

    //技能按钮状态通知
    public interface ISkillButtonStatusNotify
    {
        void OnSkillButtonPress(bool pressed, int buttonIndex);
    }


    //////////////////////////////////////////////////////////////////////////
    // Skill外部通知查询扩展
    public partial class Skill
    {
        //////////////////////////////////////////////////////////////////////////
        //系统外界信息的通知, 将通知传播给各个CustomNode和其子Node
        List<ISkillButtonStatusNotify> mSkillButtonNotifyList = new List<ISkillButtonStatusNotify>();

        protected override void ClearInterfaceCache()
        {
            base.ClearInterfaceCache();
            mSkillButtonNotifyList.Clear();
        }
        protected override void CacheInterface(CustomNode node)
        {
            base.CacheInterface(node);
            CustomNode.TraverseCollectInterface(ref mSkillButtonNotifyList, node);
        }

        //技能按钮状态通知
        public void OnSkillButtonPress(bool pressed, int buttonIndex)
        {
            foreach (var c in mSkillButtonNotifyList)
            {
                var node = c as ICustomNode;
                if (node != null && node.IsActive)
                {
                    c.OnSkillButtonPress(pressed, buttonIndex);
                }
            }
        }
    }
}