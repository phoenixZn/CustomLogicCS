/********************************************************************
	created:	2015/10/29
	purpose:	*  CustomLogicGenInfo 外部使用方 构造自定义逻辑的初始数据
                * 创建CustomLogic的运行时初始化信息
                * 接口类，多个项目使用，不要直接修改
                * 业务扩展可以新加一个 CustomLogicGenInfo 的继承类
	change list:
*********************************************************************/


using System.Collections;
using System.Collections.Generic;

namespace CoreGame.Custom
{
    //运行时初始化信息（不应被修改）
    public class ICustomLogicGenInfo
    {
        //逻辑配置ID
        protected int mLogicConfigID = 0;
        //初始数据黑板
        protected KVBlackBoard mPreBlackboard = null;
        //配置组名
        protected string mConfigGroupName;
        
        public int LogicConfigID
        {
            get { return mLogicConfigID; }
            set { mLogicConfigID = value; }
        }
        
        public KVBlackBoard PreBlackboard        
        {
            get { return mPreBlackboard; }
            set { mPreBlackboard = value; }
        }
        
        public string ConfigGroupName        
        {
            get { return mConfigGroupName; }
            set { mConfigGroupName = value; }
        }

    }


}