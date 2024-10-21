// Decompiled with JetBrains decompiler
// Type: WeChatWASM.Analysis.OptimizeOverview
// Assembly: wx-editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 491FB96F-C7AE-469B-A755-45BC3F066C9B
// Assembly location: C:\Users\pengt\Documents\SVN\Luck\Client\Lucky\Packages\com.qq.weixin.minigame@981890fdaa\Editor\wx-editor.dll
// XML documentation location: C:\Users\pengt\Documents\SVN\Luck\Client\Lucky\Packages\com.qq.weixin.minigame@981890fdaa\Editor\wx-editor.xml

using UnityEditor;
using UnityEngine;

#nullable disable
namespace Analysis
{
    public class OptimizeOverview
    {
        private EditorWindow ap;
        private static OptimizeOverview aQ;
        private static readonly object aq = new object();

        private OptimizeOverview() => this.ap = AnalysisWindow.GetCurrentWindow();

        public static OptimizeOverview GetInstance()
        {
            lock (OptimizeOverview.aq)
            {
                if (OptimizeOverview.aQ == null)
                    OptimizeOverview.aQ = new OptimizeOverview();
            }

            return OptimizeOverview.aQ;
        }

        public void Show()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(StringUtils.HS, GUILayout.Width(140f), GUILayout.Height(40f)))
                this.GetOverview();
            GUILayout.EndHorizontal();
        }

        public void GetOverview() => BaseWindow<TextureWindow>.GetInstance().CollectAssets();

        public void DrawOverview()
        {
            GUILayout.BeginVertical();
            BaseWindow<TextureWindow>.GetInstance();
            GUILayout.EndVertical();
        }
    }
}