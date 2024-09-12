using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using size_t = System.UInt32;
using int32_t = System.Int32;

namespace Easy
{
    public class DataDef
    {
        public static void COORD_WORLD_TO_LOGIC(ref Vector3 vLogic, ref Vector3 v3D)
        {
            vLogic.x = v3D.x * TILE_LENGTH * TILE_COUNT_PER_METER / REVISE_VALUE;
            vLogic.y = v3D.z * TILE_LENGTH * TILE_COUNT_PER_METER / REVISE_VALUE;
            vLogic.z = v3D.y * TILE_LENGTH * TILE_COUNT_PER_METER / REVISE_VALUE + ZERO_Z_POINT_IN_GFX;
        }

        public static void COORD_LOGIC_TO_WORLD(ref Vector3 vLogic, ref Vector3 v3D)
        {
            v3D.x = vLogic.x * REVISE_VALUE / TILE_LENGTH / TILE_COUNT_PER_METER;
            v3D.z = vLogic.y * REVISE_VALUE / TILE_LENGTH / TILE_COUNT_PER_METER;
            v3D.y = (vLogic.z - ZERO_Z_POINT_IN_GFX) * REVISE_VALUE / TILE_LENGTH / TILE_COUNT_PER_METER;
        }

        public enum TILE_TYPE
        {
            ttPlain = 0,
            ttWater, // 1
            ttfly, // 2
            ttBreakable = ttfly, // 2
            ttTotal, //3
            ttWall = ttTotal, //3
            ttSwitchable, //4

            ttOutPlanin // 5, 场外
        };

        public enum LogicInfoType
        {
            litInvalid = 0,
            litNpc = 1,
            litObstacle = 2,
            litTrap = 3,
            litCanWalk = 4,
            litCanSwimming = 7,
            litCanFly = 8,
            litCanSwitch = 9,
            litOutCanWalk = 10 //场外
        };

        public enum TilePosition
        {
            tpCenter,
            tpLeft,
            tpLBottom,
            tpBottom,
            tpRBottom,
            tpRight,
            tpRTop,
            tpTop,
            tpLTop,
            tpTotal,
        };

        [StructLayout(LayoutKind.Sequential, Size = 9, Pack = 1)]
        public struct SDYNAMIC_POINT_DATA
        {
            public int32_t nX; // X坐标				// 4byte

            public int32_t nY; // Y坐标				// 4byte

            //public int32_t nZ;                         // Z坐标				// 4byte

            public byte byEnumType; //枚举类型          //  1byte
            //public byte byFlag;                         //  1byte

            //public byte byFaceDirection;            // 朝向8个方向			// 1byte

            //public unsafe fixed byte byReserved[82];             // 保留					// 82byte
        }

        [StructLayout(LayoutKind.Sequential, Size = 13, Pack = 1)]
        public struct SDYNAMIC_ITEM_DATA
        {
            public int32_t dwTemplateID; // 模板ID				// 4byte

            public int32_t nX; // X坐标				// 4byte

            public int32_t nY; // Y坐标				// 4byte

            public byte byLinkFlag; // 1byte
        }

        [StructLayout(LayoutKind.Sequential, Size = 13, Pack = 1)]
        public struct SNPC_DATA
        {
            //public unsafe fixed byte szNpcName[_UI_STRING_LEN];  // 名字					// 32byte

            public int32_t dwTemplateID; // 模板ID				// 4byte

            public int32_t nX; // X坐标				// 4byte

            public int32_t nY; // Y坐标				// 4byte

            //public int32_t nZ;                         // Z坐标				// 4byte

            public byte byFaceDirection; // 朝向8个方向			// 1byte

            //public byte byKind;                     // NPC类型				// 1byte

            //public int32_t dwScriptID;                 // 调用脚本ID			// 4byte

            //public int32_t dwReliveID;                 // 重生ID				// 4byte

            //public int32_t dwRandomID;                 // 随机分组ID，0不随机  // 4byte

            //public int32_t dwAIType;                   // AI类型				// 4byte

            //public int32_t dwThreatLinkID;             // 仇恨链接分组ID		// 4byte

            //public int32_t nPatrolPathID;              // 巡逻路线ID			// 4byte

            //public int32_t nOrderIndex;                // NPC队伍中的位置		// 4byte

            //public int32_t dwRepresentID;              // 渲染ID				// 4byte

            //public int32_t nProcessID;                 // 处理ID				// 4byte

            //public unsafe fixed byte byReserved[10];             // 保留					// 10byte
        };

        [StructLayout(LayoutKind.Explicit, Size = 4, Pack = 1)]
        public struct _STile_
        {
            [FieldOffset(0)] private byte data0;
            [FieldOffset(1)] private byte data1;
            [FieldOffset(2)] private byte data2;
            [FieldOffset(3)] private byte data3;

