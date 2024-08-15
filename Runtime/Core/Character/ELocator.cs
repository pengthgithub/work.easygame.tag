using System;
using System.Collections.Generic;
using UnityEngine;

namespace Easy
{
    //[ExecuteAlways]
    public class ELocator : MonoBehaviour
    {
        /// <summary>
        ///     插槽集合
        /// </summary>
        [SerializeField] public List<GameObject> socketList = new List<GameObject>();

        /// <summary>
        /// 蒙皮动画列表
        /// </summary>
        [SerializeField] public Renderer[] renderArrays;

        private GameObject[] gameObjectArrays;
        [SerializeField] public List<Material> renderMaterials;

        [SerializeField] [Rename("刻帧用的")] internal bool active = false;
        private int _layer;
        public int Layer
        {
            get => _layer;
            set
            {
                _layer = value;
                if (gameObjectArrays != null)
                {
                    foreach (var render in gameObjectArrays)
                    {
                        render.layer = value;
                    }
                } 
            }
        }

        /// <summary>
        ///     是否调试，用于打断点
        /// </summary>
        [SerializeField] public bool debugModel;

        private Animation _animation;

        private void Awake()
        {
            _animation = gameObject.GetComponent<Animation>();
            renderArrays = gameObject.GetComponentsInChildren<Renderer>();
            renderMaterials = new List<Material>();
            gameObjectArrays = new GameObject[renderArrays.Length];
            //if (_propertyBlock == null) _propertyBlock = new MaterialPropertyBlock();
            var index = 0;
            foreach (Renderer skinnedMeshRenderer in renderArrays)
            {
                gameObjectArrays[index] = skinnedMeshRenderer.gameObject;
                //skinnedMeshRenderer.SetPropertyBlock(_propertyBlock);
                index++;
                Material[] mats;
                mats = skinnedMeshRenderer.materials;
                foreach (var mat in mats)
                {
                    renderMaterials.Add(mat);
                }
            }

            foreach (var sock in socketList)
            {
                if (sock)
                {
                    locatorMap[sock.name] = sock.transform;
                }
            }
            _matRimParam = new Vector4();
            Init();
        }

        public void Init()
        {
            active = false;
            lastActive = false;
            
            matFade = 1;
            outLineWidth = 1;
            
            Alpha = 1;
            rimPower = 0; 
        }
        
        private bool lastActive = false;
        internal void Show(bool value, bool isForce = false)
        {
            if (lastActive == value && !isForce) return;
            lastActive = value;
            #if UNITY_DEBUG
            Debug.LogWarning($"{transform.parent.name} {Time.frameCount} value:{value}.");
            #endif
            foreach (Renderer skinnedMeshRenderer in renderArrays)
            {
                skinnedMeshRenderer.enabled = value;
            }
        }

        private Dictionary<string, Transform> locatorMap = new Dictionary<string, Transform>();

        public Transform FindSocket(string sockName)
        {
            if (socketList == null) return transform;

            bool result = locatorMap.TryGetValue(sockName, out Transform ts);
            if (result) return ts;

            return transform;
        }

        #region 材质属性修改

        [SerializeField] [Tooltip("半透")] private float matFade = 1;

        [Header("边缘光")] [SerializeField] [Tooltip("边缘光颜色")]
        private Color matRimColor;

        [SerializeField] private Vector4 _matRimParam;
        [SerializeField] [Tooltip("边缘光范围")] private float rimRange;
        [SerializeField] [Tooltip("边缘光强度")] private float rimPower;
        [SerializeField] [Tooltip("边缘光区域")] private float rimArea;

        [Header("描边")] [SerializeField] [Tooltip("Alpha小于128,使用公共的描边颜色。")]
        private Color outLineColor;

        [SerializeField] [Tooltip("描边")] private bool enableOutLine;
        [SerializeField] [Tooltip("描边宽度")] private float outLineWidth = 1.0f;

        public float Alpha
        {
            get => matFade;
            set
            {
                if (!value.Equals(matFade))
                {
                    matFade = value;
                }
            }
        }

        public Color RimColor
        {
            get => matRimColor;
            set
            {
                if (matRimColor.Equals(value) == false)
                {
                    matRimColor = value;
                }
            }
        }

