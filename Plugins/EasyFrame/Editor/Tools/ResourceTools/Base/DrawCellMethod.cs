// Decompiled with JetBrains decompiler
// Type: WeChatWASM.Analysis.DrawCellMethod`1
// Assembly: wx-editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 491FB96F-C7AE-469B-A755-45BC3F066C9B
// Assembly location: C:\Users\pengt\Documents\SVN\Luck\Client\Lucky\Packages\com.qq.weixin.minigame@981890fdaa\Editor\wx-editor.dll
// XML documentation location: C:\Users\pengt\Documents\SVN\Luck\Client\Lucky\Packages\com.qq.weixin.minigame@981890fdaa\Editor\wx-editor.xml

using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

#nullable disable
namespace Analysis
{
    public delegate void DrawCellMethod<in T>(Rect cellRect, T item);
    
    public delegate bool FilterMethod<in T>(T data, string std);
    
    public delegate void SelectMethod<T>(List<T> datas);
    
    public delegate int CompareMethod<in T>(T data1, T data2);
    
    public class AssetViewItem : TreeViewItem
    {
        public ReferenceFinderData.AssetDescription data;
    }
}