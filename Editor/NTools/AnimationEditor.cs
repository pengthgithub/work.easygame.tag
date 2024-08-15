using System;
using System.Collections;
using System.Collections.Generic;
using Easy;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Animation))]
public class AnimationPlay : Editor
{
    public void OnEnable()
    {
        EditorApplication.update += UpdateAnimation;
    }

    public void OnDisable()
    {
        EditorApplication.update -= UpdateAnimation;
    }

    private double startTime = 0;
    private bool _StartPlay = false;
    private float updateTime = 0;

    public void UpdateAnimation()
    {
        if (Application.isPlaying) return;
        if (_StartPlay == false) return;

        updateTime = (float)(EditorApplication.timeSinceStartup - startTime);
        var ani = target as Animation;
        ani.clip.SampleAnimation(ani.gameObject, updateTime);

        if (ani.clip.length < updateTime) _StartPlay = false;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Label("Ê±¼ä:" + updateTime);
        if (GUILayout.Button("Play"))
        {
            var ani = target as Animation;
            if (Application.isPlaying)
            {
                ani.Play();
                return;
            }

            startTime = EditorApplication.timeSinceStartup;
            _StartPlay = true;
        }
    }
}



[CustomEditor(typeof(EAnimation))]
public class EAnimationEditor : Editor
{
    private string aniName = "show";
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("AniName");
        aniName = GUILayout.TextField(aniName);
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("Play"))
        {
           var ec =  target as EAnimation;
           if (ec) ec.PlayAnimation(aniName);
        }
    }
}

[CustomEditor(typeof(ECharacter))]
public class ECharacterEditor : Editor
{
    private string aniName = "show";
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("AniName");
        aniName = GUILayout.TextField(aniName);
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("Play"))
        {
            ECharacter ec =  target as ECharacter;
            if (ec) ec.AnimationName = aniName;
        }
    }
}