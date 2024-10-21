// Decompiled with JetBrains decompiler
// Type: WeChatWASM.Analysis.OptimizeTexture
// Assembly: wx-editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 491FB96F-C7AE-469B-A755-45BC3F066C9B
// Assembly location: C:\Users\pengt\Documents\SVN\Luck\Client\Lucky\Packages\com.qq.weixin.minigame@981890fdaa\Editor\wx-editor.dll
// XML documentation location: C:\Users\pengt\Documents\SVN\Luck\Client\Lucky\Packages\com.qq.weixin.minigame@981890fdaa\Editor\wx-editor.xml

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

#nullable disable
namespace Analysis
{
    public static class OptimizeTexture
    {
        private static string aR = StringUtils.HV;

        public static bool CheckNeedOptimization(Texture texture, out TextureImporter textureImporter)
        {
            TextureWindow instance = BaseWindow<TextureWindow>.GetInstance();
            string assetPath = AssetDatabase.GetAssetPath((UnityEngine.Object)texture);
            textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (!(bool)(UnityEngine.Object)textureImporter || !(bool)(UnityEngine.Object)texture)
                return false;
            TextureImporterPlatformSettings platformTextureSettings =
                textureImporter.GetPlatformTextureSettings(StringUtils.Hs);
            if (instance.checkMipMap && !textureImporter.mipmapEnabled)
                return false;
            if (instance.formatError)
            {
                List<TextureImporterFormat> textureImporterFormatList = new List<TextureImporterFormat>()
                {
                    TextureImporterFormat.DXT5,
                    TextureImporterFormat.DXT5Crunched,
                    TextureImporterFormat.DXT1,
                    TextureImporterFormat.DXT1Crunched
                };
                TextureImporterFormat textureImporterFormat =
                    platformTextureSettings.format == TextureImporterFormat.Automatic
                        ? textureImporter.GetAutomaticFormat(StringUtils.Hs)
                        : platformTextureSettings.format;
                if (OptimizeTexture.A(texture.width) && OptimizeTexture.A(texture.height))
                    return false;
            }

            return (!instance.checkIsReadable || textureImporter.isReadable) &&
                   (!instance.checkMaxSize || platformTextureSettings.maxTextureSize >= 512);
        }

        private static bool A([In] int obj0) => (obj0 & obj0 - 1) == 0;

        public static void Optimize(List<TextureInfo> textureInfos)
        {
            OptimizeTexture.RecordSettings(textureInfos);
            TextureWindow instance = BaseWindow<TextureWindow>.GetInstance();
            int count1 = textureInfos.Count;
            int num1 = 0;
            List<Texture> textureList = new List<Texture>();
            int count2 = textureInfos.Count;
            int num2 = 0;
            foreach (TextureInfo textureInfo in textureInfos)
            {
                ++num2;
                EditorUtility.DisplayProgressBar(string.Format(StringUtils.HT, (object)num2, (object)count2),
                    StringUtils.Ht + textureInfo.assetPath, (float)num2 * 1f / (float)count2);
                ++num1;
                TextureImporter atPath = AssetImporter.GetAtPath(textureInfo.assetPath) as TextureImporter;
                TextureImporterPlatformSettings platformTextureSettings =
                    atPath.GetPlatformTextureSettings(StringUtils.Hs);
                platformTextureSettings.overridden = true;
                int num3 = Math.Max(textureInfo.width, textureInfo.height) / 2;
                bool flag = false;
                if (instance.disableReadable)
                {
                    flag = true;
                    atPath.isReadable = false;
                }

                if (instance.disableMipmap)
                {
                    flag = true;
                    atPath.mipmapEnabled = false;
                }

                if (instance.changeMaxSize)
                {
                    flag = true;
                    platformTextureSettings.maxTextureSize = instance.selectedMaxSizeIdx != 0
                        ? int.Parse(instance.maxSizeOptions[instance.selectedMaxSizeIdx])
                        : OptimizeTexture.a(num3);
                }

                if (instance.changeFormat)
                {
                    flag = true;
                    Dictionary<string, TextureImporterFormat> formatMap = instance.formatMap;
                    List<string> stringList = new List<string>((IEnumerable<string>)formatMap.Keys);
                    int selectedFormat = instance.selectedFormat;
                    TextureImporterFormat textureImporterFormat = formatMap[stringList[selectedFormat]];
                    platformTextureSettings.name = StringUtils.Hs;
                    platformTextureSettings.format = textureImporterFormat;
                }

                if (flag)
                {
                    Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(textureInfo.assetPath);
                    textureList.Add(texture);
                    EditorUtility.DisplayCancelableProgressBar(StringUtils.HU, StringUtils.Hu + num1.ToString(),
                        (float)num1 / (float)count1);
                    atPath.SetPlatformTextureSettings(platformTextureSettings);
                    atPath.SaveAndReimport();
                }
            }

            EditorUtility.ClearProgressBar();
        }

