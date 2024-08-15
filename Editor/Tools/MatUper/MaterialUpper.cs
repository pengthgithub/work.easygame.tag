//using System.Collections.Generic;
//using System.Threading;

//using UnityEditor;

//using UnityEngine;
//using UnityEngine.SceneManagement;

////using static UnityEditor.BaseShaderGUI;

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
//                 UpdateScene();
//        }
//        if (GUILayout.Button("Remove Missing Script"))
//        {
//            RemoveMissingScript();
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
//                // ��������Զ�ÿ�����ʽ��д��������ӡ���ƻ�ִ����������
//                //Shader newShader = UniversalRenderPipeline.asset.defaultShader;
//                //if (material.shader.name != newShader.name)
//                //{
//                //    // ��ȡUniversal Render Pipeline (URP)��Ĭ����ɫ��

//                //    // ��ȡ�ɵ���ͼ����
//                //    Texture oldTexture = material.GetTexture("_MainTex");
//                //    material.shader = newShader;
//                //    material.SetTexture("_BaseMap", oldTexture);

//                //}
//            }


//        }
//    }

//    //=================================================================
//    // ��������
//    //=================================================================
//    #region ��������
//    private void UpdateScene()
//    {
//        EditorUtility.DisplayProgressBar("��������", "������ ", 0);
//        bool update = true;
//        Scene currentScene = SceneManager.GetActiveScene();
//        if (currentScene == null) update = false;

//        if(update)
//        {
//            GameObject[] rootGameObject = currentScene.GetRootGameObjects();
//            int _count = rootGameObject.Length;
//            for (int i = 0; i < _count; i++)
//            {
//                GameObject _root = rootGameObject[i];
//                float _len = i * 1.0f / _count;
//                EditorUtility.DisplayProgressBar("��������", "������" + _root.name , _len);
//                UperSceneMaterial(_root);
//                Thread.Sleep(100);
//            }
//        }
//        EditorUtility.ClearProgressBar();
//    }
//    private void UperSceneMaterial(GameObject uperNode)
//    {
//        if (!uperNode) return;

//        ParticleSystemRenderer[] pArrays = uperNode.GetComponentsInChildren<ParticleSystemRenderer>();
//        MeshRenderer[] mArrays = uperNode.GetComponentsInChildren<MeshRenderer>();

//        // ��ȡ��Ч�Ų㼶
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

//        // ������Ч
//        for (int i = 0; i < pArrays.Length; i++)
//        {
//            ParticleShaderUpdate(pArrays[i].sharedMaterial, queueArray);
//            ParticleShaderUpdate(pArrays[i].trailMaterial, queueArray);
//        }
//        for (int i = 0; i < mArrays.Length; i++)
//        {
//            for (int j = 0; j < mArrays[i].sharedMaterials.Length; j++)
//            {
//                ParticleShaderUpdate(mArrays[i].sharedMaterials[j], queueArray);
//            }
//        }
//    }

//    #region ParticleRegion
//    /// <summary>
//    /// �ɵ� LayaShader ������URPShader
//    /// </summary>
//    /// <param name="material"></param>
//    /// <param name="Return"> Fase ��ʾ����������Ч��True Ϊ������Ч</param>
//    private bool ParticleShaderUpdate(Material material, List<int> queueArray)
//    {
//        if (!material) return false;
//        string oldShaderName = material.shader.name;
//        if (oldShaderName != "LayaAir3D/Particle/ShurikenParticle") return false;

//        Color baseColor = material.GetColor("_TintColor");
//        Texture oldTexture = material.GetTexture("_MainTex");
//        float renderModle = material.GetFloat("_Mode");
//        //�޸� ADDģʽ��shader

//        string assetPath = UnityEditor.AssetDatabase.GetAssetPath(material);
//        Material originMat = (Material)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Material));
//        if (!originMat) return true;

//        var sort = originMat.renderQueue;
//        int k = queueArray.IndexOf(sort);

//        Shader urpParticleShader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
//        originMat.shader = urpParticleShader;

