/*
using System.Collections.Generic;
using System.Xml;

namespace CoreGame.Custom
{
    //状态ID是枚举类型的 FSM 配置
    public class EnumTypeFSMCfg<TStateEnum> : FSMNodeCfg where TStateEnum : struct
    {

        public override bool ParseFromXml(XmlNode xmlNode)
        {
            string str = XmlHelper.GetAttribute(xmlNode, "DefaultState");
            TStateEnum stateID;
            CLHelper.Assert(XmlHelper.StrToEnumValue<TStateEnum>(str, out stateID));
            DefaultState = stateID.GetHashCode();

            XmlNodeList subNodeList = xmlNode.SelectNodes("State");
            if (subNodeList == null)
                return false;

            StateList = new List<ICustomNodeCfg>();
            foreach (XmlNode subNode in subNodeList)
            {
                ICustomNodeCfg cfg = CustomLogicConfigMng.CreateNodeCfg(subNode);

                //这里篡改了StateNodeCfg默认的配置方式， 使用StateEnum属性来配置 StateID 、StateName
                var stateCfg = cfg as StateNodeCfg;
                if (stateCfg != null)
                {
                    XmlElement cusNode = subNode as XmlElement;
                    string strStateType = cusNode.GetAttribute("StateEnum");
                    if (!string.IsNullOrEmpty(strStateType))
                    {
                        CLHelper.Assert(XmlHelper.StrToEnumValue<TStateEnum>(strStateType, out stateID));
                        stateCfg.StateID = stateID.GetHashCode();
                        stateCfg.StateName = stateID.ToString();
                    }
                }

                StateList.Add(cfg);
            }
            if (this.StateList.Count == 0)
            {
                LogWrapper.LogError("EnumTypeFSMCfg.ParseFromXml() CndCfgList.Count == 0");
                CLHelper.AssertBreak();
                return false;
            }

            return true;
        }
    }


    //public static partial class NodeConfigTypeRegistry
    //{
    //    private static bool _ActorFSMCfg = Register(typeof(ActorFSMCfg), NodeCategory.Bhv);
    //}
    //public class ActorFSMCfg : EnumTypeFSMCfg<ActorStateID>, ICustomNodeXmlCfg
    //{
    //    public override System.Type NodeType()
    //    {
    //        return typeof(FSMNode);
    //    }
    //}
}
*/