        private static int a([In] int obj0)
        {
            if (obj0 <= 32)
                return 32;
            if (obj0 > 32 && obj0 <= 64)
                return 64;
            if (obj0 > 64 && obj0 <= 128)
                return 128;
            if (obj0 > 128 && obj0 <= 256)
                return 256;
            if (obj0 > 256 && obj0 <= 512)
                return 512;
            return obj0 > 512 && obj0 <= 1024 ? 1024 : 1024;
        }

        public static void RecordSettings(List<TextureInfo> textureInfos)
        {
            if (textureInfos.Count == 0)
                return;
            if (File.Exists(OptimizeTexture.aR))
                File.Delete(OptimizeTexture.aR);
            List<string> graph1 = new List<string>();
            List<TextureInfo> graph2 = new List<TextureInfo>();
            foreach (TextureInfo textureInfo in textureInfos)
            {
                TextureInfo baseInfo = new TextureInfo();
                baseInfo.assetPath = textureInfo.assetPath;
                baseInfo.maxTextureSize = textureInfo.maxTextureSize;
                baseInfo.mipmapEnabled = textureInfo.mipmapEnabled;
                baseInfo.isReadable = textureInfo.isReadable;
                baseInfo._webglFormat = textureInfo._webglFormat;
                graph1.Add(AssetDatabase.AssetPathToGUID(textureInfo.assetPath));
                graph2.Add(baseInfo);
            }

            using (FileStream serializationStream = File.OpenWrite(OptimizeTexture.aR))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize((Stream)serializationStream, (object)graph1);
                binaryFormatter.Serialize((Stream)serializationStream, (object)graph2);
            }
        }

        public static void Recover(List<TextureInfo> textureInfos)
        {
            if (!File.Exists(OptimizeTexture.aR))
                return;
            List<string> stringList = new List<string>();
            List<TextureInfo> baseInfoList = new List<TextureInfo>();
            using (FileStream serializationStream = File.OpenRead(OptimizeTexture.aR))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                stringList = (List<string>)binaryFormatter.Deserialize((Stream)serializationStream);
                baseInfoList = (List<TextureInfo>)binaryFormatter.Deserialize((Stream)serializationStream);
            }

            int count = textureInfos.Count;
            for (int index = 0; index < count; ++index)
            {
                string assetPath = textureInfos[index].assetPath;
                if (!string.IsNullOrEmpty(assetPath))
                {
                    TextureImporterPlatformSettings platformSettings = new TextureImporterPlatformSettings();
                    EditorUtility.DisplayCancelableProgressBar(StringUtils.HU, StringUtils.Hu + index.ToString(),
                        (float)index / (float)count);
                    TextureImporter atPath = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                    TextureInfo baseInfo = baseInfoList[index];
                    atPath.maxTextureSize = baseInfo.maxTextureSize;
                    atPath.mipmapEnabled = baseInfo.mipmapEnabled;
                    atPath.isReadable = baseInfo.isReadable;
                    platformSettings.name = StringUtils.Hs;
                    platformSettings.format = baseInfo._webglFormat;
                    atPath.SetPlatformTextureSettings(platformSettings);
                    atPath.SaveAndReimport();
                }
            }

            File.Delete(OptimizeTexture.aR);
            EditorUtility.ClearProgressBar();
        }
    }
}