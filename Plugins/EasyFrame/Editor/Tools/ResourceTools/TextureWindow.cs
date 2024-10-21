// Decompiled with JetBrains decompiler
// Type: WeChatWASM.Analysis.TextureWindow
// Assembly: wx-editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 491FB96F-C7AE-469B-A755-45BC3F066C9B
// Assembly location: C:\Users\pengt\Documents\SVN\Luck\Client\Lucky\Packages\com.qq.weixin.minigame@981890fdaa\Editor\wx-editor.dll
// XML documentation location: C:\Users\pengt\Documents\SVN\Luck\Client\Lucky\Packages\com.qq.weixin.minigame@981890fdaa\Editor\wx-editor.xml

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

#nullable disable
namespace Analysis
{
    public class TextureWindow : BaseWindow<TextureWindow>
    {
        private TextureWindow.AssetDataTable BN;
        public List<TextureInfo> textureInfos = new List<TextureInfo>();
        public List<TextureInfo> renderTextureInfos = new List<TextureInfo>();
        public List<TextureInfo> selectedTextureInfos = new List<TextureInfo>();
        public bool checkMipMap = false;
        public bool formatError = false;
        public bool checkIsReadable = false;
        public bool checkMaxSize = false;
        public bool disableReadable = true;
        public bool disableMipmap = true;
        public bool changeMaxSize = true;
        public bool changeFormat = true;
        public int selectedMaxSizeIdx = 0;

        public string[] maxSizeOptions = new string[8]
        {
            StringUtils.hI,
            StringUtils.hi,
            StringUtils.hJ,
            StringUtils.hj,
            StringUtils.hK,
            StringUtils.hk,
            StringUtils.hL,
            StringUtils.hl
        };

        public int selectedFormat = 0;

        public Dictionary<string, TextureImporterFormat> formatMap = new Dictionary<string, TextureImporterFormat>()
        {
            {
                StringUtils.hM,
                TextureImporterFormat.Automatic
            },
            {
                StringUtils.hm,
                TextureImporterFormat.Alpha8
            },
            {
                StringUtils.hN,
                TextureImporterFormat.RGB24
            },
            {
                StringUtils.hn,
                TextureImporterFormat.RGBA32
            },
            {
                StringUtils.hO,
                TextureImporterFormat.RGB16
            },
            {
                StringUtils.ho,
                TextureImporterFormat.R16
            },
            {
                StringUtils.hP,
                TextureImporterFormat.DXT1
            },
            {
                StringUtils.hp,
                TextureImporterFormat.DXT5
            },
            {
                StringUtils.hQ,
                TextureImporterFormat.DXT1Crunched
            },
            {
                StringUtils.hq,
                TextureImporterFormat.DXT5Crunched
            },
            {
                StringUtils.hR,
                TextureImporterFormat.R8
            }
        };

        public string[] extOptions = new List<string>().ToArray();
        public int extSelected = 0;
        public string[] textureFormatOptions = new List<string>().ToArray();
        public int textureFormatSelected = 0;
        public string[] webglFormatOptions = new List<string>().ToArray();
        public int webglFormatSelected = 0;

        public string[] isReadableOptions = new string[3]
        {
            StringUtils.hr,
            StringUtils.DJ,
            StringUtils.eE
        };

        public int isReadableSelected = 0;

        public string[] mipmapEnableOptions = new string[3]
        {
            StringUtils.hr,
            StringUtils.DJ,
            StringUtils.eE
        };

        public int mipmapEnableSelected = 0;

        public TextureWindow()
        {
            //if (WXExtEnvDef.GETDEF(StringUtils.Fq))
            {
                this.formatMap.Add(StringUtils.hS, TextureImporterFormat.ASTC_8x8);
                this.formatMap.Add(StringUtils.hs, TextureImporterFormat.ASTC_5x5);
                this.formatMap.Add(StringUtils.hT, TextureImporterFormat.ASTC_6x6);
                this.formatMap.Add(StringUtils.ht, TextureImporterFormat.ASTC_4x4);
            }

            if (this.BN != null)
                return;
            this.BN = new TextureWindow.AssetDataTable(new List<TextureInfo>(), this.GetOverViewColumn(),
                new FilterMethod<TextureInfo>(this.A), new SelectMethod<TextureInfo>(this.A), new Action<bool>(this.A));
        }

