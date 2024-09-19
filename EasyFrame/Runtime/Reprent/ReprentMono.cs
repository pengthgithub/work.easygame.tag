using System;
using System.Collections.Generic;
using UnityEngine;

namespace Easy
{
    /// <summary>
    /// 客户端表现对象，用于控制所有的表现
    /// </summary>
    public partial class Represent : MonoBehaviour
    {
        //========================================================================
        //  调试信息
        //========================================================================
        #region 调试信息
        [SerializeField] public string debugStr;
        [SerializeField] [Rename("调试")] public bool debug;
        
        [SerializeField] [Rename("唯一ID")] public int ID;
        
        [SerializeField] [Rename("创建时间")] private int createFrame;
        [SerializeField] [Rename("删除时间")] protected string destroyFrame;
        
        [SerializeField] [Rename("创建完成时间")] protected int waitCreateTime; //-1：重置加载时间表示已经加载好了 0：开始加载
        [SerializeField] [Rename("更新位置")] public Vector3 lastPosition;

        [SerializeField] [Rename("资源路径")] protected string url;
        [SerializeField] [Rename("删除时间")] private float _delayTime;
        [SerializeField] [Rename("是否删除")] protected bool isDisposed;
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
                }
                else
                {
                    _Owner = value;
                    _Owner.AddOwnerSfx(this);

                    Position = _Owner.Position;
                }
            }
        }

        public Represent Target { get; set; }

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
            debugStr = "主动调用删除";
            destroyFrame += $"{Time.frameCount}-";
#endif
            // 同一对象重复调用回收处理，直接返回
            if (isDisposed) return;
            isDisposed = true;
            disposeEvent?.Invoke();
            //主动调用删除之后不在回调
            disposeEvent = null;
            
            _delayTime = delayTime;
            dureationDelayTIme = 0;
            //如果时间为0 直接删除
            if (_delayTime == 0) OnDispose();
        }
        private float dureationDelayTIme = 0;

        protected void LateUpdate()
        {
            LateLoadPrefab();
            
            UpdateTag();
            
            if (_delayTime <= 0) return;
            dureationDelayTIme += Time.deltaTime * Speed;
            if (dureationDelayTIme > _delayTime)
            {
                _delayTime = 0;
                dureationDelayTIme = 0;
                OnDispose();
            }
        }

        private bool disposeEnd = false;
        /// <summary>
        /// 回收方法
        /// </summary>
        protected void OnDispose()
        {
            if(disposeEnd) return;
            lateLoad = false;
            OnAnimationPlayEnd = null;

            Owner = null;
            Target = null;
            
            isDisposed = true;
            lastPosition = Vector3.zero;
            DisposeOwnerSfx();
            DisposeTag();
            Release(this);
            disposeEnd = true;
        }

        #endregion

        #region 逻辑实现

        internal void Init()
        {
            transform.Reset();
            enabled = true;
            playSpeed = 1;
            _active = true;
            lateLoad = false;
            destroyFrame = "";
            _delayTime = 0;
            isDisposed = false;
            disposeEnd = false;
            createFrame = Time.frameCount;
            gameObject.SetActive(true);
            _animationName = "";
            waitCreateTime = 0;
            ID = GetInstanceID();
        }
        #endregion

        private void OnDestroy()
        {
            
        }


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