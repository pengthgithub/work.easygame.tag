using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Easy.Logic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Easy
{
    [ExecuteAlways]
    public class ESfx : EDisplay
    {
        public static bool HideAllSfx = false;

        /// <summary>
        ///     目标
        /// </summary>
        [SerializeField] public ECharacter target;

        /// <summary>
        ///     拥有者
        /// </summary>
        [SerializeField] private ECharacter _owner;

        public ECharacter Owner
        {
            get => _owner;
            set
            {
                _owner = value;
                // 目前项目中 不会存在 角色删除得情况，所以也不比写设个代码，这里有gc触发，
                // 如何避免。1、双方对象有互相持有对象， 多得一方 使用 list 删除得时候 互相置空就可以了。
                //if (_owner) _owner.disposeEvent += OnOwnerDestory; // gctodo 
            }
        }

        /// <summary>
        /// 静音
        /// </summary>
        public bool mute;

        private void OnOwnerDestory()
        {
            if (Owner)
            {
                _owner.disposeEvent -= OnOwnerDestory;
            }

            OnDispose();
        }

        /** 播放进度 */
        public float CurrentTime
        {
            get => updatedTime;
        }

        public bool LockDirection { get; private set; }

        [SerializeField] private bool checkLifeTag;

        /// <summary>
        /// 检测标签是否制作错误
        /// </summary>
        public bool CheckLifeTag
        {
            get => checkLifeTag;
            set => checkLifeTag = value;
        }

        //=================================================================================
        //  资源加载
        //=================================================================================
        [SerializeField] private SfxParticle sfxOrigin;

        #region 资源加载 模块

        internal void LoadPrefab(Action loadEndCallBack)
        {
            sfxCount++;
            _onDisposed = false;
            waitCreateTime = Time.frameCount;
            _loadEndCallBack = loadEndCallBack;
            if (sfxOrigin == null)
            {
                ELoader.LoadPrefabAsync<SfxParticle>(url, _LoadCallBack);
                return;
            }

            OnLoadEnd();
        }

        private Action _loadEndCallBack;

        public void _LoadCallBack(SfxParticle origin)
        {
            sfxOrigin = origin;
            if (!sfxOrigin)
            {
                Debug.LogError($"{url} 加载失败.");
                return;
            }

            maxLifeTime = sfxOrigin.lifeTime;
            if (!HideAllSfx)
            {
                InitTag(sfxOrigin);
            }
            OnLoadEnd();
        }

        private void OnLoadEnd()
        {
            foreach (var _tag in _logicTag)
            {
                _tag.Init(this);
            }

            updatedTime = 0;
            _EnableUpdate = true;

#if UNITY_DEBUG
            if (CheckLifeTag && maxLifeTime == 0)
            {
                Debug.LogError($"标签的生命周期为：0，配置的检查生命{CheckLifeTag}，该特效会永久存在,出现内存爆炸。{url},需要索索和牛奶对一下，看标签怎么配置.");
            }
#endif
            waitCreateTime = -waitCreateTime; //重置加载时间表示已经加载好了
        }

        /// <summary>
        /// 标签更新逻辑
        /// </summary>
        private readonly List<ITag> _logicTag = new();

        public bool hasCasterAnimation = false;

        private void InitTag(SfxParticle sfxOrigin)
        {
            if (_logicTag.Count != 0) return;

            foreach (SfxPrefab sfx in sfxOrigin.sfxPrefab)
            {
                _logicTag.Add(new PrefabTag(sfx));
            }

            foreach (SfxOwner sfx in sfxOrigin.sfxOwner)
            {
                if (string.IsNullOrEmpty(sfx.clipName) && sfx.animationClip)
                {
                    sfx.clipName = sfx.animationClip.name;
                }

                if (string.IsNullOrEmpty(sfx.clipName) == false)
                {
                    hasCasterAnimation = true;
                }

                _logicTag.Add(new CasterTag(sfx));
            }

            foreach (SfxCameraShark sfx in sfxOrigin.sfxShark)
            {
                _logicTag.Add(new CameraTag(sfx));
            }

            foreach (SfxSound sfx in sfxOrigin.sfxSound)
            {
                _logicTag.Add(new SoundTag(sfx));
            }

            foreach (SfxLineTag sfx in sfxOrigin.sfxLines)
            {
                _logicTag.Add(new LineTag(sfx));
            }
        }

        #endregion

        #region 内置方法

        private void Awake()
        {
            Init();
        }

        /// <summary>
        /// 标签的生命周期
        /// </summary>
        public float maxLifeTime;

        /// <summary>
        /// 标签是否更新
        /// </summary>
        private bool _EnableUpdate;

        private void LateUpdate()
        {
           
            if (_loadEndCallBack != null && waitCreateTime < 0)
            {
                _loadEndCallBack.Invoke();
                _loadEndCallBack = null;
            }

            if (waitCreateTime > 0) return;

            base.LateUpdate();
            if (deathTime > 0)
            {
                durationDeathTime += Time.deltaTime * playSpeed;
                if (durationDeathTime > deathTime)
                {
                    _OnDispose();
                    durationDeathTime = 0;
                    deathTime = 0;
                }
            }

            if (!_EnableUpdate) return;
            
            var realSpeed = playSpeed;
            if (sfxOrigin) realSpeed *= sfxOrigin.speed;
            if (Owner) realSpeed *= Owner.EffectSpeed;

            updatedTime += Time.smoothDeltaTime * realSpeed;

            if (!HideAllSfx)
            {
                foreach (var _tag in _logicTag)
                {
                    _tag.Update(updatedTime);
                }
            }

            if (updatedTime >= maxLifeTime && maxLifeTime != 0)
            {
                _EnableUpdate = false;
#if UNITY_DEBUG
                debugStr = "生命周期结束删除.";
#endif
                OnDispose();
            }
        }

        #endregion

        //===============================================================================
        // 标签销毁
        //===============================================================================

        #region 标签销毁

        /// <summary>
        /// 死亡后需要做的事情
        /// 1、如果需要播放死亡特效，就播放一下。
        /// 2、 如果配置了动画，就把动画完成了在删除
        /// </summary>
        /// <param name="callBack"></param>
        protected override void OnDispose()
        {
            _EnableUpdate = false;
            CheckLifeTag = false;
            isDisposed = true;
            base.OnDispose();

            float maxTime = 0;
            // 如果有死亡标签,就创建一个死亡标签
            if (sfxOrigin && sfxOrigin.deathSfx != null)
            {
                ESfx deathTag = Create(sfxOrigin.deathSfx.name);

                if (deathTag != null)
                {
                    deathTag.transform.Clone(transform);
                    deathTag.LockDirection = true;
                    deathTag.CheckLifeTag = true;
                }
            }

            //检测目标上是否有死亡动画，有的话就播放，并获取最大的播放时间
            foreach (var _tag in _logicTag)
            {
                var deathTime = _tag.Death();
                if (maxTime < deathTime) maxTime = deathTime;
            }

            if (maxTime == 0) _OnDispose();
            else
            {
                deathTime = maxTime;
                durationDeathTime = 0;
            }
        }

        private float deathTime;
        private float durationDeathTime;
        private bool _onDisposed = false;

        private void _OnDispose()
        {
            if (_onDisposed) return;
            _onDisposed = true;

            if(_owner)_owner.OnSfxDispose(url);
            
            sfxCount--;
            if (!gameObject) return;
            if (_logicTag != null)
            {
                foreach (var _tag in _logicTag)
                {
                    _tag.Dispose();
                }
            }

            enabled = false;
            transform.Reset();
            Recover(this);
            target = null;
            _owner = null;

#if UNITY_DEBUG
            destroyFrame += $"{Time.frameCount}-";
#endif
        }

        public override void Destroy()
        {
            enabled = false;
            if(sfxOrigin) GameObject.Destroy(sfxOrigin);
            sfxOrigin = null;
  
            if (gameObject == null) return;

            if (_logicTag != null)
            {
                foreach (var _tag in _logicTag)
                {
                    _tag.Destory();
                }

                _logicTag.Clear();
            }

            _EnableUpdate = false;
            ELoader.UnLoadAsset(url);
        }

        #endregion

        protected override void OnAnimationChange(string _name)
        {
            base.OnAnimationChange(_name);
            Debug.LogWarning("标签不支持动画切换，每个标签节点都有自己的动画，无法做到换动画");
        }

        //=====================================================================================================
        //            静态方法
        //=====================================================================================================
        public static PoolUtil<ESfx> sfxPool = new PoolUtil<ESfx>(500);
        
        private static UnityObjectPool<ESfx> _maps =
            new UnityObjectPool<ESfx>((url) =>
            {
                ESfx sfx = sfxPool.GetPool();
                if (sfx == null)
                {
                    return sfx;
                }
                   
#if UNITY_DEBUG
                string name = Path.GetFileNameWithoutExtension(url);
                int index = _maps.MaxCount(url);
                sfx.name = $"{name}+{index}";
#endif
                sfx.url = url;
                
                return sfx;
            }, (string url, ESfx ck) =>
            {
                if (ck)
                {
                    GameObject.DestroyImmediate(ck.gameObject);
                }
            });

        public static int sfxCount = 0;

        public static ESfx Create(string url, ECharacter owner = null, Action
            onLoadEndCallBack = null, Action onDisposeCallBack = null)
        {
            if (string.IsNullOrEmpty(url)) return null;
            if (owner && owner.hasSfx(url)) return null;

            ESfx poolSfx = _maps.Get(url);
            if (poolSfx == null)
            {
                Debug.LogError("sfx create error url: " + url);
                return null;
            }

            poolSfx.enabled = true;
            //poolSfx.debugStr = "";
            poolSfx.Init();
            poolSfx.CheckLifeTag = false;
            if (owner) poolSfx.Owner = owner;

            if (onDisposeCallBack != null) poolSfx.disposeEvent += onDisposeCallBack;
            poolSfx.LoadPrefab(onLoadEndCallBack);
            return poolSfx;
        }

        protected static void Recover(ESfx display)
        {
#if UNITY_DEBUG
            string name = Path.GetFileNameWithoutExtension(display.url);
            display.name = name;
#endif
            if (string.IsNullOrEmpty(display.url))
            {
                return;
            }
            _maps.Release(display.url, display);
        }

        public static void DestoryAll()
        {
            _maps.DestoryAll();
            sfxPool.UnInit();
        }

        public static void AutoClear(int count, List<string> ignoreUrlList)
        {
            _maps.AutoClear(ignoreUrlList,count );
        }
    }
}