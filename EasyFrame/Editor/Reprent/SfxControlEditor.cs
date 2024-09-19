#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Easy
{
    [CustomEditor(typeof(SfxControl))]
    public class SfxControlEditor : Editor
    {
        private SfxControl _sfxControl;
        private void OnEnable()
        {
            _sfxControl = target as SfxControl;
            _sfxControl.CalSize();
        }

        private void OnDisable()
        {
            
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.BeginHorizontal();
            // if (GUILayout.Button("粒子数加倍"))
            // {
            //     _sfxControl.Add();
            // }
            // if (GUILayout.Button("粒子数减半"))
            // {
            //     _sfxControl.Sub(); 
            // }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("播放"))
            {
                _sfxControl.Play("");
            }
        }
    }
}
#endif