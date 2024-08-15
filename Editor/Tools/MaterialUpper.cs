//using System.Collections.Generic;

//using UnityEditor;

//using UnityEngine;
//using UnityEngine.SceneManagement;


//public class GameTools : EditorWindow
//{
//    [MenuItem("Tools/GameTools")]
//    public static void ShowWindow()
//    {
//        GetWindow<GameTools>("GameTools");
//    }

//    private void OnGUI()
//    {
//        if (GUILayout.Button("Uper Materials To URP"))
//        {
//            UperSceneMaterial();
//        }
//        if (GUILayout.Button("Remove Missing Script"))
//        {
//            RemoveMissingScript();
//        }

//        if (GUILayout.Button("Uper Behaviour"))
//        {
//            GameObject[] gos = SceneManager.GetActiveScene().GetRootGameObjects();

//            //foreach (GameObject go in gos)0
//            {
//                ChangeToEffect.UpdateEffect(Selection.activeGameObject);

//            }
//        }

//    }

//    private void UperMaterials()
//    {
//        string[] guids = AssetDatabase.FindAssets("t:Material");
//        foreach (string guid in guids)
//        {
//            string path = AssetDatabase.GUIDToAssetPath(guid);
//            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);

//            string oldShaderName = material.shader.name;
//            if (oldShaderName == "LayaAir3D/Particle/ShurikenParticle")
//            {
//                //ParticleShaderUpdate(material);
//            }
//            else
//            {
//                // 在这里可以对每个材质进行处理，例如打印名称或执行其他操作
//                //Shader newShader = UniversalRenderPipeline.asset.defaultShader;
//                //if (material.shader.name != newShader.name)
//                //{
//                //    // 获取Universal Render Pipeline (URP)的默认着色器

//                //    // 获取旧的贴图引用
//                //    Texture oldTexture = material.GetTexture("_MainTex");
//                //    material.shader = newShader;
//                //    material.SetTexture("_BaseMap", oldTexture);

//                //}
//            }


//        }
//    }

//    private void UperSceneMaterial()
//    {
//        GameObject uperNode = Selection.activeGameObject;
//        if (!uperNode) return;

//        ParticleSystemRenderer[] pArrays = uperNode.GetComponentsInChildren<ParticleSystemRenderer>();
//        MeshRenderer[] mArrays = uperNode.GetComponentsInChildren<MeshRenderer>();

//        List<int> queueArray = new List<int>();
//        for (int i = 0; i < pArrays.Length; i++)
//        {
//            var _mat = pArrays[i].sharedMaterial;
//            if (_mat)
//            {
//                int a = _mat.renderQueue;
//                if (queueArray.IndexOf(a) == -1)
//                {
//                    queueArray.Add(a);
//                }
//            }
//            _mat = pArrays[i].trailMaterial;
//            if (_mat)
//            {
//                int b = _mat.renderQueue;
//                if (queueArray.IndexOf(b) == -1)
//                {
//                    queueArray.Add(b);
//                }
//            }
//        }
//        for (int i = 0; i < mArrays.Length; i++)
//        {
//            for (int j = 0; j < mArrays[i].sharedMaterials.Length; j++)
//            {
//                int c = mArrays[i].materials[j].renderQueue;
//                if (queueArray.IndexOf(c) == -1)
//                {
//                    queueArray.Add(c);
//                }
//            }
//        }
//        queueArray.Sort();

//        for (int i = 0; i < pArrays.Length; i++)
//        {
//            ParticleShaderUpdate(pArrays[i].material, queueArray);
//            ParticleShaderUpdate(pArrays[i].trailMaterial, queueArray);
//        }
//        for (int i = 0; i < mArrays.Length; i++)
//        {
//            for (int j = 0; j < mArrays[i].materials.Length; j++)
//            {
//                ParticleShaderUpdate(mArrays[i].materials[j], queueArray);
//            }
//        }
//    }
//    /// <summary>
//    /// 旧的 LayaShader 升级到URPShader
//    /// </summary>
//    /// <param name="material"></param>
//    private void ParticleShaderUpdate(Material material, List<int> queueArray)
//    {
//        if (!material) return;
//        string oldShaderName = material.shader.name;
//        if (oldShaderName != "LayaAir3D/Particle/ShurikenParticle") return;

//        Texture oldTexture = material.GetTexture("_MainTex");

//        float renderModle = material.GetFloat("_Mode");
//        // 修改 ADD模式的shader
//        if (renderModle == 0)
//        {
//            string assetPath = UnityEditor.AssetDatabase.GetAssetPath(material);
//            Material originMat = (Material)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Material));

//            var sort = originMat.renderQueue;

//            int k = queueArray.IndexOf(sort);

//            Shader urpParticleShader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
//            originMat.shader = urpParticleShader;
//            originMat.SetTexture("_BaseMap", oldTexture);

//            originMat.SetFloat("_QueueOffset", k);

//            originMat.SetFloat("_Surface", (float)SurfaceType.Transparent);
//            originMat.SetFloat("_Blend", (float)BlendMode.Additive);
//            BaseShaderGUI.SetupMaterialBlendMode(originMat);
//        }

//    }

//    private void _RemoveNullComponent(Transform parent)
//    {
//        // 遍历所有子节点
//        foreach (Transform child in parent)
//        {
//            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(child.gameObject);
//            // 递归获取子节点的子节点
//            _RemoveNullComponent(child);
//        }
//    }
//    private void RemoveMissingScript()
//    {
//        string[] guids = AssetDatabase.FindAssets("t:Prefab");
//        foreach (string guid in guids)
//        {
//            string path = AssetDatabase.GUIDToAssetPath(guid);

//            GameObject _prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
//            if (_prefab)
//                _RemoveNullComponent(_prefab.transform);
//        }
//        AssetDatabase.SaveAssets();
//    }
//}
