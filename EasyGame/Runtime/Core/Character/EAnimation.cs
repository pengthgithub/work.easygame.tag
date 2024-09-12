using System;
using System.Collections.Generic;
using UnityEngine;

namespace Easy
{
    /// <summary>
    /// 动画控制播放
    /// </summary>
    //[ExecuteAlways]
    public class EAnimation : MonoBehaviour
    {
        [SerializeField] [ReadOnly] public int boneCount;
        [SerializeField] [ReadOnly] public int nodeCount;

        /// <summary>
        /// 默认位置
        /// </summary>
        [SerializeField] [ReadOnly] private Vector3 localPosition;

        /// <summary>
        /// 默认旋转
        /// </summary>
        [SerializeField] [ReadOnly] private Vector3 localRotation;

        /// <summary>
        /// 默认缩放
        /// </summary>
        [SerializeField] [ReadOnly] private Vector3 localScale;

        /// <summary>
        /// 速度
        /// </summary>
        [SerializeField] [Range(0, 4)] private float speed;

        /// <summary>
        /// 动画序列
        /// </summary>
        [SerializeField] [Range(0, 10)] private int animationIndex = 0;

        /// <summary>
        /// 需要控制的动画
        /// </summary>
        [SerializeField] private List<string> animationList = new List<string>();

        /// <summary>
        /// 动画组件
        /// </summary>
        [SerializeField] [ReadOnly] public Animation animation;

        /// <summary>
        /// 动画组件
        /// </summary>
        [SerializeField] [ReadOnly] public Animator animator;

        [SerializeField] [ReadOnly] private string _logPlayingName;


        private ELocator _locator;
        private float _oldSpeed;
        private int _oldAnimationIndex;
        private int _lastPlayAnimFrame;
        private string _lastAnimationName;
        private Dictionary<string, float> _animationClipMap;
        public Dictionary<string, float> ClipMap;

        private void Awake()
        {
            speed = 1;
            animator = null;
            ClipMap = null;
            _oldSpeed = speed;
            _oldAnimationIndex = 0;
            _locator = this.GetComponent<ELocator>();
            var ts = transform;
            localPosition = ts.localPosition;
            localRotation = ts.localEulerAngles;
            localScale = ts.localScale;
            InitAnimation();
        }

        private void InitAnimation()
        {
            if (!animation)
            {
                animation = this.GetComponent<Animation>();
            }

            if (animation)
            {
                _animationClipMap ??= new Dictionary<string, float>();
                foreach (AnimationState anim in animation)
                {
                    _animationClipMap[anim.name] = anim.clip.length;
                }
            }

            if (!animator)
            {
                animator = gameObject.GetComponentInChildren<Animator>();
            }

            if (!animator) return;
            RuntimeAnimatorController controller = animator.runtimeAnimatorController;
            if (!controller) return;

            AnimationClip[] clips = controller.animationClips;
            ClipMap ??= new Dictionary<string, float>();
            ClipMap.Clear();
            foreach (AnimationClip clip in clips)
            {
                ClipMap[clip.name] = clip.length;
            }
        }

        public void Init()
        {
            var ts = transform;
            ts.localPosition = localPosition;
            ts.localEulerAngles = localRotation;
            ts.localScale = localScale;
        }

#if UNITY_EDITOR
        private void OnEnable()
        {
            Init();
        }
#endif
        private int lastNormalizeTime;
        public Action oncePlayEnd2;

