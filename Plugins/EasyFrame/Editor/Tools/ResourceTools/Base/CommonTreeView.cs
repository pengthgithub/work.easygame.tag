// Decompiled with JetBrains decompiler
// Type: WeChatWASM.Analysis.CommonTreeView`1
// Assembly: wx-editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 491FB96F-C7AE-469B-A755-45BC3F066C9B
// Assembly location: C:\Users\pengt\Documents\SVN\Luck\Client\Lucky\Packages\com.qq.weixin.minigame@981890fdaa\Editor\wx-editor.dll
// XML documentation location: C:\Users\pengt\Documents\SVN\Luck\Client\Lucky\Packages\com.qq.weixin.minigame@981890fdaa\Editor\wx-editor.xml

using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

#nullable disable
namespace Analysis
{
    internal class CommonTreeViewItem<T> : TreeViewItem where T : BaseInfo
    {
        public T info = null;

        public CommonTreeViewItem(in int id,in int depth,in T obj2)
            : base(id, depth, (object) obj2 == null ? StringUtils.HP : obj2.name)
        {
            info = obj2;
        }
    }
    
    public class CommonTreeView<T> : TreeView where T : BaseInfo
    {
        private List<CommonTreeViewItem<T>> aJ;
        private bool aj;
        private Action<bool> aK;
        private readonly List<T> ak;
        private readonly FilterMethod<T> aL;
        [CanBeNull] private readonly SelectMethod<T> al;
        private string aM;

        public CommonTreeView(
            TreeViewState state,
            MultiColumnHeader multiColumnHeader,
            List<T> datas,
            FilterMethod<T> filter,
            SelectMethod<T> select = null,
            Action<bool> toggleSelectAll = null)
            : base(state, multiColumnHeader)
        {
            this.ak = datas;
            this.aL = filter;
            this.al = select;
            this.aK = toggleSelectAll;
            multiColumnHeader.sortingChanged += new MultiColumnHeader.HeaderCallback(this.a);
            multiColumnHeader.visibleColumnsChanged += new MultiColumnHeader.HeaderCallback(this.A);
            this.showAlternatingRowBackgrounds = true;
            this.showBorder = true;
            this.rowHeight = EditorGUIUtility.singleLineHeight;
        }

        private void A([In] MultiColumnHeader obj0) => this.Reload();

        private void a([In] MultiColumnHeader obj0)
        {
            this.SortColumn(this.GetRows(), this.multiColumnHeader.sortedColumnIndex);
        }

        protected override TreeViewItem BuildRoot() => (TreeViewItem)new CommonTreeViewItem<T>(-1, -1, default(T));

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            if (this.aJ == null)
            {
                this.aJ = new List<CommonTreeViewItem<T>>();
                for (int index = 0; index < this.ak.Count; ++index)
                {
                    T obj = this.ak[index];
                    this.aJ.Add(new CommonTreeViewItem<T>(index, 0, obj));
                }
            }

            List<CommonTreeViewItem<T>> source = this.aJ;
            if (!string.IsNullOrEmpty(this.aM))
                source = this.A((IEnumerable<CommonTreeViewItem<T>>)source);
            List<TreeViewItem> treeViewItemList = new List<TreeViewItem>();
            foreach (CommonTreeViewItem<T> a in source)
                treeViewItemList.Add((TreeViewItem)a);
            if (this.multiColumnHeader.sortedColumnIndex >= 0)
                this.SortColumn((IList<TreeViewItem>)treeViewItemList, this.multiColumnHeader.sortedColumnIndex);
            return (IList<TreeViewItem>)source.Cast<TreeViewItem>().ToList<TreeViewItem>();
        }

        private List<CommonTreeViewItem<T>> A(in IEnumerable<CommonTreeViewItem<T>> obj0_1)
        {
            IEnumerable<CommonTreeViewItem<T>> source = obj0_1;
            if (this.a(0) && this.aL != null)
                source = source.Where(((obj0_2) => this.aL(obj0_2.info, this.aM)));
            return source.ToList<CommonTreeViewItem<T>>();
        }

        private CommonTableColumn<T> A([In] int obj0)
        {
            return (CommonTableColumn<T>)this.multiColumnHeader.state.columns[obj0];
        }

