using System;
using System.Diagnostics;
using System.IO;
using UnityEditor.AddressableAssets.Build.BuildPipelineTasks;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace UnityEditor.AddressableAssets
{
    internal static class AddressablesPreferences
    {
#if UNITY_2021_2_OR_NEWER
        internal const string kBuildAddressablesWithPlayerBuildKey = "Addressables.BuildAddressablesWithPlayerBuild";
#endif
        private class GUIScope : UnityEngine.GUI.Scope
        {
            float m_LabelWidth;

            public GUIScope(float layoutMaxWidth)
            {
                m_LabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 250;
                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUILayout.BeginVertical();
                GUILayout.Space(15);
            }

            public GUIScope() : this(500)
            {
            }

            protected override void CloseScope()
            {
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                EditorGUIUtility.labelWidth = m_LabelWidth;
            }
        }

        internal class Properties
        {
            public static readonly GUIContent buildSettings = EditorGUIUtility.TrTextContent("Build Settings");

            public static readonly GUIContent buildLayoutReport = EditorGUIUtility.TrTextContent("Debug Build Layout",
                $"A debug build layout file will be generated as part of the build process. The file will put written to {BuildLayoutGenerationTask.m_LayoutFilePath}");

            #if UNITY_2022_2_OR_NEWER
            internal static readonly GUIContent autoOpenAddressablesReport = EditorGUIUtility.TrTextContent("Open Addressables Report after build");
            #endif
            public static readonly GUIContent buildLayoutReportFileFormat = EditorGUIUtility.TrTextContent("File Format", $"The file format of the debug build layout file.");
            public static readonly GUIContent autoAddConfig = EditorGUIUtility.TrTextContent("Auto Add Config", $"Auto Add Config.");

#if UNITY_2021_2_OR_NEWER
            public static readonly GUIContent playerBuildSettings = EditorGUIUtility.TrTextContent("Player Build Settings");

            public static readonly GUIContent enableAddressableBuildPreprocessPlayer = EditorGUIUtility.TrTextContent("Build Addressables on build Player",
                $"If enabled, will perform a new Addressables build before building a Player. Addressable Asset Settings value can override the user global preferences.");
#endif

#if !ENABLE_ADDRESSABLE_PROFILER && UNITY_2021_2_OR_NEWER
            public static readonly GUIContent installProfilingCoreHelp = EditorGUIUtility.TrTextContent("In order to run Addressables profiler using the debug layout. Profiling.Core package is required for etc. Install now?",
                $"Profiling.Core is needed to transmit and display data from the runtime.");
            public static readonly GUIContent installProfilingCoreButton = EditorGUIUtility.TrTextContent("Install now",
                $"Enables Profiling.Core package in this project");
#endif
        }

        static AddressablesPreferences()
        {
        }

        [SettingsProvider]
        static SettingsProvider CreateAddressableSettingsProvider()
        {
            var provider = new SettingsProvider("Preferences/Addressables", SettingsScope.User, SettingsProvider.GetSearchKeywordsFromGUIContentProperties<Properties>());
            provider.guiHandler = sarchContext => OnGUI();
            return provider;
        }

        static void OnGUI()
        {
            using (new GUIScope())
            {
                DrawProperties();
            }
        }

        static void DrawProperties()
        {
            GUILayout.Label(Properties.buildSettings, EditorStyles.boldLabel);

            ProjectConfigData.GenerateBuildLayout = EditorGUILayout.Toggle(Properties.buildLayoutReport, ProjectConfigData.GenerateBuildLayout);
            if (ProjectConfigData.GenerateBuildLayout)
            {
                EditorGUI.indentLevel++;
                ProjectConfigData.ReportFileFormat buildLayoutReportFileFormat = ProjectConfigData.BuildLayoutReportFileFormat;
                int formatOldIndex = (int)buildLayoutReportFileFormat;
                int formatNewIndex = EditorGUILayout.Popup(Properties.buildLayoutReportFileFormat, formatOldIndex, new[] {"TXT and JSON", "JSON"});
                if (formatNewIndex != formatOldIndex)
                    ProjectConfigData.BuildLayoutReportFileFormat = (ProjectConfigData.ReportFileFormat)formatNewIndex;
                EditorGUI.indentLevel--;

#if UNITY_2022_2_OR_NEWER
                EditorGUI.indentLevel++;
                ProjectConfigData.AutoOpenAddressablesReport = EditorGUILayout.Toggle(Properties.autoOpenAddressablesReport, ProjectConfigData.AutoOpenAddressablesReport);
                EditorGUI.indentLevel--;
#endif
            }

#if !ENABLE_ADDRESSABLE_PROFILER && UNITY_2021_2_OR_NEWER
            GUILayout.Space(15);
            EditorGUILayout.HelpBox(Properties.installProfilingCoreHelp);
            var rect = EditorGUILayout.GetControlRect();
            if (UnityEngine.GUI.Button(rect, Properties.installProfilingCoreButton))
            {
                UnityEditor.PackageManager.Client.Add("com.unity.profiling.core@1.0.2");
            }
#endif

            GUILayout.Space(15);

#if UNITY_2021_2_OR_NEWER
            bool buildWithPlayerValue = EditorPrefs.GetBool(kBuildAddressablesWithPlayerBuildKey, true);

            GUILayout.Label(Properties.playerBuildSettings, EditorStyles.boldLabel);
            int index = buildWithPlayerValue ? 0 : 1;
            int val = EditorGUILayout.Popup(Properties.enableAddressableBuildPreprocessPlayer, index,
                new[] {"Build Addressables on Player Build", "Do Not Build Addressables on Player Build"});
            if (val != index)
            {
                bool newValue = val == 0 ? true : false;
                EditorPrefs.SetBool(kBuildAddressablesWithPlayerBuildKey, newValue);
                buildWithPlayerValue = newValue;
            }

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings != null)
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    if (settings.BuildAddressablesWithPlayerBuild == AddressableAssetSettings.PlayerBuildOption.BuildWithPlayer &&
                        buildWithPlayerValue == false)
                    {
                        EditorGUILayout.TextField(" ", "Enabled in AddressableAssetSettings (priority)");
                    }
                    else if (settings.BuildAddressablesWithPlayerBuild == AddressableAssetSettings.PlayerBuildOption.DoNotBuildWithPlayer &&
                             buildWithPlayerValue)
                    {
                        EditorGUILayout.TextField(" ", "Disabled in AddressableAssetSettings (priority)");
                    }
                }
            }
