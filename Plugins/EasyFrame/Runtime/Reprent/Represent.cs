using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace Easy
{
    [Serializable] public enum LocatorType
    {
        none,         // 无
        origin,       // 脚底
        body,         // 身体
        top,          // 头顶
        bullet01,     // 子弹
        bullet02,     // 子弹
        bip_l_hand,   // 左手武器
        bip_r_hand,    // 右手武器
        bip_bullet,
        bip_bullet01,
        bullet03,
        bullet04,
        bullet05
    }

    public enum CustomLayer
    {
        Default = 0,
        Character = 3,
        UI = 5,
        UICharacter = 6,
    }

    public partial class Represent
    {
        /// <summary>
        /// 检测生命周期
        /// </summary>
        public bool CheckLifeTag;
        
        /// <summary>
        /// 中心旋转
        /// </summary>
        public float CenterAngle
        {
            get=> __0Control? __0Control.CenterAngle: 0;
            set
            {
                if (__0Control)
                {
                    __0Control.CenterAngle = value;
                }
            }
        }
        
        /// <summary>
        /// 加载完成后调用
        /// </summary>
        public bool FlipX
        {
            get
            {
                return __0Control? __0Control.FlipX: false;
            }
            set
            {
                if(__0Control) __0Control.FlipX = value;
            }
        }
        
        /// <summary>
        /// 加载完成后调用
        /// </summary>
        public bool FlipY
        {
            get
            {
                return __0Control? __0Control.FlipY: false;
            }
            set
            {
                if(__0Control) __0Control.FlipY = value;
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
                    if (__0Control) __0Control.Layer = (int)_layer;
                }
            }
        }

        private void Play()
        {
            if(!__0Control) return;
            __0Control.Play(_animationName);
        }
        private void PlayEnd(string name)
        {
            playCompleteEvent?.Invoke(this, name);
        }
        
        private void SetActive()
        {
            if(!__0Control) return;
            __0Control.Active = _active;
        }

        public bool OutLine
        {
            get
            {
                if(!__0Control) return false;
                return __0Control.EnableOutLine;
            }
            set
            {
                if(!__0Control) return;
                __0Control.OutLineColor = Color.white;
                __0Control.EnableOutLine = value;
            }
        }

        //====================================================================
        // 资源加载
        //====================================================================
        #region 资源加载
        public Control __0Control;
        private bool __0LoadEnd = false;
        
        private float _loadResUsingTime; //资源加载耗时，用于动画播放的时候，从动画中间开始播放
        /// <summary>
        /// 加载表现
        /// </summary>
        private void LoadRepresent()
        {
            if (string.IsNullOrEmpty(url)) return;
            
            _loadResUsingTime = Time.realtimeSinceStartup;
            
            __stat = RepresentStat.RsLoadingRes;
            // 同一份资源不重复加载
            if (!__0LoadEnd)
            {
                LoaderMgr.LoadPrefabAsync<Object>(url, LoadAssetEnd);
            }
            else
            {
                Show();
            }
        }
        
        private async void LoadAssetEnd(Object origin)
        {
            if (__stat >= RepresentStat.RsDisposing  || !origin) return;
           
            if (origin is SfxParticle)
            {
                var _sfxParticle = ScriptableObject.Instantiate(origin) as SfxParticle;
                if (__stat >= RepresentStat.RsDisposing) return;
                InitTag(_sfxParticle);
            }
            else
            {
                var _represent = Instantiate(origin as GameObject, gameObject.transform, false);
                __0Control = _represent.GetComponent<Control>();
                __0Control.complete = PlayEnd;
            }
            Show();
            __0LoadEnd = true;
        }
        
        private void Show()
        {
            if (Owner)
            {
                transform.position = Owner.Position;
            }
            
            ResetTag();
            
            var useTime = Time.realtimeSinceStartup - _loadResUsingTime;
            SetActive();

            if (__0Control)
            {
                __0Control.Init();
                __0Control.Play(_animationName, false, useTime);
            }
            
            completeEvent?.Invoke();

            __stat = RepresentStat.RsLoadEnd;
        }

        /// <summary>
        /// 获取插槽名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Transform GetLocator(LocatorType locator)
        {
            if (__0Control)
            {
                return __0Control.GetLocator(locator);
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