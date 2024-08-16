using System.IO;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

namespace Easy
{
    public class HardCharacterShader : BaseLitShaderGUI
    {
        // collect properties from the material properties
        public override void FindProperties(MaterialProperty[] properties)
        {
            base.FindProperties(properties);
            _customPro.Init(properties);
        }

        protected override void CustomValidateMaterial(Material material)
        {
            bool hasRamp = material.GetTexture("_RampMap") != null;
            CoreUtils.SetKeyword(material, "_RAMP_ON", hasRamp);

            float changeColorEnable = material.GetFloat("_ChangeColorEnable");
            CoreUtils.SetKeyword(material, "_CHANGECOLOR", changeColorEnable != 0);

            float colorEnable = material.GetFloat("_DiffuseColorEnable");
            CoreUtils.SetKeyword(material, "_DIFFUSE_ON", colorEnable != 0);
        }

        /// <summary>
        /// 自定义的属性有那些
        /// </summary>
        private struct CustomProperties
        {
            //模板属性设置=======================================================
            public MaterialProperty StencilCompProp { get; set; }
            public MaterialProperty StencilProp { get; set; }
            public MaterialProperty StencilOpProp { get; set; }
            public MaterialProperty StencilWriteMaskProp { get; set; }

            public MaterialProperty StencilReadMaskProp { get; set; }

            // 自定义属性=======================================================
            public MaterialProperty ShadowOffsetProp { get; set; }
            public MaterialProperty EmissionColorProp { get; set; }
            public MaterialProperty ChangeColorProp { get; set; }
            public MaterialProperty ChangeColorEnableProp { get; set; }
            public MaterialProperty RimColorProp { get; set; }
            public MaterialProperty RimParamProp { get; set; }
            public MaterialProperty SpecularColorProp { get; set; }
            public MaterialProperty SpecularOffsetProp { get; set; }
            public MaterialProperty SpecRangeProp { get; set; }
            public MaterialProperty SaturationProp { get; set; }
            public MaterialProperty RampMapProp { get; set; }
            public MaterialProperty RampEditorProp { get; set; }
            public MaterialProperty RampLightOffsetProp { get; set; }
            public MaterialProperty RampLightOffset2Prop { get; set; }
            public MaterialProperty RampParamProp { get; set; }

            public void Init(MaterialProperty[] properties)
            {
                //模板参数设置=======================================================
                StencilCompProp = FindProperty("_StencilComp", properties, false);
                StencilProp = FindProperty("_Stencil", properties, false);
                StencilOpProp = FindProperty("_StencilOp", properties, false);
                StencilWriteMaskProp = FindProperty("_StencilWriteMask", properties, false);
                StencilReadMaskProp = FindProperty("_StencilReadMask", properties, false);

                //模板参数设置=======================================================
                ShadowOffsetProp = FindProperty("_ShadowOffset", properties, false);
                EmissionColorProp = FindProperty("_EmissionColor", properties, false);
                ChangeColorProp = FindProperty("_ChangeColor", properties, false);
                ChangeColorEnableProp = FindProperty("_ChangeColorEnable", properties, false);

                RimColorProp = FindProperty("_RimColor", properties, false);
                RimParamProp = FindProperty("_RimParam", properties, false);

                SpecularColorProp = FindProperty("_SpecularColor", properties, false);
                SpecularOffsetProp = FindProperty("_SpecularOffset", properties, false);
                SpecRangeProp = FindProperty("_SpecularRange", properties, false);

                SaturationProp = FindProperty("_SaturationStrength", properties, false);

                RampMapProp = FindProperty("_RampMap", properties, false);
                RampEditorProp = FindProperty("_RampEditor", properties, false);
                RampLightOffsetProp = FindProperty("_RampLightOffset", properties, false);
                RampLightOffset2Prop = FindProperty("_RampLightOffset2", properties, false);
                RampParamProp = FindProperty("_RampParam", properties, false);
            }
        }

        private CustomProperties _customPro;

        protected void DrawVectorProperty(MaterialProperty property, string xLabel = "", string yLable = "",
            string zLabel = "",
            string wLabel = "")
        {
            Vector4 rimProp = property.vectorValue;
            rimProp.x = EditorGUILayout.Slider(xLabel, rimProp.x, -1, 1);
            rimProp.y = EditorGUILayout.Slider(yLable, rimProp.y, 0, 10);
            rimProp.z = EditorGUILayout.FloatField(zLabel, rimProp.z);
            rimProp.w = EditorGUILayout.FloatField(zLabel, rimProp.w);
            property.vectorValue = rimProp;
        }

        private Vector3 color = new Vector3();

        protected void DrawVectorColorPowerProperty(MaterialProperty property, string colorLabel = "",
            string powerLabel = "")
        {
            Vector4 rimProp = property.vectorValue;
            color.Set(rimProp.x, rimProp.y, rimProp.z);
            color = EditorGUILayout.Vector3Field(colorLabel, color);
            if (!string.IsNullOrEmpty(powerLabel))
            {
                rimProp.w = EditorGUILayout.Slider(powerLabel, rimProp.w, 0, 1);
            }

            rimProp.Set(color.x, color.y, color.z, rimProp.w);
            property.vectorValue = rimProp;
        }

        private static readonly GUIContent rampMap = EditorGUIUtility.TrTextContent("Ramp Map", "阴影控制图.");
        private static Ramp2TextureEditor rampEditor = new();

