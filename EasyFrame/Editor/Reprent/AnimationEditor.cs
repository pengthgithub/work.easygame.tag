#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// 动画播放编辑器
/// </summary>
[CustomEditor(typeof(Animation))]
public class AnimationPlay : UnityEditor.Editor
{
    private int _index = 0;
    private string[] _aniArray;
    public override void OnInspectorGUI()
    {
        var ani = target as Animation;
        base.OnInspectorGUI();
        var c = ani.GetClipCount();
        if (_aniArray == null || c != _aniArray.Length)
        {
            _aniArray = new string[c];
            int i = 0; 
            foreach (AnimationState anim in ani)
            {
                var clip = anim.clip;
                if (clip)
                {
                    _aniArray[i] = clip.name;
                    i++;
                }
            }
        }

        if (c != 0)
        {
            EditorGUILayout.BeginHorizontal();
            _index = EditorGUILayout.Popup(_index, _aniArray); 
            if (GUILayout.Button("Play"))
            {
                if (Application.isPlaying)
                {
                    var name = _aniArray[_index];    
                    ani.Play(name);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif // UNITY_EDITOR
