// Decompiled with JetBrains decompiler
// Type: WeChatWASM.Analysis.AudioWindow
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
    public class AudioWindow : BaseWindow<AudioWindow>
    {
        private AudioWindow.AssetDataTable ad;
        private List<AudioInfo> aE;
        private List<AudioInfo> ae;
        private List<AudioInfo> aF;

        public AudioWindow()
        {
            if (this.ad != null)
                return;
            this.ad = new AudioWindow.AssetDataTable(new List<AudioInfo>(), this.GetViewColumn(),
                new FilterMethod<AudioInfo>(this.A), new SelectMethod<AudioInfo>(this.OnRowSelect));
        }

        public CommonTableColumn<AudioInfo>[] GetViewColumn()
        {
            CommonTableColumn<AudioInfo>[] viewColumn = new CommonTableColumn<AudioInfo>[2];
            CommonTableColumn<AudioInfo> commonTableColumn1 = new CommonTableColumn<AudioInfo>();
            commonTableColumn1.headerContent = new GUIContent(StringUtils.Hl);
            commonTableColumn1.canSort = true;
            commonTableColumn1.minWidth = 170f;
            commonTableColumn1.width = 170f;
            commonTableColumn1.Compare =
                (CompareMethod<AudioInfo>)(( obj0,  obj1) => -obj0.name.CompareTo(obj1.name));
            commonTableColumn1.DrawCell =
                (DrawCellMethod<AudioInfo>)((obj0, obj1) => EditorGUI.LabelField(obj0, obj1.name));
            viewColumn[0] = commonTableColumn1;
            CommonTableColumn<AudioInfo> commonTableColumn2 = new CommonTableColumn<AudioInfo>();
            commonTableColumn2.headerContent = new GUIContent(StringUtils.HM);
            commonTableColumn2.canSort = true;
            commonTableColumn2.minWidth = 350f;
            commonTableColumn2.width = 350f;
            commonTableColumn2.Compare =
                (CompareMethod<AudioInfo>)(( obj0, obj1) => -obj0.assetPath.CompareTo(obj1.assetPath));
            commonTableColumn2.DrawCell =
                (DrawCellMethod<AudioInfo>)(( obj0,  obj1) => EditorGUI.LabelField(obj0, obj1.assetPath));
            viewColumn[1] = commonTableColumn2;
            return viewColumn;
        }

        public void OnRowSelect(List<AudioInfo> datas)
        {
            this.currentAssetPathList =
                datas.Select<AudioInfo, string>((Func<AudioInfo, string>)((obj0) => obj0.assetPath))
                    .ToArray<string>();
            this.aE = new List<AudioInfo>((IEnumerable<AudioInfo>)datas);
            List<AudioClip> audioClipList = new List<AudioClip>();
            foreach (AudioInfo data in datas)
            {
                AudioClip info = data._info;
                audioClipList.Add(info);
            }

            Selection.objects = (UnityEngine.Object[])audioClipList.ToArray();
        }

        private bool A([In] AudioInfo obj0, [In] string obj1)
        {
            string str = obj1;
            return str.Length == 0 || obj0.name.ToLower().IndexOf(str.ToLower()) > -1;
        }

        public override void RefreshTable()
        {
            if (this.needUpdateMainContent)
            {
                this.needUpdateMainContent = false;
                this.ad = new AudioWindow.AssetDataTable(this.aF, this.GetViewColumn(),
                    new FilterMethod<AudioInfo>(this.A), new SelectMethod<AudioInfo>(this.OnRowSelect));
            }

            this.ad.OnGUI();
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
            this.ae = new List<AudioInfo>();
            string[] assets = AssetDatabase.FindAssets(StringUtils.searchAudio, new string[1]
            {
                this.currentFolder
            });
            int length = assets.Length;
            int num = 0;
            foreach (string guid in assets)
            {
                ++num;
                EditorUtility.DisplayCancelableProgressBar(StringUtils.searchTitle, StringUtils.HO + num.ToString(),
                    (float)num / (float)length);
                AudioClip audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(AssetDatabase.GUIDToAssetPath(guid));
                string assetPath = AssetDatabase.GetAssetPath((UnityEngine.Object)audioClip);
                this.ae.Add(new AudioInfo(audioClip, assetPath));
            }

            EditorUtility.ClearProgressBar();
            this.aF = new List<AudioInfo>((IEnumerable<AudioInfo>)this.ae);
            this.needUpdateMainContent = true;
            Selection.objects = (UnityEngine.Object[])null;
        }

        public class AssetDataTable : CommonTable<AudioInfo>
        {
            public AssetDataTable(
                List<AudioInfo> datas,
                CommonTableColumn<AudioInfo>[] cs,
                FilterMethod<AudioInfo> onfilter,
                SelectMethod<AudioInfo> onselect = null)
                : base(datas, cs, onfilter, onselect)
            {
            }
        }
    }
}