#endif
            AutoConfig();
        }
        
        //==========================================================================='
        // 绘制自动添加的配置
        //==========================================================================='
        #region 绘制自动添加的配置
        private static void AutoConfig()
        {
            GUILayout.Space(15);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(Properties.autoAddConfig, EditorStyles.boldLabel, GUILayout.Width(200));
            ProjectConfigData.AutoAddConfig = EditorGUILayout.Toggle(ProjectConfigData.AutoAddConfig);
            EditorGUILayout.EndHorizontal();
            
            if (ProjectConfigData.AutoAddConfig)
            {
                DrawAutoConfig();
            }
        }
        public enum FileType
        {
            full,
            simple,
            simpleExten
        }
        private static void DrawAutoConfig()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("名字", GUILayout.Width(80));
            EditorGUILayout.LabelField("路径", GUILayout.Width(140));
            EditorGUILayout.LabelField("分类", GUILayout.Width(120));
            EditorGUILayout.LabelField("名字类型", GUILayout.Width(120));
            EditorGUILayout.LabelField("顶级目录", GUILayout.Width(80));
            if (GUILayout.Button("+", GUILayout.Width(22)))
            {
                AddConfigItem();
            }
            EditorGUILayout.EndHorizontal();

            var acs = ProjectConfigData.AutoConfigSetting;
            for (int i = 0; i < acs.Count; i++)
            {
                var config = acs[i].Split("#");
                if (config.Length != 5)
                {
                    ProjectConfigData.RemoveAutoConfigSetting(i);
                    return;
                }
                
                string label = config[0];
                string dirPath = config[1];
                string filter = config[2];
                string fileType = config[3];
                string topDir = config[4];
                
                FileType _ft = FileType.simple;
                if (fileType == "full")
                {
                    _ft = FileType.full;
                }
                if (fileType == "simpleExten")
                {
                    _ft = FileType.simpleExten;
                }
                bool topDirection = false;
                if (topDir == "True")
                {
                    topDirection = true;
                }
                
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.BeginHorizontal();
                label = EditorGUILayout.TextField(label, GUILayout.Width(80));
                EditorGUILayout.LabelField(dirPath, GUILayout.Width(120));
                if (GUILayout.Button("S", GUILayout.Width(18)))
                {
                    string path = EditorUtility.OpenFolderPanel("选择目录", "Assets/Art", "");
                    if (!string.IsNullOrEmpty(path) && path.Contains("Assets/Art"))
                    {
                        int k = path.IndexOf("Assets/Art");
                        dirPath = path.Substring(k, path.Length - k);
                    }
                }
                filter = EditorGUILayout.TextField(filter, GUILayout.Width(120));
                _ft = (FileType)EditorGUILayout.EnumPopup(_ft, GUILayout.Width(120));
     
                topDirection = EditorGUILayout.Toggle(topDirection,GUILayout.Width(80));
                if (GUILayout.Button("-", GUILayout.Width(22)))
                {
                    ProjectConfigData.RemoveAutoConfigSetting(i);
                    return;
                }
                EditorGUILayout.EndHorizontal();
                if (EditorGUI.EndChangeCheck())
                {
                    var item = $"{label}#{dirPath}#{filter}#{_ft}#{topDirection}";
                    ProjectConfigData.ModifyAutoConfigSetting(i, item);
                }
            }
            
        }
        private static void AddConfigItem()
        {
            var item = "role#Assets/Role#*.prefab#simple#true";
            ProjectConfigData.AddAutoConfigSetting(item);
        }

        [InitializeOnEnterPlayMode]
        public static void AutoAddConfig()
        {
            if(ProjectConfigData.AutoAddConfig == false) return;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
            }
            if (settings == null)
            {
                Debug.LogError("AddressSetting 创建失败.");
                return;
            }
            var defaultGroup = settings.FindGroup("Default Local Group");
            if (defaultGroup == null)
            {
                defaultGroup = CreateAddressableGroup("Default Local Group");
            }
            settings.DefaultGroup = defaultGroup;
            
            var acs = ProjectConfigData.AutoConfigSetting;
            for (int i = 0; i < acs.Count; i++)
            {
                var config = acs[i].Split("#");
                if (config.Length != 5)
                {
                    ProjectConfigData.RemoveAutoConfigSetting(i);
                    return;
                }
                
                string label = config[0];
                string dirPath = config[1];
                string filter = config[2];
                string fileType = config[3];
                string topDir = config[4];
                
                FileType _ft = FileType.simple;
                if (fileType == "full")
                {
                    _ft = FileType.full;
                }
                if (fileType == "simpleExten")
                {
                    _ft = FileType.simpleExten;
                }
                bool topDirection = false;
                if (topDir == "True")
                {
                    topDirection = true;
                }
                
                AddGroupToAddress(label, dirPath, filter, _ft, topDirection);
            }
            AddGroupToAddress("end", "Assets/Art", "*.end", FileType.simple, false);

            stopwatch.Stop();
            UnityEngine.Debug.Log($"添加资源引用耗时: {stopwatch.ElapsedMilliseconds} ms");          
        }
        
        private static void AddGroupToAddress(string group, string path, string filterName, FileType fileType,
            bool top = false)
        {
            if (Directory.Exists(path) == false) return;
            AddressableAssetGroup aGroup = CreateGroup(group);
            
            var files = Directory.GetFiles(path, filterName,
                top ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var extension = Path.GetExtension(file);
                if (extension == ".meta") continue;
                var fileName = "";
                if (fileType == FileType.simple)
                {
                    fileName = Path.GetFileNameWithoutExtension(file);
                }
                if (fileType == FileType.full)
                {
                    fileName = file.Replace("\\", "/");
                }
                if (fileType == FileType.simpleExten)
                {
                    fileName = Path.GetFileName(file);
                }
                
                AddToAddressable(aGroup, fileName, file);
            }
        }
        
        private static AddressableAssetGroup CreateAddressableGroup(string groupName)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            Type[] _types = new Type[3]
            {
                typeof(AddressableAssetGroup),
                typeof(ContentUpdateGroupSchema),
                typeof(BundledAssetGroupSchema)
            };
            return settings.CreateGroup(groupName, false, false, false, null, _types);
        }
        
        private static AddressableAssetGroup CreateGroup(string groupName)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        
            AddressableAssetGroup group = settings.FindGroup(groupName);
            if (group == null)
            {
                group = CreateAddressableGroup(groupName);
            }
            if (group != null) group.RemoveAllAssetEntry();
        
            var labels = settings.GetLabels();
            if (labels.Contains(groupName) == false)
            {
                settings.AddLabel(groupName);
            }
            return group;
        }
        
        private static void AddToAddressable(AddressableAssetGroup group, string fileName, string path)
        {
            //文件夹直接返回
            if (Directory.Exists(path)) return;
        
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            var guid = AssetDatabase.GUIDFromAssetPath(path).ToString();
            AddressableAssetEntry entry = group.GetAssetEntry(guid);
            if (entry == null)
            {
                entry= settings.CreateOrMoveEntry(guid, group, true, false);
                entry.address = fileName;
                entry.SetLabel(group.name, true);
            }
        }
        #endregion
    }
}
