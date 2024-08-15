using UnityEngine;

namespace Easy
{
    public static class EExten
    {
        internal static PersonBirth shiKuaiBirth;
        internal static DisplayCabinet displayCabinet;

        /// <summary>
        /// 石块出生
        /// 在组件中会有至多 18个节点，每个节点都有对应的序列
        /// </summary>
        public static void ShowPersonPos(int index)
        {
            if (shiKuaiBirth) shiKuaiBirth.ShowPersonPos(index);
        }

        /// <summary>
        /// 陈列柜 角色位置
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Vector3 DisplayPosition(int index)
        {
            if (displayCabinet) return displayCabinet.DisplayPosition(index);
            return Vector3.zero;
        }
    }
}