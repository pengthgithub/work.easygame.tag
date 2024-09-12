using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Easy
{
    /// <summary>
    ///     角色的操作，用于角色数据的操作
    /// </summary>
    [ExecuteAlways]
    public class ECharacter : EDisplay
    {
        public int Layer
        {
            set { Locator.Layer = value; }
        }

        public static bool HideAllCharacter = false;

        public string birthTag = "";
        //====================================================================
        // 动画播放逻辑
        //====================================================================

        #region 动画模块

        [SerializeField] public EAnimation _eAnimation;

        private UIShow _uishow;

        public void PlayUIShow()
        {
            if (_uishow) _uishow.Play();
        }

        /// <summary>
        /// 动画优先级
        /// </summary>
        internal int AniPriority { get; set; }

        /// <summary>
        /// 动画ID
        /// </summary>
        internal int ClassID { get; private set; }

        private void InitAni()
        {
            AniPriority = 0;
            if (!_eAnimation) _eAnimation = gameObject.GetComponentInChildren<EAnimation>(true);
            if (_eAnimation)
            {
                _eAnimation.owner = this;
                _eAnimation.Init();
                _eAnimation.PlayAnimation(_animationName, waitCreateTime);
            }

            if (!_uishow) _uishow = gameObject.GetComponentInChildren<UIShow>(true);

            if (_eAnimation == null)
            {
                //Debug.LogWarning($"资产中没有动画，请检查{url}");
            }
        }

        protected override void OnAnimationChange(string _name)
        {
            if (string.IsNullOrEmpty(_name))
            {
                if (_eAnimation != null) _eAnimation.Stop();
                AniPriority = 0;
                return;
            }

            base.OnAnimationChange(_name);
            if (_eAnimation != null) _eAnimation.PlayAnimation(_name, 0, debug);
        }

        protected override void OnChangeColor(bool val)
        {
            base.OnChangeColor(val);
            if (Locator) Locator.ChangeColor = val;
        }

        protected override void OnSpeedChange(float speed)
        {
            base.OnSpeedChange(speed);
            if (_eAnimation != null) _eAnimation.Speed = speed;
        }

        internal void StopNodeAnimation(int classID)
        {
            //为什么要用这个id来判断，是因为 角色上 会有很多标签做设置，
            //如果不这么判断，会取消掉别的标签播放的动画，我们只是想取消当前标签播放的动画
            if (classID == ClassID && _eAnimation)
            {
                _eAnimation.Stop();
            }
        }

        /// <summary>
        /// 播放节点动画，当目标上没有节点动画时，在目标上添加节点动画
        /// </summary>
        /// <param name="clip">节点动画</param>
        /// <param name="priority">优先级，在同一个对象上，优先级高的动画会覆盖优先级低的动画</param>
        /// <param name="repeatPlay"></param>
        /// <param name="objectID"></param>
        /// <returns></returns>
        internal bool PlayNodeAnimation(AnimationClip clip, string clipName, int priority, bool repeatPlay, int classID)
        {
            // 对象没有结束
            if (!_eAnimation || string.IsNullOrEmpty(clipName))
                return false;

            /// 当前动画在播放时，如果上一次的动画优先级大于当前优先级，则不播放
            /// 优先级重置 是在标签播放完毕时重置
            if (AniPriority > priority)
            {
                return false;
            }

            if (AniPriority == priority)
            {
                if (!repeatPlay && _eAnimation.NodeAnimationIsPlaying())
                {
                    return false; //循环播放为false时，会把动画播放没结束前，不会接收动画播放。 repeatPlay为Ture时，会停止当前动画，来播放一次
                }
            }

            AniPriority = priority;
            ClassID = classID;
            _eAnimation.AddClip(clip, clipName);
            AnimationName = clipName;

            return true;
        }

        #endregion
        
        //====================================================================
        // 资源加载逻辑
        //====================================================================

        #region 资源加载逻辑

        private GameObject _represent;
        private Action _loadEndCallBack;

        internal void LoadPrefab(Action loadEndCallBack)
        {
            ownSfxList.Clear();
            roleCount++;
            canUpdate = false;
            waitCreateTime = Time.frameCount;
            _loadEndCallBack = loadEndCallBack;
            if (!_represent && HideAllCharacter == false)
            {
                //不用处理资产为加载完成，然后删除，然后有被启用，导致的逻辑问题，
                //1、我时保存的资源对象，删除在启用，还是这个对象，加载完成，没有加载完成都没有关系，表现是对的，而且也是我们需要的表现
                ELoader.LoadPrefabAsync<GameObject>(url, LoadEndCallBack);
                return;
            }

            OnLoadEnd();
        }

        private void LoadEndCallBack(GameObject originPrefab)
        {
            if (isDisposed) return;
            if (!originPrefab)
            {
                Debug.LogError($"{url}加载失败,要么路径填写错误，要么没有加入到Addressable的group中去.");
                return;
            }

            if (_represent == null && HideAllCharacter == false)
                _represent = Instantiate(originPrefab, gameObject.transform, false);
            OnLoadEnd();
        }

        private void OnLoadEnd()
        {
            if (_represent) _represent.SetActive(true);
            InitLocator();
            // 资源预制件准备好了，之后是获取资源上的对象，也只获取一次，回收的时候不置空
            InitAni();

            if (!string.IsNullOrEmpty(birthTag))
            {
                if (Locator && ESfx.HideAllSfx == false)
                {
                    Locator.Show(false, true);
                }

                firstSfx = ESfx.Create(birthTag, this, OnFirstEffectLoadEnd);
                if (firstSfx && firstSfx.maxLifeTime == 0) firstSfx.maxLifeTime = 1;
               
                if (!firstSfx && Locator)
                {
                    Locator.Show(true, true);
                }
            }

            gameObject.SetActive(true);
            waitCreateTime = -waitCreateTime; //重置加载时间表示已经加载好了
        }

        private ESfx firstSfx;
        
        private void OnFirstEffectLoadEnd()
        {
            if(gameObject.activeSelf) StartCoroutine(DelayShow());
        }
        IEnumerator DelayShow()
        {
            yield return new WaitForSeconds(0.15f);
            if (Locator)
            {
                Locator.Show(true);
            }
        }

        protected new void LateUpdate()
        {
            base.LateUpdate();

            if (waitCreateTime < 0)
            {
                _loadEndCallBack?.Invoke();
                _loadEndCallBack = null;
                canUpdate = true;
                if (Locator) Locator.canUpdate = true;
            }
        }

        private bool canUpdate = false;

        #endregion

        protected override void OnActiveChange(bool val)
        {
            base.OnActiveChange(val);

            if (Locator)
            {
                Locator.Show(val, true);
            }
        }

        #region 插槽模块

        private void InitLocator()
        {
            if (!Locator)
            {
                Locator = gameObject.GetComponentInChildren<ELocator>();
            }

            if (Locator)
            {
                Locator.gameObject.SetActive(true);
                Locator.Layer = LayerMask.NameToLayer("Default");
                Locator.Init();
            }
        }

        /// <summary>
        /// 获取插槽
        /// </summary>
        /// <param name="_name"></param>
        /// <returns></returns>
        public override Transform GetLocator(string _name)
        {
            if (!Locator) return transform;

            var soc = Locator.FindSocket(_name);
            if (soc) return soc.transform;
            return transform;
        }

        #endregion

        #region 内置方法

        private void Awake()
        {
            base.Init();
        }

        public override void Destroy()
        {
            enabled = false;
            if(_represent) GameObject.Destroy(_represent);
            _represent = null;
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            PlaySpeed = playSpeed;
        }
#endif

        #endregion

        protected override void OnDispose()
        {
            enabled = false;
            roleCount--;
            oncePlayEnd = null;
            OnAnimationPlayEnd = null;
            transform.Reset();
            
            if (_represent) _represent.SetActive(false);
            
            if (_eAnimation) _eAnimation.Stop();

            if (gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                GameObject.Destroy(gameObject);
                return;
            }
            
            if (Locator) Locator.Layer = LayerMask.NameToLayer("Default");
            Recover(this);
        }
        
        /// <summary>
        /// 优化
        /// 一个角色上，同一时间，只有一个同类特效
        /// </summary>
        [SerializeField] private List<string> ownSfxList = new List<string>();
        public bool hasSfx(string sfxUrl)
        {
            if (ownSfxList.Contains(sfxUrl))
                return true;
            ownSfxList.Add(sfxUrl);
            return false;
        }
        public void OnSfxDispose(string sfxUrl)
        {
            ownSfxList.Remove(sfxUrl);
        }
        //=====================================================================================================
        //            静态方法
        //=====================================================================================================
        public static int roleCount = 0;
        public static PoolUtil<ECharacter> actorPool = new PoolUtil<ECharacter>(100);
        private static UnityObjectPool<ECharacter> _charactersMaps =
            new UnityObjectPool<ECharacter>((url) =>
            {
                ECharacter eCharacter = actorPool.GetPool();
                eCharacter.url = url;
#if UNITY_DEBUG
                string name = System.IO.Path.GetFileNameWithoutExtension(url);
                int index = _charactersMaps.MaxCount(url);
                eCharacter.name = $"{name}+{index}";
#endif
                return eCharacter;
            }, (string url, ECharacter ck) =>
            {
                if (ck)
                {
                    ck.enabled = false;
                    actorPool.Release(ck);
                    ck.Destroy();
                }
            }, 10);

        /// <summary>
        /// 创建角色，自带缓存，不用了 直接 Dispose，销毁，使用Destroy
        /// </summary>
        /// <param name="url"></param>
        /// <param name="startAnimationName"> 初始播放动画 </param>
        /// <param name="onLoadEndCallBack">加载完成回调事件</param>
        /// <param name="onDisposeCallBack">卸载回调事件</param>
        /// <param name="birthTag">出生标签</param>
        /// <returns></returns>
        public static ECharacter Create(string url, string startAnimationName = "",
            Action onLoadEndCallBack = null, Action onDisposeCallBack = null, string birthTag = "")
        {
            ECharacter eCharacter = _charactersMaps.Get(url);
            if (eCharacter == null)
            {
                return null;
            }

            eCharacter.enabled = true;
            eCharacter.Init();
            eCharacter.birthTag = birthTag;
            if (onDisposeCallBack != null) eCharacter.disposeEvent += onDisposeCallBack;
            eCharacter.LoadPrefab(onLoadEndCallBack);

            //一开始确认动画名，避免第一次动画播放出现，从idle跳回来的情况
            if (!string.IsNullOrEmpty(startAnimationName))
            {
                eCharacter.AnimationName = startAnimationName;
            }

            return eCharacter;
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="display"></param>
        protected static void Recover(ECharacter display)
        {
#if UNITY_DEBUG
            string name = Path.GetFileNameWithoutExtension(display.url);
            display.name = name;
#endif
            _charactersMaps.Release(display.url, display);
        }

        public static void DestoryAll()
        {
            _charactersMaps.DestoryAll();
            actorPool.UnInit();
        }
        public static void AutoClear(int count, List<string> ignoreUrlList)
        {
            _charactersMaps.AutoClear(ignoreUrlList, count);
        }
    }
}