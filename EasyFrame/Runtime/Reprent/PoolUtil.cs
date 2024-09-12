using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Easy
{
    public class PoolUtil<T> where T : MonoBehaviour
    {
        private T _origin;
        private List<T> _poolList;
        private GameObject root;
        public PoolUtil(int count)
        {
            Init(count);
        }

        public bool IsInit
        {
            get
            {
                if (_origin == null) return false;
                return true;
            }
        }

        public void Init(int count)
        {
            if(_poolList == null) _poolList = new List<T>();
            root = new GameObject("Node");
            
            var go = new GameObject("1");
            _origin = go.AddComponent<T>();
            _origin.enabled = false;
            go.hideFlags = HideFlags.HideInHierarchy;
            go.transform.SetParent(root.transform);

            for (var i = 0; i < count; i++)
            {
                var cache = Object.Instantiate(_origin, Vector3.zero, Quaternion.identity, root.transform);
                #if UNITY_DEBUG
                cache.name = i.ToString();
                #endif
                _poolList.Add(cache);
            }
        }

        public void UnInit()
        {
            if (_origin)
            {
                GameObject.Destroy(_origin.gameObject);
                _origin = null;
            }
            _poolList?.Clear();
        }

        public T GetPool()
        {
            if (_origin == null)
            {
                return null;
            }
            var i = _poolList.Count - 1;
            if (i < 0)
            {
                var p1 = Object.Instantiate(_origin,Vector3.zero, Quaternion.identity,root.transform);
                p1.enabled = true;
                return p1;
            }
            
            var p = _poolList[i];
            _poolList.RemoveAt(i);
            p.enabled = true;
           return p;
        }

        public void Release(T e)
        {
            _poolList.Add(e);
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
    
    public class CustomPool<T> where T : Object
    {
        private readonly Dictionary<string, PoolData<T>> _maps;
        private readonly int _maxSize;
        private readonly Func<string, T> _createFunc;
        private readonly Action<string, T> _destoryFunc;
        public CustomPool(
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
                list.releaseCount--;
                element = list.items[0];
                list.items.RemoveAt(0);
            }
            else
            {
                list.createCount++;
                element = _createFunc(key);
            }

            list.lastUseFrame = Time.frameCount;
            list.activeItems.Add(element);
            _maps[key] = list;
            return element;
        }

        public void Remove(string key, T element)
        {
            _maps.TryGetValue(key, out PoolData<T> list);
            list.activeItems?.Remove(element);
            list.items?.Remove(element);
        }
        public bool Release(string key, T element)
        {
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
        public void Clear()
        {
            for (int i = _maps.Keys.Count -1; i >=0; i--)
            {
                var key = _maps.Keys.ElementAt(i);
                var pool = _maps[key];
                var usedFrame = Time.frameCount - pool.lastUseFrame;

                if (usedFrame < 1000  || pool.activeItems.Count > 0 )
                {
                   continue;
                }

                if (pool.items == null)
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