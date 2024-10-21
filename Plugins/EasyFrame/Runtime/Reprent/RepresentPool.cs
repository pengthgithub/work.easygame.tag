using System;
using UnityEngine;
using UnityEngine.Profiling;

namespace Easy
{
    public partial class Represent
    {
        //====================================================================
        // 资源创建
        //====================================================================
        #region
        private static PoolUtil<Represent> _representPool;
        private static CustomPool<Represent> _maps;

        public static void PoolInit(int count = 200)
        {
            if(_maps != null) return;
            
            if (_representPool == null)
            {
                _representPool = new (count);
            }
            _maps = new ((url) =>
            {
                Represent represent = _representPool.GetPool();
                if (represent == null)
                {
                    return null;
                }
                   
#if UNITY_DEBUG
                int index = _maps.MaxCount(url);
                represent.name = $"{url}+{index}";
#endif
                represent.url = url;
                
                return represent;
            }, (url,ck) =>
            {
                if (ck)
                {
                    GameObject.DestroyImmediate(ck.gameObject);
                }
            });
#if UNITY_EDITOR
            EasyProfilerRuntime.Initialise();
#endif
        }

        public static void PoolDispose()
        {
            if (_maps != null)
            {
                _maps.Clear();
                _maps = null;
            }

            if (_representPool != null)
            {
                _representPool.UnInit();
                _representPool = null;
            }
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="url">资源路径</param>
        /// <param name="owner">拥有者</param>
        /// <param name="complate">完成事件</param>
        /// <param name="dispose">销毁事件</param>
        /// <param name="birthRepresent">出生表现</param>
        /// <param name="birthAni">出生动画</param>
        /// <returns></returns>
        public static Represent Create(
            string url, Represent owner = null,
            Action complate = null, Action dispose = null,
            string birthRepresent = "",string birthAni = "idle"
            )
        {
            Profiler.BeginSample("Represent.Create");
            if (string.IsNullOrEmpty(url)) return null;

            Represent represent = _maps.Get(url);
            if (represent == null)
            {
                Debug.LogError("represent create error url: " + url);
                return null;
            }

            represent.Init();
            if (owner)
            {
                represent.Owner = owner;
            }
            if (complate != null) represent.completeEvent = complate;
            if (dispose != null) represent.disposeEvent = dispose;
            
#if UNITY_EDITOR
            var c = _maps.MaxCount(url);
            represent.gameObject.name = url + "__" + c;
            //represent.gameObject.hideFlags = HideFlags.None;
#endif
            represent.gameObject.SetActive(true);
            Profiler.EndSample();
            
#if UNITY_EDITOR  
            EasyProfilerRuntime.ReprentCreate();
#endif
            return represent;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="represent"></param>
        private static void Release(Represent represent)
        {
            if (represent == null || _maps == null)
            {
                return;
            }
            represent.gameObject.SetActive(false);
            _maps.Release(represent.url, represent);
            
#if UNITY_EDITOR   
            EasyProfilerRuntime.ReprentRemove();
#endif
        }

#if UNITY_EDITOR
        /// <summary>
        /// 编辑器删除
        /// </summary>
        /// <param name="represent"></param>
        public static void RemoveWithEditorModel(Represent represent)
        {
            if(represent == null) return;
            _maps.Remove(represent.url, represent);
        }
#endif
        
        #endregion

    }
}