            public void reset()
            {
                data0 = data1 = data2 = data3 = 0;
            }


            public byte dwTileType
            {
                get { return (byte)(data0); }
                set
                {
                    data0 &= 0x00;
                    data0 |= (byte)(value & 0xff);
                }
            }

            /*
            public byte dwTileType
            {
                get { return (byte)(data0 & 0x03); }
                set { data0 &= 0xfc; data0 |= (byte)(value & 0x03); }
            }

            public byte dwGradientDirection
            {
                get { return (byte)((data0 & 0x1c) >> 2); }
                set { data0 &= 0xe3; data0 |= (byte)((value & 0x07) << 2); }
            }

            public byte dwGradientDegree
            {
                get { return (byte)((data0 & 0xe0) >> 5); }
                set { data0 &= 0x1f; data0 |= (byte)((value & 0x07) << 5); }
            }
            */
            //data1
            public byte dwBarrierDirection
            {
                get { return (byte)(data1 & 0x07); }
                set
                {
                    data1 &= 0xf8;
                    data1 |= (byte)(value & 0x07);
                }
            }

            public byte dwDynamic
            {
                get { return (byte)((data1 >> 3) & 0x01); }
                set
                {
                    data1 &= 0xf7;
                    data1 |= (byte)((value & 0x01) << 3);
                }
            }

            public byte dwScriptIndex
            {
                get { return (byte)((data1 & 0xf0) >> 4); }
                set
                {
                    data1 &= 0x0f;
                    data1 |= (byte)((value & 0x0f) << 4);
                }
            }

            public byte dwPartitionIndex
            {
                get { return (byte)(data2 & 0x07); }
                set
                {
                    data2 &= 0xf8;
                    data2 |= (byte)((value & 0x07));
                }
            }

            public byte dwBlockCharacter
            {
                get { return (byte)((data2 >> 3) & 0x01); }
                set
                {
                    data2 &= 0xf7;
                    data2 |= (byte)((value & 0x01) << 3);
                    //                  Log._Info(data2.ToString());
                }
            }

            public short dwHeight
            {
                //get { return (short)((data2 & 0x0f) << 8 + data3); }
                //set { data2 &= 0xf0; data2 |= (byte)((value >> 8) & 0x0f); data3 = (byte)(value & 0xff); }
                get { return (short)((data2 & 0xf0) >> 4 + (data3 << 4)); }
                set
                {
                    data2 &= 0x0f;
                    data2 |= (byte)(((value) & 0x0f) << 4);
                    data3 = (byte)((value & 0xff0) >> 4);
                }
            }
        }

        public struct _SRegionHeader_
        {
            public int32_t nVersion;
            public int32_t nRegionX;
            public int32_t nRegionY;
            public int32_t nRegionDataOffset;
            public int32_t nNpcOffset;
            public int32_t nNpcCount;
            public int32_t nPointOffset;
            public int32_t nPointCount;
            public int32_t nItemOffset;

            public int32_t nItemCount;
            //public unsafe fixed int32_t nReserved[30];
        }

        public static int _UI_STRING_LEN = 32;

        // 格子中的象素点精度
        public static int TILE_LENGTH_BIT_NUM = 5;
        public static int TILE_LENGTH = 1 << TILE_LENGTH_BIT_NUM;

        public static int TILE_COUNT_PER_METER = 1;

        //private const int TILE_COUNT_PER_METER = 1;
        public static int REVISE_VALUE = 1;
        public static int REGION_GRID_WIDTH_BIT_NUM = 6;

        public static int REGION_GRID_HEIGHT_BIT_NUM = 6;

        //private const int REGION_GRID_WIDTH_BIT_NUM = 5;
        //private const int REGION_GRID_HEIGHT_BIT_NUM = 5;
        //-------------------------------------------------------------
        public static int REGION_GRID_WIDTH = 1 << REGION_GRID_WIDTH_BIT_NUM;
        public static int REGION_GRID_HEIGHT = (1 << REGION_GRID_HEIGHT_BIT_NUM);

        public static int MAX_Z_POINT_BIT_NUM = 12; // 最大高度
        public static int ZERO_Z_POINT_IN_GFX = 1 << (MAX_Z_POINT_BIT_NUM - 1);

        public static int SCRIPT_COUNT_PER_REGION = 16;

        public static int SCRIPT_DATA_SIZE = sizeof(int) * SCRIPT_COUNT_PER_REGION;

        //private const int SIZE_PER_REGION = ((REGION_GRID_WIDTH * REGION_GRID_HEIGHT) * sizeof(_STile_) + SCRIPT_DATA_SIZE);
        //public unsafe static int SIZE_PER_REGION =
        //     ((REGION_GRID_WIDTH * REGION_GRID_HEIGHT) * sizeof(_STile_) + SCRIPT_DATA_SIZE);
    }
}