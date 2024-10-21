// Decompiled with JetBrains decompiler
// Type: WeChatWASM.Analysis.PrefabWindow
// Assembly: wx-editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 491FB96F-C7AE-469B-A755-45BC3F066C9B
// Assembly location: C:\Users\pengt\Documents\SVN\Luck\Client\Lucky\Packages\com.qq.weixin.minigame@981890fdaa\Editor\wx-editor.dll
// XML documentation location: C:\Users\pengt\Documents\SVN\Luck\Client\Lucky\Packages\com.qq.weixin.minigame@981890fdaa\Editor\wx-editor.xml

using System;
using System.Collections.Generic;
using System.Linq;
using Easy;
using UnityEditor;
using UnityEngine;

#nullable disable
namespace Analysis
{
    public class MaterialWindow : BaseWindow<MaterialWindow>
    {
        private MaterialWindow.AssetDataTable assetDataTable;
        private List<MaterialInfo> selectInfo;
        private List<MaterialInfo> searchInfos;
        private List<MaterialInfo> refreshInfo;

        public MaterialWindow()
        {
            if (this.assetDataTable != null)
                return;
            this.assetDataTable = new MaterialWindow.AssetDataTable(new List<MaterialInfo>(), this.GetViewColumn(),
                new FilterMethod<MaterialInfo>(this.SearchFilter), new SelectMethod<MaterialInfo>(this.OnRowSelect));
        }

        public CommonTableColumn<MaterialInfo>[] GetViewColumn()
        {
            CommonTableColumn<MaterialInfo>[] viewColumn = new CommonTableColumn<MaterialInfo>[2];
            CommonTableColumn<MaterialInfo> commonTableColumn1 = new CommonTableColumn<MaterialInfo>();
            commonTableColumn1.headerContent = new GUIContent(StringUtils.Hl);
            commonTableColumn1.canSort = true;
            commonTableColumn1.minWidth = 210f;
            commonTableColumn1.width = 210f;
            commonTableColumn1.Compare =
                (CompareMethod<MaterialInfo>)(( obj0,  obj1) => -obj0.name.CompareTo(obj1.name));
            commonTableColumn1.DrawCell =
                (DrawCellMethod<MaterialInfo>)(( obj0,  obj1) => EditorGUI.LabelField(obj0, obj1.name));
            viewColumn[0] = commonTableColumn1;
            CommonTableColumn<MaterialInfo> commonTableColumn2 = new CommonTableColumn<MaterialInfo>();
            commonTableColumn2.headerContent = new GUIContent(StringUtils.HM);
            commonTableColumn2.canSort = true;
            commonTableColumn2.minWidth = 650f;
            commonTableColumn2.width = 650f;
            commonTableColumn2.Compare =
                (CompareMethod<MaterialInfo>)(( obj0,  obj1) => -obj0.assetPath.CompareTo(obj1.assetPath));
            commonTableColumn2.DrawCell =
                (DrawCellMethod<MaterialInfo>)(( obj0,  obj1) => EditorGUI.LabelField(obj0, obj1.assetPath));
            viewColumn[1] = commonTableColumn2;
            return viewColumn;
        }

        public void OnRowSelect(List<MaterialInfo> datas)
        {
            this.currentAssetPathList =
                datas.Select<MaterialInfo, string>((Func<MaterialInfo, string>)(( obj0) => obj0.assetPath))
                    .ToArray<string>();
            this.selectInfo = new List<MaterialInfo>((IEnumerable<MaterialInfo>)datas);
            List<Material> gameObjectList = new List<Material>();
            foreach (MaterialInfo data in datas)
            {
                Material info = data._info;
                gameObjectList.Add(info);
            }

            Selection.objects = (UnityEngine.Object[])gameObjectList.ToArray();
        }

        private bool SearchFilter( MaterialInfo obj0,  string obj1)
        {
            string str = obj1;
            return str.Length == 0 || obj0.name.ToLower().IndexOf(str.ToLower()) > -1;
        }

