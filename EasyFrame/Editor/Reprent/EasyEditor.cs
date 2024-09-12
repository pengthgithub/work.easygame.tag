#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Easy
{
    [InitializeOnLoad]
    public partial class EasyEditor
    {
        [MenuItem("GameObject/场景/节点排序")]
        public static void SortChildren()
        {
            GameObject gameObject = Selection.activeGameObject;
            if (gameObject == null) return;

            List<Transform> children = new List<Transform>();
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                children.Add(gameObject.transform.GetChild(i));
            }

            children.Sort((a, b) => a.name.CompareTo(b.name));

            for (int i = 0; i < children.Count; i++)
            {
                children[i].SetSiblingIndex(i);
            }
        }
        
        static EasyEditor()
        {
            checkRes = EditorPrefs.GetBool("CheckRes");
            EditorApplication.update += OnUpdate;
            SceneView.duringSceneGui += OnSceneViewGUI;
        }

        static void OnSceneViewGUI(SceneView view)
        {
            Handles.BeginGUI();
            SceneView sceneView = SceneView.currentDrawingSceneView;
            var width = sceneView.position.width;
            var height = sceneView.position.height;

            float currentTime = 0;
            if (SfxParticleEditor.sfx)
            {
                currentTime = SfxParticleEditor.sfx._durationTime;
            }

            if (SfxParticleEditor.sfx)
            {
                GUILayout.BeginArea(new Rect(width - 150, height - 50, 145, 20)); // 规定显示区域为屏幕大小
                Color originalColor = EditorStyles.label.normal.textColor;
                // 设置标签颜色为红色
                EditorStyles.label.normal.textColor = Color.red;
                GUILayout.Label($"已播放时间：{currentTime}s", EditorStyles.label);
                // 将标签颜色恢复为原始颜色
                EditorStyles.label.normal.textColor = originalColor;
                GUILayout.EndArea();
            }
            Handles.EndGUI();
        }

        #region "游戏启动按钮"
        private static readonly Type kToolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");
        private static ScriptableObject sCurrentToolbar;
        private static void OnUpdate()
        {
            if (sCurrentToolbar == null)
            {
                UnityEngine.Object[] toolbars = Resources.FindObjectsOfTypeAll(kToolbarType);
                sCurrentToolbar = toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;
                if (sCurrentToolbar != null)
                {
                    FieldInfo root = sCurrentToolbar.GetType()
                        .GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
                    
                    // var concreteRoot = root.GetValue(sCurrentToolbar);
                    // VisualElement toolbarZone = concreteRoot.Q("ToolbarZoneRightAlign");
                    // VisualElement parent = new VisualElement()
                    // {
                    //     style =
                    //     {
                    //         flexGrow = 1,
                    //         flexDirection = FlexDirection.Row,
                    //     }
                    // };
                    // IMGUIContainer container = new IMGUIContainer();
                    // container.onGUIHandler += OnGuiBody;
                    // parent.Add(container);
                    // toolbarZone.Add(parent);
                }
            }
        }
        private static bool checkRes = false;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            var playBtnStart = EditorPrefs.GetBool("Play", false);
            EditorPrefs.SetBool("Play", false);
            
            if (SceneManager.GetActiveScene().name == "Game")
            {
                return;
            }
            if (playBtnStart)
            {
                SceneManager.LoadScene("Game");
            }
        }
       
        private static void OnGuiBody()
        {
            //自定义按钮加在此处
            GUILayout.BeginHorizontal();
            if (Application.isPlaying == false)
            {
                if (GUILayout.Button(new GUIContent("启动游戏", EditorGUIUtility.FindTexture("PlayButton"))))
                {
                    if (checkRes)
                    {
                        //S2DEditor.AutoAddAddress();
                    }
                    EditorPrefs.SetBool("Play", true);
                    EditorApplication.EnterPlaymode();
                }

                EditorGUI.BeginChangeCheck();
                checkRes = GUILayout.Toggle(checkRes, new GUIContent(""));
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetBool("CheckRes", checkRes);
                }
            }

            Time.timeScale = EditorGUILayout.Slider("", Time.timeScale, 0, 3);

            GUILayout.EndHorizontal();
        }
        #endregion
        
    }
}
#endif