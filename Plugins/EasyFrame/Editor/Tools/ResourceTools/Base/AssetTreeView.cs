// Decompiled with JetBrains decompiler
// Type: WeChatWASM.Analysis.AssetTreeView
// Assembly: wx-editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 491FB96F-C7AE-469B-A755-45BC3F066C9B
// Assembly location: C:\Users\pengt\Documents\SVN\Luck\Client\Lucky\Packages\com.qq.weixin.minigame@981890fdaa\Editor\wx-editor.dll
// XML documentation location: C:\Users\pengt\Documents\SVN\Luck\Client\Lucky\Packages\com.qq.weixin.minigame@981890fdaa\Editor\wx-editor.xml

using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

#nullable disable
namespace Analysis
{
    public class AssetTreeView : TreeView
    {
        private const float aC = 18f;
        private const float ac = 20f;
        public AssetViewItem assetRoot;

        private GUIStyle aD = new GUIStyle()
        {
            richText = true,
            alignment = TextAnchor.MiddleCenter
        };

        public AssetTreeView(TreeViewState state, MultiColumnHeader multicolumnHeader)
            : base(state, multicolumnHeader)
        {
            this.useScrollView = false;
            this.rowHeight = 20f;
            this.columnIndexForTreeFoldouts = 0;
            this.showAlternatingRowBackgrounds = true;
            this.showBorder = false;
            this.customFoldoutYOffset = (float)((20.0 - (double)EditorGUIUtility.singleLineHeight) * 0.5);
            this.extraSpaceBeforeIconAndLabel = 18f;
        }

        protected override void DoubleClickedItem(int id)
        {
            AssetViewItem assetViewItem = (AssetViewItem)this.FindItem(id, this.rootItem);
            if (assetViewItem == null)
                return;
            Object @object = AssetDatabase.LoadAssetAtPath(assetViewItem.data.path, typeof(Object));
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = @object;
            EditorGUIUtility.PingObject(@object);
        }

        public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState(float treeViewWidth)
        {
            return new MultiColumnHeaderState(new MultiColumnHeaderState.Column[2]
            {
                new MultiColumnHeaderState.Column()
                {
                    headerContent = new GUIContent(StringUtils.Hk),
                    headerTextAlignment = TextAlignment.Center,
                    sortedAscending = false,
                    width = 200f,
                    minWidth = 60f,
                    autoResize = false,
                    allowToggleVisibility = false,
                    canSort = false
                },
                new MultiColumnHeaderState.Column()
                {
                    headerContent = new GUIContent(StringUtils.HL),
                    headerTextAlignment = TextAlignment.Center,
                    sortedAscending = false,
                    width = 360f,
                    minWidth = 60f,
                    autoResize = false,
                    allowToggleVisibility = false,
                    canSort = false
                }
            });
        }

        protected override TreeViewItem BuildRoot() => (TreeViewItem)this.assetRoot;

        protected override void RowGUI(TreeView.RowGUIArgs args)
        {
            AssetViewItem assetViewItem = (AssetViewItem)args.item;
            for (int visibleColumnIndex = 0; visibleColumnIndex < args.GetNumVisibleColumns(); ++visibleColumnIndex)
                this.LoadTexture(args.GetCellRect(visibleColumnIndex), assetViewItem,
                    (AssetTreeView.TreeViewStat)args.GetColumn(visibleColumnIndex), ref args);
        }

        private void LoadTexture(Rect obj0, in AssetViewItem obj1,
            in AssetTreeView.TreeViewStat obj2, ref TreeView.RowGUIArgs obj3)
        {
            this.CenterRectUsingSingleLineHeight(ref obj0);
            switch (obj2)
            {
                case AssetTreeView.TreeViewStat.CI:
                    Rect position = obj0;
                    position.x += this.GetContentIndent((TreeViewItem)obj1);
                    position.width = 18f;
                    if ((double)position.x < (double)obj0.xMax)
                    {
                        Texture2D image = this.LoadTexture(obj1.data.path);
                        if ((Object)image != (Object)null)
                            GUI.DrawTexture(position, (Texture)image, ScaleMode.ScaleToFit);
                    }

                    obj3.rowRect = obj0;
                    base.RowGUI(obj3);
                    break;
                case AssetTreeView.TreeViewStat.Ci:
                    GUI.Label(obj0, obj1.data.path);
                    break;
                case AssetTreeView.TreeViewStat.CJ:
                    GUI.Label(obj0, ReferenceFinderData.GetInfoByState(obj1.data.state), this.aD);
                    break;
            }
        }

        private Texture2D LoadTexture([In] string obj0)
        {
            Object @object = AssetDatabase.LoadAssetAtPath(obj0, typeof(Object));
            if (!(@object != (Object)null))
                return (Texture2D)null;
            Texture2D texture2D = AssetPreview.GetMiniThumbnail(@object);
            if ((Object)texture2D == (Object)null)
                texture2D = AssetPreview.GetMiniTypeThumbnail(((object)@object).GetType());
            return texture2D;
        }

        private enum TreeViewStat
        {
            CI,
            Ci,
            CJ,
        }
    }
}