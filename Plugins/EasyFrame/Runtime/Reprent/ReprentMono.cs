using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Serialization;

namespace Easy
{
    /// <summary>
    /// 客户端表现对象，用于控制所有的表现
    /// </summary>
    public partial class Represent : MonoBehaviour
    {
        public enum RepresentStat
        {
            RsCreate,       //创建状态
            RsLoadingRes,   //资源加载状态
            RsLoadEnd,      //加载完成
            
            RsDisposing,    //释放中
            RsDisposeEnd,   //资源释放结束
            
            RsDeathing,     //死亡中表现
            RsDeathEnd,     //死亡结束
            RsRelease       //释放完毕
        }
        //========================================================================
        //  调试信息
        //========================================================================
        #region 调试信息
        [SerializeField] public string debugStr;
        [SerializeField] [Rename("调试")] public bool debug;
        
        [SerializeField] [Rename("唯一ID")] public int ID;
        
        [SerializeField] [Rename("存活时间")] private float currentLifeTime;
        
        [SerializeField] [Rename("创建完成时间")] protected int waitCreateTime; //-1：重置加载时间表示已经加载好了 0：开始加载
        [SerializeField] [Rename("更新位置")] public Vector3 lastPosition;

        [SerializeField] [Rename("资源路径")] protected string url;
        [SerializeField] [Rename("延迟删除时间")] private float _delayTime;
        
        [SerializeField] [Rename("当前状态")] protected RepresentStat __stat;
        
        [SerializeField] [Rename("播放速度")] protected float playSpeed;
        [SerializeField] [Rename("播放的动画名")] protected string _animationName;
        
        [SerializeField] private List<Vector3> debugList;
        [SerializeField] private Represent _Owner;
        #endregion
        
        //====================================================================
        //  基础属性设置
        //====================================================================
        #region 基础方法
        public string URL
        {
            get => url;
        }
        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }

        public Vector3 LocalPosition
        {
            get=> transform.localPosition;
            set=> transform.localPosition = value;
        }

        public Vector3 EulerAngles
        {
            get => transform.eulerAngles;
            set
            {
                transform.eulerAngles = value;
            }
        }

        public Vector3 LocalEulerAngles
        {
            get => transform.localEulerAngles; 
            set => transform.localEulerAngles = value;
        }
        public float Angle
        {
            get=> transform.eulerAngles.y;
            set
            {
                var _angle = transform.eulerAngles;
                _angle.y = value;
                LocalEulerAngles = _angle;
            }
        }
        public Vector3 LocalScale
        {
            get=> transform.localScale; 
            set=> transform.localScale = value;
        }
        public float Scale
        {
            get => transform.localScale.x;
            set=> transform.localScale = new Vector3(value, value, value);
        }
        public int LogicID { get; set; }
       
        /// <summary>
        /// 不能public的原因是由于时序问题，可能会导致特效设置时效，导致表现不正确
        /// </summary>
        internal Represent Owner
        {
            get => _Owner;
            set
            {
                if (value == null)
                {
                    if (_Owner)
                    {
                        _Owner.RemvoveOwnerSfx(this);
                    }
                    _Owner = null;
                    hasOwner = false;
                }
                else
                {
                    _Owner = value;
                    _Owner.AddOwnerSfx(this);
                    hasOwner = true;
                    Position = _Owner.Position;
                }
            }
        }

        public bool hasOwner = false;
        public bool hasTarget = false;
        [SerializeField]private Represent _Target;
        public Represent Target
        {
            get=> _Target;
            set
            {
                _Target = value;
                if (value) hasTarget = true;
                else hasTarget = false;
            } 
        }

        public float Speed
        {
            get
            {
                if(_Owner) return playSpeed * _Owner.Speed;
               return playSpeed;
            } 
            set
            {
                if (playSpeed != value) OnSpeedChange(value);
                playSpeed = value;
                _effectSpeed = value;
            }
        }
        /// <summary>
        /// 特效速度
        /// </summary>
        private float _effectSpeed;
        /// <summary>
        /// 特效速度
        /// 默认和动画播放速度是一致的，不需要单独去设置，
        /// 但是，当动画为0的时候，我们需要特效也可以正常播放的时候，就先设置动画播放速度为0，然后在来设置特效播放速度就可以解决
        /// </summary>
        public float EffectSpeed
        {
            get => _effectSpeed;
            set { _effectSpeed = value; }
        }
        
