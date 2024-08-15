// using System.Diagnostics;
// using System.IO;
// using System.Runtime.InteropServices;
// using System;
// #if UNITY_EDITOR
// using UnityEditor;
// #endif
// using UnityEngine;
//
// namespace Easy
// {
//
//
//     [ExecuteInEditMode]
//     public class EditorGridRenderer : MonoBehaviour
//     {
// #if UNITY_EDITOR
//         [MenuItem("Tools/Logic Export Tool")]
//         public static void LogicExportTool()
//         {
//             EditorGridRenderer logicRender = GameObject.FindAnyObjectByType<EditorGridRenderer>();
//             if (logicRender == null)
//             {
//                 GameObject logicExport = new GameObject("LogicExport");
//                 logicExport.transform.position = new Vector3(0, 0.5f, 0);
//                 logicRender = logicExport.AddComponent<EditorGridRenderer>();
//             }
//
//             //����logic�ļ��У�Ȼ���
//             UnityEngine.SceneManagement.Scene scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
//             if (scene != null)
//             {
//                 string name = scene.name;
//                 string path = $"{Application.dataPath}/Art/Scene/{name}/Logic/";
//                 if (!System.IO.Directory.Exists(path))
//                 {
//                     System.IO.Directory.CreateDirectory(path);
//                     AssetDatabase.Refresh();
//                 }
//
//                 path = $"Assets/Art/Scene/{name}/Logic/{name}.asset";
//                 if (!File.Exists(path))
//                 {
//                     var logicData = ScriptableObject.CreateInstance<LogicData>();
//                     AssetDatabase.CreateAsset(logicData, path); //�ڴ����·���д�����Դ
//                     AssetDatabase.SaveAssets(); //�洢��Դ
//                     AssetDatabase.Refresh(); //
//                 }
//
//                 LogicData dataAsset = AssetDatabase.LoadAssetAtPath<LogicData>(path);
//                 logicRender.LogicData = dataAsset;
//             }
//
//             Selection.activeGameObject = logicRender.gameObject;
//
//             logicRender.OnEnable();
//         }
// #endif
//
//         [Tooltip("Shift ����")] [SerializeField]
//         public LogicData LogicData;
//
//         [SerializeField] [Tooltip("Space:++, Ctrl+Space:--")] [Range(1, 32)]
//         public int scaleSize = 1;
//
//         private int logicType;
//
//         public int LogicType
//         {
//             get { return logicType; }
//             set { logicType = value; }
//         }
//
//         private void OnEnable()
//         {
//             if (LogicData == null) return;
//             LogicData.InitGrid();
//
//             LogicData.render.transform.SetParent(transform, false);
//             LogicData.render.hideFlags = HideFlags.HideAndDontSave;
//         }
//
//         private void OnDisable()
//         {
//             GameObject.DestroyImmediate(LogicData.render);
//             LogicData.render = null;
//         }
//
//         private void Update()
//         {
//             LogicData?.Update();
//         }
//
//         public void DrawTile(Vector3 pos)
//         {
// #if UNITY_EDITOR
//             EditorUtility.SetDirty(LogicData);
// #endif
//             if (LogicType < 10)
//             {
//                 LogicData.DrawPoint(pos, logicType, scaleSize);
//             }
//             else
//             {
//                 LogicData.DrawPrefab(pos, logicType);
//             }
//
//         }
//     }
//
// }

