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
    public class PrefabWindow : BaseWindow<PrefabWindow>
    {
        private PrefabWindow.AssetDataTable assetDataTable;
        private List<PrefabInfo> selectInfo;
        private List<PrefabInfo> searchInfos;
        private List<PrefabInfo> refreshInfo;

        public PrefabWindow()
        {
            if (this.assetDataTable != null)
                return;
            this.assetDataTable = new PrefabWindow.AssetDataTable(new List<PrefabInfo>(), this.GetViewColumn(),
                new FilterMethod<PrefabInfo>(this.SearchFilter), new SelectMethod<PrefabInfo>(this.OnRowSelect));
        }

        public CommonTableColumn<PrefabInfo>[] GetViewColumn()
        {
            CommonTableColumn<PrefabInfo>[] viewColumn = new CommonTableColumn<PrefabInfo>[2];
            CommonTableColumn<PrefabInfo> commonTableColumn1 = new CommonTableColumn<PrefabInfo>();
            commonTableColumn1.headerContent = new GUIContent(StringUtils.Hl);
            commonTableColumn1.canSort = true;
            commonTableColumn1.minWidth = 210f;
            commonTableColumn1.width = 210f;
            commonTableColumn1.Compare =
                (CompareMethod<PrefabInfo>)(( obj0,  obj1) => -obj0.name.CompareTo(obj1.name));
            commonTableColumn1.DrawCell =
                (DrawCellMethod<PrefabInfo>)(( obj0,  obj1) => EditorGUI.LabelField(obj0, obj1.name));
            viewColumn[0] = commonTableColumn1;
            CommonTableColumn<PrefabInfo> commonTableColumn2 = new CommonTableColumn<PrefabInfo>();
            commonTableColumn2.headerContent = new GUIContent(StringUtils.HM);
            commonTableColumn2.canSort = true;
            commonTableColumn2.minWidth = 650f;
            commonTableColumn2.width = 650f;
            commonTableColumn2.Compare =
                (CompareMethod<PrefabInfo>)(( obj0,  obj1) => -obj0.assetPath.CompareTo(obj1.assetPath));
            commonTableColumn2.DrawCell =
                (DrawCellMethod<PrefabInfo>)(( obj0,  obj1) => EditorGUI.LabelField(obj0, obj1.assetPath));
            viewColumn[1] = commonTableColumn2;
            return viewColumn;
        }

        public void OnRowSelect(List<PrefabInfo> datas)
        {
            this.currentAssetPathList =
                datas.Select<PrefabInfo, string>((Func<PrefabInfo, string>)(( obj0) => obj0.assetPath))
                    .ToArray<string>();
            this.selectInfo = new List<PrefabInfo>((IEnumerable<PrefabInfo>)datas);
            List<GameObject> gameObjectList = new List<GameObject>();
            foreach (PrefabInfo data in datas)
            {
                GameObject info = data._info;
                gameObjectList.Add(info);
            }

            Selection.objects = (UnityEngine.Object[])gameObjectList.ToArray();
        }

        private bool SearchFilter( PrefabInfo obj0,  string obj1)
        {
            string str = obj1;
            return str.Length == 0 || obj0.name.ToLower().IndexOf(str.ToLower()) > -1;
        }

        public override void RefreshTable()
        {
            if (this.needUpdateMainContent)
            {
                this.needUpdateMainContent = false;
                this.assetDataTable = new PrefabWindow.AssetDataTable(this.refreshInfo, this.GetViewColumn(),
                    new FilterMethod<PrefabInfo>(this.SearchFilter), new SelectMethod<PrefabInfo>(this.OnRowSelect));
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
            this.searchInfos = new List<PrefabInfo>();
            string[] assets = AssetDatabase.FindAssets(StringUtils.HW, new string[1]
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
                GameObject gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
                if (PrefabUtility.GetPrefabAssetType((UnityEngine.Object)gameObject) == PrefabAssetType.Regular)
                {
                    if (Check(gameObject))
                    {
                        string assetPath = AssetDatabase.GetAssetPath((UnityEngine.Object)gameObject);
                        this.searchInfos.Add(new PrefabInfo(gameObject, assetPath));
                    }
                }
            }

            EditorUtility.ClearProgressBar();
            this.refreshInfo = new List<PrefabInfo>((IEnumerable<PrefabInfo>)this.searchInfos);
            this.needUpdateMainContent = true;
            Selection.objects = (UnityEngine.Object[])null;


            searchCount = this.searchInfos.Count.ToString();
        }

        private bool Check(GameObject sp)
        {
            if (string.IsNullOrEmpty(indexStr) && string.IsNullOrEmpty(unIndexStr) && !checkSlot ) return true;
            
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

            bool slot = false;
            if (checkSlot)
            {
                var ct = sp.GetComponent<Control>();
                if (ct && ct.hasError())
                {
                    slot = true;
                }
            }
            else
            {
                slot = true;
            }
            
            return checkStr && unCheckStr && slot;
        }
        

        public class AssetDataTable : CommonTable<PrefabInfo>
        {
            public AssetDataTable(
                List<PrefabInfo> datas,
                CommonTableColumn<PrefabInfo>[] cs,
                FilterMethod<PrefabInfo> onfilter,
                SelectMethod<PrefabInfo> onselect = null)
                : base(datas, cs, onfilter, onselect)
            {
            }
        }
    }
}