        public CommonTableColumn<TextureInfo>[] GetOverViewColumn()
        {
            CommonTableColumn<TextureInfo>[] overViewColumn = new CommonTableColumn<TextureInfo>[12];
            CommonTableColumn<TextureInfo> commonTableColumn1 = new CommonTableColumn<TextureInfo>();
            commonTableColumn1.headerContent = new GUIContent(StringUtils.Hl);
            commonTableColumn1.canSort = true;
            commonTableColumn1.minWidth = 170f;
            commonTableColumn1.width = 170f;
            commonTableColumn1.Compare =
                (CompareMethod<TextureInfo>)(( obj0,  obj1) => -obj0.name.CompareTo(obj1.name));
            commonTableColumn1.DrawCell =
                (DrawCellMethod<TextureInfo>)(( obj0,  obj1) => EditorGUI.LabelField(obj0, obj1.name));
            overViewColumn[0] = commonTableColumn1;
            CommonTableColumn<TextureInfo> commonTableColumn2 = new CommonTableColumn<TextureInfo>();
            commonTableColumn2.headerContent = new GUIContent(StringUtils.hU);
            commonTableColumn2.canSort = true;
            commonTableColumn2.width = 150f;
            commonTableColumn2.minWidth = 90f;
            commonTableColumn2.Compare =
                (CompareMethod<TextureInfo>)(( obj0,  obj1) => -obj0.webglFormat.CompareTo(obj1.webglFormat));
            commonTableColumn2.DrawCell =
                (DrawCellMethod<TextureInfo>)(( obj0,  obj1) => EditorGUI.LabelField(obj0, obj1.webglFormat));
            overViewColumn[1] = commonTableColumn2;
            CommonTableColumn<TextureInfo> commonTableColumn3 = new CommonTableColumn<TextureInfo>();
            commonTableColumn3.headerContent = new GUIContent(StringUtils.hu);
            commonTableColumn3.canSort = true;
            commonTableColumn3.width = 120f;
            commonTableColumn3.minWidth = 120f;
            commonTableColumn3.Compare =
                (CompareMethod<TextureInfo>)(( obj0,  obj1) => -obj0._memorySize.CompareTo(obj1._memorySize));
            commonTableColumn3.DrawCell = (DrawCellMethod<TextureInfo>)(( obj0,  obj1) =>
            EditorGUI.LabelField(obj0, obj1.memorySize.ToString()));
            overViewColumn[2] = commonTableColumn3;
            CommonTableColumn<TextureInfo> commonTableColumn4 = new CommonTableColumn<TextureInfo>();
            commonTableColumn4.headerContent = new GUIContent(StringUtils.hV);
            commonTableColumn4.canSort = true;
            commonTableColumn4.width = 90f;
            commonTableColumn4.minWidth = 90f;
            commonTableColumn4.Compare = (CompareMethod<TextureInfo>)(( obj0,  obj1) =>
            -obj0.maxTextureSize.CompareTo(obj1.maxTextureSize));
            commonTableColumn4.DrawCell = (DrawCellMethod<TextureInfo>)(( obj0,  obj1) =>
            EditorGUI.LabelField(obj0, obj1.maxTextureSize.ToString()));
            overViewColumn[3] = commonTableColumn4;
            CommonTableColumn<TextureInfo> commonTableColumn5 = new CommonTableColumn<TextureInfo>();
            commonTableColumn5.headerContent = new GUIContent(StringUtils.hv);
            commonTableColumn5.canSort = true;
            commonTableColumn5.width = 90f;
            commonTableColumn5.minWidth = 90f;
            commonTableColumn5.Compare =
                (CompareMethod<TextureInfo>)(( obj0,  obj1) => -obj0.width.CompareTo(obj1.width));
            commonTableColumn5.DrawCell = (DrawCellMethod<TextureInfo>)(( obj0,  obj1) =>
            EditorGUI.LabelField(obj0, obj1.width.ToString()));
            overViewColumn[4] = commonTableColumn5;
            CommonTableColumn<TextureInfo> commonTableColumn6 = new CommonTableColumn<TextureInfo>();
            commonTableColumn6.headerContent = new GUIContent(StringUtils.hW);
            commonTableColumn6.canSort = true;
            commonTableColumn6.width = 90f;
            commonTableColumn6.minWidth = 90f;
            commonTableColumn6.Compare =
                (CompareMethod<TextureInfo>)(( obj0,  obj1) => -obj0.height.CompareTo(obj1.height));
            commonTableColumn6.DrawCell = (DrawCellMethod<TextureInfo>)(( obj0,  obj1) =>
            EditorGUI.LabelField(obj0, obj1.height.ToString()));
            overViewColumn[5] = commonTableColumn6;
            CommonTableColumn<TextureInfo> commonTableColumn7 = new CommonTableColumn<TextureInfo>();
            commonTableColumn7.headerContent = new GUIContent(StringUtils.hw);
            commonTableColumn7.canSort = true;
            commonTableColumn7.width = 90f;
            commonTableColumn7.minWidth = 90f;
            commonTableColumn7.Compare =
                (CompareMethod<TextureInfo>)(( obj0,  obj1) => -obj0.ext.CompareTo(obj1.ext));
            commonTableColumn7.DrawCell =
                (DrawCellMethod<TextureInfo>)(( obj0,  obj1) => EditorGUI.LabelField(obj0, obj1.ext));
            overViewColumn[6] = commonTableColumn7;
            CommonTableColumn<TextureInfo> commonTableColumn8 = new CommonTableColumn<TextureInfo>();
            commonTableColumn8.headerContent = new GUIContent(StringUtils.hX);
            commonTableColumn8.canSort = true;
            commonTableColumn8.width = 90f;
            commonTableColumn8.minWidth = 90f;
            commonTableColumn8.Compare =
                (CompareMethod<TextureInfo>)(( obj0,  obj1) => -obj0.dimension.CompareTo(obj1.dimension));
            commonTableColumn8.DrawCell =
                (DrawCellMethod<TextureInfo>)(( obj0,  obj1) => EditorGUI.LabelField(obj0, obj1.dimension));
            overViewColumn[7] = commonTableColumn8;
            CommonTableColumn<TextureInfo> commonTableColumn9 = new CommonTableColumn<TextureInfo>();
            commonTableColumn9.headerContent = new GUIContent(StringUtils.hx);
            commonTableColumn9.canSort = true;
            commonTableColumn9.width = 90f;
            commonTableColumn9.minWidth = 90f;
            commonTableColumn9.Compare =
                (CompareMethod<TextureInfo>)(( obj0,  obj1) => -obj0.textureType.CompareTo(obj1.textureType));
            commonTableColumn9.DrawCell =
                (DrawCellMethod<TextureInfo>)(( obj0,  obj1) => EditorGUI.LabelField(obj0, obj1.textureType));
            overViewColumn[8] = commonTableColumn9;
            CommonTableColumn<TextureInfo> commonTableColumn10 = new CommonTableColumn<TextureInfo>();
            commonTableColumn10.headerContent = new GUIContent(StringUtils.hZ);
            commonTableColumn10.canSort = true;
            commonTableColumn10.width = 120f;
            commonTableColumn10.minWidth = 120f;
            commonTableColumn10.Compare = (CompareMethod<TextureInfo>)(( obj0,  obj1) =>
            -obj0.textureFormat.CompareTo(obj1.textureFormat));
            commonTableColumn10.DrawCell = (DrawCellMethod<TextureInfo>)(( obj0,  obj1) =>
            EditorGUI.LabelField(obj0, obj1.textureFormat));
            overViewColumn[9] = commonTableColumn10;
            CommonTableColumn<TextureInfo> commonTableColumn11 = new CommonTableColumn<TextureInfo>();
            commonTableColumn11.headerContent = new GUIContent(StringUtils.hy);
            commonTableColumn11.canSort = true;
            commonTableColumn11.width = 90f;
            commonTableColumn11.minWidth = 90f;
            commonTableColumn11.Compare =
                (CompareMethod<TextureInfo>)(( obj0,  obj1) => -obj0.isReadable.CompareTo(obj1.isReadable));
            commonTableColumn11.DrawCell = (DrawCellMethod<TextureInfo>)(( obj0,  obj1) =>
            EditorGUI.LabelField(obj0, obj1.isReadable.ToString()));
            overViewColumn[10] = commonTableColumn11;
            CommonTableColumn<TextureInfo> commonTableColumn12 = new CommonTableColumn<TextureInfo>();
            commonTableColumn12.headerContent = new GUIContent(StringUtils.hY);
            commonTableColumn12.canSort = true;
            commonTableColumn12.width = 90f;
            commonTableColumn12.minWidth = 90f;
            commonTableColumn12.Compare = (CompareMethod<TextureInfo>)(( obj0,  obj1) =>
            -obj0.mipmapEnabled.CompareTo(obj1.mipmapEnabled));
            commonTableColumn12.DrawCell = (DrawCellMethod<TextureInfo>)(( obj0,  obj1) =>
            EditorGUI.LabelField(obj0, obj1.mipmapEnabled.ToString()));
            overViewColumn[11] = commonTableColumn12;
            return overViewColumn;
        }

