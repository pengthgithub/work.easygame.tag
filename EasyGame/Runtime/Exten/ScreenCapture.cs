#if UNITY_EDITOR
using UnityEngine;
using System.IO;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Overlays;

/*
Usage:
1. Create a camera for rendering texture.
2. Create a RenderTexture object and set it as targetTexture on renderer camera.
1. Attach this script to the renderer camera's game object.
2. Set renderer camera's Clear Flags field to Solid Color and set Background color's alpha to zero.
3. Use the inspector to set frameRate and framesToCapture.
4. Choose your desired resolution in Unity's Game window (must be less than or equal to your screen resolution).
5. Turn on "Maximise on Play".
6. Play your scene. Screenshots will be saved to YourUnityProject/Screenshots by default.
*/

public class ScreenCapture : MonoBehaviour
{
    [SerializeField] private Camera renderCamera;
    [SerializeField] public List<GameObject> prefabs;
    [HideInInspector]public int frameRate = 24;
    
    [HideInInspector]public string prefabDirectory = "";
    [HideInInspector]public string outputDirectory = "";
    #region private fields
    private string folderName = "";
    private bool done = false;
    private int width;
    private int height;
    private RenderTexture renderTexture;
    private Texture2D outputTexture;
    #endregion

    private GameObject captureObject;
    private string currentAniName;
    [SerializeField] private float frameCount;
    private int currentFrame;
    private float estimatedTime;
    [SerializeField] private List<AnimationClip> clips;
    void Awake()
    {
        int width = Screen.width;
        int height = Screen.height;
        renderTexture = new RenderTexture(width, height, 24);
        renderCamera.targetTexture = renderTexture;
        CacheAndInitialiseFields();
        Time.captureFramerate = frameRate;
        clips = new List<AnimationClip>();
        renderCamera.backgroundColor = new Color(0, 0, 0, 0);
    }

    [SerializeField] private Vector3 boundSize;
    private Animator currentAni;
    void LateUpdate()
    {
        if (prefabs.Count == 0 && clips.Count == 0)
        {
            //Application.Quit();
            return;
        }
        if (captureObject == null)
        {
            captureObject = GameObject.Instantiate(prefabs[0]);
            captureObject.transform.eulerAngles = new Vector3(0, 180, 0);
            prefabs.RemoveAt(0);
            currentAni = captureObject.GetComponentInChildren<Animator>();
            if (!currentAni)
            {
                Debug.LogError(captureObject.name +"没有动画，不到处截图");
                GameObject.Destroy(captureObject);
                return;
            }
               
            RuntimeAnimatorController controller = currentAni.runtimeAnimatorController;
            if (!controller) return;
            clips.Clear();
            foreach (var clip in controller.animationClips)
            {
                clips.Add(clip);
            }

            Bounds unitedBounds = new Bounds();
            // 获取包围盒大小，然后设置摄像机的fov
           var meshs= captureObject.GetComponentsInChildren<SkinnedMeshRenderer>();
           foreach (var mes in meshs)
           {
               unitedBounds.Encapsulate( mes.bounds);
           }

           boundSize = unitedBounds.size;
           CalculateCameraOrthographicSize();
        }

        if (clips.Count == 0)
        {
            GameObject.Destroy(captureObject);
            captureObject = null;
            return;
        }

        if (frameCount == 0)
        {
            frameCount = clips[0].length;
            currentAniName = clips[0].name;
            clips.RemoveAt(0);
            currentAni.Play(currentAniName);
            currentFrame = 0;
            estimatedTime = 0;
            return;
        }

        if (estimatedTime > frameCount)
        {
            estimatedTime = 0;
            currentFrame = 0;
            frameCount = 0;
            return;
        }

        estimatedTime += Time.deltaTime;
        RenderTextureToPNG();
        currentFrame++;
    }
    void CalculateCameraOrthographicSize()
    {
        renderCamera.transform.position = new Vector3(0, boundSize.y * 0.5f + 0.2f, 10);
        // 设置相机的正交投影视野
        renderCamera.orthographicSize = boundSize.x * 0.5f + 1.0f;
    }
    void CacheAndInitialiseFields()
    {
        width = renderTexture.width;
        height = renderTexture.height;
        outputTexture = new Texture2D(width, height);
    }

    public void RenderTextureToPNG()
    {
        RenderTexture oldRT = RenderTexture.active; // Save old active render texture
        RenderTexture.active = renderTexture;

        outputTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        outputTexture.Apply();

        SavePng();
        RenderTexture.active = oldRT;
    }

    void SavePng()
    {
        string dirPath = $"{outputDirectory}/{captureObject.name.Replace("(Clone)", "")}/{currentAniName}/";
        if (Directory.Exists(dirPath) == false)
        {
            Directory.CreateDirectory(dirPath);
        }
        string path = string.Format("{0}/{1:D04}.png", dirPath, currentFrame);
        var pngShot = outputTexture.EncodeToPNG();
        File.WriteAllBytes(path, pngShot);
    }
}

[CustomEditor(typeof(ScreenCapture))]
public class PrefabBrowserWindow : Editor
{
    private ScreenCapture _capture;
    private void OnEnable()
    {
        _capture = target as ScreenCapture;
    }
    private Vector2 scrollPosition;
    public override void OnInspectorGUI()
    {   
        base.OnInspectorGUI();
        //EditorGUILayout.BeginHorizontal();
        // 选择Prefab目录
        if (GUILayout.Button("输出目录"))
        {
            _capture.outputDirectory = EditorUtility.OpenFolderPanel("Select OutPut Directory", "", "");
        }       
        // 显示Prefab目录路径
        EditorGUILayout.LabelField("OutPut Directory:", _capture.outputDirectory);
        //EditorGUILayout.EndHorizontal();

        //EditorGUILayout.BeginHorizontal();
        // 选择Prefab目录
        if (GUILayout.Button("预制件目录"))
        {
            string newDirectory = EditorUtility.OpenFolderPanel("Select Prefab Directory", "Assets", "");
            if (!string.IsNullOrEmpty(newDirectory) && newDirectory.StartsWith(Application.dataPath))
            {
                _capture.prefabDirectory = "Assets" + newDirectory.Substring(Application.dataPath.Length);
            }
            // 显示目录下的所有Prefab
            string[] prefabPaths = Directory.GetFiles(_capture.prefabDirectory, "*.prefab", SearchOption.AllDirectories);
          
            for (int i = 0; i < prefabPaths.Length; i++)
            {
                var prefab = (GameObject)AssetDatabase.LoadAssetAtPath<GameObject>(prefabPaths[i]);
                _capture.prefabs.Add(prefab);
            }
        }
        // 显示Prefab目录路径
        EditorGUILayout.LabelField("Prefab Directory:", _capture.prefabDirectory);
        //EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("帧率");
        EditorGUI.BeginChangeCheck();
        _capture.frameRate = EditorGUILayout.IntSlider(_capture.frameRate,24,120);
        if (EditorGUI.EndChangeCheck())
        {
            Time.captureFramerate = _capture.frameRate;
        }
       
        EditorGUILayout.EndHorizontal();
        
        //EditorGUILayout.PropertyField(prefabsProperty);
        serializedObject.ApplyModifiedProperties();
    }
}
#endif