namespace Plugins.Easy.Editor.Helper
{
    public static class EditorHelper
    {
        // public static Rect PropertySingleLine(this MaterialEditor editor,
        //     GUIContent label,
        //     MaterialProperty textureProp,
        //     MaterialProperty extraProperty1,
        //     MaterialProperty extraProperty2)
        // {
        //     Rect rectForSingleLine = editor.GetControlRectForSingleLine();
        //     bool flag = extraProperty1 != null || extraProperty2 != null;
        //     if (flag)
        //     {
        //         MaterialEditor.BeginProperty(rectForSingleLine, textureProp);
        //     }
        //
        //     if (extraProperty1 != null)
        //     {
        //         MaterialEditor.BeginProperty(rectForSingleLine, extraProperty1);
        //     }
        //
        //     if (extraProperty2 != null)
        //     {
        //         MaterialEditor.BeginProperty(rectForSingleLine, extraProperty2);
        //     }
        //
        //     editor.TexturePropertyMiniThumbnail(rectForSingleLine, textureProp, label.text, label.tooltip);
        //     if (!flag)
        //     {
        //         return rectForSingleLine;
        //     }
        //
        //     int indentLevel = EditorGUI.indentLevel;
        //     EditorGUI.indentLevel = 0;
        //     if (extraProperty1 == null || extraProperty2 == null)
        //     {
        //         MaterialProperty property = extraProperty1 ?? extraProperty2;
        //         editor.ExtraPropertyAfterTexture(MaterialEditor.GetRectAfterLabelWidth(rectForSingleLine), property,
        //             false);
        //     }
        //     else if (extraProperty1.type == this.MaterialProperty.PropType.Color)
        //     {
        //         editor.ExtraPropertyAfterTexture(
        //             MaterialEditor.GetFlexibleRectBetweenFieldAndRightEdge(rectForSingleLine),
        //             extraProperty2);
        //         editor.ExtraPropertyAfterTexture(MaterialEditor.GetLeftAlignedFieldRect(rectForSingleLine),
        //             extraProperty1);
        //     }
        //     else
        //     {
        //         editor.ExtraPropertyAfterTexture(MaterialEditor.GetRightAlignedFieldRect(rectForSingleLine),
        //             extraProperty2);
        //         editor.ExtraPropertyAfterTexture(MaterialEditor.GetFlexibleRectBetweenLabelAndField(rectForSingleLine),
        //             extraProperty1);
        //     }
        //
        //     EditorGUI.indentLevel = indentLevel;
        //     if (extraProperty2 != null)
        //     {
        //         MaterialEditor.EndProperty();
        //     }
        //
        //     if (extraProperty1 != null)
        //     {
        //         MaterialEditor.EndProperty();
        //     }
        //
        //     if (flag)
        //     {
        //         MaterialEditor.EndProperty();
        //     }
        //
        //     return rectForSingleLine;
        // }
    }
}