using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Easy
{
    public partial class Control
    {
        [Header("材质控制")]
       /// <summary>
       /// 
       /// </summary>
        [SerializeField] private List<Material> materials;
        private void InitMat()
        {
            if (materials != null)
            {
                materials.Clear();
            }
            else
            {
                materials = new List<Material>();
            }
            if (renders == null || renders.Length == 0) return;
            foreach (var render in renders)
            {
                foreach (var mat in render.materials)
                {
                    materials.Add(mat);
                }
            }
        }
        
        [SerializeField] [Tooltip("半透")][Range(0,1)] private float matFade = 1;

        [SerializeField] [Tooltip("边缘光颜色")]
        private Color matRimColor;

        [SerializeField] private Vector4 _matRimParam;
        [SerializeField] [Tooltip("边缘光范围")] private float rimRange;
        [SerializeField] [Tooltip("边缘光强度")] private float rimPower;
        [SerializeField] [Tooltip("边缘光区域")] private float rimArea;

        [SerializeField] [Tooltip("Alpha小于128,使用公共的描边颜色。")]
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
            foreach (var mat in materials)
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
            foreach (var mat in materials)
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
        }

        internal void ResetMaterial()
        {
            
        }
    }
}