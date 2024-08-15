using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Easy
{
    [InitializeOnLoad, ExcludeFromCoverage]
    public static class UnityEditorExtern
    {
        static UnityEditorExtern()
        {
            Editor.finishedDefaultHeaderGUI += OnPostHeaderGUI;
        }
        
         [UnityEngine.TestTools.ExcludeFromCoverage]
        static void OnPostHeaderGUI(Editor editor)
        {
            if (editor.targets.Length > 0)
            {
                // only display for the Prefab/Model importer not the displayed GameObjects
                if (editor.targets[0].GetType() == typeof(GameObject))
                    return;
            }

            bool prevEnabledState = UnityEngine.GUI.enabled;
            GUILayout.BeginHorizontal();
            // GUILayout.Toggle(true, ".prefab");
            // GUILayout.Toggle(true, ".asset");
            // GUILayout.Toggle(true, "*.bytes");
            // GUILayout.Toggle(true, "*.unity");
            // GUILayout.Toggle(true, "*.*");
            GUILayout.EndHorizontal();
            UnityEngine.GUI.enabled = prevEnabledState;
        }
    }
}