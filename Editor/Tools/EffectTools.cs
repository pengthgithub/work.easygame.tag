#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Easy;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

//using Behaviour;

public class EffectTools : EditorWindow
{
    private GameObject gameObject;

    [Obsolete]
    private void OnGUI()
    {
        if (GUILayout.Button("Uper CharacterMaterials To URP"))
        {
            GameObject[] goes = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (GameObject go in goes)
            {
                UperCharacterMaterial(go);
            }
        }

        EditorGUILayout.BeginHorizontal();
        gameObject = EditorGUILayout.ObjectField("������", gameObject, typeof(GameObject), true) as GameObject;
        if (GUILayout.Button("Uper Materials To URP"))
        {
            UperSceneMaterial(gameObject);
        }

        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Next Material"))
        {
            if (!gameObject)
            {
                return;
            }

            MeshRenderer[] mArrays = gameObject.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < mArrays.Length; i++)
            {
                Material mat = mArrays[i].sharedMaterial;
                if (mat && mat.shader.name == "Fancy/Chess/CharacterShading")
                {
                    Selection.activeGameObject = mArrays[i].gameObject;
                    break;
                }
            }
        }


        if (GUILayout.Button("Remove Behaviour"))
        {
            GameObject[] goes = SceneManager.GetActiveScene().GetRootGameObjects();
            //GameObject go = Selection.activeGameObject;
            int index = 0;
            foreach (GameObject go in goes)
            {
                Selection.activeGameObject = go;
                index++;
                //var tag = go.GetComponent<BehaviourTag>();
                //GameObject.DestroyImmediate(tag);


                PrefabUtility.ApplyPrefabInstance(go, InteractionMode.UserAction);
            }
        }

        GUILayout.BeginHorizontal();
        bInit = GUILayout.Toggle(bInit, "");
        if (GUILayout.Button("Uper Socket"))
        {
            InitSockets();

            GameObject[] goes = SceneManager.GetActiveScene().GetRootGameObjects();
            //GameObject go = Selection.activeGameObject;
            int index = 0;
            foreach (GameObject go in goes)
            {
                Selection.activeGameObject = go;
                GeneralMeshCollider(go);
                GenralSocket(go);
                Debug.Log("��Ӳ�۽���:" + (index / goes.Length));
                index++;
            }
        }

        GUILayout.EndHorizontal();

        if (GUILayout.Button("Uper Caster"))
        {
            GameObject[] goes = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (GameObject go in goes)
            {
                UperCaster(go);
            }
        }

        if (GUILayout.Button("Animator Controller"))
        {
            //GameObject[] goes = SceneManager.GetActiveScene().GetRootGameObjects();
            //foreach (GameObject go in goes)
            {
                UpdateController(Selection.activeGameObject);
            }
        }


        if (GUILayout.Button("Change Root Layer"))
        {
            GameObject[] goes = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (GameObject go in goes)
            {
                go.layer = LayerMask.NameToLayer("Character");
                SkinnedMeshRenderer[] renders = go.GetComponentsInChildren<SkinnedMeshRenderer>();

                foreach (SkinnedMeshRenderer item in renders)
                {
                    item.gameObject.layer = LayerMask.NameToLayer("Character");
                }

                PrefabUtility.ApplyPrefabInstance(go, InteractionMode.UserAction);
            }
        }

