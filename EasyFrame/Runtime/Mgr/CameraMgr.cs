using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Easy
{
    /// <summary>
    /// 镜头管理器
    /// </summary>
    [RequireComponent(typeof(Animation)), DisallowMultipleComponent]
    public class CameraMgr : MonoBehaviour
    {
        public static CameraMgr Instance { get; private set; }

        //================================================================
        [SerializeField] public Animation _animation;
        private bool _bPlaying;
        private void Awake()
        {
            Instance = this;
            _animation = GetComponent<Animation>();
            _bPlaying = false;
        }
        
        //==============================================================
        #region 摄像机
        /// <summary>
        /// 住摄像机
        /// </summary>
        public static Camera MainCamera { get; set; }
        private static Vector3 _screenTemp = new Vector3();
        public static int HitGameObject(float x, float y)
        {
            if (!MainCamera) return -1;
            _screenTemp.Set(x, y, 0);
            Ray screenRay = MainCamera.ScreenPointToRay(_screenTemp);

            // Bit shift the index of the layer (8) to get a bit mask
            //layerMask += 1 << 3; //增加层级使用 | 或者 + 都可以
            // This would cast rays only against colliders in layer 8.
            // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
            // layerMask = ~layerMask;
            int layerMask = 1 << LayerMask.NameToLayer("BoxCollider");
            
            // Does the ray intersect any objects excluding the player layer
            if (!Physics.Raycast(screenRay, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                return -1;
            }

            var cc = hit.collider.gameObject.GetComponentInParent<Represent>();
            if (cc)
            {
                return (int)cc.LogicID;
            }

            return -1;
        }
        
        /// <summary>
        /// 根据屏幕坐标获取引擎坐标
        /// </summary>
        public static Vector3 ScreenToEngine(float screenX, float screenY, float z)
        {
            if (!MainCamera) return Vector3.zero;
            
            _screenTemp.Set(screenX,screenY, z);
            var pos = MainCamera.ScreenToWorldPoint(_screenTemp);
            return pos;
        }

        /// <summary>
        /// 根据引擎坐标获取屏幕坐标
        /// </summary>
        public static Vector3 EngineToScreen(float x, float y, float z)
        {
            return EngineToScreen(new Vector3(x, y, z));
        }

        /// <summary>
        /// 世界坐标转换为屏幕坐标
        /// </summary>
        /// <param name="worldPos"></param>
        /// <returns></returns>
        public static Vector3 EngineToScreen(Vector3 worldPos)
        {
            if (!MainCamera) return Vector3.zero;
            
            return MainCamera.WorldToScreenPoint(worldPos);
        }
        #endregion

        #region 适配,默认: 828x1792
        public static float UIAspect = 0.462f;
        /// <summary>
        /// 修改分辨率
        /// </summary>
        public static void ApplyScreenWidth()
        {
            var aspect = 828.0f / 1792.0f;
            var displayAspect = UIAspect;
            if (aspect.Equals(displayAspect)) return;

            if (MainCamera == null) return;
            float defaultFov = 15.0f;
            MainCamera.fieldOfView = defaultFov / (displayAspect / aspect);
        }
        #endregion
        
        //================================================================
        // 动画播放
        //================================================================
        #region 动画播放

        /// <summary>
        /// 播放摄像机动画
        /// </summary>
        /// <param name="animName">动画名字</param>
        public void Play(AnimationClip clip)
        {
            if (_bPlaying || !_animation || !clip) return;

            var clipName = clip.name;
            var aniClip = _animation.GetClip(clipName);
            if (aniClip == null)
            {
                _animation.AddClip(clip, clipName);
            }
            _animation.Play(clipName);
        }

        /// <summary>
        /// 停止播放摄像机动画
        /// </summary>
        public void Stop()
        {
            if (!_animation) return;
            _animation.gameObject.transform.Reset();
        }
        
        
        #endregion
        
        //==================================================================
        // 摄象机朝力的方向抖动
        //==================================================================
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

            Stop();
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
    
        //==================================================================
        // 摄象机朝力的方向抖动
        //==================================================================
        #region 切换摄像机
        [SerializeField] private Transform levelNode;
        /// <summary>
        /// 切换摄象机
        /// </summary>
        /// <param name="dir">0:默认 1:敌对 后续要加载</param>
        private void RotationScene(int dir = 0)
        {
            var defaultAngle = dir;
            if (levelNode)
            {
                /// 转角度
                var angle = levelNode.eulerAngles;
                angle.y = defaultAngle;
                levelNode.eulerAngles = angle;
            }
        }
        #endregion
        
        //==================================================================
        // 摄象机动画
        //==================================================================
        #region 摄象机动画
        [SerializeField] [Rename("1v1 默认值")] private Vector3 defaultPos;
        [SerializeField] [Rename("2vp 默认值")] private Vector3 mutlPos;
        
        [Rename("开场动画(单人)")] [DropdownAsset("Assets/Art/Scene/common/ani", "Camera")]
        [SerializeField] private AnimationClip singleCameraClip;

        [Rename("开场动画(多人)")] [DropdownAsset("Assets/Art/Scene/common/ani", "Camera")]
        [SerializeField] private AnimationClip mutlCameraClip;

        [Rename("退出动画")] [DropdownAsset("Assets/Art/Scene/common/ani", "Camera")]
        [SerializeField] private AnimationClip quitCameraClip;
        
        #endregion
        
        /// <summary>
        /// 摄像机类型
        /// </summary>
        public void CameraType(int t)
        {
        }
    }
}