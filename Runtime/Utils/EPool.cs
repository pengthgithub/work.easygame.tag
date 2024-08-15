using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Easy
{
    /// <summary>
    ///     资源缓存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EPool<T> where T : class
    {
        private readonly Dictionary<string, ObjectPool<T>> _poolMaps = new();

        public T Get(string url, Func<T> createFunc, Action<T> actionOnDestroy = null)
        {
            _poolMaps.TryGetValue(url, out ObjectPool<T> pool);
            bool isNull = pool == null;
            if (!isNull)
            {
                return pool.Get();
            }

            pool = new ObjectPool<T>(createFunc, null, null, actionOnDestroy, true, 1, 100);
            _poolMaps[url] = pool;

            return pool.Get();
        }

        public int MaxCount(string url)
        {
            _poolMaps.TryGetValue(url, out ObjectPool<T> pool);
            bool isNull = pool == null;
            return !isNull ? pool.CountAll : 0;
        }

        public void Release(string url, T element)
        {
            _poolMaps.TryGetValue(url, out ObjectPool<T> pool);
            if (pool != null)
            {
                pool.Release(element);
            }
        }

        public void Clear()
        {
            foreach (ObjectPool<T> pool in _poolMaps.Values)
            {
                pool.Dispose();
            }

            _poolMaps.Clear();
        }
    }

    public struct PoolData<T>
    {
        public int lastUseFrame;
        public int createCount;
        public int releaseCount;
        public int destoryCount;
        public List<T> items;
        public List<T> activeItems;
    }

    public class UnityObjectPool<T> where T : Object
    {
        private readonly Dictionary<string, PoolData<T>> _maps;
        private readonly int _maxSize;
        private readonly Func<string, T> _createFunc;
        private readonly Action<string, T> _destoryFunc;

        public UnityObjectPool(
            Func<string, T> createFunc,
            Action<string, T> destoryFunc = null,
            int maxSize = 30)
        {
            _maxSize = maxSize;
            _maps = new Dictionary<string, PoolData<T>>();
            this._createFunc = createFunc;
            _destoryFunc = destoryFunc;
        }

        public T Get(string key)
        {
            _maps.TryGetValue(key, out PoolData<T> list);
            if (list.items == null)
            {
                list = default;
                list.items = new List<T>(_maxSize);
                list.activeItems = new List<T>(_maxSize);
                _maps[key] = list;
            }

            T element;
            if (list.items.Count > 0)
            {
                // TODO:优化
                list.releaseCount--;
                element = list.items[0];
                list.items.RemoveAt(0); 
                if (element == null)
                {
                    Debug.LogError("unity object pool get element is null : key: " + key);
                }
            }
            else
            {
                list.createCount++;
                element = _createFunc(key);
                if (element == null)
                {
                    Debug.LogError("unity object pool get element create null: key: " + key);
                }
            }

            list.lastUseFrame = Time.frameCount;
            if (element != null)
            {
                list.activeItems.Add(element);
            }
            _maps[key] = list;
            
            return element;
        }

        public bool Release(string key, T element)
        {
            if (element == null)
            {
                Debug.LogWarning($"{key}, value is null.");
                return false;
            }
            _maps.TryGetValue(key, out PoolData<T> list);
            if (list.items != null && list.items.Count < _maxSize)
            {
                list.releaseCount++;
                list.activeItems.Remove(element);
                list.items.Add(element);
                _maps[key] = list;
                return true;
            }

            if (list.items == null)
            {
                Debug.LogWarning($"{key},is not Create by Pool.");
            }
            else
            {
                _destoryFunc?.Invoke(key, element);
            }

            return false;
        }

        public int MaxCount(string url)
        {
            _maps.TryGetValue(url, out PoolData<T> pool);
            return pool.activeItems.Count;
        }
        
        /// <summary>
        /// 清楚所有
        /// </summary>
        public void DestoryAll()
        {
            for (int i = _maps.Keys.Count - 1; i >= 0; i--)
            {
                var key = _maps.Keys.ElementAt(i);
                var pool = _maps[key];
                pool.activeItems.Clear();
                pool.items.Clear();
                ELoader.DestoryAsset(key as string);
            }
        }

        public void AutoClear(List<string> ignoreList, int frameCount = 7200)
        {
            for (int i = _maps.Keys.Count - 1; i >= 0; i--)
            {
                var key = _maps.Keys.ElementAt(i);
                var pool = _maps[key];
                var usedFrame = Time.frameCount - pool.lastUseFrame;

                if (usedFrame < frameCount || pool.activeItems.Count > 0 || ignoreList.Contains(key as string))
                {
                    continue;
                }
                
                if (pool.items == null )
                {
                    Debug.LogError($"{key} is valid.");
                    continue;
                }

                foreach (var obj in pool.items)
                {
                    _destoryFunc?.Invoke(key, obj);
                }

                foreach (var obj in pool.activeItems)
                {
                    _destoryFunc?.Invoke(key, obj);
                }

                pool.activeItems.Clear();
                pool.items.Clear();
            }
        }
    }
}