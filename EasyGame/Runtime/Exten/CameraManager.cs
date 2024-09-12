using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Easy
{
    /// <summary>
    ///     摄象机上的功能
    ///     1、摄象机切换，敌对目标，和 自己。功能需要保持 界面画面不变。
    ///     所以，用了2个摄象机，然后其中一个摄象机旋转180度，然后在把场景旋转180度，就可以保持画面不变，但是又是敌对视角.
    ///     2、摄象机抖动
    ///     3、摄象机辅助线，在摄象机的中心点，绘制一条射线，方便美术对齐位置.
    /// </summary>
    //[ExecuteAlways]
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] [Rename("长宽比")] public static float uiAspect = 0.462f;

        [SerializeField] [Rename("1v1 默认值")] private Vector3 defaultPos;

        [SerializeField] [Rename("2vp 默认值")] private Vector3 mutlPos;

        [Header("启用和非启用会赋值参数")] [SerializeField] [ReadOnly]
        private Camera defaultCamera;

        [SerializeField] [ReadOnly] private Transform defaultNode;

        [SerializeField] [ReadOnly] private Transform uiNode;

        [SerializeField] [ReadOnly] private Animation _animation;
        [Header("需要设置的参数")] [SerializeField] private Transform levelNode;

        [SerializeField] [Rename("单人动画")] [DropdownAsset("Assets/Art/Scene/common/ani", "Camera")]
        private AnimationClip singleCameraClip;

        [SerializeField] [Rename("多人动画")] [DropdownAsset("Assets/Art/Scene/common/ani", "Camera")]
        private AnimationClip mutlCameraClip;

        [SerializeField] [Rename("退出动画")] [DropdownAsset("Assets/Art/Scene/common/ani", "Camera")]
        private AnimationClip quitCameraClip;

        [SerializeField] [Rename("默认FOV")] private float defaultFov = 15.0f;
        [SerializeField] [Rename("设计宽度")] private float designWidth = 828.0f;
        [SerializeField] [Rename("设计高度")] private float designHeight = 1792;

        [SerializeField] [Rename("UI场景")] private bool uiScene;
        [SerializeField] [Rename("玩家位置")] public PersonBirth birthNode;

        [FormerlySerializedAs("_ameraType")]
        [Header("调试信息")]
        [SerializeField]
        [Rename("摄象机位置(调试)")]
        [CustomPop("默认视角", "敌对视角", "多人视角")]
        private int _cameraType = -1;

        [SerializeField] [Rename("摄象机位置(调试)")] private bool _camaerAni;

        private SceneRoot wujianRoot;

        public static CameraManager Instance { get; private set; }


        /// <summary>
        ///     主摄象机
        /// </summary>
        public Camera MainCamera { get; set; }

        /// <summary>
        ///     0:默认摄象机  1：敌对摄象机  2：多人摄象机 3: UI场景上的摄象机
        /// </summary>
        public int CameraType
        {
            get => _cameraType;
            set
            {
                _cameraType = value;
                if (!MainCamera || uiScene) return;

                switch (value)
                {
                    case 0:
                    case 1:
                    {
                        MainCamera.transform.SetParent(defaultNode, false);
                        RotationScene(value == 0 ? 0 : 180);
                        MainCamera.transform.Reset();
                        MainCamera.transform.localPosition = defaultPos;
                    }
                        break;
                    case 2:
                    {
                        MainCamera.transform.SetParent(defaultNode, false);
                        RotationScene();
                        MainCamera.transform.Reset();
                        MainCamera.transform.localPosition = mutlPos;
                    }
                        break;
                    case 3:
                    {
                        MainCamera.transform.SetParent(uiNode);
                        MainCamera.transform.Reset();
                    }
                        break;
                }
            }
        }

        public bool CameraAnimation
        {
            get => _camaerAni;
            set
            {
                _camaerAni = value;
                if (value == false)
                {
                    //退出动画
                    if (quitCameraClip) _animation.Play(quitCameraClip.name);
                    return;
                }

                if (_cameraType == 0 || _cameraType == 1) _animation.Play(singleCameraClip.name);

                if (_cameraType == 2) _animation.Play(mutlCameraClip.name);
            }
        }

        public void Awake()
        {
            Instance = this;
            wujianRoot = GetComponent<SceneRoot>();
            if (wujianRoot == null) wujianRoot = gameObject.AddComponent<SceneRoot>();
            if (defaultCamera == null) defaultCamera = gameObject.GetComponentInChildren<Camera>();
            wujianRoot.wujian = levelNode;
        }

        private void Start()
        {
            MainCamera = defaultCamera;
            if (MainCamera) MainCamera.enabled = true;
            MainCamera.fieldOfView = defaultFov;
            MainCamera.transform.Reset();
            if (uiScene)
            {
                MainCamera.transform.SetParent(uiNode);
                MainCamera.transform.Reset();
            }
            else
            {
                MainCamera.transform.localPosition = defaultPos;
            }

            ApplyScreenWidth();
            ECamera.Init();
        }

        public void CreateSceneElement(string url, Vector3 pos)
        {
            if (wujianRoot) wujianRoot.CreateSceneElement(url, pos);
        }

        public void ChangeBattleRes(string bgUrl, string islandUrl, string effectUrl)
        {
            if (wujianRoot) wujianRoot.ChangeBattleRes(bgUrl, islandUrl, effectUrl);
        }

        public void ApplyScreenWidth()
        {
            var aspect = designWidth / designHeight;
            var displayAspect = uiAspect;
            if (aspect.Equals(displayAspect)) return;

            if (MainCamera == null) return;

            MainCamera.fieldOfView = defaultFov / (displayAspect / aspect);
        }

        /// <summary>
        ///     切换摄象机
        /// </summary>
        /// <param name="dir">0:默认 1:敌对 后续要加载</param>
        private void RotationScene(int dir = 0)
        {
            var defaultAngle = dir;
            if (levelNode)
                if (levelNode)
                {
                    /// 转角度
                    var angle = levelNode.eulerAngles;
                    angle.y = defaultAngle;
                    levelNode.eulerAngles = angle;
                }
        }

