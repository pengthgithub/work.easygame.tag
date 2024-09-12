using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Easy
{
    [Serializable] public enum LocatorType
    {
        none,       //无
        origin,     //脚底
        body,       //身体
        top,        //头顶
        bullet01,     //子弹
        bullet02     //子弹
    }

    public enum CustomLayer
    {
        Default = 0,
        UI = 5
    }

    public partial class Represent
    {
        public bool CheckLifeTag;
        
        /// <summary>
        /// 中心旋转
        /// </summary>
        public float CenterAngle
        {
            get=> transform.eulerAngles.y;
            set
            {
                var ts = transform;
                var angle = ts.eulerAngles;
                angle.y = value;
                ts.eulerAngles = angle;
            }
        }

        private CustomLayer _layer;
        public CustomLayer Layer
        {
            get => _layer;
            set
            {
                if (_layer != value)
                {
                    _layer = value;
                    if (_0Control) _0Control.Layer = (int)_layer;
                }
            }
        }

        private void Play()
        {
            if(!_0Control) return;
            _0Control.complete = PlayEnd;
            _0Control.Play(_animationName);
        }
        private void PlayEnd(string name)
        {
            playCompleteEvent?.Invoke(this, name);
        }
        
        private void SetActive()
        {
            if(!_0Control) return;
            _0Control.Active = _active;
        }

        public bool OutLine
        {
            set
            {
            }
        }

        //====================================================================
        // 资源加载
        //====================================================================
        #region 资源加载
        public Control _0Control;
        private bool _0LoadEnd = false;
        
        
        private float _loadTime;
        /// <summary>
        /// 加载表现
        /// </summary>
        private void LoadRepresent()
        {
            if (string.IsNullOrEmpty(url)) return;
            _loadTime = Time.realtimeSinceStartup;
            // 同一份资源不重复加载
            if (!_0LoadEnd)
            {
                LoaderMgr.LoadPrefabAsync<Object>(url, LoadAssetEnd);
            }
            else
            {
                Show();
            }
        }
        private bool lateLoad = false;
        private void LateLoadPrefab()
        {
            if(lateLoad) return;
            lateLoad = true;
            LoadRepresent();
        }

        private async void LoadAssetEnd(Object origin)
        {
            if (isDisposed || !origin) return;
           
            if (origin is SfxParticle)
            {
                var _sfxParticle = ScriptableObject.Instantiate(origin) as SfxParticle;
                while (Owner && Owner._0LoadEnd == false)
                {
                    await Task.Delay(1);
                }
                if (isDisposed) return;
                InitTag(_sfxParticle);
            }
            else
            {
                var _represent = Instantiate(origin as GameObject, gameObject.transform, false);
                _0Control = _represent.GetComponent<Control>();
#if UNITY_EDITOR
                _0Control.hideChild = true;
#endif
            }
            Show();
            _0LoadEnd = true;
        }
        
        private void Show()
        {
            if (Owner)
            {
                transform.position = Owner.Position;
            }
            SetOwner();
            
            var useTime = Time.realtimeSinceStartup - _loadTime;
            SetActive();
            if(_0Control) _0Control.Play(_animationName, false, useTime);
            
            completeEvent?.Invoke();
        }

        /// <summary>
        /// 获取插槽名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Transform GetLocator(LocatorType locator)
        {
            if (_0Control)
            {
                return _0Control.GetLocator(locator);
            }
            return transform;
        }
        
        #endregion
        
        /// <summary>
        /// 拥有的表现列表
        /// </summary>
        private List<Represent> ownerSfxList = new List<Represent>();
        private void AddOwnerSfx(Represent represent)
        {
            ownerSfxList.Add(represent);
        }
        private void RemvoveOwnerSfx(Represent represent)
        {
            ownerSfxList.Remove(represent);
        }

        private void DisposeOwnerSfx()
        {
            if (ownerSfxList.Count != 0)
            {
                for (int i = 0, n = ownerSfxList.Count - 1; i >= 0; i--)
                {
                    ownerSfxList[i].Dispose();
                }
                ownerSfxList.Clear();  
            }
        }
    }
}