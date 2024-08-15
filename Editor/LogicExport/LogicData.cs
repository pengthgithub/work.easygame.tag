// using System.Collections.Generic;
// using UnityEditor;
//
// using UnityEngine;
// using Color = UnityEngine.Color;
//
// namespace Easy
// {
// [CreateAssetMenu(fileName = "LogicData", menuName = "LogicData", order = 51)]
// public class LogicData : ScriptableObject
// {
//     [SerializeField]
//     [RangeStep(1, 128, 1)]
//     public int gridSize = 12;
//     [SerializeField]
//     [Range(0.20f, 2)]
//     public float cellSize = 0.2f;
//     private Material gridMat;
//
//     [SerializeField]
//     public Color editorColor = new Color(1,0,0,0.5f);
//     [SerializeField]
//     public Color walkAble = new Color(0, 1, 0, 0.5f);
//     [SerializeField]
//     public Color trap =  new Color(1, 1, 0, 0.5f);
//     [SerializeField]
//     public Color water = new Color(0, 0, 1, 0.5f);
//     [SerializeField]
//     public Color outWalkAble =  new Color(1, 1, 1, 0.5f);
//
//     [SerializeField]
//     public Color32[] cellColors;
//     [SerializeField]
//     public int[] cellDatas;
//
//     [SerializeField]
//     public List<LogicItem> itemList = new List<LogicItem>();
//     [SerializeField]
//     public List<LogicPoint> pointList = new List<LogicPoint>();
//     [SerializeField]
//     public List<LogicNPC> npcList = new List<LogicNPC>();
//     [SerializeField]
//     public List<LogicTrap> trapList = new List<LogicTrap>();
//     //=================================================
//     [HideInInspector]
//     public GameObject render;
//     [HideInInspector]
//     public Texture2D renderTexture;
//     private int _size;
//
//     public void OnValidate()
//     {
//         if (gridSize < cellSize)
//             gridSize = Mathf.CeilToInt(cellSize);
//         _size = RealSize();
//         var _len = _size * _size;
//         if (cellColors.Length != _len)
//         {
//             cellColors = new Color32[_len];
//             cellDatas = new int[_len];
//             for (int i = 0; i < _len; i++)
//             {
//                 cellColors[i] = editorColor;
//                 cellDatas[i] = 0;
//             }
//         }
//     }
//     public void InitGrid()
//     {
//         if (!render)        {
//             render = GameObject.CreatePrimitive(PrimitiveType.Plane);
//         }
//         if(gridMat == null)
//         {
//             //render.hideFlags = HideFlags.HideInHierarchy;
//             gridMat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
//             gridMat.name = "GridMat";
//             gridMat.SetFloat("_Surface", 1.0f);
//         }
//
//         // �����������
//         render.GetComponent<MeshRenderer>().sharedMaterial = gridMat;
//
//         OnValidate();
//        
//         renderTexture = new Texture2D(_size, _size, TextureFormat.RGBA32, false);
//         renderTexture.wrapMode = TextureWrapMode.Clamp;
//         renderTexture.filterMode = FilterMode.Point;
//         renderTexture.SetPixels32(cellColors);
//         renderTexture.Apply();
//         gridMat.SetTexture("_BaseMap", renderTexture);
//     }
//
//     /// <summary>
//     /// ����
//     /// </summary>
//     public void Update()
//     {
//         if (!render) return;
//         var s = gridSize * 0.1f;
//         render.transform.localScale = new Vector3(s, s, s);
//         
//         var p = gridSize * 0.5f;
//         var _pos = render.transform.position;
//         _pos.Set(p, _pos.y, p);
//         render.transform.position = _pos;
//     }
//
//     /// <summary>
//     /// ����
//     /// </summary>
//     public void ResetTex()
//     {
//         int _r = RealSize();
//         if (renderTexture)
//         {
//             for (int i = 0; i < _r * _r; i++)
//             {
//                 cellColors[i] = editorColor;
//                 cellDatas[i] = 0;
//             }
//             renderTexture.SetPixels32(cellColors);
//             renderTexture.Apply();
//         }
//     }
//
//     public void DrawPrefab(Vector3 hitPos, int LogicType)
//     {
//         switch (LogicType)
//         {
//             case 10:
//                 {
//                     GameObject go = new GameObject("Item");
//                     go.transform.parent = render.transform;
//                     var item = go.AddComponent<LogicItem>();
//                     itemList.Add(item);
//                 }
//              break;
//             case 11:
//                 {
//                     GameObject go = new GameObject("Point");
//                     go.transform.parent = render.transform;
//                     var item = go.AddComponent<LogicPoint>();
//                     pointList.Add(item);
//                 }
//                 break;
//             case 12:
//                 {
//                     GameObject go = new GameObject("NPC");
//                     go.transform.parent = render.transform;
//                     var item = go.AddComponent<LogicNPC>();
//                     npcList.Add(item);
//                 }
//                 break;
//             case 13:
//                 {
//                     GameObject go = new GameObject("TRAP");
//                     go.transform.parent = render.transform;
//                     var item = go.AddComponent<LogicTrap>();
//                     trapList.Add(item);
//                 }
//                 break;
//             default: break;
//         }
//     }
//     private GameObject CreatePrefab(string url)
//     {
//         return null;
//     }
//
//     /// <summary>
//     /// ����
//     /// </summary>
//     /// <param name="hit"></param>
//     public void DrawPoint(Vector3 hitPos, int LogicType, int drawScale = 1)
//     {
//         Color color = editorColor;
//         switch (LogicType)
//         {
//             case 1:
//                 color = walkAble; break;
//             case 2:
//                 color = water; break;
//             case 3:
//                 color = outWalkAble; break;
//             default: break;
//         }
//
//         Vector3 pos = hitPos;
//         int x = Mathf.FloorToInt(pos.x / cellSize);
//         int y = Mathf.FloorToInt(pos.z / cellSize);
//         for (int j = 0; j < drawScale; j++)
//         {
//             for (int i = 0; i < drawScale; i++)
//             {
//                 SetCellColor(x + i, y + j, color, LogicType);
//             }
//         }
//     }
//     /// <summary>
//     /// �ı���ɫ
//     /// </summary>
//     /// <param name="x"></param>
//     /// <param name="y"></param>
//     /// <param name="color"></param>
//     private void SetCellColor(int x, int y, Color32 color, int LogicType)
//     {
//         int index = (_size - y) * _size - (x + 1);
//
//         if (index > cellColors.Length) return;
//
//         cellColors[index] = color;
//         cellDatas[index] = LogicType;
//
//         if (renderTexture)
//         {
//             renderTexture.SetPixels32(cellColors);
//             renderTexture.Apply();
//         }
// #if UNITY_EDITOR
//         if(Time.frameCount % 120 == 0)
//         {
//             UnityEditor.AssetDatabase.SaveAssets();
//         }
//
// #endif
//     }
//
//     /// <summary>
//     /// ����ʵ�ʴ�С
//     /// </summary>
//     /// <returns></returns>
//     public int RealSize()
//     {
//         if (gridSize < cellSize)
//         {
//             gridSize = Mathf.CeilToInt(cellSize);
//         }
//         return Mathf.FloorToInt(gridSize / cellSize);
//     }
//
//     public void ExportBytes()
//     {
//         DataDefExport.INSTANCE.doExportSceneLogic(this);
//         UnityEditor.AssetDatabase.SaveAssets();
//     }
// }
//
// }

