using System.Collections;
using System.Collections.Generic;

namespace CoreGame.Custom
{
    //////////////////////////////////////////////////////////////////////////
    // 系统内部 运行时接口：
    //////////////////////////////////////////////////////////////////////////
    public interface IInterfaceCollector
    {
        //获取当前对象上的接口
        void CollectInterface<T>(ref List<T> interfaceList) where T : class;

        //获取所有子对象上的接口（不包含当前对象）
        void CollectInterfaceInChildren<T>(ref List<T> interfaceList) where T : class;
    }


    // 需要每帧更新
    // 不实现此接口，则节点不会按帧更新
    public interface INeedUpdate
    {
        bool Update(float dt);
    }

    // 需要检查是否执行结束
    // 不实现此接口，则节点在当前帧就会结束
    public interface INeedStopCheck
    {
        bool CanStop();
    }

    public interface ICanReset
    {
        void Reset();
    }

    public interface IForceEnd
    {
        void ForceEnd();
    }

}