        private void SortColumn(in IList<TreeViewItem> obj0, in int obj1)
        {
            // ISSUE: object of a compiler-generated type is created
            // ISSUE: variable of a compiler-generated type
            CommonTreeView<T>.SortItem a = new CommonTreeView<T>.SortItem();
            bool flag = this.multiColumnHeader.IsSortedAscending(obj1);
            // ISSUE: reference to a compiler-generated field
            a.CS = this.A(obj1).Compare;
            List<TreeViewItem> treeViewItemList = (List<TreeViewItem>)obj0;
            // ISSUE: reference to a compiler-generated field
            if (a.CS == null)
                return;
            // ISSUE: reference to a compiler-generated method
            Comparison<TreeViewItem> comparison1 = new Comparison<TreeViewItem>(a.SortUp);
            // ISSUE: reference to a compiler-generated method
            Comparison<TreeViewItem> comparison2 = new Comparison<TreeViewItem>(a.SortDown);
            treeViewItemList.Sort(!flag ? comparison2 : comparison1);
        }

        protected override void RowGUI(TreeView.RowGUIArgs args)
        {
            CommonTreeViewItem<T> a = (CommonTreeViewItem<T>)args.item;
            for (int visibleColumnIndex = 0; visibleColumnIndex < args.GetNumVisibleColumns(); ++visibleColumnIndex)
                this.A(args.GetCellRect(visibleColumnIndex), a.info, args.GetColumn(visibleColumnIndex));
        }

        private void A([In] Rect obj0, [In] T obj1, [In] int obj2)
        {
            this.CenterRectUsingSingleLineHeight(ref obj0);
            CommonTableColumn<T> column = (CommonTableColumn<T>)this.multiColumnHeader.GetColumn(obj2);
            if (column.DrawCell == null)
                return;
            column.DrawCell(obj0, obj1);
        }

        public void OnCheckAllGUI()
        {
            if (this.GetSelection().Count != this.ak.Count)
                this.aj = false;
            bool flag = GUI.Toggle(new Rect(5f, 20f, 50f, 20f), this.aj, StringUtils.Hp);
            if (flag == this.aj)
                return;
            this.aj = flag;
            if (this.aj)
            {
                if (this.ak.Count != 0)
                    this.SelectAllRows();
            }
            else
                this.SetSelection((IList<int>)new List<int>());

            if (this.aK != null)
                this.aK(this.aj);
        }

        public void OnFilterGUI(Rect r)
        {
            EditorGUI.BeginChangeCheck();
            float width = r.width;
            float num = 16f;
            r.width = num;
            r.x += num;
            r.width = GUI.skin.label.CalcSize(StringUtils.BK).x;
            r.width = Mathf.Min(width - (r.x + r.width), 300f);
            r.x = (float)((double)width - (double)r.width + 25.0);
            this.SearchFilterGUI(r);
            if (!EditorGUI.EndChangeCheck())
                return;
            this.Reload();
        }

        private void SearchFilterGUI([In] Rect obj0)
        {
            obj0.width -= 15f;
            obj0.height = 20f;
            this.aM = EditorGUI.DelayedTextField(obj0, GUIContent.none, this.aM);
            obj0.x += obj0.width;
            obj0.width = 15f;
            bool flag = this.aM != string.Empty;
            if (!(GUI.Button(obj0, GUIContent.none) & flag))
                return;
            this.aM = string.Empty;
            GUIUtility.keyboardControl = 0;
        }

        private bool a([In] int obj0)
        {
            // ISSUE: object of a compiler-generated type is created
            // ISSUE: reference to a compiler-generated method
            return ((IEnumerable<int>)this.multiColumnHeader.state.visibleColumns).Any<int>(new Func<int, bool>(
                new CommonTreeView<T>.SortItem()
                {
                    Cs = obj0
                }.ab));
        }

        protected override void KeyEvent()
        {
            if (Event.current.type != UnityEngine.EventType.KeyDown || Event.current.character != '\t')
                return;
            GUI.FocusControl(StringUtils.Bg);
            Event.current.Use();
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            List<T> datas = new List<T>();
            foreach (int selectedId in (IEnumerable<int>)selectedIds)
            {
                if (selectedId < 0 || selectedId > this.ak.Count)
                {
                    Debug.Log((object)(selectedId.ToString() + StringUtils.HQ));
                }
                else
                {
                    T obj = this.ak[selectedId];
                    datas.Add(obj);
                }
            }

            if (this.al == null)
                return;
            this.al(datas);
        }
        
        public class SortItem
        {
            public CompareMethod<T> CS;
            public int Cs;
            public int SortUp(TreeViewItem  a, TreeViewItem  b)
            {
                return string.Compare(a.displayName, b.displayName);
            }
            public int SortDown(TreeViewItem  a, TreeViewItem  b)
            {
                return string.Compare(b.displayName, a.displayName);
            }

            public bool ab(int columnIndex)
            {
                return columnIndex == Cs; // 示例逻辑
            }
        }
    }
}