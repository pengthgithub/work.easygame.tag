using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Easy
{
    /// <summary>
    ///     自定义滑块范围
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class RangeStepAttribute : PropertyAttribute
    {
        public readonly float Max;
        public readonly float Min;
        public readonly float Step;

        public RangeStepAttribute(float min, float max, float step)
        {
            Min = min;
            Max = max;
            Step = step;
        }
    }


    /// <summary>
    ///     只读
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ReadOnlyAttribute : PropertyAttribute
    {
    }

    /// <summary>
    ///     重命名属性
    ///     <para>ZhangYu 2018-06-21</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class RenameAttribute : PropertyAttribute
    {
        /// <summary> 文本颜色 </summary>
        public readonly string HtmlColor = "#ffffff";

        /// <summary> 重命名属性 </summary>
        /// <param name="name">新名称</param>
        public RenameAttribute(string name)
        {
            Name = name;
        }

        /// <summary> 重命名属性 </summary>
        /// <param name="name">新名称</param>
        /// <param name="htmlColor">文本颜色 例如："#FFF FFF" 或 "black"</param>
        public RenameAttribute(string name, string htmlColor)
        {
            Name = name;
            HtmlColor = htmlColor;
        }

        /// <summary> 枚举名称 </summary>
        public string Name { get; }
    }

    /// <summary>
    ///     添加标题属性
    ///     <para>ZhangYu 2018-06-21</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class TitleAttribute : PropertyAttribute
    {
        /// <summary> 文本颜色 </summary>
        public readonly string HtmlColor = "#B3B3B3";

        /// <summary> 标题名称 </summary>
        private string _title;

        /// <summary> 在属性上方添加一个标题 </summary>
        /// <param name="title">标题名称</param>
        public TitleAttribute(string title)
        {
            _title = title;
        }

        /// <summary> 在属性上方添加一个标题 </summary>
        /// <param name="title">标题名称</param>
        /// <param name="htmlColor">文本颜色 例如："#FFFFFF" 或 "black"</param>
        public TitleAttribute(string title, string htmlColor)
        {
            _title = title;
            HtmlColor = htmlColor;
        }

        public string Title { get; set; }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class RenameInEditorAttribute : PropertyAttribute
    {
        /// <summary> 新名称 </summary>
        public string Name = "";

        /// <summary> 重命名属性 </summary>
        /// <param name="name">新名称</param>
        public RenameInEditorAttribute(string name)
        {
            Name = name;
        }
    }


    [AttributeUsage(AttributeTargets.Field)]
    public class CustomPopAttribute : PropertyAttribute
    {
        public readonly string[] Options;

        public CustomPopAttribute(params string[] options)
        {
            Options = options;
        }
    }

    public class DropdownAssetAttribute : PropertyAttribute
    {
        public string directory;
        public string content;
        public string replaceStr;
        public string[] options;
        public List<UnityEngine.Object> assets;

        public DropdownAssetAttribute(string directory, string include = "", string replace = "")
        {
            this.directory = directory;
            content = include;
            replaceStr = replace;
        }
    }
}