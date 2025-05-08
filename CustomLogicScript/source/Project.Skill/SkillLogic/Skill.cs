using System.Collections.Generic;
using System.Linq;

namespace CoreGame.Custom
{
    public static partial class NodeConfigTypeRegistry
    {
        private static bool _SkillLogicConfig = Register(typeof(SkillCfg), NodeCategory.Mixture);
    }

    public class SkillCfg : CustomLogicCfg
    {
        public override System.Type NodeType()
        {
            return typeof(Skill);
        }
    }

    //////////////////////////////////////////////////////////////////////////
    // 自定义技能
    //////////////////////////////////////////////////////////////////////////
    public partial class Skill : CustomLogic, ISkill, ISkillInterface
    {
        protected SkillGenInfo m_skillGenInfo;
        protected bool m_forceEnd = false;
        protected GameEntity m_ownerEntity = null;

        public override void InitializeNode(ICustomNodeCfg cfg, CustomNodeContext context)
        {
            PreInitializeNode(context);
            base.InitializeNode(cfg, context);

            m_forceEnd = false;
        }

        private void PreInitializeNode(CustomNodeContext context)
        {
            //-------------前置初始化信息---------------
            m_skillGenInfo = (SkillGenInfo)context.GenInfo;
            m_ownerEntity = OwnerEntity as GameEntity;
        }


        public override void Destroy()
        {
            m_ownerEntity = null;
            m_skillGenInfo.Clear();
            m_forceEnd = false;

            base.Destroy();
        }

        //////////////////////////////////////////////////////////////////////////
        //IForceEnd
        public void ForceEnd()
        {
            m_forceEnd = true;
        }

        //////////////////////////////////////////////////////////////////////////
        //IHasOwnerEntity
        public Entity OwnerEntity
        {
            get
            {
                return m_ownerEntity;
            }
        }

        //////////////////////////////////////////////////////////////////////////
        //ISkill
        public SkillGenInfo SkillGenInfo
        {
            get
            {
                return m_skillGenInfo;
            }
        }

        public override bool CanStop()
        {
            if (m_forceEnd)
                return true;
            return base.CanStop();
        }

        public void UpdateSkill(float dt)
        {
            Update(dt);
        }

    }
}

