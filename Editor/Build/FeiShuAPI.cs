using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

/// <summary>
/// 飞书消息发送
/// </summary>
public class FeiShuAPI 
{
    /// <summary>
    /// 耗时统计
    /// </summary>
   public static Stopwatch stopwatch = new Stopwatch();
   public static void Init()
   {
       if (File.Exists(BuildScript.config.qrCodePath))
       {
           File.Delete(BuildScript.config.qrCodePath);
       }
   }
    public static async Task<bool> Send()
    {
        if (File.Exists(BuildScript.config.qrCodePath) == false)
        {
           Debug.LogWarning("预览失败，二维码文件不存在");
           return false;
        }
        
        var access = await get_access_token();
        if(string.IsNullOrEmpty(access)) return false;
        var qrCode = await get_qr_code(access);
        if(string.IsNullOrEmpty(qrCode)) return false;
        var result = await send_group_message(qrCode);
        return result;
    }
    
    private static async Task<string> get_access_token()
    {
        string json = "";
        json += "{";
        json += $"    \"app_id\": \"{BuildScript.config.appid}\",";
        json += $"    \"app_secret\": \"{BuildScript.config.appsecret}\"";
        json += " }";
        
        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(BuildScript.config.accessUrl, json))
        {
            www.method = UnityWebRequest.kHttpVerbPOST;
            www.SetRequestHeader("Content-Type", "application/json; charset=utf-8");
            www.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            www.downloadHandler = new DownloadHandlerBuffer();

            // 发送请求并等待响应
            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                await System.Threading.Tasks.Task.Yield(); // 等待下一帧
            }

            if (www.result == UnityWebRequest.Result.Success)
            {
                string response = www.downloadHandler.text;
                int k = response.IndexOf("t-");
                response = response.Substring(k, response.Length - k);
                var accessToken = response.Replace("\"}","");
                Debug.Log("accessToken: " + accessToken);
                return accessToken;
            }

            Debug.LogError("accessToken: " + www.error);
            return "";
        }
    }
    
    private static async Task<string> get_qr_code(string accessToken)
    {
        var base64String = File.ReadAllText(BuildScript.config.qrCodePath);
        byte[] imageBytes = Convert.FromBase64String(base64String);
        WWWForm form = new WWWForm();
        form.AddField("image_type", "message");
        form.AddBinaryData("image", imageBytes, "qrCode", "image/png");
        using (UnityWebRequest www = UnityWebRequest.Post(BuildScript.config.imgUploadUrl, form))
        {
            www.method = UnityWebRequest.kHttpVerbPOST;
            www.SetRequestHeader("Authorization", "Bearer " + accessToken);
            var operation = www.SendWebRequest();
            while (!operation.isDone)
            {
                await System.Threading.Tasks.Task.Yield(); // 等待下一帧
            }

            if (www.result == UnityWebRequest.Result.Success)
            {
                string response = www.downloadHandler.text;
                int k = response.IndexOf("img_");
                response = response.Substring(k, response.Length - k);
                k = response.IndexOf("}");
                response = response.Substring(0, k - 1);
                Debug.Log("图片Key: " + response);
                return response;
            }
    
            Debug.LogError("图片Key: " + www.error);
            return "";
        }
    }
    
    private static async Task<bool> send_group_message(string imgKey)
    {
        stopwatch.Stop();

        var useTime = stopwatch.ElapsedMilliseconds * 0.001f * 0.01666666f;
        var json = "{\"msg_type\":\"text\",\"content\":{\"text\":\"预览版二维码,时效25分钟,耗时:"+useTime+"分\"}}";
        var result = await send_message(json);
        json = "{\"msg_type\":\"image\",\"content\":{\"image_key\":\""+imgKey+"\"}}";
        result = await send_message(json);
        return result;
    }

    private static async Task<bool> send_message(string json)
    {
        var webhookUrl = $"{BuildScript.config.hookUrl}{BuildScript.config.webhook}";
        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(webhookUrl, json))
        {
            www.method = UnityWebRequest.kHttpVerbPOST;
            www.SetRequestHeader("Content-Type", "application/json");
            www.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            www.downloadHandler = new DownloadHandlerBuffer();

            // 发送请求并等待响应
            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                await System.Threading.Tasks.Task.Yield(); // 等待下一帧
            }

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("群消息发送成功");
                return true;
            }
 
            Debug.LogError("群消息: " + www.error);
            return false;
        }
    }
}
