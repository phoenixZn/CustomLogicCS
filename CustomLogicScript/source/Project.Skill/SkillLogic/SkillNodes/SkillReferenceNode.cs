using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace CoreGame.Custom
{
    public static partial class NodeConfigTypeRegistry
    {
        static bool _SkillReferenceNodeCfg = Register(typeof(LogicReferenceCfg), NodeCategory.Mixture);
    }

    public class SkillReferenceCfg : LogicReferenceCfg
    {
        public override System.Type NodeType() { return typeof(SkillReference); }
    }

    //////////////////////////////////////////////////////////////////////////
    // Skill引用节点
    //////////////////////////////////////////////////////////////////////////
    public class SkillReference : Skill
    {

        public override void InitializeNode(ICustomNodeCfg cfg, CustomNodeContext context)
        {
            LogicReference.SwitchReferenceCfg(this, ref cfg, ref context);
            base.InitializeNode(cfg, context);
        }

    }
}