        /// <summary>
        /// 添加的自定义GUI 表现
        /// </summary>
        /// <param name="material"></param>
        protected override void CustomInput(Material material)
        {
            materialEditor.ShaderProperty(_customPro.ShadowOffsetProp, "影子偏移");

            GUILayout.Space(8);
            materialEditor.ShaderProperty(_customPro.EmissionColorProp, "自发光颜色");

            GUILayout.Space(8);
            //敌对角色换色功能
            materialEditor.ShaderProperty(_customPro.ChangeColorEnableProp, "启用敌对换色");
            if (_customPro.ChangeColorEnableProp is { floatValue: 1 })
            {
                materialEditor.ShaderProperty(_customPro.ChangeColorProp, "敌对换色");
            }

            GUILayout.Space(8);
            materialEditor.ShaderProperty(_customPro.RimColorProp, "边缘光颜色");
            Vector4 rimProp = _customPro.RimParamProp.vectorValue;
            rimProp.x = EditorGUILayout.Slider("边缘光范围", rimProp.x, -1, 1);
            rimProp.y = EditorGUILayout.Slider("边缘光强度", rimProp.y, 0, 10);
            rimProp.z = EditorGUILayout.FloatField("边缘光区域", rimProp.z);
            _customPro.RimParamProp.vectorValue = rimProp;

            GUILayout.Space(8);
            materialEditor.ShaderProperty(_customPro.SpecularColorProp, "高光颜色");
            DrawVectorColorPowerProperty(_customPro.SpecularOffsetProp, "高光偏移", "高光强度");
            materialEditor.ShaderProperty(_customPro.SpecRangeProp, "高光范围");

            GUILayout.Space(8);
            materialEditor.ShaderProperty(_customPro.SaturationProp, "饱和度");

            //光影 Ramp Texture
            materialEditor.TexturePropertySingleLine(rampMap, _customPro.RampMapProp,
                _customPro.RampEditorProp);

            if (_customPro.RampEditorProp.floatValue != 0)
            {
                rampEditor.DrawArea(material);
                GUILayout.Space(8);
            }

            Vector4 rampProp = _customPro.RampParamProp.vectorValue;
            DrawVectorColorPowerProperty(_customPro.RampLightOffsetProp, "正光偏移");
            rampProp.x = EditorGUILayout.Slider("正光内圈", rampProp.x, -1, 1);
            rampProp.y = EditorGUILayout.Slider("正光外圈", rampProp.y, 0, 10);
            GUILayout.Space(8);
            DrawVectorColorPowerProperty(_customPro.RampLightOffset2Prop, "补光偏移", "强度");
            rampProp.z = EditorGUILayout.Slider("补光内圈", rampProp.z, 0, 1);
            rampProp.w = EditorGUILayout.Slider("补光外圈", rampProp.w, 0, 1);
            _customPro.RampParamProp.vectorValue = rampProp;
        }

        private string[] _materialContent;

        private Vector4 ParseLineVector4(string line, string param)
        {
            if (line.Contains(param))
            {
                var k = line.IndexOf(":") + 1;
                var _line = line.Substring(k, line.Length - k).Replace("{", "").Replace("}", "").Trim();
                var data = _line.Replace("r:", "").Replace("g:", "").Replace("b:", "").Replace("a:", "");
                string[] _vector = data.Trim().Split(",");
                return new Vector4(float.Parse(_vector[0]), float.Parse(_vector[1]),
                    float.Parse(_vector[2]), float.Parse(_vector[3]));
            }

            return Vector4.zero;
        }

        private float ParseLineFloat(string line, string param)
        {
            if (line.Contains(param))
            {
                var k = line.IndexOf(":") + 1;
                var _line = line.Substring(k, line.Length - k).Trim();
                return float.Parse(_line.Trim());
            }

            return 0;
        }

        private bool parse = false;

        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            base.AssignNewShaderToMaterial(material, oldShader, newShader);
            if (parse) return;

            if (_materialContent == null || _materialContent.Length == 0)
            {
                string path = AssetDatabase.GetAssetPath(material);
                _materialContent = File.ReadAllLines(path);
            }

            foreach (var matLine in _materialContent)
            {
                // if (matLine.Contains("_SpecColor"))
                // {
                //     material.SetVector("_RampLightOffset", ParseLineVector4(matLine, "_Light1Dir"));
                // }
                //
                // if (matLine.Contains("_SpecularOffset"))
                // {
                //     material.SetVector("_RampLightOffset", ParseLineVector4(matLine, "_Light1Dir"));
                // }

                if (matLine.Contains("_Light1Dir"))
                {
                    material.SetVector("_RampLightOffset", ParseLineVector4(matLine, "_Light1Dir"));
                }

                if (matLine.Contains("_Light2Dir"))
                {
                    material.SetVector("_RampLightOffset2", ParseLineVector4(matLine, "_Light2Dir"));
                }

                if (matLine.Contains("_Saturation"))
                {
                    material.SetFloat("_SaturationStrength", ParseLineFloat(matLine, "_Saturation"));
                }

                Vector4 param = new Vector4();
                if (matLine.Contains("_Light1OutRange"))
                {
                    param.x = ParseLineFloat(matLine, "_Light1OutRange");
                }

                if (matLine.Contains("_Light1InRange"))
                {
                    param.y = ParseLineFloat(matLine, "_Light1InRange");
                }

                if (matLine.Contains("_Light2OutRange"))
                {
                    param.z = ParseLineFloat(matLine, "_Light2OutRange");
                }

                if (matLine.Contains("_Light2InRange"))
                {
                    param.w = ParseLineFloat(matLine, "_Light2InRange");
                }

                material.SetVector("_RampParam", param);
            }

            parse = true;
        }
    }
}