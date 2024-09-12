using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Easy
{
    /// <summary>
    /// A volume component that holds settings for the OutLine effect.
    /// </summary>
    [Serializable,
     VolumeComponentMenuForRenderPipeline("Post-processing/Global Param", typeof(UniversalRenderPipeline))]
    public sealed partial class GlobalParam : VolumeComponent, IPostProcessComponent
    {
        [Header("Character")] public ClampedFloatParameter outLineWidth = new ClampedFloatParameter(0.0f, 0f, 6f);
        public ColorParameter outLineColor = new ColorParameter(Color.black, true, true, true);
        public ColorParameter shadowColor = new ColorParameter(Color.black, false, true, true);
        public FloatParameter shadowHeight = new FloatParameter(0.0f);

        //public Vector3Parameter shadowDir = new Vector3Parameter(Vector3.zero);
        public Vector3Parameter lightDir = new Vector3Parameter(Vector3.up);
        [Header("Scene")] public ClampedFloatParameter shadowEdge = new ClampedFloatParameter(0.90f, 0f, 1f);
        public ClampedFloatParameter shadowSaturation = new ClampedFloatParameter(0f, -1f, 1f);
        public ColorParameter sceneShadowColor = new ColorParameter(Color.black, false, true, true);
        [Header("Editor")] public BoolParameter showLightMap = new BoolParameter(false);

        /// <inheritdoc/>
        public bool IsActive() => true;

        /// <inheritdoc/>
        public bool IsTileCompatible() => false;

        protected override void OnEnable()
        {
            base.OnEnable();
            SetGlobalParam();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Shader.SetGlobalFloat("_OutlineWidth", 0);
        }

        private void SetGlobalParam()
        {
            Shader.SetGlobalFloat("_OutlineWidth", outLineWidth.value);
            Shader.SetGlobalColor("_OutlineColor", outLineColor.value);
            Shader.SetGlobalFloat("_PlanarShadowHeight", shadowHeight.value);
            Shader.SetGlobalColor("_ShadowColor", shadowColor.value);

            Shader.SetGlobalFloat("_ShadowEdge", shadowEdge.value);
            Shader.SetGlobalFloat("_SceneSaturation", shadowSaturation.value);
            Shader.SetGlobalColor("_SceneShadowColor", sceneShadowColor.value);

            //Shader.SetGlobalVector("_ShadowDir", shadowDir.value);
            Shader.SetGlobalVector("_LightDir", lightDir.value);

            if (showLightMap.value) Shader.EnableKeyword("_SHOW_LIGHTMAP_ON");
            else Shader.DisableKeyword("_SHOW_LIGHTMAP_ON");
        }

#if UNITY_EDITOR
        public override void Override(VolumeComponent state, float interpFactor)
        {
            base.Override(state, interpFactor);
            SetGlobalParam();
        }
#endif
    }
}