        public override void RefreshTable()
        {
            if (this.needUpdateMainContent)
            {
                this.needUpdateMainContent = false;
                this.assetDataTable = new MaterialWindow.AssetDataTable(this.refreshInfo, this.GetViewColumn(),
                    new FilterMethod<MaterialInfo>(this.SearchFilter), new SelectMethod<MaterialInfo>(this.OnRowSelect));
            }

            this.assetDataTable.OnGUI();
        }

        private string indexStr;
        private string unIndexStr;
        private bool checkSlot;
        private string searchCount;
        public override void DrawOptionArea()
        {
            GUILayout.Space(40f);
            if (GUILayout.Button(StringUtils.SearchTxt, GUILayout.Width(160f), GUILayout.Height(40f)))
                this.CollectAssets();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(StringUtils.hasStr,GUILayout.Width(70f));
            indexStr = EditorGUILayout.TextField(indexStr,GUILayout.Width(90f));
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(StringUtils.unHasStr,GUILayout.Width(70f));
            unIndexStr = EditorGUILayout.TextField(unIndexStr,GUILayout.Width(90f));
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(StringUtils.checkSlot,GUILayout.Width(70f));
            checkSlot = EditorGUILayout.Toggle(checkSlot,GUILayout.Width(90f));
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(80);
            EditorGUILayout.LabelField(StringUtils.countLabel,GUILayout.Width(160f));
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(StringUtils.resourceLabel,GUILayout.Width(80f));
            EditorGUILayout.LabelField(searchCount,GUILayout.Width(80f));
            EditorGUILayout.EndHorizontal();
        }

        public void CollectAssets(bool needRefreshCurrentFolder = true)
        {
            if (needRefreshCurrentFolder)
                this.currentFolder = this.GetCurrentFolder();
            this.searchInfos = new List<MaterialInfo>();
            string[] assets = AssetDatabase.FindAssets(StringUtils.searchMaterial, new string[1]
            {
                this.currentFolder
            });
            int length = assets.Length;
            int num = 0;
            foreach (string guid in assets)
            {
                ++num;
                EditorUtility.DisplayCancelableProgressBar(StringUtils.Hw, StringUtils.HO + num.ToString(),
                    (float)num / (float)length);
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid));
                if (PrefabUtility.GetPrefabAssetType((UnityEngine.Object)mat) == PrefabAssetType.Regular)
                {
                    if (Check(mat))
                    {
                        string assetPath = AssetDatabase.GetAssetPath((UnityEngine.Object)mat);
                        this.searchInfos.Add(new MaterialInfo(mat, assetPath));
                    }
                }
            }

            EditorUtility.ClearProgressBar();
            this.refreshInfo = new List<MaterialInfo>((IEnumerable<MaterialInfo>)this.searchInfos);
            this.needUpdateMainContent = true;
            Selection.objects = (UnityEngine.Object[])null;


            searchCount = this.searchInfos.Count.ToString();
        }

        private bool Check(Material sp)
        {
            if (string.IsNullOrEmpty(indexStr) && string.IsNullOrEmpty(unIndexStr) ) return true;
            
            bool checkStr = false;
            if (!string.IsNullOrEmpty(indexStr))
            {
                if(sp.name.Contains(indexStr))
                    checkStr = true; 
            }
            else
            {
                checkStr = true;
            }
            
            bool unCheckStr = false;
            if (!string.IsNullOrEmpty(unIndexStr))
            {
                if(sp.name.Contains(unIndexStr) == false)
                    unCheckStr = true; 
            }
            else
            {
                unCheckStr = true;
            }

            
            
            return checkStr && unCheckStr;
        }
        

        public class AssetDataTable : CommonTable<MaterialInfo>
        {
            public AssetDataTable(
                List<MaterialInfo> datas,
                CommonTableColumn<MaterialInfo>[] cs,
                FilterMethod<MaterialInfo> onfilter,
                SelectMethod<MaterialInfo> onselect = null)
                : base(datas, cs, onfilter, onselect)
            {
            }
        }
    }
}