// Decompiled with JetBrains decompiler
// Type: WeChatWASM.Analysis.TextureInfo
// Assembly: wx-editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 491FB96F-C7AE-469B-A755-45BC3F066C9B
// Assembly location: C:\Users\pengt\Documents\SVN\Luck\Client\Lucky\Packages\com.qq.weixin.minigame@981890fdaa\Editor\wx-editor.dll
// XML documentation location: C:\Users\pengt\Documents\SVN\Luck\Client\Lucky\Packages\com.qq.weixin.minigame@981890fdaa\Editor\wx-editor.xml

using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

#nullable disable
namespace Analysis
{
    public class TextureInfo : BaseInfo
    {
        public bool mipmapEnabled;
        public int maxTextureSize;
        public string textureFormat;
        public string textureType;
        public bool sRGBTexture;
        public bool isReadable;
        public int compressionQuality;
        public int width;
        public int height;
        public string memorySize;
        public long _memorySize;
        public TextureImporterFormat _webglFormat;
        public string webglFormat;
        public string dimension;
        public string ext;
        public long originalMemorySize;

        private TextureImporter DK;
        private Texture Dk;

        public TextureInfo()
        {
        }

        public Texture texture => this.Dk;

        public TextureInfo(TextureImporter info, Texture texture)
        {
            TextureImporterPlatformSettings platformTextureSettings = info.GetPlatformTextureSettings(StringUtils.Hs);
            this.DK = info;
            this.Dk = texture;
            this.mipmapEnabled = info.mipmapEnabled;
            this.maxTextureSize = platformTextureSettings.maxTextureSize;
            this.textureFormat = info.textureCompression.ToString();
            if (platformTextureSettings.format == TextureImporterFormat.Automatic)
                this._webglFormat = info.GetAutomaticFormat(StringUtils.Hs);
            else
                this._webglFormat = platformTextureSettings.format;
            this.webglFormat = this._webglFormat.ToString();
            this.textureType = info.textureType.ToString();
            this.sRGBTexture = info.sRGBTexture;
            this.isReadable = info.isReadable;
            this.compressionQuality = info.compressionQuality;
            this.assetPath = info.assetPath;
            this.width = texture.width;
            this.height = texture.height;
            this.originalMemorySize = Profiler.GetRuntimeMemorySizeLong((Object)texture);
            this.memorySize = EditorUtility.FormatBytes(this.originalMemorySize);
            this._memorySize = this.originalMemorySize;
            this.name = texture.name;
            this.dimension = texture.dimension.ToString();
            this.ext = Path.GetExtension(AssetDatabase.GetAssetPath((Object)texture)).ToString();
        }
    }
}