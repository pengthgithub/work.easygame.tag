using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Easy
{

    [Serializable]
    public struct ClipData
    {
        public string name;
        public float length;
    }
    public partial class Control
    {
        [Header("动画控制")]
        /// <summary>
        /// 速度
        /// </summary>
        [SerializeField] [Range(-1, 4)] private float speed = 1;
        [SerializeField] private Transform center;
        [SerializeField] private float randomTime = 0;
        /// <summary>
        /// 动画组件
        /// </summary>
        [SerializeField] private Animation nodeAnimation;
        [SerializeField] private List<string> nodeAniList = new List<string>();
        
        /// <summary>
        /// 动画组件
        /// </summary>
        [SerializeField] private Animator animator;
        /// <summary>
        /// 动画序列
        /// </summary>
        [SerializeField] [Range(-1, 10)] private int animationIndex = -1;
        /// <summary>
        /// 需要控制的动画
        /// </summary>
        [SerializeField] private List<string> skinList = new List<string>();
        [SerializeField] public List<ClipData> allAni = new List<ClipData>();
        
               
        /// <summary>
        /// 中心点旋转
        /// </summary>
        /// <param name="value"></param>
        public float CenterAngle
        {
            get=> center ? center.localEulerAngles.x : 0;
            set { 
                if (center)
                {
                    var rangle =  center.localEulerAngles;
                    rangle.x = value;
                    center.localEulerAngles = rangle;
                }
            }

        }
        
        //==============================================================================
        // 回调处理，这个方式可能需要在改
        //==============================================================================
        #region 动画
        /// <summary>
        /// 数据优化，离线获取，如果离线获取失败,运行的时候，会在获取一次
        /// </summary>
        private void InitAnimation()
        {
            if(allAni.Count != 0) return;
            
#if !UNITY_EDITOR
            Debug.Log($"离线获取动画数据失败，优化无效 {gameObject.name}");
#endif
            allAni.Clear();
            if (!nodeAnimation)
            {
                nodeAnimation = gameObject.GetComponent<Animation>();
            }
            if (nodeAnimation)
            {
                nodeAniList.Clear();
                foreach (AnimationState anim in nodeAnimation)
                {
                    var clip = anim.clip;
                    if (clip)
                    {
                        nodeAniList.Add(clip.name);
                        allAni.Add(new ClipData() { name = clip.name, length = clip.length });
                    }
                }

                nodeAnimation.playAutomatically = false;
            }
      
            if (!animator)
            {
                animator = gameObject.GetComponentInChildren<Animator>();
            }
            if (animator && skinList.Count == 0)
            {
                RuntimeAnimatorController controller = animator.runtimeAnimatorController;
                if (!controller) return;
                skinList.Clear();
                AnimationClip[] clips = controller.animationClips;
                foreach (AnimationClip clip in clips)
                {
                    skinList.Add(clip.name);
                    allAni.Add(new ClipData() { name = clip.name, length = clip.length });
                }
            }
        }
        /// <summary>
        /// 检测是那个类型的动画
        /// </summary>
        /// <param name="name">动画名</param>
        /// <returns>0：错误 1：ani 2:animator</returns>
        private int AnimationStat(string name)
        {
            for (int i = 0, n = nodeAniList.Count; i < n; i++)
            {
                if (nodeAniList[i].Equals(name))
                {
                    return 1;
                }
            }
            for (int i = 0, n = skinList.Count; i < n; i++)
            {
                if (skinList[i].Equals(name))
                {
                    return 2;
                }
            }
            //Debug.LogError($"{gameObject.name} 没有动画:{name} 必须修改.");
            return 0;
        }

        internal void PlayClip(string clipName, AnimationClip clip = null)
        {
            if (!nodeAnimation) return;
            if (clip != null)
            {
                float len = -1;
                foreach (var ani in allAni)
                {
                    if (ani.name == clipName)
                    {
                        len = ani.length;
                        break;
                    }
                }
               
                if (len == -1)
                {
                    var _clip = nodeAnimation.GetClip(clipName);
                    if (!_clip)
                    {
                        nodeAnimation.AddClip(clip, clipName);
                        nodeAniList.Add(clipName);
                        allAni.Add(new ClipData() { name = clipName, length = clip.length });
                    }
                }
            }
            nodeAnimation.Play(clipName);
        }

        internal void PlayAnimator(string clipName, float useTime)
        {
            float aniOffset = 0;
            if (randomTime != 0) //如果启用了随机播放时间
            {
                aniOffset = randomTime;
                randomTime = 0;
            }
            
            if (useTime != 0)
            {
                float len = -1;
                foreach (var ani in allAni)
                {
                    if (ani.name == clipName)
                    {
                        len = ani.length;
                        aniOffset = useTime / len;
                        if(aniOffset >= 1) return;
                        animator.Play(clipName, 0, aniOffset);
                        return;
                    }
                }
            }
            else
            {
                animator.CrossFade(clipName, 0.05f, 0, aniOffset);
            }
        }

        public void Play(string aniName, bool calBack = false, float useTime = 0)
        {
            if(string.IsNullOrEmpty(aniName)) return;
            lastNormalizeTime = -1;
            playingName = aniName;
            needCallBack = calBack;
            int result = AnimationStat(aniName);
            switch (result)
            {
                case 0:break;
                case 1: PlayClip(aniName); break;
                case 2: PlayAnimator(aniName,useTime); break;
                default:break;
            }
        }

        public void Stop()
        {
            
        }
        #endregion
        
        private void UpdateAni()
        {
            IsCompolete();
            UpdateFrameEvent();
        }
        
        //==============================================================================
        // 回调处理，这个方式可能需要在改
        //==============================================================================
        #region 动画播放完毕回调

        private bool needCallBack = false;
        private string playingName;
        public Action<string> complete = null;
        private int lastNormalizeTime;
        /// <summary>
        /// 检测动画是否播放完毕然后回调
        /// </summary>
        private void IsCompolete()
        {
            if(!needCallBack) return;
            if (animator)
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
                    complete?.Invoke(playingName);

                    if(stat.loop == false) needCallBack = false;
                }
            }
        }
        #endregion
        //==============================================================================
        // 动画刻帧 切换 Animator上的动画
        //==============================================================================
        #region 动画刻帧
        private int _oldAnimationIndex = 0;
        private void UpdateFrameEvent()
        {
            if (_oldAnimationIndex == animationIndex)
            {
                return;
            }

            var index = animationIndex - 1;
            _oldAnimationIndex = animationIndex;
            if (animationIndex > skinList.Count || index < 0)
            {
                return;
            }

            var aniName = skinList[index];
            animator.Play(aniName);
        }
        #endregion
        
    }
}