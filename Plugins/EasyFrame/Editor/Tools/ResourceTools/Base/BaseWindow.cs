using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

#nullable disable
namespace Analysis
{
    public class BaseWindow<T> where T : class, new()
    {
        private Vector2 af = Vector2.zero;
        private float aG = 1000f;
        private Rect ag;
        private float aH = 5f;
        private bool ah;
        private Vector2 aI = Vector2.zero;
        public EditorWindow win;
        public string currentFolder;
        public bool needRefreshCurrentFolder;
        public string[] currentAssetPathList;
        public AssetTreeView assetTreeView;
        public List<string> selectedAssetGuid = new List<string>();
        public static ReferenceFinderData assetRefrenceDatas = new ReferenceFinderData();
        public bool initializedRefrenceData = false;
        public TreeViewState treeViewState;
        public bool needUpdateMainContent = false;
        public bool needUpdateAssetTree = false;
        public static T instance;
        public static readonly object locker = new object();
        private HashSet<string> ai = new HashSet<string>();

        public BaseWindow() => this.win = AnalysisWindow.GetCurrentWindow();

        public static T GetInstance()
        {
            lock (BaseWindow<T>.locker)
            {
                if ((object)BaseWindow<T>.instance == null)
                    BaseWindow<T>.instance = new T();
            }

            return BaseWindow<T>.instance;
        }

        public void Show()
        {
            GUILayout.BeginHorizontal();
            this.DrawOptionBtn();
            this.DrawMainContent();
            this.DrawSplitter();
            this.DrawReferenceLayout();
            GUILayout.EndHorizontal();
        }

        public void DrawOptionBtn()
        {
            GUILayout.BeginVertical();
            this.DrawOptionArea();
            GUILayout.EndVertical();
        }

        public virtual void DrawOptionArea()
        {
        }

        public string GetCurrentFolder()
        {
            string path = StringUtils.Fk;
            foreach (Object assetObject in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(assetObject);
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                    break;
                }
            }

            return path;
        }

        public void DrawMainContent()
        {
            this.af = GUILayout.BeginScrollView(this.af, GUILayout.Width(this.aG), GUILayout.MinWidth(this.aG),
                GUILayout.MaxWidth(this.aG));
            this.RefreshTable();
            GUILayout.EndScrollView();
        }

        public virtual void RefreshTable()
        {
        }

        public void DrawSplitter()
        {
            GUILayout.Box(string.Empty, GUILayout.Width(this.aH), 
                GUILayout.MaxWidth(this.aH), GUILayout.MinWidth(this.aH), GUILayout.ExpandHeight(true));
            this.ag = GUILayoutUtility.GetLastRect();
            if (Event.current == null)
                return;
            switch (Event.current.rawType)
            {
                case UnityEngine.EventType.MouseDown:
                    if (this.ag.Contains(Event.current.mousePosition))
                    {
                        this.ah = true;
                        break;
                    }

                    break;
                case UnityEngine.EventType.MouseUp:
                    if (this.ah)
                    {
                        this.ah = false;
                        break;
                    }

                    break;
                case UnityEngine.EventType.MouseDrag:
                    if (this.ah)
                    {
                        this.aG += Event.current.delta.x;
                        this.win.Repaint();
                        break;
                    }

                    break;
            }
        }

        public void DrawReferenceLayout()
        {
            GUILayout.BeginVertical();
            if (GUILayout.Button(StringUtils.Ho, GUILayout.Width(160f), GUILayout.Height(40f)) &&
                this.currentAssetPathList.Length != 0)
            {
                this.selectedAssetGuid.Clear();
                this.selectedAssetGuid.Add(AssetDatabase.AssetPathToGUID(this.currentAssetPathList[0]));
                this.needUpdateAssetTree = true;
            }

            Rect lastRect = GUILayoutUtility.GetLastRect();
            float num1 = this.win.position.width - this.ag.xMax;
            float num2 = lastRect.yMax + 5f;
            this.aI = GUILayout.BeginScrollView(this.aI, GUILayout.Width(num1), GUILayout.MinWidth(num1),
                GUILayout.MaxWidth(num1), GUILayout.ExpandHeight(true));
            this.UpdateAssetTree();
            if (this.assetTreeView != null)
            {
                Rect rect = GUILayoutUtility.GetRect(0.0f, (float)Screen.width, 0.0f, (float)Screen.height);
                if (Event.current.type != UnityEngine.EventType.Layout)
                    this.assetTreeView.OnGUI(rect);
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        public void UpdateAssetTree()
        {
            if (!this.needUpdateAssetTree || this.selectedAssetGuid.Count == 0)
                return;
            AssetViewItem assetViewItem = this.A(this.selectedAssetGuid);
            if (this.assetTreeView == null)
            {
                if (this.treeViewState == null)
                    this.treeViewState = new TreeViewState();
                this.assetTreeView = new AssetTreeView(this.treeViewState,
                    new MultiColumnHeader(
                        AssetTreeView.CreateDefaultMultiColumnHeaderState(this.win.position.width - this.ag.x)));
            }

            this.assetTreeView.assetRoot = assetViewItem;
            this.assetTreeView.CollapseAll();
            this.assetTreeView.Reload();
            this.needUpdateAssetTree = false;
        }

        private AssetViewItem A([In] List<string> obj0)
        {
            this.ai.Clear();
            int num1 = 0;
            AssetViewItem assetViewItem1 = new AssetViewItem();
            assetViewItem1.id = num1;
            assetViewItem1.depth = -1;
            assetViewItem1.displayName = StringUtils.HP;
            assetViewItem1.data = (ReferenceFinderData.AssetDescription)null;
            AssetViewItem assetViewItem2 = assetViewItem1;
            int num2 = 0;
            foreach (string str in obj0)
                assetViewItem2.AddChild((TreeViewItem)this.A(str, ref num1, num2));
            this.ai.Clear();
            return assetViewItem2;
        }

        private AssetViewItem A([In] string obj0, [In] ref int obj1, [In] int obj2)
        {
            if (!this.ai.Contains(obj0))
            {
                BaseWindow<T>.assetRefrenceDatas.UpdateAssetState(obj0);
                this.ai.Add(obj0);
            }

            ++obj1;
            ReferenceFinderData.AssetDescription assetDescription = BaseWindow<T>.assetRefrenceDatas.assetDict[obj0];
            AssetViewItem assetViewItem1 = new AssetViewItem();
            assetViewItem1.id = obj1;
            assetViewItem1.displayName = assetDescription.name;
            assetViewItem1.data = assetDescription;
            assetViewItem1.depth = obj2;
            AssetViewItem assetViewItem2 = assetViewItem1;
            foreach (string reference in assetDescription.references)
                assetViewItem2.AddChild((TreeViewItem)this.A(reference, ref obj1, obj2 + 1));
            return assetViewItem2;
        }
    }
}