        private void AnimCallBack()
        {
            if (animator && owner && owner.OnAnimationPlayEnd != null)
            {
                var stat = animator.GetCurrentAnimatorStateInfo(0);
                if (lastNormalizeTime < 0 || lastNormalizeTime > (int)stat.normalizedTime)
                {
                    lastNormalizeTime = (int)stat.normalizedTime;
                }

                float currentTime = stat.normalizedTime - lastNormalizeTime;
                if (currentTime >= 1)
                {
                    lastNormalizeTime = (int)stat.normalizedTime;
                    owner.OnAnimationPlayEnd?.Invoke(_lastAnimationName);
                }
            }

            if (canCallBack && animator)
            {
                var stat = animator.GetCurrentAnimatorStateInfo(0);
                if (stat.shortNameHash != animNameHash) return;
                if (stat.normalizedTime >= 1)
                {
                    canCallBack = false;
                    if (owner)
                    {
                        owner.oncePlayEnd?.Invoke(owner, _lastAnimationName);
                        owner.oncePlayEnd = null;
                    }

                    oncePlayEnd2?.Invoke();
                    oncePlayEnd2 = null;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            AnimCallBack();
            if (animation && !animation.isPlaying) return;

            if (_lastAnimationName == "") return;
            if (_oldSpeed.Equals(speed) == false)
            {
                _oldSpeed = speed;
                Speed = speed;
            }

            if (_oldAnimationIndex == animationIndex)
            {
                return;
            }

            var index = animationIndex - 1;
            _oldAnimationIndex = animationIndex;
            if (animationIndex > animationList.Count || index < 0)
            {
                return;
            }

            var aniName = animationList[index];
            //_logPlayingName = aniName;
            animator.Play(aniName);
        }

        private void OnDestroy()
        {
            if (animator)
            {
                // 停止动画播放
                animator.speed = 0;
                animator.StopPlayback();
                animator = null;
            }
        }

        /// <summary>
        /// 动画播放速度 只和 animator有关
        /// </summary>
        public float Speed
        {
            get => animator.speed;
            set => animator.speed = value;
        }

        public void AddClip(AnimationClip clip, string clipName)
        {
            if (!animation) return;
            var name = string.Intern(clipName);
            var _clip = animation.GetClip(name);
            if (!_clip)
            {
                animation.AddClip(clip, name);
                _animationClipMap[name] = clip.length;
            }
        }

        /// <summary>
        /// animation 播放动画
        /// </summary>
        /// <param name="aniName"></param>
        private bool _PlayAnimation(string aniName)
        {
            if (_animationClipMap == null)
            {
                return false;
            }

            _animationClipMap.TryGetValue(aniName, out float aniClip);
            if (aniClip == 0)
            {
                return false;
            }

            animation.Stop();
            animation.Play(aniName);
            return true;
        }

        /// <summary>
        /// 判断节点动画是否在播放
        /// </summary>
        /// <returns></returns>
        internal bool NodeAnimationIsPlaying()
        {
            return animation != null && animation.isPlaying;
        }

        private bool logOnce = false;
        public ECharacter owner;
        [SerializeField, Rename("回调")] private bool canCallBack = false;
        private int animNameHash;

        /// <summary>
        ///     播放动画，如果是不循环动画，动画播放完毕后会直接切回battle_idle状态
        ///     循环动画，则不处理
        /// </summary>
        /// <param name="aniName"> 动作名字</param>
        /// <param name="firstPlay"></param>
        public void PlayAnimation(string aniName, float animationPlayedTime = 0, bool debug = false)
        {
            //同一帧，反复调用播放同一动画直接返回
            if (_lastAnimationName == aniName && _lastPlayAnimFrame == Time.frameCount)
            {
                _logPlayingName = "";
                return;
            }

            if (string.IsNullOrEmpty(aniName))
            {
                _logPlayingName = "";
                return;
            }

            _lastPlayAnimFrame = Time.frameCount;
            _logPlayingName = aniName;
            bool result = _PlayAnimation(aniName);
            _lastAnimationName = aniName;
            if (result || !animator) return; // 上一层有动画，直接返回

            float clip = 0.001f;
            // animator 上的动画
            if (ClipMap != null)
            {
                ClipMap.TryGetValue(aniName, out clip);
            }

            animNameHash = Animator.StringToHash(aniName);
            if (clip == 0)
            {
                result = animator.HasState(0, animNameHash);
            }

            if (clip == 0 && result == false && logOnce == false) // 都没有动画的时候报错
            {
#if UNITY_DEBUG
                logOnce = true;
                Debug.LogError($"播放角色<{transform.name}>上的动画失败.检测角色上是否存在动画：{aniName}");
#endif
                return;
            }

            float normalizedTimeOffset = 0;
            if (animationPlayedTime > 0)
            {
                normalizedTimeOffset = animationPlayedTime / clip;
                animator.Play(animNameHash, 0, normalizedTimeOffset);
            }
            else
            {
                animator.CrossFade(animNameHash, 0.05f, 0, normalizedTimeOffset);
            }

            if (owner && (owner.oncePlayEnd != null) || oncePlayEnd2 != null)
            {
                canCallBack = true;
            }

            lastNormalizeTime = -1;
        }

        /// <summary>
        /// 停止动画
        /// </summary>
        public void Stop()
        {
            //同一动画停止时,才停止
            if (animation)
            {
                animation.Stop();
            }

            var ts = transform;
            ts.localPosition = localPosition;
            ts.localEulerAngles = localRotation;
            ts.localScale = localScale;

            speed = 1;
            animationIndex = 0;
        }
    }
}