        /// <summary>
        /// 显示改变颜色
        /// </summary>
        public bool ShowChangeColor
        {
            get => _changeColor;
            set
            {
                if (value != _changeColor) OnChangeColor(value);
                _changeColor = value;
            }
        }
        private bool _changeColor;

        [SerializeField] [Rename("是否启用")] protected bool _active = true;
        public bool Active
        {
            get => _active;
            set
            {
                if (_active != value)
                {
                    _active = value;
                    SetActive();
                }
            }
        }
        #endregion

        //====================================================================
        //  扩展方法
        //====================================================================

        #region 扩展方法
        /// <summary>
        /// 动画播放，名字为空 停止播放动画
        /// </summary>
        public string AnimationName
        {
            get => _animationName;
            set
            {
                _animationName = value;
                Play();
            }
        }

        /// <summary>
        /// 播放动画，当前动画播放完毕后触发回调
        /// </summary>
        public Action<string> OnAnimationPlayEnd;
        /// <summary>
        /// 播放动画，当前动画播放完毕后触发回调
        /// </summary>
        public Action<Represent, string> playCompleteEvent;

        /// <summary>
        /// 延迟事件
        /// </summary>
        public Action disposeEvent;
        public Action completeEvent;
        /// <summary>
        /// 回收缓存池
        /// 时间为0 destory 为true 表示立马删除
        /// UI 对象调用 Dispose 后也是立马删除
        /// </summary>
        /// <param name="delayTime"> 延迟回收或者删除时间 </param>
        /// <param name="destroy"> 是否直接删除 </param>
        public void Dispose(float delayTime = 0)
        {
#if UNITY_DEBUG
            
            
#endif

            // 同一对象重复调用回收处理，直接返回
            if (__stat >= RepresentStat.RsDisposing) return;
#if UNITY_EDITOR
            debugStr = "主动调用删除";
            currentLifeTime = Time.realtimeSinceStartup - _loadResUsingTime;
#endif
            //如果时间为0 直接删除
            if (_delayTime == 0) OnDispose();
            else
            {
                _dureationDelayTIme = 0;
                _delayTime = delayTime;
                __stat = RepresentStat.RsDisposing;
            }
        }
        private float _dureationDelayTIme = 0;
        private void UpdateDisposeDelay()
        {
            if (_delayTime <= 0) return;
            _dureationDelayTIme += Time.deltaTime * Speed;
            if (_dureationDelayTIme > _delayTime)
            {
                _delayTime = 0;
                _dureationDelayTIme = 0;
                OnDispose();
            }
        }
        
        protected void LateUpdate()
        {
            //这里是延迟加载资源
            if(__stat < RepresentStat.RsLoadingRes)  LoadRepresent();
            
            //是标签采取更新
            if(__IsSfxTag) UpdateTag();
            
            //在延迟的时候才刷新
            if(__stat == RepresentStat.RsDisposing) UpdateDisposeDelay();
        }
        
        /// <summary>
        /// 回收方法
        /// </summary>
        protected void OnDispose()
        {
            if (__stat == RepresentStat.RsDisposeEnd) return;
            __stat = RepresentStat.RsDisposeEnd;

            _animationName = "";
            disposeEvent?.Invoke();
            //主动调用删除之后不在回调
            disposeEvent = null;
            OnAnimationPlayEnd = null;
            Owner = null;
            Target = null;
            lastPosition = Vector3.zero;
            if(__0Control) __0Control.Dispose();
            
            DisposeOwnerSfx();

            if (__IsSfxTag)
            {
                DisposeTag();
            }
            else
            {
                __stat = RepresentStat.RsRelease;
                Release(this);
            }
        }

        #endregion

        #region 逻辑实现

        internal void Init()
        {
            transform.Reset();
            enabled = true;
            playSpeed = 1;
            _active = true;
            _delayTime = 0;
            currentLifeTime = 0;
            gameObject.SetActive(true);
            _animationName = "";
            waitCreateTime = 0;
            ID = GetInstanceID();
            __stat = RepresentStat.RsCreate;
        }
        #endregion
        
        //====================================================================
        //  编辑器调试信息
        //====================================================================

        //====================================================================
        // 虚方法
        //====================================================================
        #region 
        protected virtual void OnChangeColor(bool val) { }
        protected virtual void OnSpeedChange(float speed) { }
        protected virtual void Destroy() { }
        #endregion
    }
}