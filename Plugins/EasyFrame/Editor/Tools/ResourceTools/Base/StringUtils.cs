// Decompiled with JetBrains decompiler
// Type: <PrivateImplementationDetails>{3C81E251-B09F-4EB2-8F7E-B65846A22096}.11B4BE4C-AAEE-47CE-A7D2-B8942CDCF11D
// Assembly: wx-editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 491FB96F-C7AE-469B-A755-45BC3F066C9B
// Assembly location: C:\Users\pengt\Documents\SVN\Luck\Client\Lucky\Packages\com.qq.weixin.minigame@981890fdaa\Editor\wx-editor.dll
// XML documentation location: C:\Users\pengt\Documents\SVN\Luck\Client\Lucky\Packages\com.qq.weixin.minigame@981890fdaa\Editor\wx-editor.xml

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

#nullable disable
namespace Analysis
{
    [StructLayout(LayoutKind.Auto, CharSet = CharSet.Auto)]
    internal class StringUtils
    {
        internal const string texture = "Texture";
        internal const string font = "Font";
        internal const string audio = "Audio";
        internal const string prefab = "Prefab";
        internal const string sfx = "SFX";
        internal const string material = "Material";
        internal const string titile = "资源优化工具";

        internal const string Fk = "Assets/";
        internal const string Hk = "";
        internal const string HL = "";
        internal const string Ho = "检查依赖";
        internal const string HP = "Empty";
        internal const string HQ = "";
        internal const string Hs = "";
        internal const string HS = "";
        internal const string HV = "";
        internal const string HT = "";
        internal const string Ht = "";
        internal const string HU = "";
        internal const string Hu = "";
        internal const string HX = "";
        internal const string Hx = "";
        internal const string HY = "";
        internal const string Hy = "";
        internal const string HZ = "";
        internal const string Hz = "";
        internal const string hA = "";
        internal const string Hl = "名字";
        internal const string HM = "路径";
        internal const string Hm = "";
        internal const string searchAudio = "t:AudioClip";
        internal const string Hn = "";
        internal const string HO = "进度 ";
        internal const string Hq = "";
        internal const string searchFont = "t:Font";
        internal const string Hr = "";
        internal const string SearchTxt = "搜索当前目录";
        internal const string HW = "t:Prefab";
        internal const string searchMaterial = "t:Material";
        internal const string Hw = "搜索";
        internal const string hr = "all";
        internal const string DJ = "true";
        internal const string eE = "false";
        internal const string searchSfx = "t:SfxParticle";
        internal const string searchTitle = "查询中";

        internal const string searchRole = "搜索规则";
        internal const string checkLife = "生命周期";
        internal const string checkReference = "引用为空";
        internal const string hasStr = "包含";
        internal const string unHasStr = "不包含";
        internal const string checkPrefab = "带预制件";
        internal const string checkOwner = "带拥有者";
        internal const string checkCamera = "带摄像机";
        internal const string checkAudio = "带音效";
        internal const string missScript = "脚本丢失";
        internal const string countLabel = "总览";
        internal const string resourceLabel = "资源个数"; 
        internal const string resourceLabelMemory = "资源内存"; 
        internal const string checkSlot = "插槽为空";
        
        
        internal const string Fq = "";
        internal const string hU = "WebFormat";
        internal const string hV = "MaxTextureSize";
        internal const string hv = "Width";
        internal const string hW = "Height";
        internal const string hw = "Extension";
        internal const string hX = "Dimension";
        internal const string hx = "TextureType";
        internal const string hY = "MipmapEnable";
        internal const string hy = "Is Readable";
        internal const string hZ = "TextureFormat";
        internal const string hz = "";
        internal const string checkMipMap = "已开启MipMap";
        internal const string IB = "FormatError";
        internal const string checkMaxSize = "MaxSize大于512";    
        internal const string IC = "修复选中资源";
        internal const string Ic = "还原选中资源";
        internal const string ID = "修复规则";
        internal const string Id = "禁用 isReadable";
        internal const string IE = "禁用 MipMap";
        internal const string Ie = "优化 MaxSize";
        internal const string IF = "改变纹理压缩格式";
        internal const string If = "MaxSize";
        internal const string IG = "Format";
        internal const string Ig = "列表筛选项";
        internal const string IH = "";
        internal const string Ih = "IsReadable";
        internal const string II = "";
        internal const string Ii = "";
        internal const string IJ = "";
        internal const string Ij = "";
        internal const string searchTexture = "t:Texture";
        internal const string Ik = "";
        internal const string hu = "MemorySize";
        internal const string Bg = "";
        internal const string hf = "搜索";
        internal const string hG = "";
        internal const string hg = "";
        internal const string fS = "";
        internal const string hH = "Button";
        internal const string hh = "ButtonSelected";
        internal const string Hp = "全选";
        internal const string compare = "不检测";
        internal const string compare_0 = "等于";
        internal const string compare_1 = "大于等于";
        
        public static readonly GUIContent BK = new GUIContent(StringUtils.hf);
        public static readonly GUIContent Bk = new GUIContent(StringUtils.hG);
        public static readonly GUIContent BL = new GUIContent(StringUtils.hg);
        public static readonly GUIStyle Bl = (GUIStyle)StringUtils.fS;
        public static readonly GUIStyle BM = (GUIStyle)StringUtils.hH;
        public static readonly GUIStyle Bm = (GUIStyle)StringUtils.hh;
        
        internal const string hI = "自动减半";
        internal const string hi = "32";
        internal const string hJ = "64";
        internal const string hj = "128";
        internal const string hK = "256";
        internal const string hk = "512";
        internal const string hL = "1024";
        internal const string hl = "2048";

        internal const string hM = "Auto";
        internal const string hm = "Alpha 8";
        internal const string hN = "RGB 24 bit";
        internal const string hn = "RGBA 32 bit";
        internal const string hO = "RGB 16 bit";
        internal const string ho = "R 16 bit";
        internal const string hP = "RGB Compressed DXT1";
        internal const string hp = "RGBA Compressed DXT5";
        internal const string hQ = "RGB Crunched DXT1";
        internal const string hq = "RGBA Crunched DXT5";
        internal const string hR = "R 8";
        internal const string hS = "ASTC 8x8";
        internal const string hs = "ASTC 5x5";
        internal const string hT = "ASTC 6x6";
        internal const string ht = "ASTC 4x4";
    }
}