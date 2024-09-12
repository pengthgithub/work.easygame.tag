using System;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

namespace Easy
{
    /// <summary>
    /// 客户端表现对象，用于控制所有的表现
    /// </summary>
    public abstract class EDisplay : MonoBehaviour
    {
        [SerializeField] public string debugStr;
        public static int instanceID = -99999;

        public int logicID;
        [SerializeField] [Rename("调试")] public bool debug;
        [SerializeField] [Rename("资源路径")] protected string url;
        [SerializeField] [Rename("唯一ID")] public int ID;
        [SerializeField] [Rename("创建时间")] private int createFrame;
        [SerializeField] [Rename("删除时间")] protected string destroyFrame;
        [SerializeField] [Rename("更新时间")] public float updatedTime;
        [SerializeField] [Rename("创建完成时间")] protected int waitCreateTime; //-1：重置加载时间表示已经加载好了 0：开始加载
        [SerializeField] [Rename("更新位置")] public Vector3 lastPosition;

        public string URL
        {
            get => url;
        }
        //====================================================================
        //  基础属性设置
        //====================================================================

        #region 基础方法

        [SerializeField] [Rename("播放速度")] protected float playSpeed;

        public float PlaySpeed
        {
            get => playSpeed;
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

        protected virtual void OnChangeColor(bool val)
        {
        }

        protected virtual void OnSpeedChange(float speed)
        {
        }

        [SerializeField] [Rename("是否启用")] protected bool _active = true;

        public bool Active
        {
            get => _active;
            set
            {
                if (_active != value) OnActiveChange(value);
                _active = value;
            }
        }

        protected virtual void OnActiveChange(bool val)
        {
        }

        /// <summary>
        ///     缩放
        /// </summary>
        public float Scale
        {
            get => transform.localScale.x;
            set
            {
                temp.Set(value, value, value);
                //if (transform.localScale.Equals(temp)) return;
                transform.localScale = temp;
            }
        }

        /// <summary>
        ///     缩放
        /// </summary>
        public Vector3 LocalScale
        {
            get => transform.localScale;
            set
            {
                //if (transform.localScale.Equals(value)) return;
                transform.localScale = value;
            }
        }

        private Vector3 temp = new Vector3();

        public void SetScale(float x, float y, float z)
        {
            temp.Set(x, y, z);
            LocalScale = temp;
        }

        /// <summary>
        ///     旋转
        /// </summary>
        public float RotateAngle
        {
            get => transform.eulerAngles.y;
            set
            {
                var transform1 = transform;
                Vector3 eulerAngle = transform1.eulerAngles;
                if (eulerAngle.y.Equals(value)) return;
                eulerAngle.y = value;
                eulerAngle.z = 0;
                transform1.eulerAngles = eulerAngle;
            }
        }

        public Vector3 EulerAngle
        {
            get => transform.eulerAngles;
            set => transform.eulerAngles = value;
        }

        public void SetCenterAngle(float x, float y, float z = 0)
        {
            temp.Set(x, y, z);
            if (Locator)
            {
                //if (Locator.transform.localEulerAngles.Equals(temp)) return;
                Locator.transform.localEulerAngles = temp;
            }
        }
        
        /// <summary>
        ///     本地位置
        /// </summary>
        public Vector3 LocalPosition
        {
            get => transform.localPosition;
            set
            {
                //if (transform.localPosition.Equals(value)) return;
                transform.localPosition = value;
            }
        }

        public void SetLocalPosition(float x, float y, float z)
        {
            temp.Set(x, y, z);
            LocalPosition = temp;
        }

        /// <summary>
        ///  世界位置
        /// </summary>
        public Vector3 Position
        {
            get => transform.position;
            set
            {
                //if (lastPosition == Vector3.zero) lastPosition = value;
                //if (transform.position.Equals(value)) return;
                transform.position = value;
            }
        }

        public void SetPosition(float x, float y, float z)
        {
            temp.Set(x, y, z);
            Position = temp;
        }

        #endregion

        //====================================================================
        //  扩展方法
        //====================================================================

        #region 扩展方法

        [SerializeField] [Rename("播放的动画名")] protected string _animationName;

        /// <summary>
        /// 动画播放，名字为空 停止播放动画
        /// </summary>
        public string AnimationName
        {
            get => _animationName;
            set
            {
                OnAnimationChange(value);
                _animationName = value;

#if UNITY_DEBUG
                animationPlayedList ??= new List<string>();
                animationPlayedList.Add(string.Format("{0} play {1}", Time.frameCount, value));
#endif
            }
        }

        /// <summary>
        /// 播放动画，当前动画播放完毕后触发回调
        /// </summary>
        public Action<string> OnAnimationPlayEnd;

        /// <summary>
        /// 播放动画，当前动画播放完毕后触发回调
        /// </summary>
        public Action<ECharacter, string> oncePlayEnd;

        protected virtual void OnAnimationChange(string _name)
        {
        }

        /// <summary>
        /// 获取控制脚本，这里名字不符，
        /// </summary>
        /// <param name="_name"></param>
        /// <returns></returns>
        internal ELocator Locator { get; set; }

        /// <summary>
        /// 获取插槽名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual Transform GetLocator(string _name)
        {
            return transform;
        }

        [SerializeField] [Rename("删除时间")] private float _delayTime;

        [SerializeField] [Rename("是否删除")] protected bool isDisposed;

        /// <summary>
        /// 延迟事件
        /// </summary>
        [SerializeField] public Action disposeEvent;

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
            //主动调用删除之后不在回调
            disposeEvent = null;
            
            _delayTime = delayTime;
            dureationDelayTIme = 0;
            //如果时间为0 直接删除
            if (_delayTime == 0) OnDispose();
        }

        public virtual void Destroy()
        {
        
        }

        private float dureationDelayTIme = 0;

        protected void LateUpdate()
        {
            if (_delayTime <= 0) return;
            dureationDelayTIme += Time.deltaTime * PlaySpeed;
            if (dureationDelayTIme > _delayTime)
            {
                _delayTime = 0;
                dureationDelayTIme = 0;
                OnDispose();
            }
        }

        /// <summary>
        /// 回收方法
        /// </summary>
        protected virtual void OnDispose()
        {
            OnAnimationPlayEnd = null;
            disposeEvent?.Invoke();
            disposeEvent = null;
            isDisposed = true;
            lastPosition = Vector3.zero;
#if UNITY_DEBUG
            animationPlayedList?.Clear();
#endif
        }

        #endregion

        #region 逻辑实现

        internal void Init()
        {
            playSpeed = 1;
            _active = true;
            destroyFrame = "";
            updatedTime = 0;
            _delayTime = 0;
            isDisposed = false;
#if UNITY_DEBUG
            gameObject.name = gameObject.name.Replace("Recover", ""); // gctodo 
#endif
            createFrame = Time.frameCount;
            gameObject.SetActive(true);
            _animationName = "";
            waitCreateTime = 0;
            ID = instanceID++;
        }

        #endregion

        //====================================================================
        //  编辑器调试信息
        //====================================================================

        #region 编辑器调试信息

#if UNITY_DEBUG
        [SerializeField] [ReadOnly] private List<string> animationPlayedList;
#endif

        #endregion
    }
}