using UnityEngine;
namespace Easy
{
    /// <summary>
    /// 镜头管理器
    /// </summary>
    [RequireComponent(typeof(Animation)), DisallowMultipleComponent]
    public abstract class CameraMgr : MonoBehaviour
    {
        public static CameraMgr Instance { get; private set; }
        
        /// <summary>
        /// 住摄像机
        /// </summary>
        public static Camera MainCamera { get; set; }
        
        public Animation cameraAnimation;
        private bool _bPlaying;

        private void Awake()
        {
            Instance = this;
            cameraAnimation = GetComponent<Animation>();
            _bPlaying = false;
        }

        /// <summary>
        /// 播放摄像机动画
        /// </summary>
        /// <param name="animName">动画名字</param>
        public void Play(AnimationClip clip)
        {
            if (!clip) return;
            if (_bPlaying)
            {
                return;
            }

            if (!cameraAnimation)
            {
                return;
            }

            var _clip = cameraAnimation.GetClip(clip.name);
            if (_clip == null)
            {
                cameraAnimation.AddClip(clip, clip.name);
            }

            cameraAnimation.Play(clip.name);
        }

        /// <summary>
        /// 停止播放摄像机动画
        /// </summary>
        public void Stop()
        {
            if (!cameraAnimation) return;
            cameraAnimation.gameObject.transform.Reset();
        }
    }
}