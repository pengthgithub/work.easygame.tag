
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Easy
{
    [Serializable] public class AniData
    {
        [SerializeField] public string name;
        [SerializeField] public bool loop;
    }
    
    public class EasySpriteEditor : EditorWindow
    {
        [MenuItem("Tools/SpriteRenderPrefab")]
        private static void ShowWindow()
        {
            var window = GetWindow<EasySpriteEditor>("SpriteRenderPrefab");
            window.minSize = new Vector2(400, 400);
            window.Show();
        }
        
        List<AniData> aniSetting = new List<AniData>();
        private string[] directories;
        private string[] generaledPrefabs;
        List<bool> needGeneratePrefab = new List<bool>();
        private bool selectAll = false;
        private Vector2 scrollPosition = new Vector2(0,0);
        private string[] fpsArray = new[] {"24","25","30","50","60"};
        private int fpsIndex = 2;
        private int fps = 30;
        private bool setting = false;
        private bool general = true;
        
        //============================================================
        // 事件方法
        //============================================================
        #region 方法
        private void OnEnable()
        {
            fpsIndex =  EditorPrefs.GetInt("fps");
            directories = Directory.GetDirectories("Assets\\ArtRes", "*", SearchOption.TopDirectoryOnly);
            generaledPrefabs = Directory.GetFiles("Assets\\Art\\Character", "*.prefab", SearchOption.AllDirectories);
            needGeneratePrefab.Clear();
            foreach (var dir in directories)
            {
                bool needGeneral = true;
                foreach (var prefab in generaledPrefabs)
                {
                    var prefabName = Path.GetFileName(prefab);
                    var dirName = Path.GetFileName(dir);
                    if (prefabName.Contains(dirName))
                    {
                        needGeneral = false;
                        break;
                    }
                }
                needGeneratePrefab.Add(needGeneral);
            }
           var count = EditorPrefs.GetInt("ani");
            for (int i = 0; i < count; i++)
            {
                AniData data = new AniData();
                data.name = EditorPrefs.GetString("ani"+i+"0");
                data.loop = EditorPrefs.GetBool("ani"+i+"1");
                aniSetting.Add(data);
            }
        }
        private void OnDisable()
        {
            EditorPrefs.SetInt("fps", fpsIndex);
            EditorPrefs.SetInt("ani", aniSetting.Count);
            for (int i = 0; i < aniSetting.Count; i++)
            {
                AniData data = aniSetting[i];
                EditorPrefs.SetString("ani"+i+"0", data.name);
                EditorPrefs.SetBool("ani"+i+"1", data.loop);
            }
        }
        private void OnGUI()
        {
            EditorGUILayout.LabelField("生成预制件工具");
            
            if (GUILayout.Button("设置"))
            {
                setting = !setting;
            }
            if (setting)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("FPS");
                fpsIndex = EditorGUILayout.Popup(fpsIndex, fpsArray);
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("+"))
                {
                    aniSetting.Add(new AniData());
                }
                if (GUILayout.Button("-"))
                {
                    if (aniSetting.Count > 1)
                    {
                        int c = aniSetting.Count - 1;
                        aniSetting.RemoveAt(c);
                    }
                }
                EditorGUILayout.EndHorizontal();
                
                foreach (var ani in aniSetting)
                {
                    EditorGUILayout.BeginHorizontal();
                    ani.name = EditorGUILayout.TextField(ani.name);
                    ani.loop =  EditorGUILayout.Toggle(ani.loop);
                    EditorGUILayout.EndHorizontal();
                }
            }
            if (GUILayout.Button("预制件"))
            {
                general = !general;
            }
            if (general)
            {
                var rect =  GUILayoutUtility.GetLastRect();
                rect.y += 20;
                // 绘制一个高度为 200 的 Box
                EditorGUI.DrawRect(rect, new Color(1f, 0.4f,0.0f, 0.5f));
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("需要生成预制件的资源:");
                selectAll = EditorGUILayout.Toggle("全选",selectAll);
                EditorGUILayout.EndHorizontal();
                if (EditorGUI.EndChangeCheck())
                {
                    for (int j = 0; j < needGeneratePrefab.Count; j++)
                    {
                        needGeneratePrefab[j] = selectAll;
                    }
                }
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Width(position.width), GUILayout.Height(200));
                int i = 0;
                foreach (var dir in directories)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(dir);
                    needGeneratePrefab[i] = EditorGUILayout.Toggle("强制生成",needGeneratePrefab[i]);
                
                    EditorGUILayout.EndHorizontal();
                    i++;
                }
                EditorGUILayout.EndScrollView();

                if (GUILayout.Button("生成", GUILayout.Width(200)))
                {
                    GeneralPrefab();
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        private void GeneralPrefab()
        {
            fps = int.Parse(fpsArray[fpsIndex]);
            var currentScene = EditorSceneManager.GetActiveScene();
            if (currentScene.name != "Auto")
            {
                EditorSceneManager.OpenScene("Assets/Editor/Auto.unity");
            }
            
            int i = 0;
            foreach (var dir in directories)
            { 
                var dirName = Path.GetFileName(dir);
                bool needGeneral = needGeneratePrefab[i];

                if (needGeneral)
                {
                    _GeneralPrefab(dirName, dir);
                }

                i++;
            }
        }
        private void _GeneralPrefab(string prefabName, string originDir)
        {
            string dir = $"Assets\\Art\\Character\\{prefabName}\\";
            string prefabPath = $"{dir}{prefabName}.prefab";
            Directory.CreateDirectory(dir);
            AssetDatabase.CopyAsset("Assets\\Editor\\P_template.prefab", prefabPath);
            AssetDatabase.Refresh();
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab == null)
            {
                Debug.LogError("模板数据不在了，需要跟新在生成.");
                return;
            }
            var render = prefab.GetComponentInChildren<SpriteRenderer>();
            if (render)
            {
                render.sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{originDir}\\idle_01.png");
            }
            
            var animator = prefab.GetComponentInChildren<Animator>();
            CreateAnimation(prefabName, originDir, animator);
        }
        private void CreateAnimation(string prefabName, string originDir,Animator animator)
        {
            string aniDir = "Assets\\Art\\Character\\" + prefabName + "\\ani\\";
            Directory.Delete("Assets\\Art\\Character\\" + prefabName,true);
            Directory.CreateDirectory(aniDir);
            
            Dictionary<string, List<string>> animationList =  new Dictionary<string, List<string>>();
            var files =  Directory.GetFiles(originDir, "*.png", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var k = fileName.IndexOf("_");
                var aniName = fileName.Substring(0, k);
                
                animationList.TryGetValue(aniName, out var aniList);
                if (aniList == null)
                {
                    animationList[aniName] = new List<string>();
                }
                animationList[aniName].Add(file);
            }

            string acc = aniDir + prefabName + ".controller";
            AssetDatabase.CopyAsset("Assets\\Editor\\Template.controller", acc);
            
            // 将动画添加到 Animator
            AnimatorController animatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(acc);
            animator.runtimeAnimatorController = animatorController;
            
            foreach (var ani in animationList)
            {
                // 创建 Animation
                AnimationClip animationClip = new AnimationClip();
                animationClip.frameRate = fps; // 设置帧率
                animationClip.legacy = false;
                if (ani.Key == "idle" || ani.Key == "run")
                {
                    animationClip.wrapMode = WrapMode.Loop;
                }

                ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[ani.Value.Count];
                // 添加关键帧
                for (int i = 0; i < ani.Value.Count; i++)
                {
                    string textPath = ani.Value[i];
                    ObjectReferenceKeyframe keyframe = new ObjectReferenceKeyframe
                    {
                        time = i * 1.0f / fps, // 根据帧率设置时间
                        value = AssetDatabase.LoadAssetAtPath<Sprite>(textPath)
                    };
                    keyFrames[i] = keyframe;
                }
                AnimationUtility.SetObjectReferenceCurve(animationClip, EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite"), keyFrames);

                // 保存动画
                string aniPath = aniDir + ani.Key + ".anim";
                AssetDatabase.CreateAsset(animationClip, aniPath);
                AssetDatabase.SaveAssets();

                if (ani.Key == "idle" || ani.Key == "run")
                {
                    string text = File.ReadAllText(aniPath);
                    string modifiedText = text.Replace("m_LoopTime: 0", "m_LoopTime: 1");
                    // 保存修改后的内容
                    File.WriteAllText(aniPath, modifiedText);
                }

                AnimatorState state =  animatorController.layers[0].stateMachine.AddState(ani.Key);
                state.motion = animationClip;
            }
            
            ChildAnimatorState[] states = animatorController.layers[0].stateMachine.states;
            bool isIdle = false;
            foreach (var state in states)
            {
                if (state.state.name == "idle")
                {
                    animatorController.layers[0].stateMachine.defaultState = state.state;
                    isIdle = true;
                }
            }

            if (isIdle == false)
            {
                foreach (var state in states)
                {
                    if (state.state.name == "run")
                    {
                        animatorController.layers[0].stateMachine.defaultState = state.state;
                    }
                } 
            }

        }
        #endregion
    }
}
