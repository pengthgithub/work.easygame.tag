using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Easy
{
    /// <summary>
    /// 资源引用
    /// </summary>
    public struct ObjectReference
    {
        private uint _refCount;
        public Object OriginObject;

        public ObjectReference(Object originObject)
        {
            OriginObject = originObject;
            _refCount = 0;
        }

        public void AddRef()
        {
            _refCount++;
        }

        public void RemoveRef()
        {
            _refCount--;
            if (_refCount == 0 && OriginObject != null)
            {
#if UNITY_EDITOR
                Debug.Log($"释放资源：{OriginObject.name}");
#endif
                Addressables.Release(OriginObject);
                OriginObject = null;
            }
        }
    }
    
    /// <summary>
    /// 加载管理器
    /// </summary>
    public abstract class LoaderMgr
    {
        private static readonly Dictionary<string, ObjectReference> OriginSource = new();

        /// <summary>
        /// 只处理加载预制件，如果不是预制件资源可能有问题
        /// </summary>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        public static void LoadPrefabAsync<T>(string key, Action<T> callback) where T : Object
        {
            bool result = OriginSource.TryGetValue(key, out ObjectReference reference);
            if (result)
            {
                reference.AddRef();
                callback.Invoke(reference.OriginObject as T);
                return;
            }

            var handle = Addressables.LoadAssetAsync<T>(key);
            if (handle.IsValid() == false)
            {
                return;
            }

            handle.Completed += (AsyncOperationHandle<T> obj) =>
            {
                if (obj.Status == AsyncOperationStatus.Succeeded)
                {
                    ObjectReference or = new(obj.Result);
                    or.AddRef();
                    OriginSource[key] = or;
                    callback.Invoke(obj.Result as T);
                }
                else
                {
                    Debug.LogError("Failed to load resource with key: " + key);
                    callback.Invoke(null);
                }
            };
        }

        /// <summary>
        ///     加载资源
        /// </summary>
        /// <typeparam name="T"> 资源类型 </typeparam>
        /// <param name="url"> 路径 </param>
        /// <returns></returns>
        public static async Task<T> LoadAsset<T>(string url) where T : Object
        {
            OriginSource.TryGetValue(url, out ObjectReference result);
            if (result.OriginObject != null)
            {
                result.AddRef();
                return result.OriginObject as T;
            }

            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(url);
            await handle.Task;
            T handleResult = handle.Result; // gctodo 
            if (!handleResult)
            {
                Debug.LogError($"EasyLoader.LoadAsset: Load [{url}] Failed");
                return null;
            }

            ObjectReference or = new(handleResult);
            or.AddRef();
            OriginSource[url] = or;
            return handleResult;
        }

        /// <summary>
        ///     释放资产
        /// </summary>
        /// <param name="url"></param>
        public static void UnLoadAsset(string url)
        {
            if (string.IsNullOrEmpty(url)) return;
            OriginSource.TryGetValue(url, out ObjectReference result);
            if (result.OriginObject == null)
            {
                return;
            }

            result.RemoveRef();
            OriginSource.Remove(url);
        }

        /// <summary>
        /// 清楚所有引用
        /// </summary>
        public static void Clear()
        {
            foreach (ObjectReference val in OriginSource.Values)
            {
                val.RemoveRef();
            }

            OriginSource.Clear();
        }
    }
}