using CoreGame.Custom;

namespace CoreGame
{
    public interface IHasOwnerEntity
    {
        Entity OwnerEntity { get; }
    }
    public interface IHasSourceEntity
    {
        Entity SourceEntity { get; }
    }

    public interface ISkill : IHasOwnerEntity, IForceEnd
    {
        SkillGenInfo SkillGenInfo { get; }
        void UpdateSkill(float dt);
    }

    public class SkillGenInfo : ICustomLogicGenInfo
    {
        public int SkillID => LogicConfigID;
        public int SkillLevel;
        public int OwnerID;

        public void Clear()
        {
            SkillLevel = -1;
            OwnerID = -1;
        }
    }
}