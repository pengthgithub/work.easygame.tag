// Decompiled with JetBrains decompiler
// Type: WeChatWASM.Analysis.FontWindow
// Assembly: wx-editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 491FB96F-C7AE-469B-A755-45BC3F066C9B
// Assembly location: C:\Users\pengt\Documents\SVN\Luck\Client\Lucky\Packages\com.qq.weixin.minigame@981890fdaa\Editor\wx-editor.dll
// XML documentation location: C:\Users\pengt\Documents\SVN\Luck\Client\Lucky\Packages\com.qq.weixin.minigame@981890fdaa\Editor\wx-editor.xml

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

#nullable disable
namespace Analysis
{
    public class FontWindow : BaseWindow<FontWindow>
    {
        private FontWindow.AssetDataTable an;
        private List<FontInfo> aO;
        private List<FontInfo> ao;
        private List<FontInfo> aP;

        public FontWindow()
        {
            if (this.an != null)
                return;
            this.an = new FontWindow.AssetDataTable(new List<FontInfo>(), this.GetViewColumn(),
                new FilterMethod<FontInfo>(this.A), new SelectMethod<FontInfo>(this.OnRowSelect));
        }

        public CommonTableColumn<FontInfo>[] GetViewColumn()
        {
            CommonTableColumn<FontInfo>[] viewColumn = new CommonTableColumn<FontInfo>[2];
            CommonTableColumn<FontInfo> commonTableColumn1 = new CommonTableColumn<FontInfo>();
            commonTableColumn1.headerContent = new GUIContent(StringUtils.Hl);
            commonTableColumn1.canSort = true;
            commonTableColumn1.minWidth = 170f;
            commonTableColumn1.width = 170f;
            commonTableColumn1.Compare =
                (CompareMethod<FontInfo>)(( obj0,  obj1) => -obj0.name.CompareTo(obj1.name));
            commonTableColumn1.DrawCell =
                (DrawCellMethod<FontInfo>)(( obj0,  obj1) => EditorGUI.LabelField(obj0, obj1.name));
            viewColumn[0] = commonTableColumn1;
            CommonTableColumn<FontInfo> commonTableColumn2 = new CommonTableColumn<FontInfo>();
            commonTableColumn2.headerContent = new GUIContent(StringUtils.HM);
            commonTableColumn2.canSort = true;
            commonTableColumn2.minWidth = 350f;
            commonTableColumn2.width = 350f;
            commonTableColumn2.Compare =
                (CompareMethod<FontInfo>)(( obj0,  obj1) => -obj0.assetPath.CompareTo(obj1.assetPath));
            commonTableColumn2.DrawCell =
                (DrawCellMethod<FontInfo>)(( obj0,  obj1) => EditorGUI.LabelField(obj0, obj1.assetPath));
            viewColumn[1] = commonTableColumn2;
            return viewColumn;
        }

        public void OnRowSelect(List<FontInfo> datas)
        {
            this.currentAssetPathList =
                datas.Select<FontInfo, string>((Func<FontInfo, string>)(( obj0) => obj0.assetPath))
                    .ToArray<string>();
            this.aO = new List<FontInfo>((IEnumerable<FontInfo>)datas);
            List<Font> fontList = new List<Font>();
        }

        private bool A( FontInfo obj0,  string obj1)
        {
            string str = obj1;
            return str.Length == 0 || obj0.name.ToLower().IndexOf(str.ToLower()) > -1;
        }

        public override void RefreshTable()
        {
            if (this.needUpdateMainContent)
            {
                this.needUpdateMainContent = false;
                this.an = new FontWindow.AssetDataTable(this.aP, this.GetViewColumn(),
                    new FilterMethod<FontInfo>(this.A), new SelectMethod<FontInfo>(this.OnRowSelect));
            }

            this.an.OnGUI();
        }

        public override void DrawOptionArea()
        {
            GUILayout.Space(40f);
            if (!GUILayout.Button(StringUtils.SearchTxt, GUILayout.Width(160f), GUILayout.Height(40f)))
                return;
            this.CollectAssets();
        }

        public void CollectAssets(bool needRefreshCurrentFolder = true)
        {
            if (needRefreshCurrentFolder)
                this.currentFolder = this.GetCurrentFolder();
            this.ao = new List<FontInfo>();
            string[] assets = AssetDatabase.FindAssets(StringUtils.searchFont, new string[1]
            {
                this.currentFolder
            });
            int length = assets.Length;
            int num = 0;
            foreach (string guid in assets)
            {
                ++num;
                EditorUtility.DisplayCancelableProgressBar(StringUtils.Hr, StringUtils.HO + num.ToString(),
                    (float)num / (float)length);
                Font font = AssetDatabase.LoadAssetAtPath<Font>(AssetDatabase.GUIDToAssetPath(guid));
                string assetPath = AssetDatabase.GetAssetPath((UnityEngine.Object)font);
                this.ao.Add(new FontInfo(font, assetPath));
            }

            EditorUtility.ClearProgressBar();
            this.aP = new List<FontInfo>((IEnumerable<FontInfo>)this.ao);
            this.needUpdateMainContent = true;
            Selection.objects = (UnityEngine.Object[])null;
        }

        public class AssetDataTable : CommonTable<FontInfo>
        {
            public AssetDataTable(
                List<FontInfo> datas,
                CommonTableColumn<FontInfo>[] cs,
                FilterMethod<FontInfo> onfilter,
                SelectMethod<FontInfo> onselect = null)
                : base(datas, cs, onfilter, onselect)
            {
            }
        }
    }
}