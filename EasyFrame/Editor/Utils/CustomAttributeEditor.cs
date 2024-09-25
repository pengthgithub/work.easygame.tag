using System.IO;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEditor;

namespace Easy
{

    /// <summary>
    ///     自定义滑块范围绘制
    /// </summary>
    [CustomPropertyDrawer(typeof(RangeStepAttribute))]
    public class RangeStepDrawer : PropertyDrawer
    {
        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // First get the attribute since it contains the range for the slider
            RangeStepAttribute range = attribute as RangeStepAttribute;
            if (range == null)
            {
                return;
            }

            // Now draw the property as a Slider or an IntSlider based on whether it's a float or integer.
            if (property.propertyType == SerializedPropertyType.Float)
            {
                EditorGUI.Slider(position, property, range.Min, range.Max, label);
            }
            else if (property.propertyType == SerializedPropertyType.Integer)
            {
                EditorGUI.IntSlider(position, property, Convert.ToInt32(range.Min), Convert.ToInt32(range.Max), label);
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use Range with float or int.");
            }

            if (property.propertyType == SerializedPropertyType.Float)
            {
                property.floatValue = (float)Math.Floor(property.floatValue / range.Step) * range.Step;
            }

            if (property.propertyType == SerializedPropertyType.Integer)
            {
                property.intValue = (int)(Math.Floor(property.intValue / range.Step) * range.Step);
            }
        }
    }


    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property);
            GUI.enabled = true;
        }
    }


    [CustomPropertyDrawer(typeof(RenameAttribute))]
    public class RenameDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // 替换属性名称
            RenameAttribute rename = (RenameAttribute)attribute;
            label.text = rename.Name;

            // 重绘GUI
            Color defaultColor = EditorStyles.label.normal.textColor;

            EditorStyles.label.normal.textColor = HtmlToColor(rename.HtmlColor);

            bool isElement = Regex.IsMatch(property.displayName, "Element \\d+");
            if (isElement)
            {
                //label.text = property.displayName;
            }

            if (property.propertyType == SerializedPropertyType.Enum)
            {
                DrawEnum(position, property, label);
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }

            EditorStyles.label.normal.textColor = defaultColor;
        }

        // 绘制枚举类型
        private void DrawEnum(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();

            // 获取枚举相关属性
            Type type = fieldInfo.FieldType;
            string[] names = property.enumNames;
            string[] values = new string[names.Length];
            Array.Copy(names, values, names.Length);
            while (type != null && type.IsArray)
            {
                type = type.GetElementType();
            }

            if (type == null)
            {
                return;
            }

            // 获取枚举所对应的RenameAttribute
            int i = 0;
            for (; i < names.Length; i++)
            {
                FieldInfo info = type.GetField(names[i]);
                RenameAttribute[] attributes =
                    (RenameAttribute[])info.GetCustomAttributes(typeof(RenameAttribute), true);
                if (attributes.Length != 0)
                {
                    values[i] = attributes[0].Name;
                }
            }

            // 重绘GUI
            int index = EditorGUI.Popup(position, label.text, property.enumValueIndex, values);
            if (EditorGUI.EndChangeCheck() && index != -1)
            {
                property.enumValueIndex = index;
            }
        }

        /// <summary> Html颜色转换为Color </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static Color HtmlToColor(string hex)
        {
            // 编辑器默认颜色
            if (string.IsNullOrEmpty(hex))
            {
                return new Color(0.705f, 0.705f, 0.705f);
            }

#if UNITY_EDITOR
            // 转换颜色
            hex = hex.ToLower();
            if (hex.IndexOf("#") == 0 && hex.Length == 7)
            {
                int r = Convert.ToInt32(hex.Substring(1, 2), 16);
                int g = Convert.ToInt32(hex.Substring(3, 2), 16);
                int b = Convert.ToInt32(hex.Substring(5, 2), 16);
                return new Color(r / 255f, g / 255f, b / 255f);
            }

            return hex switch
            {
                "red" => Color.red,
                "green" => Color.green,
                "blue" => Color.blue,
                "yellow" => Color.yellow,
                "black" => Color.black,
                "white" => Color.white,
                "cyan" => Color.cyan,
                "gray" => Color.gray,
                "grey" => Color.grey,
                "magenta" => Color.magenta,
                "orange" => new Color(1, 165 / 255f, 0),
                _ => new Color(0.705f, 0.705f, 0.705f)
            };

#endif
        }
    }


    [CustomPropertyDrawer(typeof(TitleAttribute))]
    public class TitleAttributeDrawer : DecoratorDrawer
    {
        // 文本样式
        private readonly GUIStyle _style = new(EditorStyles.label);

        public override void OnGUI(Rect position)
        {
            // 获取Attribute
            TitleAttribute rename = (TitleAttribute)attribute;
            _style.fixedHeight = 18;
            _style.normal.textColor = RenameDrawer.HtmlToColor(rename.HtmlColor);

            // 重绘GUI
            position = EditorGUI.IndentedRect(position);
            GUI.Label(position, rename.Title, _style);
        }

        public override float GetHeight()
        {
            return base.GetHeight() - 3;
        }
    }

    
    [CustomPropertyDrawer(typeof(CustomPopAttribute))]
    public class CustomPopDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                EditorGUI.BeginProperty(position, label, property);

                if (attribute is not CustomPopAttribute popAttribute)
                {
                    return;
                }

                int selectedIndex = Mathf.Max(0, Array.IndexOf(popAttribute.Options, property.stringValue));
                selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, popAttribute.Options);

                property.stringValue = popAttribute.Options[selectedIndex];

                EditorGUI.EndProperty();
            }

            if (property.propertyType == SerializedPropertyType.Integer)
            {
                EditorGUI.BeginProperty(position, label, property);

                if (attribute is not CustomPopAttribute popAttribute)
                {
                    return;
                }

                property.intValue = EditorGUI.Popup(position, label.text, property.intValue, popAttribute.Options);

                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }

    // 自定义 PropertyDrawer 类
    [CustomPropertyDrawer(typeof(DropdownAssetAttribute))]
    public class DropdownAssetDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // 获取 Attribute 的属性
            DropdownAssetAttribute dropdownAsset = (DropdownAssetAttribute)attribute;
            if (dropdownAsset == null)
            {
                return;
            }

            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                ShowObject(dropdownAsset, position, property, label);
            }

            if (property.propertyType == SerializedPropertyType.String)
            {
                if (dropdownAsset.options == null)
                {
                    var files = Directory.GetFiles(dropdownAsset.directory, dropdownAsset.content,
                        SearchOption.AllDirectories);

                    dropdownAsset.options = new string[files.Length + 1];
                    dropdownAsset.options[0] = "none";
                    var i = 0;
                    foreach (var file in files)
                    {
                        i++;
                        var name = Path.GetFileNameWithoutExtension(file);
                        if (dropdownAsset.replaceStr != "")
                        {
                            name = name.Replace(dropdownAsset.replaceStr, "");
                        }

                        dropdownAsset.options[i] = name;
                    }
                }

                int selectedIndex = Array.IndexOf(dropdownAsset.options, property.stringValue);
                int newIndex = EditorGUI.Popup(position, label.text, selectedIndex, dropdownAsset.options);

                // 更新属性值
                property.stringValue = newIndex > 0 ? dropdownAsset.options[newIndex] : "none";
            }

            if (property.propertyType == SerializedPropertyType.Integer)
            {
            }
        }

        private void ShowObject(DropdownAssetAttribute dropdownAsset, Rect position, SerializedProperty property,
            GUIContent label)
        {
            if (dropdownAsset.options == null)
            {
                // 查找指定目录下的特定资源类型
                List<UnityEngine.Object> assets =
                    FindAssetsOfType(dropdownAsset.directory, property.type);

                // 创建下拉框选项
                List<UnityEngine.Object> dropAssets = new List<UnityEngine.Object>();
                dropdownAsset.options = new string[assets.Count];
                for (int i = 0; i < assets.Count; i++)
                {
                    string name = assets[i].name;
                    if (!string.IsNullOrEmpty(dropdownAsset.content))
                    {
                        if (name.Contains(dropdownAsset.content))
                        {
                            dropAssets.Add(assets[i]);
                        }
                    }
                    else
                    {
                        dropAssets.Add(assets[i]);
                    }
                }

                dropdownAsset.options = new string[dropAssets.Count + 1];
                dropdownAsset.options[0] = "none";
                for (int i = 0; i < dropAssets.Count; i++)
                {
                    dropdownAsset.options[i + 1] = dropAssets[i].name;
                }

                dropdownAsset.assets = dropAssets;
            }

            // 显示下拉框
            int selectedIndex = Array.IndexOf(dropdownAsset.options, property.objectReferenceValue?.name);
            int newIndex = EditorGUI.Popup(position, label.text, selectedIndex, dropdownAsset.options);

            // 更新属性值
            property.objectReferenceValue = newIndex > 0 ? dropdownAsset.assets[newIndex - 1] : null;
        }

        private static List<UnityEngine.Object> FindAssetsOfType(string directory, string _type)
        {
            string assetType = _type.Replace("PPtr<$", "").Replace(">", "");
            List<UnityEngine.Object> assets = new List<UnityEngine.Object>();
            string[] guids = AssetDatabase.FindAssets($"t:{assetType}", new[] { directory });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<Object>(path);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }

            return assets;
        }
    }
}