//        // ������ͼ
//        originMat.SetTexture("_BaseMap", oldTexture);
//        originMat.SetFloat("_ColorPower", 2.0f);
//        // ˳��
//        originMat.SetFloat("_QueueOffset", k);
//        // ͸��
//        originMat.SetFloat("_Surface", (float)SurfaceType.Transparent);
//        // ��ɫ
//        originMat.SetColor("_BaseColor", baseColor);
//        // ԭʼΪAdd ģʽ
//        if (renderModle == 0)
//        {
//            originMat.SetFloat("_Blend", (float)BlendMode.Additive);
//        }
//        // ԭʼΪAlpha ģʽ
//        if (renderModle == 1)
//        {
//            originMat.SetFloat("_Blend", (float)BlendMode.Alpha);
//        }
//        // CutOff
//        if (renderModle == 2)
//        {
//        }
//        // Custom
//        if (renderModle == 3)
//        {
//        }
//        // Billboard
//        if (renderModle == 4)
//        {
//        }

//        BaseShaderGUI.SetupMaterialBlendMode(originMat);
//        originMat.SetFloat("_ColorMode", (float)UnityEditor.Rendering.Universal.ShaderGUI.ParticleGUI.ColorMode.Overlay);
//        UnityEditor.Rendering.Universal.ShaderGUI.ParticleGUI.SetupMaterialWithColorMode(originMat);
//        return true;
//    }

//    private bool LitShaderUpdate(Material material, List<int> queueArray)
//    {
//        if (!material) return false;
//        string oldShaderName = material.shader.name;
//        if (oldShaderName != "Fancy/Chess/CharacterShading") return false;

//        Color baseColor = material.GetColor("_TintColor");
//        Texture oldTexture = material.GetTexture("_MainTex");
//        float renderModle = material.GetFloat("_Mode");
//        //�޸� ADDģʽ��shader

//        string assetPath = UnityEditor.AssetDatabase.GetAssetPath(material);
//        Material originMat = (Material)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Material));
//        if (!originMat) return true;

//        var sort = originMat.renderQueue;
//        int k = queueArray.IndexOf(sort);

//        Shader urpParticleShader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
//        originMat.shader = urpParticleShader;

//        // ������ͼ
//        originMat.SetTexture("_BaseMap", oldTexture);
//        originMat.SetFloat("_ColorPower", 2.0f);
//        // ˳��
//        originMat.SetFloat("_QueueOffset", k);
//        // ͸��
//        originMat.SetFloat("_Surface", (float)SurfaceType.Transparent);
//        // ��ɫ
//        originMat.SetColor("_BaseColor", baseColor);
//        // ԭʼΪAdd ģʽ
//        if (renderModle == 0)
//        {
//            originMat.SetFloat("_Blend", (float)BlendMode.Additive);
//        }
//        // ԭʼΪAlpha ģʽ
//        if (renderModle == 1)
//        {
//            originMat.SetFloat("_Blend", (float)BlendMode.Alpha);
//        }
//        // CutOff
//        if (renderModle == 2)
//        {
//        }
//        // Custom
//        if (renderModle == 3)
//        {
//        }
//        // Billboard
//        if (renderModle == 4)
//        {
//        }

//        BaseShaderGUI.SetupMaterialBlendMode(originMat);
//        originMat.SetFloat("_ColorMode", (float)UnityEditor.Rendering.Universal.ShaderGUI.ParticleGUI.ColorMode.Overlay);
//        UnityEditor.Rendering.Universal.ShaderGUI.ParticleGUI.SetupMaterialWithColorMode(originMat);
//        return true;
//    }
//    #endregion

//    #endregion

//    //=================================================================
//    // �Ƴ�����
//    //=================================================================
//    #region MissingRefresh
//    private void _RemoveNullComponent(Transform parent)
//    {
//        // ���������ӽڵ�
//        foreach (Transform child in parent)
//        {
//            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(child.gameObject);
//            // �ݹ��ȡ�ӽڵ���ӽڵ�
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
//    #endregion
//}
