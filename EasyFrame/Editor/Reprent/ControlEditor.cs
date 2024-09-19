#if UNITY_EDITOR
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Easy
{
   [CustomEditor(typeof(Control))]
    public class ControlEditor:Editor
    {
        private string lastHeader = "";
        private string hasHeadAttribute(SerializedProperty iterator)
        {
            // 获取当前属性的自定义属性
            // 获取目标对象的类型
            var targetType = target.GetType();
            // 获取字段信息
            FieldInfo fieldInfo = targetType.GetField(iterator.name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo != null)
            {
                // 获取字段上的自定义属性
                var attributes = fieldInfo.GetCustomAttributes(true);
                foreach (var attribute in attributes)
                {
                    if (attribute.GetType().Name == "HeaderAttribute")
                    {
                        var at = attribute as HeaderAttribute;
                        lastHeader = at.header;
                    }
                }
            }
            return lastHeader;
        }
        private List<int> selectedTab; // 当前选中的标签索引
        private List<string[]> tabNames; // 标签名称
        private Dictionary<string, List<SerializedProperty>> propertys;
        private void Init()
        {
            if(propertys != null) return;
            propertys = new Dictionary<string, List<SerializedProperty>>();
            var lastProperty = "Script";
            SerializedProperty iterator = serializedObject.GetIterator();
            while (iterator.NextVisible(true)) // 遍历所有可见的字段
            {
                if(iterator.propertyPath.Contains(lastProperty)) continue;

                var tabName = hasHeadAttribute(iterator);
                if (tabName != "" && iterator.propertyPath != tabName)
                {
                    var result = propertys.TryGetValue(tabName, out var list);
                    if (result == false)
                    {
                        propertys[tabName] = new List<SerializedProperty>();
                    }

                    var pro = serializedObject.FindProperty(iterator.propertyPath);
                    propertys[tabName].Add(pro);
                }
                
                lastProperty = iterator.propertyPath;
            }
            
            selectedTab = new List<int>();
            tabNames = new List<string[]>();
            var tabItem = new string[clomCount];
            int i = 0;
            foreach (var key in propertys.Keys)
            {
                tabItem[i] = key;
                i++;
                
                if (i >= clomCount)
                {
                    selectedTab.Add(-1);
                    tabNames.Add(tabItem);
                    i = 0;
                    tabItem = new string[clomCount];
                }
            }
            if(selectedTab.Count != 0) selectedTab[0] = 0;
        }

        private Control control;
        private int clomCount = 4;
        private void OnEnable()
        {
            control = target as Control;
            Init();
        }

        private void OnDisable()
        {
            selectedTab.Clear();
            tabNames.Clear();
            propertys.Clear();
        }

        private string DrawTabs()
        {
            var tabName = "";
            for (int i = 0; i < tabNames.Count; i++)
            {
                var index = GUILayout.Toolbar(selectedTab[i], tabNames[i]);
                selectedTab[i] = index;
                if (selectedTab[i] != -1)
                {
                    tabName = tabNames[i][index];
                    for (int j = 0, n = selectedTab.Count; j < n; j++)
                    {
                        if(j == i)continue;
                        selectedTab[j] = -1;
                    }
                }
            }
            return tabName;
        }
        public override void OnInspectorGUI()
        {
            if (control.tabChild == false)
            {
                base.OnInspectorGUI();
                return;
            }
            else
            {
                serializedObject.Update(); // 更新序列化对象
                var tabName = DrawTabs();
                if (propertys != null)
                {
                    propertys.TryGetValue(tabName, out var list);
                    if (list != null)
                    {
                        foreach (var iterator in list)
                        {
                            EditorGUILayout.PropertyField(iterator);
                        }

                        if (tabName == "动画控制")
                        {
                            DrawPlay();
                        }

                        if (tabName == "插槽")
                        {
                            DrawLocatorBtn();
                        }
                    }  
                }
                serializedObject.ApplyModifiedProperties(); // 应用修
            }
        }

        private int playIndex = 0;
        private void DrawPlay()
        {
            string playAni = "";
            var index = 0;
            for (int i = 0, n = control.allAni.Count; i < n; i++)
            {
                if (index == playIndex)
                {
                    playAni = control.allAni[i].name;
                    break;
                }
                index++;
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("up"))
            {
                playIndex--;
                if (playIndex < 0) playIndex = 0;
            }

            if (GUILayout.Button("播放:" + playAni))
            {
                control.complete = (ani) =>
                {
                    control.Play("hit_scale");
                    
                };
                control.Play(playAni, true);
            }
            if (GUILayout.Button("next"))
            {
                playIndex++;
                if (playIndex >= control.allAni.Count) playIndex = control.allAni.Count - 1;
            }

            if (GUILayout.Button("停止"))
            {
                control.Stop();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawLocatorBtn()
        {
            EditorGUILayout.Space();
            if (GUILayout.Button("刷新插槽", GUILayout.Width(80),GUILayout.Height(30)))
            {
                for (int i = 0; i < control.locators.Count; i++)
                {
                    var ld = control.locators[i];
                    if(i == 0) ld.Type = LocatorType.origin;
                    if(i == 1) ld.Type = LocatorType.body;
                    if(i == 2) ld.Type = LocatorType.top;
                    if(i == 3) ld.Type = LocatorType.bip_l_hand;
                    if(i == 4) ld.Type = LocatorType.bip_r_hand;
                    if(i == 5) ld.Type = LocatorType.bip_bullet;
                    if(i == 6) ld.Type = LocatorType.bip_bullet01;
                    control.locators[i] = ld;
                }
                
                for (int i = control.locators.Count - 1; i >=0; i--)
                {
                    var ld = control.locators[i];
                    if (ld.Type == LocatorType.origin)
                    {
                        ld.Locator = control.transform.FindChildByName("origin");
                    }
                    if (ld.Type == LocatorType.body)
                    {
                        ld.Locator = control.transform.FindChildByName("body");
                    }
                    if (ld.Type == LocatorType.top)
                    {
                        ld.Locator = control.transform.FindChildByName("top");
                    }
                    if (ld.Type == LocatorType.bip_l_hand)
                    {
                        ld.Locator = control.transform.FindChildByName("bip_l_hand");
                    }
                    if (ld.Type == LocatorType.bip_r_hand)
                    {
                        ld.Locator = control.transform.FindChildByName("bip_r_hand");
                    }
                    if (ld.Type == LocatorType.bip_bullet)
                    {
                        ld.Locator = control.transform.FindChildByName("bip_bullet");
                    }
                    if (ld.Type == LocatorType.bip_bullet01)
                    {
                        ld.Locator = control.transform.FindChildByName("bip_bullet01");
                    }
                    control.locators[i] = ld;
                }
                
                EditorUtility.SetDirty(control);
                AssetDatabase.SaveAssets();
            }
        }
    }
}
#endif