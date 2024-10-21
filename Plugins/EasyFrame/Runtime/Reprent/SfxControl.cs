using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Easy
{
    [Icon("Packages/EasyFrame/Editor/Icon/sfx_icon.png")][ExecuteAlways] public class SfxControl : MonoBehaviour
    {
        [SerializeField] public Animator animator;
        [SerializeField] [Range(0,3)] private float speed = 1.0f;
        [SerializeField] public List<ParticleSystem> particleSystems = new List<ParticleSystem>();
        [SerializeField] public List<TrailRenderer>  trailRenders = new List<TrailRenderer>();
        [SerializeField] public List<LineRenderer> lineRenders = new List<LineRenderer>();
        [SerializeField] public List<ClipData> animLists = new List<ClipData>();
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

        public void Stop()
        {
            if (particleSystems.Count != 0)
            {
                foreach (var ps in particleSystems)
                {
                    if (ps) ps.Stop();
                }
            }

            if (animator)
            {
                foreach (var data in animLists)
                {
                    if (data.enabled)
                    {
                        animator.Play(data.name);
                        return;
                    }
                }
            }
            
        }
        
        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            gameObject.SetActive(false);
            if (trailRenders.Count != 0)
            {
                foreach (var trail in trailRenders)
                {
                    if (trail) trail.Clear();
                }
            }

            Speed = 1;
        }
        
        //====================================================================
        // 编辑器离线优化
        //====================================================================
        #region 编辑器离线优化
        #if UNITY_EDITOR
        [Header("编辑器")]
        [SerializeField] public int nodeCount;
        [SerializeField] public int renderCount;
        [SerializeField] public int particleCount;
        [SerializeField] public bool hideChild = false;
        [SerializeField] public int fileSize;
        #endif
        #endregion
    }
}