// Decompiled with JetBrains decompiler
// Type: WeChatWASM.Analysis.CommonTable`1
// Assembly: wx-editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 491FB96F-C7AE-469B-A755-45BC3F066C9B
// Assembly location: C:\Users\pengt\Documents\SVN\Luck\Client\Lucky\Packages\com.qq.weixin.minigame@981890fdaa\Editor\wx-editor.dll
// XML documentation location: C:\Users\pengt\Documents\SVN\Luck\Client\Lucky\Packages\com.qq.weixin.minigame@981890fdaa\Editor\wx-editor.xml

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

#nullable disable
namespace Analysis
{
    public abstract class CommonTable
    {
        public abstract void OnGUI();
    }
    public class CommonTable<T> : CommonTable where T : BaseInfo
    {
        private const float Cm = 20f;
        private const float CN = 5f;
        private readonly float Cn = 20f;
        private bool CO;
        private CommonTreeView<T> treeView;
        private TreeViewState treeViewState;
        private readonly List<T> Cp;
        private readonly FilterMethod<T> CQ;
        private readonly SelectMethod<T> Cq;
        private readonly Action<bool> CR;

        protected MultiColumnHeaderState MultiColumnHeaderState { get; [param: In] private set; }

        public CommonTable(
            List<T> datas,
            CommonTableColumn<T>[] cs,
            FilterMethod<T> onfilter,
            SelectMethod<T> onselect = null,
            Action<bool> toggleSelectAll = null)
        {
            this.MultiColumnHeaderState = new MultiColumnHeaderState((MultiColumnHeaderState.Column[])cs);
            this.CQ = onfilter;
            this.Cp = datas;
            this.Cq = onselect;
            this.CR = toggleSelectAll;
        }

        private void A()
        {
            if (this.CO)
                return;
            if (this.treeViewState == null)
                this.treeViewState = new TreeViewState();
            this.treeView = new CommonTreeView<T>(this.treeViewState, new MultiColumnHeader(this.MultiColumnHeaderState), this.Cp,
                this.CQ, this.Cq, this.CR);
            this.treeView.Reload();
            this.CO = true;
        }

        public void SelectAll()
        {
            if (this.treeView == null)
                return;
            this.treeView.SelectAllRows();
        }

        public override void OnGUI()
        {
            this.A();
            Rect rect1 = GUILayoutUtility.GetRect(0.0f, (float)Screen.width, 0.0f, (float)Screen.height);
            if (Event.current.type == EventType.Layout)
                return;
            rect1.x += 5f;
            rect1.width -= 5f;
            rect1.y += 20f;
            Rect r = rect1;
            rect1.y += this.Cn;
            rect1.height = (float)((double)rect1.height - (double)this.Cn - 40.0);
            Rect rect2 = rect1;
            this.treeView.OnCheckAllGUI();
            this.treeView.OnGUI(rect2);
            this.treeView.OnFilterGUI(r);
        }
    }
}