using System;
using System.Collections.Generic;

using UnityEngine;
namespace Easy
{
    /// <summary>
    /// 控制器
    /// </summary>
    [Icon("Packages/EasyFrame/Editor/Icon/icon.png")]
    public partial class Control : MonoBehaviour
    {
        [Header("基础信息")]
        
        /// <summary>
        /// 基础信息
        /// </summary>
        [SerializeField] private bool active;
        [SerializeField] public Renderer[] renders;
        [SerializeField] private int sortOrder;
        private void Awake()
        {
            if (renders == null)
            {
                renders = gameObject.GetComponentsInChildren<Renderer>();
            }
            
            InitMat();
        }

        //====================================================================
        // 基础设置
        //====================================================================
        #region 基础设置
        public bool Active
        {
            get => active;
            set=>_active(value);
        }
        private void _active(bool val)
        {
            if(active == val) return;
            active = val;
            
            if(renders == null) return;
            foreach (var ren in renders)
            {
                ren.enabled = val;
            }
        }
        
        private int layer;
        public int Layer
        {
            get => layer;
            set
            {
                if(layer == value) return;
                layer = value;
                if(renders == null) return;
                foreach (var ren in renders)
                {
                    ren.gameObject.layer = value;
                }
            }
        }

        public int SortOrder
        {
            get => sortOrder;
            set
            {
                if(sortOrder == value) return;
                sortOrder = value;
                if(renders == null) return;
                foreach (var ren in renders)
                {
                    if (ren is SpriteRenderer)
                    {
                        var spRen = ren as SpriteRenderer;
                        spRen.sortingOrder = value;
                    }
                }
            }
        }
 
        #endregion

        private void LateUpdate()
        {
            UpdateAni();
        }

        internal void Dispose()
        {
            
        }

        //====================================================================
        // 编辑器离线优化
        //====================================================================
        #region 编辑器离线优化
#if UNITY_EDITOR
        [SerializeField] public bool debug;
        [SerializeField] public int boneCount;
        [SerializeField] public int nodeCount;
        [SerializeField] public int particleCount;
        [SerializeField][Range(0.2f,2f)] public float boxScale = 1;
        
        [SerializeField] public bool hideChild = false;
        [SerializeField] public bool tabChild = true;
        private void OnValidate()
        {
            var boxCollider = gameObject.GetComponentInChildren<BoxCollider>();
            if (boxCollider != null)
            {
               var ren = boxCollider.GetComponent<Renderer>();
               if (ren)
               {
                    boxCollider.size = ren.bounds.size * boxScale;
               }
            }
            transform.GetChild(0).hideFlags = hideChild ? HideFlags.HideInHierarchy : HideFlags.None;
            nodeCount = gameObject.GetComponentsInChildren<Transform>().Length;
            if(center == null) center = transform.FindChildByName("center");
            if (materials != null)
            {
                materials.Clear();
            }
            else
            {
                materials = new List<Material>();
            }
            renders = gameObject.GetComponentsInChildren<Renderer>();
            if (renders != null)
            {
                boneCount = 0;
                foreach (var ren in renders)
                {
                    if (ren is SkinnedMeshRenderer)
                    {
                        var skin = ren as SkinnedMeshRenderer;
                        boneCount += skin.bones.Length;
                    }
                    
                    foreach (var mat in ren.sharedMaterials)
                    {
                        materials.Add(mat);
                    }
                }
            }

            particleCount = 0;
            var ps = gameObject.GetComponentsInChildren<ParticleSystem>();
            foreach (var ren in ps)
            {
                particleCount += ren.main.maxParticles;
            }
            
            InitAnimation();
        }
        #endif
        #endregion
    }
}