using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Reflection;
using UnityEngine.UIElements;
using System;
using static UnityEngine.ParticleSystem;
using static UnityEngine.GraphicsBuffer;

[InitializeOnLoad]
public static class CustomFunExtend
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            return;
        }

        var flag = EditorPrefs.GetBool("EditorStart");
        if (flag)
        {
            EditorPrefs.SetBool("EditorStart", false);
            SceneManager.LoadScene("Game");
        }
    }


    private static readonly Type kToolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
    private static ScriptableObject sCurrentToolbar;


    static CustomFunExtend()
    {
        checkRes = EditorPrefs.GetBool("CheckRes");
        EditorApplication.update += OnUpdate;
    }

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
                VisualElement concreteRoot = root.GetValue(sCurrentToolbar) as VisualElement;

                VisualElement toolbarZone = concreteRoot.Q("ToolbarZoneRightAlign");
                VisualElement parent = new VisualElement()
                {
                    style =
                    {
                        flexGrow = 1,
                        flexDirection = FlexDirection.Row,
                    }
                };
                IMGUIContainer container = new IMGUIContainer();
                container.onGUIHandler += OnGuiBody;
                parent.Add(container);
                toolbarZone.Add(parent);
            }
        }
    }

    #if UNITY_EDITOR
    private static bool checkRes = false;

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
                    AddressTools.AutoAddAddress();
                }

                EditorPrefs.SetBool("EditorStart", true);
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
    #endif
}