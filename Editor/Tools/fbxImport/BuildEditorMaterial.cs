#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

    public class BuildEditorMaterial
    {
        public static Material BuildDefaultMateral(FBXSetting.EDITOR_RESOURCE_COLOR ecolor)
        {
            Material mat = null;
            string path = "assets/digitalgame/resource/materials/";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            string name = FBXSetting.editor_material_names[(int)ecolor];

            if (string.IsNullOrEmpty(name)) return mat;
            mat = AssetDatabase.LoadAssetAtPath<Material>(path + name + ".mat");
            if(mat == null)
            {
                Shader shader = Shader.Find("Hidden/Internal-Colored");
                mat = new Material(shader);
                
                Color color = FBXSetting.editor_color_map[(int)ecolor];

                mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                mat.SetInt("_ZWrite", 0);
                mat.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
                mat.SetColor("_Color", color);

                AssetDatabase.CreateAsset(mat, path + name + ".mat"); AssetDatabase.Refresh();
                mat = AssetDatabase.LoadAssetAtPath<Material>(path + name + ".mat");
            }

            return mat;
        }

        public static void BuildMaterailWithShader(Shader shader, string dir, string name)
        {
            if (string.IsNullOrEmpty(name)) return;
            if (shader == null) return;
            string _name = dir + "/" + name + ".mat";
            int i = 1;
            for(; ; )
            {
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(_name);
                if (mat == null) break;
                _name = string.Format(dir + "/" + name + "_{0:D2}.mat", i++);
            }

            {
                Material mat = new Material(shader);
                AssetDatabase.CreateAsset(mat, _name);
            }
        }

        public static void ApplyDefaultShaderValue(Material mat)
        {
            if (mat == null) return;
            int id = Shader.PropertyToID("_Cutoff");
            if (mat.HasProperty(id))
            {
                mat.SetFloat(id, 0f);
            }

            id = Shader.PropertyToID("_Cull");
            if (mat.HasProperty(id))
            {
                mat.SetFloat(id, 0f);
            }

            id = Shader.PropertyToID("_SrcBlend");
            if (mat.HasProperty(id))
            {
                mat.SetFloat(id, 5.0f);
            }

            id = Shader.PropertyToID("_DstBlend");
            if (mat.HasProperty(id))
            {
                mat.SetFloat(id, 10.0f);
            }

            id = Shader.PropertyToID("_ZTest");
            if (mat.HasProperty(id))
            {
                mat.SetFloat(id, 2.0f);
            }

            id = Shader.PropertyToID("_ZWrite");
            if (mat.HasProperty(id))
            {
                mat.SetFloat(id, 1.0f);
            }
        }
    }

#endif