        public float RimRange
        {
            get => rimRange;
            set
            {
                if (!value.Equals(rimRange))
                {
                    rimRange = value;
                }
            }
        }

        public float RimPower
        {
            get => rimPower;
            set
            {
                if (!value.Equals(rimPower))
                {
                    rimPower = value;
                }
            }
        }

        public float RimArea
        {
            get => rimArea;
            set
            {
                if (!value.Equals(rimArea))
                {
                    rimArea = value;
                }
            }
        }

        public Color OutLineColor
        {
            get => outLineColor;
            set
            {
                if (outLineColor.Equals(value) == false)
                {
                    outLineColor = value;
                }
            }
        }

        public bool EnableOutLine
        {
            get => enableOutLine;
            set
            {
                outLineColor.a = value ? 1 : 0;
                enableOutLine = value;
                if (value == false)
                {
                    OutLineWidth = 1;
                }
            }
        }

        public float OutLineWidth
        {
            get => outLineWidth;
            set
            {
                if (!value.Equals(outLineWidth))
                {
                    outLineWidth = value;
                }
            }
        }

        private bool _changeColor = false;

        public bool ChangeColor
        {
            get => _changeColor;
            set
            {
                if (value != _changeColor)
                {
                    ModifyMatOnce(true);
                }
                _changeColor = value;
            }
        }

        private Texture2D _maskTexture;
        public Texture2D MaskTexture
        {
            get => _maskTexture;
            set { _maskTexture = value; }
        }

        private float _maskPower;
        public float MaskPower
        {
            get => _maskPower;
            set { _maskPower = value; }
        }
        private float _lastShadowOffset;
        public float shadowOffset;

        private MaterialPropertyBlock _propertyBlock;
        internal bool canUpdate = false;

        private void ModifyMatOnce(bool changColor)
        {
            foreach (var mat in renderMaterials)
            {
                if (changColor)
                {
                    if (_changeColor) mat.EnableKeyword("_CHANGECOLOR");
                    else mat.DisableKeyword("_CHANGECOLOR");
                }
            }
        }
        
        /// <summary>
        /// 这是一个很耗时的过程，需要在每帧的最后一次调用，由标签调用
        /// </summary>
        /// <param name="modifyType"></param>
        internal void ModifyParam(bool enableAlpha, bool enableOutLine, bool rimEnable, bool enableMaskTexture)
        {
            
            foreach (var mat in renderMaterials)
            {
                if(enableAlpha) mat.SetFloat("_Alpha", matFade);
               
                if (rimEnable)
                {
                    _matRimParam.Set(rimRange, rimPower, rimArea, 1);
                    mat.SetColor("_RimColor", matRimColor);
                    mat.SetVector("_RimParam", _matRimParam);
                }

                if (enableOutLine)
                {
                    mat.SetColor("_OutLineColorOffset", outLineColor);
                    mat.SetFloat("_OutLineWithOffset", outLineWidth);
                }

                if (enableMaskTexture)
                {
                    mat.SetTexture("_EffectMap", _maskTexture);
                    mat.SetFloat("_EffectPower", _maskPower);
                }

                if (_lastShadowOffset != shadowOffset)
                {
                    mat.SetFloat("_ShadowHeightOffset", shadowOffset);
                    _lastShadowOffset = shadowOffset;
                }
            }

            // _propertyBlock.SetFloat("_Alpha", matFade);
            // _matRimParam.Set(rimRange, rimPower, rimArea, 1);
            // _propertyBlock.SetColor("_RimColor", matRimColor);
            // _propertyBlock.SetVector("_RimParam", _matRimParam);
            // _propertyBlock.SetColor("_OutLineColorOffset", outLineColor);
            // _propertyBlock.SetFloat("_OutLineWithOffset", outLineWidth);
            // // if (_changeColor) _propertyBlock.EnableKeyword("_CHANGECOLOR");
            // // else _propertyBlock.DisableKeyword("_CHANGECOLOR");
            //
            // foreach (Renderer skinnedMeshRenderer in renderArrays)
            // {
            //     skinnedMeshRenderer.SetPropertyBlock(_propertyBlock);
            // }
        }

        #endregion
    }
}