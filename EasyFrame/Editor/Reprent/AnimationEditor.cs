#if UNITY_EDITOR
using System;
using System.IO;
using Easy;
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

    private void OnEnable()
    {
        var ani = target as Animation;

        if (Application.isPlaying == false)
        {
            if (ani && ani.name.Contains("np_"))
            {
                var anis = ani.GetComponentsInChildren<Animation>();
                for (int i = anis.Length -1; i >=0; i--)
                {
                    if(ani == anis[i]) continue;
                    GameObject.DestroyImmediate(anis[i]);
                }
                
                var files = Directory.GetFiles("Assets/Art/Character/common", "*.anim", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(file);
                    var c = ani.GetClip(clip.name);
                    if (c == null)
                    { 
                        ani.AddClip(clip,clip.name);
                    }
                }
            }
        }
        
        var cC = ani.GetClipCount();
        if (cC != 0)
        {
            if (_aniArray == null || cC != _aniArray.Length)
            {
                _aniArray = new string[cC];
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
        }
    }

    public override void OnInspectorGUI()
    {
        var ani = target as Animation;
        base.OnInspectorGUI();
        var c = ani.GetClipCount();
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