// #if UNITY_EDITOR
//         private void OnValidate()
//         {
//             if (_animation == null) return;
//             CameraType = _cameraType;
//             CameraAnimation = _camaerAni;
//         }
//
//         private void OnDrawGizmos()
//         {
//             if (MainCamera == null) return;
//
//             Gizmos.color = Color.red;
//             var dis = MainCamera.farClipPlane;
//             var start = transform.position;
//             var end = transform.forward * dis;
//             Gizmos.DrawLine(start, end);
//         }
// #endif

        #region 摄象机朝力的方向抖动

        private Vector3 _defaultPoision;
        private Vector3 _aniEndPoision;
        private float _aniTime;
        private float _aniTotalTime;
        private float _aniBackTime;

        /// <summary>
        ///     摄象机抖动
        /// </summary>
        public void CameraShark(float dir, float power)
        {
            if (MainCamera == null) return;

            ECamera.Stop();
            if (!IsPlayAnimation()) _defaultPoision = MainCamera.transform.position;

            var len = power;
            if (_aniTotalTime == 0)
            {
                _aniTotalTime = 0.2f;
                _aniTime = 0.2f;
            }
            else
            {
                len = power * _aniTime / _aniTotalTime;
                _aniTime = 0.2f;
            }

            _aniEndPoision.x = MainCamera.transform.position.x - Cos(dir) * len;
            _aniEndPoision.y = MainCamera.transform.position.y - Sin(dir) * len;
            _aniEndPoision.z = MainCamera.transform.position.z;
        }


        private void Update()
        {
            if (_aniTime > 0)
            {
                _aniTime -= Time.deltaTime;
                if (_aniTime <= 0)
                {
                    _aniTime = 0;
                    _aniTotalTime = 0;
                    _aniEndPoision = Vector3.zero;
                    _aniBackTime = 0.2f;
                }
                else
                {
                    Vector3 tempPos;
                    var time = _aniTotalTime - _aniTime;

                    tempPos.x = BounceOut(time, _defaultPoision.x, _aniEndPoision.x - _defaultPoision.x, _aniTotalTime);
                    tempPos.y = BounceOut(time, _defaultPoision.y, _aniEndPoision.y - _defaultPoision.y, _aniTotalTime);
                    tempPos.z = _defaultPoision.z;

                    MainCamera.transform.position = tempPos;
                }
            }
            else if (_aniBackTime > 0)
            {
                _aniBackTime -= Time.deltaTime;
                var num = _defaultPoision.x - MainCamera.transform.position.x;
                var num2 = _defaultPoision.y - MainCamera.transform.position.y;
                var num3 = num * num + num2 * num2;
                if (num3 <= 0.001)
                    MainCamera.transform.position = _defaultPoision;
                else
                    MainCamera.transform.position = Vector3.Lerp(MainCamera.transform.position, _defaultPoision, 0.1f);
            }
        }

        public bool IsPlayAnimation()
        {
            if (_aniTime > 0 || _aniBackTime > 0) return true;

            return false;
        }

        private static float Sin(float dir)
        {
            dir = MathF.Floor(dir);
            dir = dir % 360;
            if (dir < 0) dir += 360;

            if (dir <= 90)
                return MathF.Sin(dir * 0.01745f);
            if (dir <= 180)
                return MathF.Sin((180 - dir) * 0.01745f);
            if (dir <= 270)
                return -MathF.Sin((dir - 180) * 0.01745f);
            return -MathF.Sin((360 - dir) * 0.01745f);
        }

        private static float Cos(float dir)
        {
            return Sin(90 - dir);
        }

        private static float BounceOut(float t, float b, float c, float d)
        {
            if ((t /= d) < 1 / 2.75f) return c * (7.5625f * t * t) + b;
            if (t < 2 / 2.75f) return c * (7.5625f * (t -= 1.5f / 2.75f) * t + 0.75f) + b;
            if (t < 2.5f / 2.75f) return c * (7.5625f * (t -= 2.25f / 2.75f) * t + 0.9375f) + b;
            return c * (7.5625f * (t -= 2.625f / 2.75f) * t + 0.984375f) + b;
        }

        #endregion
    }
}