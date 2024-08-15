// #if UNITY_EDITOR
//
// using UnityEditor;
//
// using UnityEngine;
//
// namespace Easy
// {
//     [CustomEditor(typeof(EditorGridRenderer))]
//     public class EditorGidEditor : Editor
//     {
//         private EditorGridRenderer m_target;
//
//         private void OnEnable()
//         {
//             int controlID = GUIUtility.GetControlID(FocusType.Passive);
//             HandleUtility.AddDefaultControl(controlID);
//
//             m_target = target as EditorGridRenderer; //��target��target�ٷ����ͣ� The object being inspected
//
//         }
//
//         private RaycastHit hitInfo = new RaycastHit();
//
//         public override void OnInspectorGUI()
//         {
//             base.OnInspectorGUI();
//
//             EditorGUILayout.BeginHorizontal();
//             if (GUILayout.Button("����"))
//             {
//                 m_target.LogicData?.ResetTex();
//             }
//
//             if (GUILayout.Button("����"))
//             {
//                 m_target.LogicData?.ExportBytes();
//             }
//
//             EditorGUILayout.EndHorizontal();
//         }
//
//
//         /// <summary>
//         /// ������ߣ��ǽ����ϵ� Gizmos ��ťû�е���
//         /// </summary>
//         public void OnSceneGUI()
//         {
//             Handles.BeginGUI();
//             const float start = 200;
//             const float w = 30;
//             if (GUI.Button(new Rect(10, start, 80, 20), "Obstracle"))
//             {
//                 m_target.LogicType = 0;
//             }
//
//             if (GUI.Button(new Rect(10, start + 1 * w, 80, 20), "WalkAble"))
//             {
//                 m_target.LogicType = 1;
//             }
//
//             if (GUI.Button(new Rect(10, start + 2 * w, 80, 20), "Water"))
//             {
//                 m_target.LogicType = 2;
//             }
//
//             if (GUI.Button(new Rect(10, start + 3 * w, 80, 20), "OutWalkAble"))
//             {
//                 m_target.LogicType = 3;
//             }
//
//             if (GUI.Button(new Rect(10, start + 4 * w, 80, 20), "Item"))
//             {
//                 m_target.LogicType = 10;
//             }
//
//             if (GUI.Button(new Rect(10, start + 5 * w, 80, 20), "Point"))
//             {
//                 m_target.LogicType = 11;
//             }
//
//             if (GUI.Button(new Rect(10, start + 6 * w, 80, 20), "NPC"))
//             {
//                 m_target.LogicType = 12;
//             }
//
//             if (GUI.Button(new Rect(10, start + 7 * w, 80, 20), "TRAP"))
//             {
//                 m_target.LogicType = 13;
//             }
//
//             Handles.EndGUI();
//
//             var e = Event.current;
//             var logic = e.shift;
//             if (m_target.LogicType > 10)
//             {
//                 if (e.keyCode == KeyCode.LeftAlt)
//                     logic = true;
//                 else logic = false;
//             }
//
//             if (logic)
//             {
//                 Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
//                 if (Physics.Raycast(ray, out hitInfo))
//                 {
//                     m_target.DrawTile(hitInfo.point);
//                 }
//             }
//
//             if (e.keyCode == KeyCode.Space && m_target && e.type == EventType.KeyUp)
//             {
//                 if (e.control)
//                 {
//                     m_target.scaleSize--;
//                 }
//                 else
//                 {
//                     m_target.scaleSize++;
//                 }
//
//                 m_target.scaleSize = Mathf.Clamp(m_target.scaleSize, 1,
//                     Mathf.FloorToInt(m_target.LogicData.gridSize * 0.5f));
//             }
//
//             if (Event.current != null && e.keyCode == KeyCode.C)
//             {
//                 m_target.LogicData?.ResetTex();
//             }
//         }
//     }
// }
// #endif