        if (GUILayout.Button("Save SFX"))
        {
            GameObject[] goes = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (GameObject go in goes)
            {
                MeshRenderer[] renders = go.GetComponentsInChildren<MeshRenderer>();

                foreach (MeshRenderer item in renders)
                {
                    foreach (var mat in item.sharedMaterials)
                    {
                        mat.shader = Shader.Find("Custom/Scene");
                    }
                }
            }
        }
    }


    [MenuItem("Tools/GameTools")]
    public static void ShowWindow()
    {
        _ = GetWindow<EffectTools>("GameTools");
    }

    private static void _RemoveNullComponent(Transform parent)
    {
        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(parent.gameObject);
        // ���������ӽڵ�
        foreach (Transform child in parent)
        {
            // �ݹ��ȡ�ӽڵ���ӽڵ�  
            _RemoveNullComponent(child);
        }
    }

    [MenuItem("GameObject/Tools/RemoveMissingScript")]
    private static void RemoveMissingScript()
    {
        GameObject[] goes = Selection.gameObjects;
        foreach (GameObject go in goes)
        {
            _RemoveNullComponent(go.transform);
        }

        foreach (GameObject go in goes)
        {
            PrefabUtility.ApplyPrefabInstance(go, InteractionMode.AutomatedAction);
        }

        // �������
        AssetDatabase.SaveAssets();
    }

    [MenuItem("GameObject/Tools/RemoveUnUsePS")]
    private static void RemoveUnUsePS()
    {
        GameObject goes = Selection.activeGameObject;
        for (int i = 0; i < goes.transform.childCount; i++)
        {
            Transform ts = goes.transform.GetChild(i);

            ParticleSystem[] psArray = ts.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem particleSystem in psArray)
            {
                if (particleSystem.gameObject.name == "01" || particleSystem.gameObject.name == "02" ||
                    particleSystem.gameObject.name == "point")
                {
                    DestroyImmediate(particleSystem);
                }
            }

            PrefabUtility.ApplyPrefabInstance(ts.gameObject, InteractionMode.AutomatedAction);
        }

        AssetDatabase.SaveAssets();
    }

    public static void ExportEffect()
    {
        GameObject[] goes = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject go in goes)
        {
            GameObject a = new(go.name);
            go.transform.SetParent(a.transform, false);

            //Selection.activeGameObject = go;
            //ChangeToEffect.UpdateEffect(go);
        }
    }

    private void UperMaterials()
    {
        string[] guids = AssetDatabase.FindAssets("t:Material");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);

            string oldShaderName = material.shader.name;
            if (oldShaderName == "LayaAir3D/Particle/ShurikenParticle")
            {
                //ParticleShaderUpdate(material);
            }
            // ��������Զ�ÿ�����ʽ��д��������ӡ���ƻ�ִ����������
            //Shader newShader = UniversalRenderPipeline.asset.defaultShader;
            //if (material.shader.name != newShader.name)
            //{
            //    // ��ȡUniversal Render Pipeline (URP)��Ĭ����ɫ��
            //    // ��ȡ�ɵ���ͼ����
            //    Texture oldTexture = material.GetTexture("_MainTex");
            //    material.shader = newShader;
            //    material.SetTexture("_BaseMap", oldTexture);
            //}
        }
    }

    private void UperSceneMaterial(GameObject uperNode)
    {
        if (!uperNode)
        {
            return;
        }

        ParticleSystemRenderer[] pArrays = uperNode.GetComponentsInChildren<ParticleSystemRenderer>();
        MeshRenderer[] mArrays = uperNode.GetComponentsInChildren<MeshRenderer>();

        TrailRenderer[] mTrailArrays = uperNode.GetComponentsInChildren<TrailRenderer>();

        for (int i = 0; i < pArrays.Length; i++)
        {
            ParticleShaderUpdate(pArrays[i].sharedMaterial);
            ParticleShaderUpdate(pArrays[i].trailMaterial);
        }

        for (int i = 0; i < mArrays.Length; i++)
        {
            for (int j = 0; j < mArrays[i].sharedMaterials.Length; j++)
            {
                ParticleShaderUpdate(mArrays[i].sharedMaterials[j]);
            }
        }

        for (int i = 0; i < mTrailArrays.Length; i++)
        {
            ParticleShaderUpdate(mTrailArrays[i].sharedMaterial);
        }
    }

    private void UperCharacterMaterial(GameObject go)
    {
        SkinnedMeshRenderer[] skinRender = go.GetComponentsInChildren<SkinnedMeshRenderer>();
        MeshRenderer[] render = go.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer item in render)
        {
            foreach (Material mat in item.sharedMaterials)
            {
                UpdateMaterial(mat, item.gameObject);
            }
        }

        foreach (SkinnedMeshRenderer item in skinRender)
        {
            foreach (Material mat in item.sharedMaterials)
            {
                UpdateMaterial(mat, item.gameObject);
            }
        }
    }

    private void UpdateMaterial(Material material, GameObject go)
    {
        string oldShaderName = material.shader.name;
        Texture oldTexture = material.GetTexture("_MainTex");
        string assetPath = AssetDatabase.GetAssetPath(material);
        Material originMat = (Material)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Material));
        Shader urpCharacterShader = Shader.Find("Custom/CharacterLit");
        originMat.shader = urpCharacterShader;
        originMat.SetTexture("_BaseMap", oldTexture);
    }

    /// <summary>
    ///     �ɵ� LayaShader ������URPShader
    /// </summary>
    /// <param name="material"></param>
    private void ParticleShaderUpdate(Material material)
    {
        if (!material)
        {
            return;
        }

        string oldShaderName = material.shader.name;
        int oldrenderQueue = material.renderQueue;
        Texture oldTexture = material.GetTexture("_MainTex");


        if (oldShaderName == "LayaAir3D/Particle/ShurikenParticle")
        {
            string assetPath = AssetDatabase.GetAssetPath(material);
            Material originMat = (Material)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Material));
            if (originMat)
            {
                Shader urpParticleShader = Shader.Find("Fx/Particle");
                originMat.shader = urpParticleShader;
                originMat.SetTexture("_MainTex", oldTexture);
                originMat.renderQueue = oldrenderQueue;
            }
        }

        if (oldShaderName == "LayaAir3D/Trail")
        {
            string assetPath = AssetDatabase.GetAssetPath(material);
            Material originMat = (Material)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Material));
            if (originMat)
            {
                Shader urpParticleShader = Shader.Find("Fx/Trail");
                originMat.shader = urpParticleShader;
                originMat.SetTexture("_MainTex", oldTexture);
                originMat.renderQueue = oldrenderQueue;
            }
        }

        if (oldShaderName == "Fancy/Chess/MeshEffect")
        {
            string assetPath = AssetDatabase.GetAssetPath(material);
            Material originMat = (Material)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Material));
            if (originMat)
            {
                Shader urpParticleShader = Shader.Find("Fx/MeshEffect");
                originMat.shader = urpParticleShader;
                originMat.SetTexture("_MainTex", oldTexture);
                originMat.renderQueue = oldrenderQueue;
            }
        }
    }

    //=====================================================================================================
    //=====================================================================================================
    private void UperCaster(GameObject go)
    {
        // EffectCaster caster = go.GetComponent<EffectCaster>();
        // if (caster && caster.transformClip)
        // {
        //     InitCastClip(caster.transformClip, caster);
        // }
        //
        // EffectSound sound = go.GetComponent<EffectSound>();
        // if (sound != null)
        // {
        //     List<AudioClip> realArray = new();
        //
        //     AudioClip[] audioClips = sound.audioClips;
        //     for (int i = audioClips.Length - 1; i >= 0; i--)
        //     {
        //         if (audioClips[i] != null)
        //         {
        //             realArray.Add(audioClips[i]);
        //         }
        //     }
        //
        //     sound.audioClips = new AudioClip[realArray.Count];
        //     for (int i = 0; i < realArray.Count; i++)
        //     {
        //         sound.audioClips[i] = realArray[i];
        //     }
        // }

        PrefabUtility.ApplyPrefabInstance(go, InteractionMode.UserAction);
    }

    // public void InitCastClip(AnimationClip clip, EffectCaster caster)
    // {
    //     if (clip == null)
    //     {
    //         return;
    //     }
    //
    //     foreach (EditorCurveBinding binding in AnimationUtility.GetCurveBindings(clip))
    //     {
    //         AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);
    //         if (binding.propertyName.IndexOf("localEuler") != -1)
    //         {
    //             if (caster.localEuler.Length == 0)
    //             {
    //                 caster.localEuler = new AnimationCurve[3];
    //             }
    //
    //             if (binding.propertyName == "localEulerAnglesRaw.x")
    //             {
    //                 caster.localEuler[0] = curve;
    //             }
    //
    //             if (binding.propertyName == "localEulerAnglesRaw.y")
    //             {
    //                 caster.localEuler[1] = curve;
    //             }
    //
    //             if (binding.propertyName == "localEulerAnglesRaw.z")
    //             {
    //                 caster.localEuler[2] = curve;
    //             }
    //         }
    //
    //         if (binding.propertyName.IndexOf("m_LocalPosition") != -1)
    //         {
    //             if (caster.localPosition.Length == 0)
    //             {
    //                 caster.localPosition = new AnimationCurve[3];
    //             }
    //
    //             if (binding.propertyName == "m_LocalPosition.x")
    //             {
    //                 caster.localPosition[0] = curve;
    //             }
    //
    //             if (binding.propertyName == "m_LocalPosition.y")
    //             {
    //                 caster.localPosition[1] = curve;
    //             }
    //
    //             if (binding.propertyName == "m_LocalPosition.z")
    //             {
    //                 caster.localPosition[2] = curve;
    //             }
    //         }
    //
    //         if (binding.propertyName.IndexOf("m_LocalScale") != -1)
    //         {
    //             if (caster.localScale.Length == 0)
    //             {
    //                 caster.localScale = new AnimationCurve[3];
    //             }
    //
    //             if (binding.propertyName == "m_LocalScale.x")
    //             {
    //                 caster.localScale[0] = curve;
    //             }
    //
    //             if (binding.propertyName == "m_LocalScale.y")
    //             {
    //                 caster.localScale[1] = curve;
    //             }
    //
    //             if (binding.propertyName == "m_LocalScale.z")
    //             {
    //                 caster.localScale[2] = curve;
    //             }
    //         }
    //     }
    // }

    //=====================================================================================================
    //=====================================================================================================

    public void UpdateController(GameObject go)
    {
        //Animator animator = go.GetComponent<Animator>();
        //animator.;
    }

    #region ��۽���

    private struct SockData
    {
        public string locator;
        public string boneName;
        public bool lockScale;
        public bool localRotation;
        public Vector3 position;
        public Vector3 eularAngle;
        public Vector3 scale;
    }

    private readonly Dictionary<string, List<SockData>> mAllSockets = new();

    private Vector3 parse(JToken val)
    {
        Vector3 a = new();
        int i = 0;
        foreach (JToken _item in val)
        {
            a[i] = float.Parse(_item.ToString());
            i++;
        }

        return a;
    }

    private bool bInit;

    private void InitSockets()
    {
        if (bInit && mAllSockets.Count != 0)
        {
            return;
        }

        bInit = true;
        mAllSockets.Clear();
        string path = "E:\\SVN\\FishingGame\\Products\\res\\character";
        string[] files = Directory.GetFiles(path, "*.obj", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            string jsonTex = File.ReadAllText(file);
            JToken json = JToken.Parse(jsonTex);
            JToken token = json["token"];
            foreach (JToken prop in token)
            {
                if (prop["items"] == null)
                {
                    continue;
                }

                foreach (JToken _item in prop["items"])
                {
                    JToken locators = _item["locator"];
                    if (locators != null)
                    {
                        foreach (JToken _loc in locators)
                        {
                            SockData sockData = new();
                            JToken data = _loc["locator"];
                            sockData.locator = data.ToString();
                            data = _loc["bone_name"];
                            sockData.boneName = data.ToString();
                            data = _loc["lock_scale"];
                            sockData.lockScale = bool.Parse(data.ToString());
                            data = _loc["local_rotation"];
                            sockData.localRotation = bool.Parse(data.ToString());
                            data = _loc["position"];

                            sockData.position = parse(data);
                            data = _loc["rotation"];
                            sockData.eularAngle = parse(data);
                            data = _loc["scale"];
                            sockData.scale = parse(data);

                            string name = Path.GetFileNameWithoutExtension(file);
                            bool result = mAllSockets.ContainsKey(name);
                            if (!result)
                            {
                                mAllSockets[name] = new List<SockData>();
                            }

                            mAllSockets[name].Add(sockData);
                        }
                    }
                }
            }
        }
    }

    private void GenralSocket(GameObject go)
    {
        if (mAllSockets.ContainsKey(go.name) == false)
        {
            return;
        }

        // EasyData esd = go.GetComponent<EasyData>();
        // if (esd == null)
        // {
        //     esd = go.AddComponent<EasyData>();
        // }

        //esd.socketList = new List<GameObject>();
        List<SockData> socks = mAllSockets[go.name];
        Transform[] ts = go.transform.GetComponentsInChildren<Transform>();
        List<Transform> childItems = ts.ToList();

        foreach (SockData _data in socks)
        {
            Transform item = childItems.Find(x => x.name == _data.locator);
            if (item == null)
            {
                item = new GameObject(_data.locator).transform;
                item.transform.localPosition = _data.position;
                item.transform.localEulerAngles = _data.eularAngle;
                item.transform.localScale = _data.scale;

                if (!string.IsNullOrEmpty(_data.boneName))
                {
                    Transform parent = childItems.Find(x => x.name == _data.boneName);
                    if (parent)
                    {
                        item.SetParent(parent);
                    }
                }
                else
                {
                    item.SetParent(go.transform);
                }
            }

            //esd.socketList.Add(item.gameObject);
        }

        PrefabUtility.ApplyPrefabInstance(go, InteractionMode.UserAction);
        //_ = PrefabUtility.SavePrefabAsset(go);
    }

    private void GeneralMeshCollider(GameObject go)
    {
        SkinnedMeshRenderer[] _array = go.GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (SkinnedMeshRenderer render in _array)
        {
            MeshCollider meshCollider = render.GetComponent<MeshCollider>();
            if (meshCollider != null)
            {
                DestroyImmediate(meshCollider);
                //meshCollider = render.gameObject.AddComponent<MeshCollider>();
            }

            //meshCollider.sharedMesh = render.sharedMesh;
            BoxCollider boxCollider = render.GetComponent<BoxCollider>();
            if (boxCollider == null)
            {
                render.gameObject.AddComponent<BoxCollider>();
            }
        }
    }

    #endregion

    //[MenuItem("Assets/工具/从以前的obj覆盖数据sfx")]
    public static void ObjToSfx()
    {
        //读取目录下 的obj文件
        var dir = "F:/SVN/FishingGame/Products/res/skilleffect/";
        var sfxDir = "C:/Users/文档/SVN/SuperGame/Client/SuperGame/Assets/assets/sfx";
        var sfxAsset = Directory.GetFiles(sfxDir, "*.asset", SearchOption.AllDirectories);
        var files = Directory.GetFiles(dir, "*.obj", SearchOption.AllDirectories);
        Dictionary<string, string[]> fileMaps = new Dictionary<string, string[]>();
        foreach (var file in files)
        {
            var name = "sfx_" + Path.GetFileNameWithoutExtension(file);
            string[] lines = File.ReadAllLines(file);
            fileMaps[name] = lines;
        }

        foreach (string sfx in sfxAsset)
        {
            var assetPath = sfx.Replace("C:/Users/文档/SVN/SuperGame/Client/SuperGame/", "");
            var sfxParticle = AssetDatabase.LoadAssetAtPath<SfxParticle>(assetPath);
            var fileName = Path.GetFileNameWithoutExtension(assetPath);
            fileMaps.TryGetValue(fileName, out string[] content);
            if (content != null && content.Length != 0)
            {
                for (int i = 0, n = content.Length; i < n; i++)
                {
                    string line = content[i];
                    if (line.Contains("TagSound"))
                    {
                        sfxParticle.sfxSound.Clear();
                        SfxSound sound = new SfxSound();
                        string bindTimeLine = content[i + 2];
                        string clipPath = "";
                        if (bindTimeLine.Contains("bindTime"))
                        {
                            bindTimeLine = bindTimeLine.Replace("\"bindTime\":", "").Replace(",", "").Trim();
                            sound.bindTime = float.Parse(bindTimeLine);
                            clipPath = content[i + 4];
                        }
                        else
                        {
                            clipPath = content[i + 3];
                        }

                        if (clipPath.Contains("res/sound"))
                        {
                            clipPath = clipPath.Replace("\"res/sound/", "").Replace("\"", "").Replace(",", "").Trim();
                        }

                        AudioClip oldAudioClip =
                            AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/assets/skilleffect/sound/" + clipPath);
                        if (oldAudioClip == null)
                            oldAudioClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Art/Sound/" + clipPath);

                        sound.audioClip = oldAudioClip;
                        sfxParticle.sfxSound.Add(sound);
                        EditorUtility.SetDirty(sfxParticle);
                    }
                }
            }
        }
    }
}


#endif