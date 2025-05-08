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

    public struct SkillGenInfo : ICustomLogicGenInfo
    {
        public int ConfigID
        {
            get { return SkillID; }
            set { SkillID = value; }
        }

        public int SkillID;
        public int SkillLevel;
        public int OwnerID;

        public void Clear()
        {
            SkillID = -1;
            SkillLevel = -1;
            OwnerID = -1;
        }
    }
}