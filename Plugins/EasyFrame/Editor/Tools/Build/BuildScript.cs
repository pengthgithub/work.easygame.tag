using System.Diagnostics;
using System.Net.NetworkInformation;
using Editor.Build;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

[ExecuteInEditMode,ExecuteAlways]
public class BuildScript
{
    public static BuildConfig config;
    
    private static void Init()
    {
        if (config == null)
        {
            config = AssetDatabase.LoadAssetAtPath<BuildConfig>("Assets/Editor/Build/BuildConfig.asset");
            config.qrCodePath = $"{Application.dataPath}/Editor/Build/code.txt";
        }
    }

    public static void BuildGame()
    {
        Init();
#if UNITY_EDITOR_WX
        if (WXConvertCore.DoExport() == WXConvertCore.WXExportError.SUCCEED) {
            Debug.Log("Webgl 导出成功");
        } else {
            Debug.LogError("Webgl 导出失败");
        }
#endif
    }
    
    public static async void BuildProject()
    {
         Init();
         
         FeiShuAPI.Init();
         FeiShuAPI.stopwatch.Start();
#if UNITY_EDITOR_WX
         if (WXConvertCore.DoExport() == WXConvertCore.WXExportError.SUCCEED) {
             Debug.Log("Webgl 导出成功");
             TestMessage();
         } else {
             Debug.LogError("Webgl 导出失败");
         }
#endif
    }
    
    public static void TestMessage()
    {
        Init();
        DoWeChatDeveloperTools();
        Send();
    }
    
    public static void Send()
    {
        Init();
        FeiShuAPI.Send();
    }
    
    static string GetLocalIPAddress()
    {
        string localIP = string.Empty;

        // 获取计算机上的所有网络接口
        foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            // 检查网络接口是否启用
            if (ni.OperationalStatus == OperationalStatus.Up)
            {
                // 获取网络接口的所有 IP 地址
                foreach (var ip in ni.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) // IPv4
                    {
                        localIP = ip.Address.ToString();
                        return localIP; // 返回第一个找到的 IPv4 地址
                    }
                }
            }
        }

        return localIP; // 如果没有找到，返回空字符串
    }
    
    [MenuItem("Tools/Build/服务器打包")]
    public static async void DoBuild()
    {
        string localIP = GetLocalIPAddress();
        SendHttp($"http://192.168.1.31:5000?action=2&ip={localIP}");
    }
    
    [MenuItem("Tools/Build/获取二维码")]
    public static async void GetQrd()
    {
        string localIP = GetLocalIPAddress();
        SendHttp($"http://192.168.1.31:5000?action=1&ip={localIP}");
    }
    
    [MenuItem("Tools/Build/本地打包")]
    public static void DoLocalBuild()
    {
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows);
            Debug.Log("Switched to " + BuildTarget.StandaloneWindows);
        }
        
        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
        PlayerSettings.defaultScreenWidth = 828;
        PlayerSettings.defaultScreenHeight = 1792;
        PlayerSettings.runInBackground = true;
        PlayerSettings.resizableWindow = true;
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;

        // 确保设置已加载
        if (settings == null)
        {
            Debug.LogError("Addressable Asset Settings not found!");
            return;
        }
        
        // 开始构建 Addressables
        AddressableAssetSettings.BuildPlayerContent();
        
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = new string[]{"Assets/Scenes/Game.unity"},
            locationPathName = "Builds/Lucky.exe", // 输出路径
            target = BuildTarget.StandaloneWindows,
            options = BuildOptions.Development | BuildOptions.ConnectWithProfiler // 启用 Development Build 和 AutoConnect Profiler
        };
        BuildPipeline.BuildPlayer(buildPlayerOptions);

    }
    private static async void SendHttp(string url)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            var operation = www.SendWebRequest();
            while (!operation.isDone)
            {
                await System.Threading.Tasks.Task.Yield(); // 等待下一帧
            }

            if (www.result == UnityWebRequest.Result.Success)
            {
                string response = www.downloadHandler.text;
                Debug.Log(response);
            }
        }
    }
    
    /// <summary>
    /// 发布完成后，微信开发者工具自动生成预览，或者上传到微信开发者平台
    /// </summary>
    private static void DoWeChatDeveloperTools()
    {
#if UNITY_EDITOR_WX
        string dir = WXConvertCore.config.ProjectConf.DST + "/minigame";
        string[] commands = new string[]
        {
            "chcp 65001",
            $"cd /d {config.wxDeveloperToolPath} ",
            $"cli preview --project \"{dir}\" --qr-output {config.qrCodePath} --qr-format base64"
        };
        ExecuteCommands(commands);
#endif
    }
    
    static void ExecuteCommands(string[] commands)
    {
        var cmd = "";
        foreach (var command in commands)
        {
            Debug.Log(command);
            cmd += command + " && ";
        }
        cmd = cmd.Substring(0, cmd.Length - 4);
        ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", "/C " + cmd);
        processInfo.RedirectStandardOutput = true;
        processInfo.UseShellExecute = false;
        processInfo.CreateNoWindow = false;

        using (Process process = Process.Start(processInfo))
        {
            // 等待命令执行完毕
            process.WaitForExit();
            string output = process.StandardOutput.ReadToEnd();
            if (output.Contains("error"))
            {
                UnityEngine.Debug.LogError("Output: " + output);
            }
            else
            {
                UnityEngine.Debug.Log("Output: " + output);
            }
        }
    }
}
