// Decompiled with JetBrains decompiler
// Type: WeChatWASM.Analysis.FontInfo
// Assembly: wx-editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 491FB96F-C7AE-469B-A755-45BC3F066C9B
// Assembly location: C:\Users\pengt\Documents\SVN\Luck\Client\Lucky\Packages\com.qq.weixin.minigame@981890fdaa\Editor\wx-editor.dll
// XML documentation location: C:\Users\pengt\Documents\SVN\Luck\Client\Lucky\Packages\com.qq.weixin.minigame@981890fdaa\Editor\wx-editor.xml

using UnityEngine;

#nullable disable
namespace Analysis
{
    public class FontInfo : BaseInfo
    {
        public FontInfo(Font info, string assetPath)
        {
            this.assetPath = assetPath;
            this.name = info.name;
        }
    }
}