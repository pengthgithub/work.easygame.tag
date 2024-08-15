using System;
using System.Text;
using Easy;
using Unity.Profiling;
using UnityEngine;

struct ProfileData
{
    public LineRenderer render;
    private int[] values;
    
    private int index;
    private int maxCount;
    private float xSpacing;
    private float normalizeValue;
    
    public ProfileData(Color color, Transform parent, float normalizedValue = 60)
    {
        index = 0;
        maxCount = 50;
        xSpacing = 0.2f;
        normalizeValue = normalizedValue;
        values = new int[maxCount];
        
        GameObject r = new GameObject();
        r.transform.SetParent(parent,false);
        render = r.AddComponent<LineRenderer>();
        render.startWidth = 0.05f;
        render.endWidth = 0.05f;
        //render.material = new Material();
        render.startColor = color;
        render.endColor = color;
        render.positionCount = maxCount;
        render.useWorldSpace = false;
    }
    
    public void UpdateLine(int value)
    {
        // 记录当前帧率
        values[index] = value;
        index = (index + 1) % maxCount;
        // 更新 LineRenderer 的位置
        for (int i = 0; i < maxCount; i++)
        {
            float x = i * xSpacing; // 增加 X轴位置间隔
            float y = values[(index + i) % maxCount] / normalizeValue; // Y轴位置，归一化到 0-1
            render.SetPosition(i, new Vector3(x, y, 0));
        }
    }

    public void BaseLine()
    {
        // 更新基准线的位置
        render.SetPosition(0, new Vector3(0, 1, 0));
        render.SetPosition(1, new Vector3(maxCount * xSpacing, 1, 0));
    }
}
public class CustomProfile : MonoBehaviour
{
    private ProfileData baseLine;
    private ProfileData sfxLine;
    private ProfileData roleLine;
    
    private ProfileData passCallLine;
    private ProfileData drawCallLine;
    private ProfileData verticesCallLine;
    ProfilerRecorder setPassCallsRecorder;
    ProfilerRecorder drawCallsRecorder;
    ProfilerRecorder verticesRecorder;
    ProfilerRecorder systemMemoryRecorder;
    ProfilerRecorder gcMemoryRecorder;
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

    }

    // void Start()
    // {
    //     baseLine = new ProfileData(Color.white,transform);
    //     baseLine.render.positionCount = 2;
    //     baseLine.BaseLine();
    //     sfxLine = new ProfileData(Color.green,transform);
    //     roleLine = new ProfileData(Color.black,transform);
    //     
    //     passCallLine = new ProfileData(Color.red,transform, 150);
    //     drawCallLine= new ProfileData(Color.yellow,transform, 250);
    //     verticesCallLine= new ProfileData(Color.black,transform, 60000);
    //     
    //     setPassCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "SetPass Calls Count");
    //     drawCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count");
    //     verticesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Vertices Count");
    //     
    //     systemMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "System Used Memory");
    //     gcMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Reserved Memory");
    // }
    // StringBuilder sb = new StringBuilder(500);
    // void Update()
    // {
    //     if (Time.frameCount % 60 != 0) return;
    //     sb.Clear();
    //     sfxLine.UpdateLine(ESfx.sfxCount);
    //     roleLine.UpdateLine(ECharacter.roleCount);
    //     sb.AppendLine($"SFX Count: {ESfx.sfxCount}");
    //     sb.AppendLine($"Character Count: {ECharacter.roleCount}");
    //     
    //     if (setPassCallsRecorder.Valid)
    //     {
    //         int batchCount = (int)setPassCallsRecorder.LastValue;
    //         passCallLine.UpdateLine(batchCount);
    //         sb.AppendLine($"SetPass Calls: {setPassCallsRecorder.LastValue}");
    //     }
    //     if (drawCallsRecorder.Valid)
    //     {
    //         int batchCount = (int)drawCallsRecorder.LastValue;
    //         drawCallLine.UpdateLine(batchCount);
    //         sb.AppendLine($"Draw Calls: {drawCallsRecorder.LastValue}");
    //     }
    //     if (verticesRecorder.Valid)
    //     {
    //         int batchCount = (int)verticesRecorder.LastValue;
    //         verticesCallLine.UpdateLine(batchCount);
    //         sb.AppendLine($"Vertices: {verticesRecorder.LastValue * 0.001f}K");
    //     }
    //
    //     if (gcMemoryRecorder.Valid)
    //     {
    //         sb.AppendLine($"GC Memory: {gcMemoryRecorder.LastValue / (1024 * 1024)} MB");
    //     }
    //
    //     if (systemMemoryRecorder.Valid)
    //     {
    //         sb.AppendLine($"System Memory: {systemMemoryRecorder.LastValue / (1024 * 1024)} MB");
    //     }
    // }
    private GUIStyle textAreaStyle;
    void OnGUI()
    {
        GUI.skin.label.fontSize = 100;
        // 计算 FPS
        float fps = 1.0f / Time.deltaTime;
        GUILayout.BeginArea(new Rect(100, 50, 600, 500));
        GUILayout.Label("FPS: " + Mathf.Round(fps).ToString());
        GUILayout.EndArea();
    }

    private void FaceCamera()
    {
        var mainCamera = CameraManager.Instance.MainCamera;
        if (mainCamera == null) return;
        var width = 10;
        Vector3 pos = mainCamera.transform.position + mainCamera.transform.forward * (mainCamera.nearClipPlane + 5);
        pos += new Vector3(width * 0.5f, -width, 0);
        // 设置面片位置
        transform.position = pos;
        // 使面片面向摄像机
        //transform.LookAt(mainCamera.transform);
    }
}