        private void A( List<TextureInfo> obj0_1)
        {
            this.currentAssetPathList = obj0_1
                .Select<TextureInfo, string>((Func<TextureInfo, string>)(( obj0_2) => obj0_2.assetPath))
                .ToArray<string>();
            this.selectedTextureInfos = new List<TextureInfo>((IEnumerable<TextureInfo>)obj0_1);
            List<Texture> textureList = new List<Texture>();
            foreach (TextureInfo textureInfo in obj0_1)
            {
                Texture texture = textureInfo.texture;
                textureList.Add(texture);
            }

            Selection.objects = (UnityEngine.Object[])textureList.ToArray();
        }

        private void A( bool obj0)
        {
            if (obj0)
                return;
            this.selectedTextureInfos = new List<TextureInfo>();
            Selection.objects = (UnityEngine.Object[])null;
        }

        private bool A( TextureInfo obj0,  string obj1)
        {
            string str = obj1;
            return str.Length == 0 || obj0.name.ToLower().IndexOf(str.ToLower()) > -1;
        }

        public override void DrawOptionArea()
        {
            GUILayout.Space(40f);
            if (GUILayout.Button(StringUtils.SearchTxt, GUILayout.Width(160f), GUILayout.Height(40f)))
                this.CollectAssets();
           
            GUILayout.Label(StringUtils.searchRole, GUILayout.Width(160));
            this.checkMipMap = EditorGUILayout.ToggleLeft(StringUtils.checkMipMap, this.checkMipMap, GUILayout.Width(160));
            this.formatError = EditorGUILayout.ToggleLeft(StringUtils.IB, this.formatError, GUILayout.Width(160));
            this.checkIsReadable = EditorGUILayout.ToggleLeft(StringUtils.hy, this.checkIsReadable, GUILayout.Width(160));
            this.checkMaxSize = EditorGUILayout.ToggleLeft(StringUtils.checkMaxSize, this.checkMaxSize, GUILayout.Width(160));
            GUILayout.Space(10f);
            if (GUILayout.Button(StringUtils.IC, GUILayout.Width(160f), GUILayout.Height(40f)) &&
                this.selectedTextureInfos.Count > 0)
                OptimizeTexture.Optimize(this.selectedTextureInfos);
            if (GUILayout.Button(StringUtils.Ic, GUILayout.Width(160f), GUILayout.Height(40f)))
                OptimizeTexture.Recover(this.selectedTextureInfos);
            GUILayout.Label(StringUtils.ID, GUILayout.Width(160));
            this.disableReadable = EditorGUILayout.ToggleLeft(StringUtils.Id, this.disableReadable, GUILayout.Width(160));
            this.disableMipmap = EditorGUILayout.ToggleLeft(StringUtils.IE, this.disableMipmap, GUILayout.Width(160));
            this.changeMaxSize = EditorGUILayout.ToggleLeft(StringUtils.Ie, this.changeMaxSize, GUILayout.Width(160));
            this.changeFormat = EditorGUILayout.ToggleLeft(StringUtils.IF, this.changeFormat, GUILayout.Width(160));
            if (this.changeMaxSize)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(StringUtils.If, GUILayout.Width(60));
                this.selectedMaxSizeIdx =
                    EditorGUILayout.Popup(this.selectedMaxSizeIdx, this.maxSizeOptions, GUILayout.Width(100));
                EditorGUILayout.EndHorizontal();
            }
           
