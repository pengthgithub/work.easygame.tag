#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEditor.Rendering.Universal.ShaderGUI;
using UnityEngine;
using UnityEngine.Rendering;


public class SceneShaderGUI : BaseShaderGUI
{
    private static readonly string[] workflowModeNames = Enum.GetNames(typeof(LitGUI.WorkflowMode));
    private LitGUI.LitProperties litProperties;
    private MaterialProperty shadowColorProp;
    private MaterialProperty shadowHeightProp;

    public override void FillAdditionalFoldouts(MaterialHeaderScopeList materialScopesList)
    {
    }

    // collect properties from the material properties
    public override void FindProperties(MaterialProperty[] properties)
    {
        base.FindProperties(properties);
        litProperties = new LitGUI.LitProperties(properties);
        shadowColorProp = FindProperty("_ShadowColorOffset", properties, false);
        shadowHeightProp = FindProperty("_ShadowHeightOffset", properties, false);
    }

    // material changed check
    public override void ValidateMaterial(Material material)
    {
        SetMaterialKeywords(material, LitGUI.SetMaterialKeywords);
    }

    // material main surface options
    public override void DrawSurfaceOptions(Material material)
    {
        // Use default labelWidth
        EditorGUIUtility.labelWidth = 0f;

        if (litProperties.workflowMode != null)
        {
            DoPopup(LitGUI.Styles.workflowModeText, litProperties.workflowMode, workflowModeNames);
        }

        base.DrawSurfaceOptions(material);
    }

    // material main surface inputs
    public override void DrawSurfaceInputs(Material material)
    {
        //Diffuse Texture
        base.DrawSurfaceInputs(material);
        DrawTileOffset(materialEditor, baseMapProp);
        //自发光
        DrawEmissionProperties(material, true);

        materialEditor.ShaderProperty(shadowColorProp, "影子颜色偏移");
        materialEditor.ShaderProperty(shadowHeightProp, "影子高度偏移");
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
        {
            throw new ArgumentNullException("material");
        }

        // _Emission property is lost after assigning Standard shader to the material
        // thus transfer it before assigning the new shader
        if (material.HasProperty("_Emission"))
        {
            material.SetColor("_EmissionColor", material.GetColor("_Emission"));
        }

        base.AssignNewShaderToMaterial(material, oldShader, newShader);

        if (oldShader == null || !oldShader.name.Contains("Legacy Shaders/"))
        {
            SetupMaterialBlendMode(material);
            return;
        }

        SurfaceType surfaceType = SurfaceType.Opaque;
        BlendMode blendMode = BlendMode.Alpha;
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
        {
            material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
        }
        else
        {
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        }

        if (oldShader.name.Equals("Standard (Specular setup)"))
        {
            material.SetFloat("_WorkflowMode", (float)LitGUI.WorkflowMode.Specular);
            Texture texture = material.GetTexture("_SpecGlossMap");
            if (texture != null)
            {
                material.SetTexture("_MetallicSpecGlossMap", texture);
            }
        }
        else
        {
            material.SetFloat("_WorkflowMode", (float)LitGUI.WorkflowMode.Metallic);
            Texture texture = material.GetTexture("_MetallicGlossMap");
            if (texture != null)
            {
                material.SetTexture("_MetallicSpecGlossMap", texture);
            }
        }
    }
}

#endif