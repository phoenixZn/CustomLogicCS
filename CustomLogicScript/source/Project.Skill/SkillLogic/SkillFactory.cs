using CoreGame.Custom;

namespace CoreGame
{
    // public class SkillFactory : CustomLogicFactory
    // {
    //     //单例
    //     private static SkillFactory sInstance = null;
    //
    //     public static SkillFactory Instance()
    //     {
    //         if (sInstance == null)
    //         {
    //             sInstance = new SkillFactory();
    //         }
    //         return sInstance;
    //     }
    //
    //     private SkillFactory()
    //     {
    //         InitConfigMng();
    //     }
    //
    //     public void InitConfigMng()
    //     {
    //         if (MConfigContainer == null)
    //         {
    //             MConfigContainer = new LogicConfigContainer();
    //             MConfigContainer.ReadXml("Config/ClientConfig/SkillLogicConfig");
    //             MConfigContainer.ReadXml("Config/ClientConfig/SkillLogicConfig_Pet");
    //             MConfigContainer.ReadXml("Config/ClientConfig/SkillLogicConfig_Monster");
    //         }
    //     }
    //
    //     public Skill CreateSkill(SkillGenInfo genInfo)
    //     {
    //         return CreateCustomLogic(genInfo) as Skill;
    //     }
    //
    //     public override void DoCache()
    //     {
    //         mObjectPool.Cache<CustomLogic>(40);
    //     }
    // }
}