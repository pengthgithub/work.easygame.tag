using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

namespace Editor.Build
{
    public class Build
    {
        [MenuItem("Tools/产品/打包前准备")]
        public static void BeforePack()
        {
            UnityEditor.AddressableAssets.AddressablesPreferences.AutoAddConfig();
        }

        [MenuItem("Tools/产品/打包后输出")]
        public static void AfterPack()
        {
            _AfterPack();
        }
        
        [MenuItem("Tools/产品/输出资源信息")]
        public static void PrintResLog()
        {
            UnityEngine.AddressableAssets.Addressables.Instance?.PrintResLog();
        }

        private static void _AfterPack()
        {
            var asset = "Assets\\Editor\\LoadOrder.txt";
            if (File.Exists(asset) == false)
            {
                Debug.LogWarning("文件不存在:" + asset);
                return;
            }
            var orderPath = AssetDatabase.LoadAssetAtPath<TextAsset>(asset);
            
            WeChatWASM.WXEditorScriptObject config = WeChatWASM.UnityUtil.GetEditorConf();
            var dstDir = config.ProjectConf.DST;
            if (string.IsNullOrEmpty(dstDir))
            {
                EditorUtility.DisplayDialog("预下载资源","微信小游戏里面需要配置导出路径，且需要先打包.","确认");
                return;
            }

            var dirAa = dstDir + "/webgl/StreamingAssets/aa";
            var files = Directory.GetFiles(dirAa, "*.*", SearchOption.AllDirectories);
            string cdnStr = "";
            for (int i = 0; i < files.Length; i++)
            {
                var fileName = files[i];
                int k = fileName.IndexOf("/webgl/");
                if (k == -1)
                {
                    break;
                }
                var path = fileName.Substring(k, fileName.Length - k);
                path = path.Replace("\\", "/");
                cdnStr += "CDN"+ path + "\n";
            }
            
            Dictionary<string, long> dic = new Dictionary<string, long>();
            string preLoadPath = "";
            string[] lines = orderPath.text.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
               var _line = line.Replace("\r", "");
                for (int i = 0; i < files.Length; i++)
                {
                    var fileName = files[i];
                    fileName = fileName.Replace("\\", "/");
                    if (fileName.IndexOf(_line) != -1)
                    { 
                        FileInfo fileInfo = new FileInfo(fileName);
                        if (fileInfo.Exists)
                        {
                            dic[fileName] = fileInfo.Length;
                        }
                        break;
                    }
                }
            }
            
            var sortedDic = dic.OrderByDescending(kvp => kvp.Value);
            int count = 0;
            preLoadPath = "";
            // 输出排序结果
            foreach (var kvp in sortedDic)
            {
                count++;
                if(count > 15) break;
                int k = kvp.Key.IndexOf("/aa/"); 
                var path = kvp.Key.Substring(k, kvp.Key.Length - k);
                preLoadPath += "\"" + path + "\",\n";
            }
            File.WriteAllText(dstDir + "/preDown.txt",preLoadPath);  
            
            if (false)
            {
                var dir = dstDir + "\\minigame\\game.js";
                var gameJs = File.ReadAllText(dir);
                int index = gameJs.IndexOf(" preloadDataList: [");
                var first = gameJs.Substring(0, index);
                first += " preloadDataList: [\n" + preLoadPath;
                first += preLoadPath;
                first += "],\n";
                index = gameJs.IndexOf("contextConfig:");
                var second = gameJs.Substring(index, gameJs.Length - index);
                first += second;
                File.WriteAllText(dir,first);  
            }

#region CopySound 拷贝音频
#if UNITY_WEBGL_WX_AUDIO
            CopyAudio(true);
#endif
        #endregion
            File.WriteAllText(dstDir + "/cdn.txt",cdnStr);  
            
            //Debug.LogWarning("预下载资源:\n" + preLoadPath);
            EditorUtility.DisplayDialog("预下载资源","文件保存目录:" + dstDir,"确认");
        }

        public static void CopyAudio(bool flag = false)
        {
            WeChatWASM.WXEditorScriptObject config = WeChatWASM.UnityUtil.GetEditorConf();
            var dstDir = config.ProjectConf.DST;
            
            var destDir = dstDir + "/sound";
            if (flag)
            {
                destDir = dstDir + "/webgl/StreamingAssets/aa/WebGL/sound";
                if (Directory.Exists(destDir))
                {
                    Directory.Delete(destDir, true);
                }
                Directory.CreateDirectory(destDir);
            }

            var shoundFiles = Directory.GetFiles("Assets//Art", "*.mp3", SearchOption.AllDirectories);
            string wxSoundConfg = "";
            foreach (var audio in shoundFiles)
            {
                if(audio.Contains(".meta")) continue;
                
                var extension = Path.GetExtension(audio);
                var fileName = Path.GetFileNameWithoutExtension(audio);
                
                var dir = Application.dataPath + audio.Replace("Assets/", "");
                dir = dir.Replace("\\", "/");
                
                if (File.Exists(dir))
                {
                    var md5 = GetFileMD5(dir);
                    wxSoundConfg += $"{fileName}={md5}\n";
                    var dest = $"{destDir}/{fileName}_{md5}{extension}";
                    if (File.Exists(dest) == false && flag)
                    {
                        File.Copy(dir, dest);
                    }
                }
            }
            
            File.WriteAllText("Assets/Art/Config/sound_md5.bytes",wxSoundConfg); 
        }
        public static string GetFileMD5(string path)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(path))
                {
                    byte[] hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}