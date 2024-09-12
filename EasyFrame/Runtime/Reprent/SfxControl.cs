using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Easy
{
    [ExecuteAlways] public class SfxControl : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] [Range(0,3)] private float speed = 1.0f;
        [SerializeField] private List<ParticleSystem> particleSystems = new List<ParticleSystem>();
        [SerializeField] private List<TrailRenderer>  trailRenders = new List<TrailRenderer>();
        [SerializeField] private List<LineRenderer> lineRenders = new List<LineRenderer>();
        
        /// <summary>
        /// 改变速度
        /// </summary>
        public float Speed
        {
            get => speed;
            set
            {
                if (!speed.Equals(value))
                {
                    speed = value;
                    ChangeSpeed();
                }
            }
        }
        private void ChangeSpeed()
        {
            if (particleSystems.Count != 0)
            {
                foreach (ParticleSystem ps in particleSystems)
                {
                    ParticleSystem.MainModule main = ps.main;
                    main.simulationSpeed = speed;
                }
            }
            if (animator) animator.speed = speed;
        }

        public void Play(string aniName)
        {
            if (particleSystems.Count != 0)
            {
                foreach (ParticleSystem ps in particleSystems)
                {
                    ps.Play();
                }
            }
            if (animator) animator.Play(aniName);
        }
        
        internal void FlowTarget(Transform locator, Transform target)
        {
            //连线必须要有目标，没有目标，连线不显示
            if (!target || lineRenders.Count == 0) return;

            Vector3 distance = locator.position - target.position;
            Vector3 directionAb = distance.normalized;
            var endPos = target.position;
            foreach (var line in lineRenders)
            {
                if (!line) continue;
                //连线特效需要当前节点 不做任何旋转和位移处理
                line.transform.position = Vector3.zero;
                line.transform.rotation = UnityEngine.Quaternion.identity;
                line.transform.localScale = Vector3.one;

                line.SetPosition(0, locator.position);
                line.SetPosition(1, endPos);
            }

            // 将粒子特效按照目标方向缩放
            if (particleSystems.Count != 0)
            {
                foreach (var ps in particleSystems)
                {
                    var psTs = ps.transform;
                    psTs.position = target.position;
                    psTs.rotation = UnityEngine.Quaternion.FromToRotation(Vector3.forward, directionAb);

                    var localScale = psTs.localScale;
                    localScale.z = distance.z;
                    psTs.localScale = localScale;
                }
            }
        }
        
        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            if (trailRenders.Count != 0)
            {
                foreach (var trail in trailRenders)
                {
                    if (trail) trail.Clear();
                }
            }
            if (particleSystems.Count != 0)
            {
                foreach (var ps in particleSystems)
                {
                    if (ps) ps.Stop();
                }
            }

            Speed = 1;
        }
        
        //====================================================================
        // 编辑器离线优化
        //====================================================================
        #region 编辑器离线优化
        #if UNITY_EDITOR
        [SerializeField] public int nodeCount;
        [SerializeField] public int particleCount;
        [SerializeField] public bool hideChild = false;
        [SerializeField] public int fileSize;
        /// <summary>
        /// 加倍
        /// </summary>
        public void Add()
        {
            var ps = gameObject.GetComponentsInChildren<ParticleSystem>();
            foreach (var ren in ps)
            {
                var main = ren.main;
                main.maxParticles += main.maxParticles;
            }
            
            OnValidate();
        }
        /// <summary>
        /// 减半
        /// </summary>
        public void Sub()
        {
            var ps = gameObject.GetComponentsInChildren<ParticleSystem>();
            foreach (var ren in ps)
            {
                var main = ren.main;
                if (main.maxParticles > 1)
                {
                    main.maxParticles = (int)(main.maxParticles * 0.5f);
                }
            }
            OnValidate();
        }

        public void CalSize()
        {
            var path = AssetDatabase.GetAssetPath(this);
            if (!string.IsNullOrEmpty(path))
            {
                FileInfo fileInfo = new FileInfo(path);
                // 检查文件是否存在
                if (fileInfo.Exists)
                {
                    fileSize = (int)(fileInfo.Length/1024); // 文件大小（以字节为单位）
                    fileInfo = null;
                }
            }
        }
        
        private void OnValidate()
        {
            transform.GetChild(0).hideFlags = hideChild ? HideFlags.HideInHierarchy : HideFlags.None;
            nodeCount = gameObject.GetComponentsInChildren<Transform>().Length;
            
            particleCount = 0;
            particleSystems.Clear();
            var ps = gameObject.GetComponentsInChildren<ParticleSystem>();
            foreach (var ren in ps)
            {
                particleSystems.Add(ren);
                particleCount += ren.main.maxParticles;
            }
            
            trailRenders.Clear();
            var renders = gameObject.GetComponentsInChildren<TrailRenderer>();
            foreach (var ren in renders)
            {
                trailRenders.Add(ren);
            } 
            
            lineRenders.Clear();
            var lineRender = gameObject.GetComponentsInChildren<LineRenderer>();
            foreach (var ren in lineRender)
            {
                lineRenders.Add(ren);
            } 
        }
        #endif
        #endregion
    }
}