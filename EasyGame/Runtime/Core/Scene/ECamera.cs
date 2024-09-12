using FairyGUI;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Easy
{
    public abstract class ECamera
    {
        public static Camera MainCamera { get; set; }
        public static Animation cameraAnimation;

        /// <summary>
        /// 初始化摄像机
        /// </summary>
        public static void Init()
        {
            if(!CameraManager.Instance) return;
            MainCamera = CameraManager.Instance.MainCamera;
            var uiCamera = StageCamera.main;
            if (uiCamera)
            {
                UniversalAdditionalCameraData uiCameraData = uiCamera.GetUniversalAdditionalCameraData();
                if (uiCameraData.renderType != CameraRenderType.Overlay)
                {
                    uiCameraData.renderType = CameraRenderType.Overlay;
                }

                UniversalAdditionalCameraData cameraData = MainCamera.GetUniversalAdditionalCameraData();
                if (cameraData.cameraStack.Count == 0)
                {
                    cameraData.cameraStack.Add(uiCamera);
                }
                
                Debug.Log("Add  Overlay UI camera");
            }

            cameraAnimation = MainCamera.gameObject.GetComponentInParent<Animation>();

            EExten.shiKuaiBirth = Object.FindObjectOfType<PersonBirth>();

            EExten.displayCabinet = Object.FindObjectOfType<DisplayCabinet>();
        }

        /// <summary>
        /// 播放摄像机动画
        /// </summary>
        /// <param name="animName">动画名字</param>
        public static void Play(AnimationClip clip)
        {
            if (!clip) return;
            if (CameraManager.Instance.IsPlayAnimation())
            {
                return;
            }

            if (!cameraAnimation)
            {
#if UNITY_EDITOR
                ECamera.Init();
#endif
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
        public static void Stop()
        {
            if (!cameraAnimation) return;
            cameraAnimation.gameObject.transform.Reset();
        }
    }
}