            if (this.changeFormat)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(StringUtils.IG, GUILayout.Width(60));
                this.selectedFormat = EditorGUILayout.Popup(this.selectedFormat,
                    new List<string>((IEnumerable<string>)this.formatMap.Keys).ToArray(), GUILayout.Width(100));
                EditorGUILayout.EndHorizontal();
            }
            
            GUILayout.Space(10f);
            GUILayout.Label(StringUtils.Ig, GUILayout.Width(160));
            string ext = string.Empty;
            string webglFormat = string.Empty;
            string textureFormat = string.Empty;
            string isReadable = string.Empty;
            string mipmapEnable = string.Empty;
            if (this.extOptions.Length != 0)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(StringUtils.IH, GUILayout.Width(60));
                int num = EditorGUILayout.Popup(this.extSelected, this.extOptions, GUILayout.Width(100));
                if (num != this.extSelected)
                    this.needUpdateMainContent = true;
                this.extSelected = num;
                ext = this.extOptions[this.extSelected];
                EditorGUILayout.EndHorizontal();
            }

            if (this.webglFormatOptions.Length != 0)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(StringUtils.Ih, GUILayout.Width(60));
                int num = EditorGUILayout.Popup( this.webglFormatSelected, this.webglFormatOptions, GUILayout.Width(100));
                if (num != this.webglFormatSelected)
                    this.needUpdateMainContent = true;
                this.webglFormatSelected = num;
                webglFormat = this.webglFormatOptions[this.webglFormatSelected];
                EditorGUILayout.EndHorizontal();
            }
           
            if (this.textureFormatOptions.Length != 0)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(StringUtils.hY, GUILayout.Width(60));
                int num = EditorGUILayout.Popup( this.textureFormatSelected,
                    this.textureFormatOptions, GUILayout.Width(100));
                if (num != this.textureFormatSelected)
                    this.needUpdateMainContent = true;
                this.textureFormatSelected = num;
                textureFormat = this.textureFormatOptions[this.textureFormatSelected];
                EditorGUILayout.EndHorizontal();
            }

            if (this.isReadableOptions.Length != 0)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(StringUtils.hy, GUILayout.Width(80));
                int num = EditorGUILayout.Popup( this.isReadableSelected, this.isReadableOptions, GUILayout.Width(80));
                if (num != this.isReadableSelected)
                    this.needUpdateMainContent = true;
                this.isReadableSelected = num;
                isReadable = this.isReadableOptions[this.isReadableSelected];
                EditorGUILayout.EndHorizontal();
            }

            if (this.mipmapEnableOptions.Length != 0)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(StringUtils.hY, GUILayout.Width(80));
                int num = EditorGUILayout.Popup( this.mipmapEnableSelected, this.mipmapEnableOptions, GUILayout.Width(80));
                if (num != this.mipmapEnableSelected)
                    this.needUpdateMainContent = true;
                this.mipmapEnableSelected = num;
                mipmapEnable = this.mipmapEnableOptions[this.mipmapEnableSelected];
                EditorGUILayout.EndHorizontal();
            }
           
            this.FilterAsset(ext, webglFormat, textureFormat, isReadable, mipmapEnable);
            GUILayout.Space(10f);
            
            GUILayout.Label(StringUtils.countLabel, GUILayout.Width(160));

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(StringUtils.resourceLabel, GUILayout.Width(60));
            EditorGUILayout.LabelField(this.renderTextureInfos.Count.ToString(), GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(StringUtils.resourceLabelMemory,GUILayout.Width(60));
            EditorGUILayout.LabelField(EditorUtility.FormatBytes(this.renderTextureInfos.Aggregate<TextureInfo, long>(0L,
                (Func<long, TextureInfo, long>)(( obj0,  obj1) => obj0 += obj1.originalMemorySize))), GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();
            
        }

        public void CollectAssets(bool needRefreshCurrentFolder = true)
        {
            if (needRefreshCurrentFolder)
                this.currentFolder = this.GetCurrentFolder();
            string[] assets = AssetDatabase.FindAssets(StringUtils.searchTexture, new string[1]
            {
                this.currentFolder
            });
            this.textureInfos = new List<TextureInfo>();
            List<string> source1 = new List<string>();
            List<string> source2 = new List<string>();
            int length = assets.Length;
            int num = 0;
            foreach (string guid in assets)
            {
                ++num;
                Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(AssetDatabase.GUIDToAssetPath(guid));
                TextureImporter textureImporter;
                if (OptimizeTexture.CheckNeedOptimization(texture, out textureImporter) &&
                    (bool)(UnityEngine.Object)textureImporter)
                {
                    EditorUtility.DisplayCancelableProgressBar(StringUtils.Ik, $"{StringUtils.HO}  {num}/{length}",
                        (float)num / (float)length);
                    source1.Add(Path.GetExtension(AssetDatabase.GetAssetPath((UnityEngine.Object)texture)).ToString());
                    source2.Add(textureImporter.GetAutomaticFormat(StringUtils.Hs).ToString());
                    this.textureInfos.Add(new TextureInfo(textureImporter, texture));
                }
            }

            EditorUtility.ClearProgressBar();
            this.renderTextureInfos = new List<TextureInfo>((IEnumerable<TextureInfo>)this.textureInfos);
            this.extOptions = source1.Distinct<string>().Prepend<string>(StringUtils.hr).ToArray<string>();
            this.webglFormatOptions = source2.Distinct<string>().Prepend<string>(StringUtils.hr).ToArray<string>();
            this.needUpdateMainContent = true;
            this.selectedTextureInfos = new List<TextureInfo>();
            Selection.objects = (UnityEngine.Object[])null;
        }

        public bool FilterAsset(
            string ext,
            string webglFormat,
            string textureFormat,
            string isReadable,
            string mipmapEnable)
        {
            // ISSUE: object of a compiler-generated type is created
            // ISSUE: variable of a compiler-generated type
            TextureWindow.B b = new TextureWindow.B()
            {
                Df = ext,
                Dg = webglFormat,
                Dh = textureFormat,
                Di = isReadable,
                Dj = mipmapEnable
            };
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            b.DF = !string.IsNullOrEmpty(b.Df) && b.Df != StringUtils.hr;
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            b.DG = !string.IsNullOrEmpty(b.Dg) && b.Dg != StringUtils.hr;
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            b.DH = !string.IsNullOrEmpty(b.Dh) && b.Dh != StringUtils.hr;
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            b.DI = !string.IsNullOrEmpty(b.Di) && b.Di != StringUtils.hr;
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            b.DJ = !string.IsNullOrEmpty(b.Dj) && b.Dj != StringUtils.hr;
            // ISSUE: reference to a compiler-generated method
            //this.renderTextureInfos = this.textureInfos.Where<TextureInfo>(new Func<TextureInfo, bool>(b.A))
            //    .ToList<TextureInfo>();
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            return b.DF | b.DG | b.DH | b.DI | b.DJ;
        }

        public override void RefreshTable()
        {
            if (this.needUpdateMainContent)
            {
                this.needUpdateMainContent = false;
                this.BN = new TextureWindow.AssetDataTable(this.renderTextureInfos, this.GetOverViewColumn(),
                    new FilterMethod<TextureInfo>(this.A), new SelectMethod<TextureInfo>(this.A),
                    new Action<bool>(this.A));
            }

            this.BN.OnGUI();
        }

        public class AssetDataTable : CommonTable<TextureInfo>
        {
            public AssetDataTable(
                List<TextureInfo> datas,
                CommonTableColumn<TextureInfo>[] cs,
                FilterMethod<TextureInfo> onfilter,
                SelectMethod<TextureInfo> onselect = null,
                Action<bool> toggleSelectAll = null)
                : base(datas, cs, onfilter, onselect, toggleSelectAll)
            {
            }
        }
        
        public class B
        {
            public string Df;
            public string Dg;
            public string Dh;
            public string Di;
            public string Dj;
            public bool DF;
            public bool DG;
            public bool DH;
            public bool DI;
            public bool DJ;
            //public TextureInfo A;
        }
    }
}