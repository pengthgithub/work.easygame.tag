using System;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEditor.Rendering.Universal.ShaderGUI;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace Easy
{
    public class LitDetailGUI
    {
        public static void DoDetailArea(LitProperties properties, MaterialEditor materialEditor)
        {
            materialEditor.TexturePropertySingleLine(Styles.detailMaskText, properties.detailMask);
            materialEditor.TexturePropertySingleLine(Styles.detailAlbedoMapText, properties.detailAlbedoMap,
                properties.detailAlbedoMap.textureValue != null ? properties.detailAlbedoMapScale : null);
            if (properties.detailAlbedoMapScale.floatValue != 1.0f)
                EditorGUILayout.HelpBox(Styles.detailAlbedoMapScaleInfo.text, MessageType.Info, true);

            var detailAlbedoTexture = properties.detailAlbedoMap.textureValue as Texture2D;
            if (detailAlbedoTexture != null && GraphicsFormatUtility.IsSRGBFormat(detailAlbedoTexture.graphicsFormat))
                EditorGUILayout.HelpBox(Styles.detailAlbedoMapFormatError.text, MessageType.Warning, true);

            materialEditor.TexturePropertySingleLine(Styles.detailNormalMapText, properties.detailNormalMap,
                properties.detailNormalMap.textureValue != null ? properties.detailNormalMapScale : null);
            materialEditor.TextureScaleOffsetProperty(properties.detailAlbedoMap);
        }

        public static void SetMaterialKeywords(Material material)
        {
            if (material.HasProperty("_DetailAlbedoMap") && material.HasProperty("_DetailNormalMap") &&
                material.HasProperty("_DetailAlbedoMapScale"))
            {
                var isScaled = material.GetFloat("_DetailAlbedoMapScale") != 1.0f;
                var hasDetailMap = material.GetTexture("_DetailAlbedoMap") || material.GetTexture("_DetailNormalMap");
                CoreUtils.SetKeyword(material, "_DETAIL_MULX2", !isScaled && hasDetailMap);
                CoreUtils.SetKeyword(material, "_DETAIL_SCALED", isScaled && hasDetailMap);
            }
        }

        internal static class Styles
        {
            public static readonly GUIContent detailInputs = EditorGUIUtility.TrTextContent("Detail Inputs",
                "These settings define the surface details by tiling and overlaying additional maps on the surface.");

            public static readonly GUIContent detailMaskText = EditorGUIUtility.TrTextContent("Mask",
                "Select a mask for the Detail map. The mask uses the alpha channel of the selected texture. The Tiling and Offset settings have no effect on the mask.");

            public static readonly GUIContent detailAlbedoMapText = EditorGUIUtility.TrTextContent("Base Map",
                "Select the surface detail texture.The alpha of your texture determines surface hue and intensity.");

            public static readonly GUIContent detailNormalMapText = EditorGUIUtility.TrTextContent("Normal Map",
                "Designates a Normal Map to create the illusion of bumps and dents in the details of this Material's surface.");

            public static readonly GUIContent detailAlbedoMapScaleInfo =
                EditorGUIUtility.TrTextContent(
                    "Setting the scaling factor to a value other than 1 results in a less performant shader variant.");

            public static readonly GUIContent detailAlbedoMapFormatError =
                EditorGUIUtility.TrTextContent("This texture is not in linear space.");
        }

        public struct LitProperties
        {
            public MaterialProperty detailAlbedoMap;
            public MaterialProperty detailAlbedoMapScale;
            public MaterialProperty detailMask;
            public MaterialProperty detailNormalMap;
            public MaterialProperty detailNormalMapScale;

            public LitProperties(MaterialProperty[] properties)
            {
                detailMask = BaseShaderGUI.FindProperty("_DetailMask", properties, false);
                detailAlbedoMapScale = BaseShaderGUI.FindProperty("_DetailAlbedoMapScale", properties, false);
                detailAlbedoMap = BaseShaderGUI.FindProperty("_DetailAlbedoMap", properties, false);
                detailNormalMapScale = BaseShaderGUI.FindProperty("_DetailNormalMapScale", properties, false);
                detailNormalMap = BaseShaderGUI.FindProperty("_DetailNormalMap", properties, false);
            }
        }
    }


    public class HSQBShader : BaseShaderGUI
    {
        private static readonly string[] workflowModeNames = Enum.GetNames(typeof(LitGUI.WorkflowMode));
        private static readonly GUIContent rampContent = new("光照图");
        private static readonly GUIContent effectContent = new("覆盖效果图");

        private readonly RampTextureEditor rampEditor = new();

        private Vector3 color;
        private MaterialProperty effectMapProp;
        private MaterialProperty effectPowerProp;

        private MaterialProperty lightOffsetProp;
        private LitDetailGUI.LitProperties litDetailProperties;

        private LitGUI.LitProperties litProperties;
        private MaterialProperty rampMapEditorProp;
        private MaterialProperty rampMapProp;
        private MaterialProperty rampParamProp;
        private MaterialProperty specularColorProp;
        private MaterialProperty specularOffsetProp;

        public override void FillAdditionalFoldouts(MaterialHeaderScopeList materialScopesList)
        {
            materialScopesList.RegisterHeaderScope(LitDetailGUI.Styles.detailInputs, Expandable.Details,
                _ => LitDetailGUI.DoDetailArea(litDetailProperties, materialEditor));
        }

        // collect properties from the material properties
        public override void FindProperties(MaterialProperty[] properties)
        {
            base.FindProperties(properties);
            litProperties = new LitGUI.LitProperties(properties);
            litDetailProperties = new LitDetailGUI.LitProperties(properties);

            lightOffsetProp = FindProperty("_LightOffset", properties, false);

            specularColorProp = FindProperty("_CustomSpecColor", properties, false);
            specularOffsetProp = FindProperty("_SpecularOffset", properties, false);
            rampMapProp = FindProperty("_RampMap", properties, false);
            rampMapEditorProp = FindProperty("_RampEditor", properties, false);
            rampParamProp = FindProperty("_RampParam", properties, false);

            effectMapProp = FindProperty("_EffectMap", properties, false);
            effectPowerProp = FindProperty("_EffectPower", properties, false);
        }

        // material changed check
        public override void ValidateMaterial(Material material)
        {
            //如果要修改材质的属性，打开然后保存材质提交即可，然后在注释
            // if (material.HasProperty("_SpecColor"))
            // {
            //     Color _color = material.GetColor("_SpecColor");
            //
            //     material.SetColor("_CustomSpecColor", _color);
            // }

            rampEditor.Init(material);
            SetMaterialKeywords(material, LitGUI.SetMaterialKeywords, LitDetailGUI.SetMaterialKeywords);
        }

        // material main surface options
        public override void DrawSurfaceOptions(Material material)
        {
            // Use default labelWidth
            EditorGUIUtility.labelWidth = 0f;

            if (litProperties.workflowMode != null)
                DoPopup(LitGUI.Styles.workflowModeText, litProperties.workflowMode, workflowModeNames);

            base.DrawSurfaceOptions(material);
        }

        // material main surface inputs
        public override void DrawSurfaceInputs(Material material)
        {
            base.DrawSurfaceInputs(material);
            LitGUI.Inputs(litProperties, materialEditor, material);
            DrawEmissionProperties(material, true);
            DrawTileOffset(materialEditor, baseMapProp);

            GUILayout.Space(8);

            materialEditor.TexturePropertySingleLine(effectContent, effectMapProp, effectPowerProp);

            materialEditor.ShaderProperty(specularColorProp, "高光颜色,Alpha:强度");

            //DrawVectorColorPowerProperty(litProperties.specColor, "高光颜色", "高光强度", 0, 1);
            DrawVectorColorPowerProperty(specularOffsetProp, "高光偏移", "高光范围");
            materialEditor.TexturePropertySingleLine(rampContent, rampMapProp, rampMapEditorProp);
            if (rampMapEditorProp.floatValue != 0) rampEditor.DrawArea(material);

            var rampProp = rampParamProp.vectorValue;
            DrawVectorColorPowerProperty(lightOffsetProp, "灯光偏移");
            //rampProp.x = EditorGUILayout.Slider("正光内圈", rampProp.x, -1, 1);
            rampProp.y = EditorGUILayout.Slider("位置偏移", rampProp.y, -1, 1);
            //GUILayout.Space(8);
            //DrawVectorColorPowerProperty(RampLightOffset2Prop, "补光偏移", "强度");
            //rampProp.z = EditorGUILayout.Slider("补光内圈", rampProp.z, 0, 1);
            //rampProp.w = EditorGUILayout.Slider("补光外圈", rampProp.w, 0, 1);
            rampParamProp.vectorValue = rampProp;
        }

        protected void DrawVectorColorPowerProperty(MaterialProperty property, string colorLabel = "",
            string powerLabel = "", float min = 0, float max = 1)
        {
            var rimProp = property.vectorValue;
            color.Set(rimProp.x, rimProp.y, rimProp.z);
            color = EditorGUILayout.Vector3Field(colorLabel, color);
            if (!string.IsNullOrEmpty(powerLabel)) rimProp.w = EditorGUILayout.Slider(powerLabel, rimProp.w, min, max);

            rimProp.Set(color.x, color.y, color.z, rimProp.w);
            property.vectorValue = rimProp;
        }

        // material main advanced options
        public override void DrawAdvancedOptions(Material material)
        {
            if (litProperties.reflections != null && litProperties.highlights != null)
            {
                materialEditor.ShaderProperty(litProperties.highlights, LitGUI.Styles.highlightsText);
                materialEditor.ShaderProperty(litProperties.reflections, LitGUI.Styles.reflectionsText);
            }

            base.DrawAdvancedOptions(material);
        }

        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            if (material == null)
                return;

            // _Emission property is lost after assigning Standard shader to the material
            // thus transfer it before assigning the new shader
            if (material.HasProperty("_Emission")) material.SetColor("_EmissionColor", material.GetColor("_Emission"));

            base.AssignNewShaderToMaterial(material, oldShader, newShader);

            if (oldShader == null || !oldShader.name.Contains("Legacy Shaders/"))
            {
                SetupMaterialBlendMode(material);
                return;
            }

            var surfaceType = SurfaceType.Opaque;
            var blendMode = BlendMode.Alpha;
            if (oldShader.name.Contains("/Transparent/Cutout/"))
            {
                surfaceType = SurfaceType.Opaque;
                material.SetFloat("_AlphaClip", 1);
            }
            else if (oldShader.name.Contains("/Transparent/"))
            {
                // NOTE: legacy shaders did not provide physically based transparency
                // therefore Fade mode
                surfaceType = SurfaceType.Transparent;
                blendMode = BlendMode.Alpha;
            }

            material.SetFloat("_Blend", (float)blendMode);

            material.SetFloat("_Surface", (float)surfaceType);
            if (surfaceType == SurfaceType.Opaque)
                material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
            else
                material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");

            if (oldShader.name.Equals("Standard (Specular setup)"))
            {
                material.SetFloat("_WorkflowMode", (float)LitGUI.WorkflowMode.Specular);
                var texture = material.GetTexture("_SpecGlossMap");
                if (texture != null)
                    material.SetTexture("_MetallicSpecGlossMap", texture);
            }
            else
            {
                material.SetFloat("_WorkflowMode", (float)LitGUI.WorkflowMode.Metallic);
                var texture = material.GetTexture("_MetallicGlossMap");
                if (texture != null)
                    material.SetTexture("_MetallicSpecGlossMap", texture);
            }
        }
    }
}