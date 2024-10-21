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
    public class SfxWindow : BaseWindow<SfxWindow>
    {
        private AssetDataTable assetDataTable;
        private List<SfxInfo> selectInfo;
        private List<SfxInfo> searchInfos;
        private List<SfxInfo> refreshInfo;

        public SfxWindow()
        {
            if (assetDataTable == null)
            {
                assetDataTable = new AssetDataTable(new List<SfxInfo>(), GetViewColumn(),
                    SearchFilter, OnRowSelect);
            }
        }

        public CommonTableColumn<SfxInfo>[] GetViewColumn()
        {
            CommonTableColumn<SfxInfo>[] viewColumn = new CommonTableColumn<SfxInfo>[2];
            
            CommonTableColumn<SfxInfo> commonTableColumn1 = new CommonTableColumn<SfxInfo>();
            commonTableColumn1.headerContent = new GUIContent(StringUtils.Hl);
            commonTableColumn1.canSort = true;
            commonTableColumn1.minWidth = 210f;
            commonTableColumn1.width = 210f;
            commonTableColumn1.Compare = ( obj0,  obj1) => -obj0.name.CompareTo(obj1.name);
            commonTableColumn1.DrawCell =( obj0,  obj1) => EditorGUI.LabelField(obj0, obj1.name);
            viewColumn[0] = commonTableColumn1;
            
            CommonTableColumn<SfxInfo> commonTableColumn2 = new CommonTableColumn<SfxInfo>();
            commonTableColumn2.headerContent = new GUIContent(StringUtils.HM);
            commonTableColumn2.canSort = true;
            commonTableColumn2.minWidth = 650f;
            commonTableColumn2.width = 650f;
            commonTableColumn2.Compare =( obj0,  obj1) => -obj0.assetPath.CompareTo(obj1.assetPath);
            commonTableColumn2.DrawCell =( obj0,  obj1) => EditorGUI.LabelField(obj0, obj1.assetPath);
            viewColumn[1] = commonTableColumn2;
            
            return viewColumn;
        }

        public void OnRowSelect(List<SfxInfo> datas)
        {
            currentAssetPathList = datas.Select( obj0 => obj0.assetPath).ToArray();
            selectInfo = new List<SfxInfo>(datas);
            
            List<SfxParticle> gameObjectList = new List<SfxParticle>();
            foreach (SfxInfo data in datas)
            {
                SfxParticle info = data._info;
                gameObjectList.Add(info);
            }
            Selection.objects = gameObjectList.ToArray();
        }

        private bool SearchFilter( SfxInfo info,  string search)
        {
            string str = search;
            return str.Length == 0 || info.name.ToLower().IndexOf(str.ToLower()) > -1;
        }

        public override void RefreshTable()
        {
            if (needUpdateMainContent)
            {
                needUpdateMainContent = false;
                assetDataTable = new AssetDataTable(refreshInfo, GetViewColumn(), SearchFilter, OnRowSelect);
            }

            assetDataTable.OnGUI();
        }

        private float checkLifeTime = -1;
        private bool checkReference = false;
        private string indexStr = "";
        private string unIndexStr = "";
        public string[] compareFunc = new string[3]
        {
            StringUtils.compare,
            StringUtils.compare_0,
            StringUtils.compare_1
        };
        private int _selectedIndex = 0;
        private bool checkPrefab;
        private bool checkOwner;
        private bool checkCamera;
        private bool checkAudio;
        public override void DrawOptionArea()
        {
            GUILayout.Space(40f);
            var clicked = GUILayout.Button(StringUtils.SearchTxt, GUILayout.Width(160f), GUILayout.Height(40f));
           
            EditorGUILayout.BeginVertical();
            
            EditorGUILayout.LabelField(StringUtils.searchRole,GUILayout.Width(160f));

            checkLifeTime = EditorGUILayout.Slider(checkLifeTime,0,10,GUILayout.Width(160f));

            EditorGUILayout.BeginHorizontal();
            _selectedIndex = EditorGUILayout.Popup(_selectedIndex, this.compareFunc,GUILayout.Width(80f));
            EditorGUILayout.LabelField(StringUtils.checkLife,GUILayout.Width(80f));
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            checkReference = EditorGUILayout.Toggle(checkReference,GUILayout.Width(20f));
            EditorGUILayout.LabelField(StringUtils.checkReference,GUILayout.Width(140f));
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(StringUtils.hasStr,GUILayout.Width(70f));
            indexStr = EditorGUILayout.TextField(indexStr,GUILayout.Width(90f));
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(StringUtils.unHasStr,GUILayout.Width(70f));
            unIndexStr = EditorGUILayout.TextField(unIndexStr,GUILayout.Width(90f));
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(StringUtils.checkPrefab,GUILayout.Width(70f));
            checkPrefab = EditorGUILayout.Toggle(checkPrefab,GUILayout.Width(90f));
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(StringUtils.checkOwner,GUILayout.Width(70f));
            checkOwner = EditorGUILayout.Toggle(checkOwner,GUILayout.Width(90f));
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(StringUtils.checkCamera,GUILayout.Width(70f));
            checkCamera = EditorGUILayout.Toggle(checkCamera,GUILayout.Width(90f));
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(StringUtils.checkAudio,GUILayout.Width(70f));
            checkAudio = EditorGUILayout.Toggle(checkAudio,GUILayout.Width(90f));
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(80);
            EditorGUILayout.LabelField(StringUtils.countLabel,GUILayout.Width(160f));
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(StringUtils.resourceLabel,GUILayout.Width(80f));
            EditorGUILayout.LabelField(searchCount,GUILayout.Width(80f));
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
            if (clicked)
            {
                CollectAssets();
            }
        }
        
        private string searchCount = "0";
        public void CollectAssets(bool needRefreshCurrentFolder = true)
        {
            if (needRefreshCurrentFolder) currentFolder = GetCurrentFolder();
            
            searchInfos = new List<SfxInfo>();
            string[] assets = AssetDatabase.FindAssets(StringUtils.searchSfx, new string[1]
            {
                currentFolder
            });
            
            int length = assets.Length;
            int num = 0;
            foreach (string guid in assets)
            {
                ++num;
                var result = EditorUtility.DisplayCancelableProgressBar(StringUtils.searchTitle, $"{StringUtils.HO}  {num}/{length}", num / (float)length);
                if (result)
                {
                    EditorUtility.ClearProgressBar();
                    return;
                }
                
                SfxParticle sp = AssetDatabase.LoadAssetAtPath<SfxParticle>(AssetDatabase.GUIDToAssetPath(guid));
                if(!sp) continue;
                if (Check(sp))
                {
                    string assetPath = AssetDatabase.GetAssetPath(sp);
                    searchInfos.Add(new SfxInfo(sp, assetPath));  
                }
            }

            EditorUtility.ClearProgressBar();
            refreshInfo = new List<SfxInfo>(searchInfos);
            needUpdateMainContent = true;
            Selection.objects = null;

            searchCount = searchInfos.Count.ToString();
        }

        private bool Check(SfxParticle sp)
        {
            if (_selectedIndex==0 && !checkReference && string.IsNullOrEmpty(indexStr)&& string.IsNullOrEmpty(unIndexStr)  && !checkPrefab && !checkOwner && !checkCamera && !checkAudio ) return true;
            
            bool chekLife = false;
            if (_selectedIndex != 0)
            {
                if (_selectedIndex == 1)
                {
                    if(sp.lifeTime == checkLifeTime)
                        chekLife = true; 
                }
                if (_selectedIndex == 2)
                {
                    if(sp.lifeTime >= checkLifeTime)
                        chekLife = true; 
                }
            }
            else
            {
                chekLife = true;
            }

            bool chekRef = false;
            if (checkReference)
            {
                foreach (var prefab in sp.sfxPrefab)
                {
                    if (prefab.prefab == null)
                    {
                        chekRef = true;
                    }
                }

                foreach (var audio in sp.sfxSound)
                {
                    if (audio.randomClips.Count != 0)
                    {
                        foreach (var clip in audio.randomClips)
                        {
                            if (clip == null)
                            {
                                chekRef = true;
                            }
                        }
                    }
                }
            }
            else
            {
                chekRef = true;
            }

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

            bool hasPrefab = checkPrefab ? sp.sfxPrefab.Count != 0 : false;
            bool hasOwner = checkOwner? hasOwner = sp.sfxOwner.Count != 0 : false;
            bool hasCamera = checkCamera? hasCamera = sp.sfxShark.Count != 0:false;
            bool hasAudio = checkAudio ? hasAudio = sp.sfxSound.Count != 0:false;

            return chekLife && chekRef && checkStr && unCheckStr && hasPrefab && hasOwner && hasCamera && hasAudio; 
        }

        public class AssetDataTable : CommonTable<SfxInfo>
        {
            public AssetDataTable(
                List<SfxInfo> datas,
                CommonTableColumn<SfxInfo>[] cs,
                FilterMethod<SfxInfo> onfilter,
                SelectMethod<SfxInfo> onselect = null)
                : base(datas, cs, onfilter, onselect)
            {
            }
        }
    }
}