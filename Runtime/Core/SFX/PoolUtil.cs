using System.Collections.Generic;
using UnityEngine;

namespace Easy
{
    public class PoolUtil<T> where T : MonoBehaviour
    {
        private T _origin;
        private List<T> _poolList;
        private GameObject root;
        public PoolUtil(int count)
        {
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
            go.transform.SetParent(root.transform);

            for (var i = 0; i < count; i++)
            {
                _poolList.Add(Object.Instantiate(_origin,Vector3.zero, Quaternion.identity, root.transform));
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
}