using UnityEditor;
using UnityEngine;

#nullable disable
namespace Analysis
{
    public class AnalysisWindow : EditorWindow
    {
        public int AssetsTypeSelected;

        public string[] AssetsTypeOptions = new string[6]
        {
            StringUtils.texture,
            StringUtils.font,
            StringUtils.audio,
            StringUtils.prefab,
            StringUtils.sfx,
            StringUtils.material
        };

        public TextureWindow TextureWindow;
        public FontWindow FontWindow;
        public AudioWindow AudioWindow;
        public PrefabWindow PrefabWindow;
        public SfxWindow SfxWindow;
        public MaterialWindow MaterialWindow;
        
        private static EditorWindow currentWindow;

        [MenuItem("Tools / 资源优化工具", false, 102)]
        private static void AAAAA()
        {
            if(currentWindow) currentWindow.Close();
            currentWindow = GetCurrentWindow();
            currentWindow.minSize = new Vector2(1600f, 800f);
            currentWindow.Show();
        }

        public static EditorWindow GetCurrentWindow()
        {
            return GetWindow(typeof(AnalysisWindow), false, StringUtils.titile, true);
        }

        public void OnEnable()
        {
            TextureWindow = BaseWindow<TextureWindow>.GetInstance();
            FontWindow = BaseWindow<FontWindow>.GetInstance();
            AudioWindow = BaseWindow<AudioWindow>.GetInstance();
            PrefabWindow = BaseWindow<PrefabWindow>.GetInstance();
            SfxWindow = BaseWindow<SfxWindow>.GetInstance();
            MaterialWindow= BaseWindow<MaterialWindow>.GetInstance();
        }

        public void OnDisable()
        {
            TextureWindow = null;
            FontWindow = null;
            AudioWindow = null;
            PrefabWindow = null;
            SfxWindow = null;
            MaterialWindow = null;
            
            EditorUtility.ClearProgressBar();
        }

        public void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayoutOption[] guiLayoutOptionArray = new GUILayoutOption[1]
            {
                GUILayout.Height(25f)
            };
            AssetsTypeSelected = GUILayout.Toolbar(AssetsTypeSelected, AssetsTypeOptions, guiLayoutOptionArray);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            switch (AssetsTypeSelected)
            {
                case 0:
                    TextureWindow.Show();
                    break;
                case 1:
                    FontWindow.Show();
                    break;
                case 2:
                    AudioWindow.Show();
                    break;
                case 3:
                    PrefabWindow.Show();
                    break;
                case 4:
                    SfxWindow.Show();
                    break;
                case 5:
                    MaterialWindow.Show();
                    break;
            }
        }
    }
}