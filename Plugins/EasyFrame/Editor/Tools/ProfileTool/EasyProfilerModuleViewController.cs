using System;
using Unity.Profiling;
using Unity.Profiling.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ProfileTool
{
    public class EasyProfilerModuleViewController: ProfilerModuleViewController
    {
        private const string k_UxmlResourcePath = "Packages/com.unity.addressables/Editor/Diagnostics/Profiler/UXML";
        private static string UnsupportedPath => k_UxmlResourcePath + "/Unsupported.uxml";

        private readonly ProfilerWindow m_ProfilerWindow;
        private VisualElement m_MainView;

        public EasyProfilerModuleViewController(ProfilerWindow profilerWindow) : base(profilerWindow)
        {
            m_ProfilerWindow = profilerWindow;
        }

        protected override VisualElement CreateView()
        {
            var asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UnsupportedPath);
            if (asset == null)
            {
                Debug.Log(UnsupportedPath + " 加载失败.");
            }
            VisualElement root = asset.Instantiate();
            
            m_ProfilerWindow.SelectedFrameIndexChanged += OnSelectedFrameIndexChanged;
            return root;
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            m_ProfilerWindow.SelectedFrameIndexChanged -= OnSelectedFrameIndexChanged;
            base.Dispose(disposing);
        }

        void OnSelectedFrameIndexChanged(long selectedFrameIndex)
        {
            ReloadData(selectedFrameIndex);
        }

        void ReloadData(long selectedFrameIndex)
        {
        }
    }
}