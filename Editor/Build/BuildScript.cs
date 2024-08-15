using System.Diagnostics;
using System.Threading.Tasks;
using Editor.Build;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using WeChatWASM;
using Debug = UnityEngine.Debug;

[ExecuteInEditMode,ExecuteAlways]
public class BuildScript:MonoBehaviour
{
    public static BuildConfig config;
    [MenuItem("Tools/Build/Build Project")]
    public static async void BuildProject()
    {
        config = AssetDatabase.LoadAssetAtPath<BuildConfig>("Assets/Editor/Build/BuildConfig.asset");
        config.qrCodePath = $"{Application.dataPath}/Editor/Build/code.txt";
        
         FeiShuAPI.Init();
         FeiShuAPI.stopwatch.Start();
         if (WXConvertCore.DoExport() == WXConvertCore.WXExportError.SUCCEED) {
             Debug.Log("Webgl 导出成功");
             DoWeChatDeveloperTools();
             await Task.Delay(3000);
             DoFeiShu();
         } else {
             Debug.LogError("Webgl 导出失败");
         }
    }
        
    [MenuItem("Tools/Build/打包")]
    public static async void DoBuild()
    {
        using (UnityWebRequest www = UnityWebRequest.PostWwwForm("http://192.168.1.6:8081?build=true", ""))
        {
            www.method = UnityWebRequest.kHttpVerbPOST;
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

    private static void DoFeiShu()
    {
        FeiShuAPI.Send();
    }
    
    /// <summary>
    /// 发布完成后，微信开发者工具自动生成预览，或者上传到微信开发者平台
    /// </summary>
    private static void DoWeChatDeveloperTools()
    {
        string dir = WXConvertCore.config.ProjectConf.DST + "/minigame";
        string[] commands = new string[]
        {
            "chcp 65001",
            $"cd /d {config.wxDeveloperToolPath} ",
            $"cli preview --project \"{dir}\" --qr-output {config.qrCodePath} --qr-format base64"
        };
        ExecuteCommands(commands);
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
