using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreGame.Custom
{
    using System.Collections.Generic;

    public interface ICanRecycle
    {
        void Destroy();
    }

    public class ObjectPool<T> where T : ICanRecycle
    {
        private Dictionary<System.Type, List<T>> m_pool = new Dictionary<System.Type, List<T>>();

        public void Clear()
        {
            m_pool.Clear();
        }

        public void Cache<CT>(int count = 1) where CT : T, new()
        {
            System.Type type = typeof(CT);

            if (!m_pool.ContainsKey(type))
            {
                m_pool.Add(type, new List<T>());
            }

            for (int i = 0; i < count; i++)
            {
                m_pool[type].Add(new CT());
            }
        }

        public CT Create<CT>() where CT : T, new()
        {
            //#if UNITY_EDITOR
            //            if (GameGlobal.Instance.EditorMode)
            //            {
            //                return new CT();
            //            }
            //#endif
            System.Type type = typeof(CT);
            CT res;

            if (!m_pool.ContainsKey(type))
            {
                m_pool.Add(type, new List<T>());
            }
            if (m_pool[type].Count > 0)
            {
                res = (CT)m_pool[type][0];
                m_pool[type].RemoveAt(0);//
                //m_pool[type].Clear();
            }
            else
            {
                res = new CT();
            }

            return res;
        }

        public CT Create<CT>(System.Type type) where CT : class, T, new()
        {
            //#if UNITY_EDITOR
            //            if (GameGlobal.Instance.EditorMode)
            //            {
            //                return System.Activator.CreateInstance(type) as CT;
            //            }
            //#endif
            CT res;

            if (!m_pool.ContainsKey(type))
            {
                m_pool.Add(type, new List<T>());
            }
            if (m_pool[type].Count > 0)
            {
                res = m_pool[type][0] as CT;
                m_pool[type].RemoveAt(0);
            }
            else
            {
                res = System.Activator.CreateInstance(type) as CT;
            }

            return res;
        }

        public void Destroy(T obj)
        {
            if (obj == null)
            {
                return;
            }
            //#if UNITY_EDITOR
            //            if (GameGlobal.Instance.EditorMode)
            //            {
            //                obj.Destroy();
            //                return;
            //            }
            //#endif
            System.Type type = obj.GetType();

            obj.Destroy();
            if (!m_pool.ContainsKey(type))
            {
                m_pool.Add(type, new List<T>());
            }
            if (!m_pool[type].Contains(obj)) // 避免重复回收
            {
                m_pool[type].Add(obj);
            }
        }
    }
}