// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// using UnityEngine.AI;
// using System.Runtime.InteropServices;
// using size_t = System.UInt32;
// using int32_t = System.Int32;
// using System;
// using System.IO;
//
// using Debug = UnityEngine.Debug;
//
// namespace Easy
// {
//     public class LogicItem : MonoBehaviour
//     {
//         public int templateID;
//     }
//
//     public class LogicPoint : MonoBehaviour
//     {
//
//     }
//
//     public class LogicNPC : MonoBehaviour
//     {
//
//     }
//
//     public class LogicTrap : MonoBehaviour
//     {
//         public string scriptname;
//     }
//
//     public class DataDefExport
//     {
//         private DataDefExport()
//         {
//
//         }
//
//         public static DataDefExport INSTANCE = new DataDefExport();
//
//         List<DataDef.SDYNAMIC_ITEM_DATA> mListDynamicItem = new List<DataDef.SDYNAMIC_ITEM_DATA>();
//         List<DataDef.SDYNAMIC_POINT_DATA> mListDynamicPoint = new List<DataDef.SDYNAMIC_POINT_DATA>();
//         List<DataDef.SNPC_DATA> mListNpc = new List<DataDef.SNPC_DATA>();
//
//         private unsafe void doGetTileByXYCoord(DataDef._STile_** pTile, byte* pbyTileHead, int nRegionX, int nRegionY,
//             int nTileX, int nTileY, int nRegionWidth, int nRegionHeight)
//         {
//             if (nRegionX >= 0 && nRegionX < nRegionWidth && nRegionY >= 0 && nRegionY < nRegionHeight)
//             {
//                 *pTile = (DataDef._STile_*)(pbyTileHead +
//                                             (nRegionY * nRegionWidth + nRegionX) * DataDef.SIZE_PER_REGION +
//                                             (nTileY * DataDef.REGION_GRID_WIDTH + nTileX) * sizeof(DataDef._STile_));
//             }
//         }
//
//         protected unsafe void doProcessRegionObstacle(LogicData logicData, ref byte* pbyTilesHead, int nReginXNum,
//             int nRegionYNum, string[] tagFilter)
//         {
//             NavMeshTriangulation tri = NavMesh.CalculateTriangulation();
//
//             float fSide = 1.0f / DataDef.TILE_COUNT_PER_METER;
//             float fHalfSide = fSide / 2;
//             float fQuarterSide = fSide / 4;
//
//             int areaIdx = 0;
//             string[] areaNames = UnityEditor.GameObjectUtility.GetNavMeshAreaNames();
//
//             for (int i = 0; i < tri.indices.Length; i += 3)
//             {
//                 int areaMask = tri.areas[areaIdx++];
//                 areaMask = UnityEditor.GameObjectUtility.GetNavMeshAreaFromName(areaNames[areaMask]);
//
//                 DataDef.LogicInfoType uLogicMask = DataDef.LogicInfoType.litCanWalk;
//
//                 if (areaMask >= 3)
//                 {
//                     uLogicMask = (DataDef.LogicInfoType)(areaMask - 3);
//                 }
//                 else if (areaMask == 0)
//                 {
//                     uLogicMask = DataDef.LogicInfoType.litCanWalk;
//                 }
//                 else
//                 {
//                     uLogicMask = DataDef.LogicInfoType.litObstacle;
//                 }
//
//                 Vector3 v1 = default(Vector3);
//                 Vector3 v2 = default(Vector3);
//                 Vector3 v3 = default(Vector3);
//
//                 v1 = tri.vertices[tri.indices[i]];
//                 v2 = tri.vertices[tri.indices[i + 1]];
//                 v3 = tri.vertices[tri.indices[i + 2]];
//
//                 float maxx = Mathf.Max(Mathf.Max(v1.x, v2.x), v3.x);
//                 float maxz = Mathf.Max(Mathf.Max(v1.z, v2.z), v3.z);
//
//                 float minx = Mathf.Min(Mathf.Min(v1.x, v2.x), v3.x);
//                 float minz = Mathf.Min(Mathf.Min(v1.z, v2.z), v3.z);
//
//                 float x, y;
//
//                 float offset = fHalfSide;
//
//                 for (y = minz - offset; y <= maxz + offset; y += fSide)
//                 {
//                     for (x = minx - offset; x <= maxx + offset; x += fSide)
//                     {
//                         Ray[] rays = new Ray[5];
//
//                         rays[0] = new Ray(new Vector3(x, 9999, y), Vector3.down);
//                         rays[1] = new Ray(new Vector3(x - fQuarterSide, 9999, y - fQuarterSide), Vector3.down);
//                         rays[2] = new Ray(new Vector3(x - fQuarterSide, 9999, y + fQuarterSide), Vector3.down);
//                         rays[3] = new Ray(new Vector3(x + fQuarterSide, 9999, y - fQuarterSide), Vector3.down);
//                         rays[4] = new Ray(new Vector3(x + fQuarterSide, 9999, y + fQuarterSide), Vector3.down);
//
//                         for (int r = 0; r < 5; r++)
//                         {
//                             RaycastHit hit = new RaycastHit();
//
//                             RaycastHit[] all = Physics.RaycastAll(rays[r]);
//                             float minDistance = float.MaxValue;
//
//                             LogicTrap tempTrap = null;
//
//                             foreach (RaycastHit v in all)
//                             {
//                                 bool processAble = true;
//                                 Transform root = v.transform.FindRoot();
//                                 foreach (string tag in tagFilter)
//                                 {
//                                     if (root.tag == tag)
//                                     {
//                                         processAble = false;
//                                         break;
//                                     }
//                                 }
//
//                                 if (!processAble) continue;
//
//                                 if (tempTrap == null)
//                                     tempTrap = root.gameObject.GetComponentInChildren<LogicTrap>();
//
//                                 if (minDistance > v.distance)
//                                 {
//                                     minDistance = v.distance;
//                                     hit = v;
//                                 }
//                             }
//
//                             //---------------------------------
//                             if (Mathf.Abs(minDistance - float.MaxValue) <= float.Epsilon)
//                                 continue;
//
//                             Vector3 v3d = hit.point;
//                             if (!GeometryHelper.PointInTriangle(v3d, v1, v2, v3))
//                             {
//                                 continue;
//                             }
//
//                             Vector3 vLogic = default(Vector3);
//                             DataDef.COORD_WORLD_TO_LOGIC(ref vLogic, ref v3d);
//
//                             int nRegionX = 0;
//                             int nRegionY = 0;
//                             int nTileX = 0;
//                             int nTileY = 0;
//
//                             DataDef._STile_* pTile = null;
//                             UInt32* pdwScripts = null;
//                             UInt32 uHashCode = 0;
//
//                             nRegionX = (int)vLogic.x / DataDef.TILE_LENGTH / DataDef.REGION_GRID_WIDTH;
//                             nRegionY = (int)vLogic.y / DataDef.TILE_LENGTH / DataDef.REGION_GRID_HEIGHT;
//                             nTileX = (int)vLogic.x / DataDef.TILE_LENGTH % DataDef.REGION_GRID_WIDTH;
//                             nTileY = (int)vLogic.y / DataDef.TILE_LENGTH % DataDef.REGION_GRID_HEIGHT;
//
//                             doGetTileByXYCoord(&pTile, pbyTilesHead, nRegionX, nRegionY, nTileX, nTileY, nReginXNum,
//                                 nRegionYNum);
//
//                             if (pTile == null)
//                             {
//                                 Debug.Log(string.Format("missing {0}:{1}::{2}:{3}", nRegionX, nRegionY, nTileX,
//                                     nTileY));
//                                 continue;
//                             }
//
//                             //vLogic.z = vLogic.z / 8;
//                             short wHeight = (short)vLogic.z;
//
//                             if (wHeight < pTile->dwHeight)
//                             {
//                                 continue;
//                             }
//
//                             pTile->dwHeight = wHeight;
//
//                             int pixelX = nRegionX * DataDef.REGION_GRID_WIDTH + nTileX;
//                             int pixelY = nRegionY * DataDef.REGION_GRID_WIDTH + nTileY;
//
//                             if (uLogicMask == DataDef.LogicInfoType.litCanWalk)
//                             {
//                                 pTile->dwTileType = (byte)DataDef.TILE_TYPE.ttPlain;
//                                 pTile->dwBlockCharacter = 0;
//                                 var _color = Color.green;
//                                 _color.a = 0.5f;
//                                 debugTex.SetPixel(pixelX, pixelY, _color);
//                             }
//                             else
//                             {
//                                 if (uLogicMask == DataDef.LogicInfoType.litCanSwimming &&
//                                     pTile->dwTileType != (byte)DataDef.TILE_TYPE.ttPlain)
//                                 {
//                                     debugTex.SetPixel(pixelX, pixelY, Color.yellow);
//                                     pTile->dwTileType = (byte)DataDef.TILE_TYPE.ttWater;
//                                     pTile->dwBlockCharacter = 1;
//                                 }
//                                 else
//                                 {
//                                     if (uLogicMask == DataDef.LogicInfoType.litCanFly &&
//                                         (pTile->dwTileType != (byte)DataDef.TILE_TYPE.ttWater &&
//                                          pTile->dwTileType != (byte)DataDef.TILE_TYPE.ttPlain))
//                                     {
//                                         pTile->dwTileType = (byte)DataDef.TILE_TYPE.ttfly;
//                                         pTile->dwBlockCharacter = 0;
//                                         debugTex.SetPixel(pixelX, pixelY, Color.blue);
//                                     }
//                                 }
//                             }
//
//                             if (uLogicMask == DataDef.LogicInfoType.litObstacle)
//                             {
//                                 debugTex.SetPixel(pixelX, pixelY, Color.red);
//                                 pTile->dwTileType = 2;
//                                 pTile->dwBlockCharacter = 1;
//                             }
//
//                             if (uLogicMask == DataDef.LogicInfoType.litOutCanWalk)
//                             {
//                                 var _color = Color.green;
//                                 _color.a = 1;
//                                 debugTex.SetPixel(pixelX, pixelY, _color);
//                                 pTile->dwTileType = (byte)DataDef.TILE_TYPE.ttOutPlanin;
//                                 pTile->dwBlockCharacter = 0;
//                             }
//
//                             if (uLogicMask == DataDef.LogicInfoType.litCanSwitch)
//                             {
//                                 debugTex.SetPixel(pixelX, pixelY, Color.gray);
//                                 pTile->dwBlockCharacter = 1;
//                                 pTile->dwTileType = (byte)DataDef.TILE_TYPE.ttSwitchable;
//                             }
//
//                             if (uLogicMask == DataDef.LogicInfoType.litTrap)
//                             {
//                                 debugTex.SetPixel(pixelX, pixelY, Color.magenta);
//                                 int iBlock = pTile->dwBlockCharacter;
//                                 pTile->dwBlockCharacter = 0;
//                                 int nScriptIndex = 0;
//                                 int nTopScriptIndex = 0;
//
//                                 pdwScripts = (UInt32*)(pbyTilesHead +
//                                                        (nRegionY * nReginXNum + nRegionX + 1) *
//                                                        DataDef.SIZE_PER_REGION -
//                                                        DataDef.SCRIPT_DATA_SIZE);
//
//                                 string scriptname = "";
//                                 if (tempTrap)
//                                 {
//                                     scriptname = tempTrap.scriptname;
//                                     scriptname = scriptname.Replace('/', '\\');
//                                     scriptname = scriptname.ToLower();
//                                 }
//
//                                 uHashCode = (uint)HashHelper.HASH_TIME_33(scriptname);
//
//                                 while (pdwScripts != null && uHashCode != 0)
//                                 {
//                                     if (*pdwScripts == uHashCode)
//                                     {
//                                         nTopScriptIndex = nScriptIndex;
//                                         break;
//                                     }
//
//                                     if (*pdwScripts == 0)
//                                     {
//                                         *pdwScripts = uHashCode;
//                                         nTopScriptIndex = nScriptIndex;
//                                         break;
//                                     }
//
//                                     nScriptIndex++;
//                                     if (nScriptIndex >= DataDef.SCRIPT_COUNT_PER_REGION)
//                                     {
//                                         break;
//                                     }
//
//                                     pdwScripts++;
//                                 }
//
//                                 pTile->dwScriptIndex = (byte)(nTopScriptIndex + 1);
//                             }
//                         }
//                     }
//                 }
//             }
//         }
//
//
//         protected unsafe void doProcessRegion(LogicData logicData, ref byte* pbyTilesHead, int nReginXNum,
//             int nRegionYNum, string[] tagFilter)
//         {
//             int w = logicData.gridSize;
//
//             Vector3 pos = new Vector3();
//
//             int _size = logicData.RealSize();
//             int _len = logicData.cellDatas.Length;
//             DataDef.LogicInfoType uLogicMask = DataDef.LogicInfoType.litCanWalk;
//             for (int i = 0; i < _len; i++)
//             {
//                 int areaMask = logicData.cellDatas[i];
//                 switch (areaMask)
//                 {
//                     case 0:
//                         uLogicMask = DataDef.LogicInfoType.litObstacle;
//                         break;
//                     case 1:
//                         uLogicMask = DataDef.LogicInfoType.litCanWalk;
//                         break;
//                     case 2:
//                         uLogicMask = DataDef.LogicInfoType.litObstacle;
//                         break;
//                     case 3:
//                         uLogicMask = DataDef.LogicInfoType.litOutCanWalk;
//                         break;
//                     case 13:
//                         uLogicMask = DataDef.LogicInfoType.litTrap;
//                         break;
//                 }
//
//                 int x = (i % _size);
//                 int y = Mathf.FloorToInt(i / _size);
//                 writeTile(x, y, uLogicMask, nReginXNum, nRegionYNum, ref pbyTilesHead, "");
//             }
//
//             //补全空白数据
//             uLogicMask = DataDef.LogicInfoType.litObstacle;
//             for (int k = _size; k < nRegionYNum * DataDef.REGION_GRID_HEIGHT; k++)
//             {
//                 for (int j = 0; j < _size; j++)
//                 {
//                     writeTile(j, k, uLogicMask, nReginXNum, nRegionYNum, ref pbyTilesHead, "");
//                 }
//             }
//
//             for (int k = 0; k < nRegionYNum * DataDef.REGION_GRID_HEIGHT; k++)
//             {
//                 for (int j = _size; j < nReginXNum * DataDef.REGION_GRID_WIDTH; j++)
//                 {
//                     writeTile(j, k, uLogicMask, nReginXNum, nRegionYNum, ref pbyTilesHead, "");
//                 }
//             }
//         }
//
//         private unsafe void writeTile(int x, int y, DataDef.LogicInfoType uLogicMask, int nReginXNum, int nRegionYNum,
//             ref byte* pbyTilesHead, string scriptname = "")
//         {
//             float fSide = 1.0f / DataDef.TILE_COUNT_PER_METER;
//             float fHalfSide = fSide / 2;
//             float fQuarterSide = fSide / 4;
//
//             int nRegionX = 0;
//             int nRegionY = 0;
//             int nTileX = x;
//             int nTileY = y;
//
//             DataDef._STile_* pTile = null;
//             UInt32* pdwScripts = null;
//             UInt32 uHashCode = 0;
//
//             nRegionX = Mathf.FloorToInt(x / DataDef.REGION_GRID_WIDTH);
//             nRegionY = Mathf.FloorToInt(y / DataDef.REGION_GRID_HEIGHT);
//
//             doGetTileByXYCoord(&pTile, pbyTilesHead, nRegionX, nRegionY, nTileX, nTileY, nReginXNum, nRegionYNum);
//
//             if (pTile == null)
//             {
//                 Debug.Log(string.Format("missing {0}:{1}::{2}:{3}", nRegionX, nRegionY, nTileX, nTileY));
//                 return;
//             }
//
//             //vLogic.z = vLogic.z / 8;
//             short wHeight = (short)2048;
//
//             if (wHeight < pTile->dwHeight)
//             {
//                 return;
//             }
//
//             pTile->dwHeight = wHeight;
//
//             int pixelX = nRegionX * DataDef.REGION_GRID_WIDTH + nTileX;
//             int pixelY = nRegionY * DataDef.REGION_GRID_WIDTH + nTileY;
//
//             if (uLogicMask == DataDef.LogicInfoType.litCanWalk)
//             {
//                 pTile->dwTileType = (byte)DataDef.TILE_TYPE.ttPlain;
//                 pTile->dwBlockCharacter = 0;
//                 var _color = Color.green;
//                 _color.a = 0.5f;
//                 debugTex.SetPixel(pixelX, pixelY, _color);
//             }
//             else
//             {
//                 if (uLogicMask == DataDef.LogicInfoType.litCanSwimming &&
//                     pTile->dwTileType != (byte)DataDef.TILE_TYPE.ttPlain)
//                 {
//                     debugTex.SetPixel(pixelX, pixelY, Color.yellow);
//                     pTile->dwTileType = (byte)DataDef.TILE_TYPE.ttWater;
//                     pTile->dwBlockCharacter = 1;
//                 }
//                 else
//                 {
//                     if (uLogicMask == DataDef.LogicInfoType.litCanFly &&
//                         (pTile->dwTileType != (byte)DataDef.TILE_TYPE.ttWater &&
//                          pTile->dwTileType != (byte)DataDef.TILE_TYPE.ttPlain))
//                     {
//                         pTile->dwTileType = (byte)DataDef.TILE_TYPE.ttfly;
//                         pTile->dwBlockCharacter = 0;
//                         debugTex.SetPixel(pixelX, pixelY, Color.blue);
//                     }
//                 }
//             }
//
//             if (uLogicMask == DataDef.LogicInfoType.litObstacle)
//             {
//                 debugTex.SetPixel(pixelX, pixelY, Color.red);
//                 pTile->dwTileType = 2;
//                 pTile->dwBlockCharacter = 1;
//             }
//
//             if (uLogicMask == DataDef.LogicInfoType.litOutCanWalk)
//             {
//                 var _color = Color.green;
//                 _color.a = 1;
//                 debugTex.SetPixel(pixelX, pixelY, _color);
//                 pTile->dwTileType = (byte)DataDef.TILE_TYPE.ttOutPlanin;
//                 pTile->dwBlockCharacter = 0;
//             }
//
//             if (uLogicMask == DataDef.LogicInfoType.litCanSwitch)
//             {
//                 debugTex.SetPixel(pixelX, pixelY, Color.gray);
//                 pTile->dwBlockCharacter = 1;
//                 pTile->dwTileType = (byte)DataDef.TILE_TYPE.ttSwitchable;
//             }
//
//             if (uLogicMask == DataDef.LogicInfoType.litTrap)
//             {
//                 debugTex.SetPixel(pixelX, pixelY, Color.magenta);
//                 int iBlock = pTile->dwBlockCharacter;
//                 pTile->dwBlockCharacter = 0;
//                 int nScriptIndex = 0;
//                 int nTopScriptIndex = 0;
//
//                 pdwScripts = (UInt32*)(pbyTilesHead + (nRegionY * nReginXNum + nRegionX + 1) * DataDef.SIZE_PER_REGION -
//                                        DataDef.SCRIPT_DATA_SIZE);
//
//                 if (scriptname != "")
//                 {
//                     scriptname = scriptname.Replace('/', '\\');
//                     scriptname = scriptname.ToLower();
//                 }
//
//                 uHashCode = (uint)HashHelper.HASH_TIME_33(scriptname);
//
//                 while (pdwScripts != null && uHashCode != 0)
//                 {
//                     if (*pdwScripts == uHashCode)
//                     {
//                         nTopScriptIndex = nScriptIndex;
//                         break;
//                     }
//
//                     if (*pdwScripts == 0)
//                     {
//                         *pdwScripts = uHashCode;
//                         nTopScriptIndex = nScriptIndex;
//                         break;
//                     }
//
//                     nScriptIndex++;
//                     if (nScriptIndex >= DataDef.SCRIPT_COUNT_PER_REGION)
//                     {
//                         break;
//                     }
//
//                     pdwScripts++;
//                 }
//
//                 pTile->dwScriptIndex = (byte)(nTopScriptIndex + 1);
//             }
//         }
//
//         private Texture2D debugTex = null;
//
//         unsafe void doBuildLogicStuff(LogicData logicData)
//         {
//             mListDynamicItem.Clear();
//             mListDynamicPoint.Clear();
//             mListNpc.Clear();
//
//             //动态NPC
//             {
//                 foreach (LogicItem item in logicData.itemList)
//                 {
//                     if (item == null) continue;
//                     var _tempId = item.templateID;
//                     if (_tempId == -1) continue;
//                     Vector3 v3d = item.transform.position;
//                     Vector3 vLogic = default(Vector3);
//                     DataDef.SDYNAMIC_ITEM_DATA data = default(DataDef.SDYNAMIC_ITEM_DATA);
//                     DataDef.COORD_WORLD_TO_LOGIC(ref vLogic, ref v3d);
//                     data.nX = (int32_t)vLogic.x;
//                     data.nY = (int32_t)vLogic.y;
//                     data.dwTemplateID = _tempId;
//                     mListDynamicItem.Add(data);
//                 }
//             }
//
//             //动态位置
//             {
//                 foreach (LogicPoint item in logicData.pointList)
//                 {
//                     if (item == null) continue;
//                     Vector3 v3d = item.transform.position;
//                     Vector3 vLogic = default(Vector3);
//                     DataDef.COORD_WORLD_TO_LOGIC(ref vLogic, ref v3d);
//                     DataDef.SDYNAMIC_POINT_DATA data = default(DataDef.SDYNAMIC_POINT_DATA);
//                     data.nX = (int32_t)vLogic.x;
//                     data.nY = (int32_t)vLogic.y;
//                     mListDynamicPoint.Add(data);
//                 }
//             }
//
//             //逻辑NPC
//             {
//                 foreach (LogicNPC item in logicData.npcList)
//                 {
//                     if (item == null) continue;
//                     Vector3 v3d = item.transform.position;
//                     Vector3 vLogic = default(Vector3);
//                     DataDef.COORD_WORLD_TO_LOGIC(ref vLogic, ref v3d);
//                     DataDef.SNPC_DATA data = default(DataDef.SNPC_DATA);
//                     data.nX = (int32_t)vLogic.x;
//                     data.nY = (int32_t)vLogic.y;
//                     mListNpc.Add(data);
//                 }
//             }
//         }
//
//         //到处场景数据
//         public void doExportSceneLogic(LogicData logicData)
//         {
//             float _s = logicData.gridSize / logicData.cellSize;
//             int nRegionXNum = Mathf.CeilToInt(_s / DataDef.REGION_GRID_WIDTH);
//             int nRegionYNum = Mathf.CeilToInt(_s / DataDef.REGION_GRID_HEIGHT);
//
//             doBuildLogicStuff(logicData);
//
//             size_t uDataLen = 0;
//             int uOneTileSize = 0;
//
//             int npcCount = mListNpc.Count;
//             int pointCount = mListDynamicPoint.Count;
//             int itemCount = mListDynamicItem.Count;
//
//             debugTex = new Texture2D(nRegionXNum * DataDef.REGION_GRID_WIDTH, nRegionYNum * DataDef.REGION_GRID_HEIGHT);
//
//             unsafe
//             {
//                 int npcSize = sizeof(DataDef.SNPC_DATA);
//                 int pointSize = sizeof(DataDef.SDYNAMIC_POINT_DATA);
//                 int itemSize = sizeof(DataDef.SDYNAMIC_ITEM_DATA);
//
//                 uOneTileSize = sizeof(DataDef._STile_);
//                 int uOneRegionSize = uOneTileSize * DataDef.REGION_GRID_WIDTH * DataDef.REGION_GRID_HEIGHT +
//                                      DataDef.SCRIPT_DATA_SIZE; //一个REGION的SIZE
//                 int uRegionsDataLen = uOneRegionSize * nRegionXNum * nRegionYNum; //场景中所有REGION的SIZE
//
//                 size_t uNpcDataLen = (size_t)npcSize * (size_t)npcCount;
//                 size_t uPointDataLen = (size_t)pointSize * (size_t)pointCount;
//                 size_t uItemDataLen = (size_t)itemSize * (size_t)itemCount;
//
//                 byte* pbyHeadPoint = null;
//                 byte* pbyTileHead = null;
//                 byte* pbyTileData = null;
//                 byte* pbyNpcData = null;
//                 DataDef._SRegionHeader_* pRegionHeader = null;
//                 byte* pbyInitTileData = null; //用作初始化TILE数据
//
//                 uDataLen += (size_t)sizeof(DataDef._SRegionHeader_); //文件头SIZE
//
//                 uDataLen += (size_t)uRegionsDataLen;
//
//                 uDataLen += uNpcDataLen;
//
//                 uDataLen += uPointDataLen;
//
//                 uDataLen += uItemDataLen;
//
//                 pbyHeadPoint = (byte*)Marshal.AllocHGlobal((int)uDataLen);
//
//                 pbyTileHead = pbyHeadPoint;
//
//                 pRegionHeader = (DataDef._SRegionHeader_*)pbyTileHead;
//                 pbyTileHead += sizeof(DataDef._SRegionHeader_);
//                 pbyTileData = pbyTileHead;
//                 pbyInitTileData = pbyTileData;
//                 pbyNpcData = pbyTileHead + uRegionsDataLen;
//
//                 pRegionHeader->nVersion = 1;
//                 pRegionHeader->nRegionX = nRegionXNum;
//                 pRegionHeader->nRegionY = nRegionYNum;
//                 pRegionHeader->nRegionDataOffset = sizeof(DataDef._SRegionHeader_);
//
//                 //初始化NPC头信息
//                 pRegionHeader->nNpcOffset = sizeof(DataDef._SRegionHeader_) + uRegionsDataLen;
//
//                 pRegionHeader->nNpcCount = npcCount;
//
//                 pRegionHeader->nPointOffset =
//                     (int32_t)(sizeof(DataDef._SRegionHeader_) + uRegionsDataLen + uNpcDataLen);
//
//                 pRegionHeader->nPointCount = pointCount;
//
//
//                 pRegionHeader->nItemOffset =
//                     (int32_t)(sizeof(DataDef._SRegionHeader_) + uRegionsDataLen + uNpcDataLen + uPointDataLen);
//
//                 pRegionHeader->nItemCount = itemCount;
//
//                 for (int nY = 0; nY < nRegionYNum; nY++)
//                 {
//                     for (int nX = 0; nX < nRegionXNum; nX++)
//                     {
//                         for (int nTileY = 0; nTileY < DataDef.REGION_GRID_HEIGHT; nTileY++)
//                         {
//                             for (int nTileX = 0; nTileX < DataDef.REGION_GRID_WIDTH; nTileX++)
//                             {
//                                 DataDef._STile_* pInitTile = (DataDef._STile_*)pbyInitTileData;
//
//                                 pInitTile->dwHeight = 2048;
//                                 pInitTile->dwTileType = (byte)DataDef.TILE_TYPE.ttTotal;
//                                 pInitTile->dwBlockCharacter = 1;
//                                 pbyInitTileData += sizeof(DataDef._STile_);
//
//                                 int pixelX = nX * DataDef.REGION_GRID_WIDTH + nTileX;
//                                 int pixelY = nY * DataDef.REGION_GRID_WIDTH + nTileY;
//
//                                 debugTex.SetPixel(pixelX, pixelY, Color.black);
//                             }
//                         }
//
//                         //Region脚本数据段
//                         pbyInitTileData += DataDef.SCRIPT_DATA_SIZE;
//                     }
//                 }
//
//                 pbyInitTileData = null;
//
//                 //do process region obstacle
//                 string[] tagFilter = { };
//                 doProcessRegion(logicData, ref pbyTileHead, nRegionXNum, nRegionYNum, tagFilter);
//                 //
//                 if (npcCount > 0)
//                 {
//                     unsafe
//                     {
//                         DataDef.SNPC_DATA[] arr = mListNpc.ToArray();
//                         for (int i = 0; i < arr.Length; i++)
//                         {
//                             DataDef.SNPC_DATA* dest = (DataDef.SNPC_DATA*)pbyNpcData;
//                             DataDef.SNPC_DATA src = arr[i];
//                             *dest = src;
//
//                             pbyNpcData += sizeof(DataDef.SNPC_DATA);
//                         }
//                     }
//                 }
//
//                 if (pointCount > 0)
//                 {
//                     unsafe
//                     {
//                         DataDef.SDYNAMIC_POINT_DATA[] arr = mListDynamicPoint.ToArray();
//                         for (int i = 0; i < arr.Length; i++)
//                         {
//                             DataDef.SDYNAMIC_POINT_DATA* dest = (DataDef.SDYNAMIC_POINT_DATA*)pbyNpcData;
//                             DataDef.SDYNAMIC_POINT_DATA src = arr[i];
//
//                             *dest = src;
//
//                             pbyNpcData += sizeof(DataDef.SDYNAMIC_POINT_DATA);
//                         }
//                     }
//                 }
//
//                 if (itemCount > 0)
//                 {
//                     unsafe
//                     {
//                         DataDef.SDYNAMIC_ITEM_DATA[] arr = mListDynamicItem.ToArray();
//                         for (int i = 0; i < arr.Length; i++)
//                         {
//                             DataDef.SDYNAMIC_ITEM_DATA* dest = (DataDef.SDYNAMIC_ITEM_DATA*)pbyNpcData;
//                             DataDef.SDYNAMIC_ITEM_DATA src = arr[i];
//
//                             *dest = src;
//
//                             pbyNpcData += sizeof(DataDef.SDYNAMIC_ITEM_DATA);
//                         }
//                     }
//                 }
//
//                 //写文件
//                 try
//                 {
//                     UnityEngine.SceneManagement.Scene scene =
//                         UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
//                     if (scene != null)
//                     {
//                         string name = scene.name;
//                         string path = $"{Application.dataPath}/Art/Scene/{name}/Logic/";
//                         if (!System.IO.Directory.Exists(path))
//                         {
//                             System.IO.Directory.CreateDirectory(path);
//                         }
//
//                         path += name + ".bytes";
//                         if (File.Exists(path))
//                             File.Delete(path);
//
//                         byte[] data = new byte[uDataLen];
//                         Marshal.Copy((IntPtr)pbyHeadPoint, data, 0, (int)uDataLen);
//                         File.WriteAllBytes(path, data);
//
//                         string destPath =
//                             string.Format(Application.dataPath + "/../../../Server/Products/SceneLogic/{0}.bytes",
//                                 name);
//                         destPath = Path.GetFullPath(destPath);
//                         if (System.IO.Directory.Exists(destPath))
//                         {
//                             if (File.Exists(destPath))
//                                 File.Delete(destPath);
//                             File.Copy(path, destPath, true);
//                         }
//
//                         debugTex.Apply();
//                         string texPath = path.Replace(".bytes", ".png");
//                         System.IO.File.WriteAllBytes(texPath, debugTex.EncodeToPNG());
//                         Texture2D.DestroyImmediate(debugTex);
//                         debugTex = null;
//                     }
//
//                 }
//                 catch (System.Exception)
//                 {
//                 }
//
//                 //清除buffer
//                 Marshal.FreeHGlobal((IntPtr)pbyHeadPoint);
//             }
//
//             UnityEditor.AssetDatabase.Refresh();
//         }
//     }
// }

