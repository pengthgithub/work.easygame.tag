using System.Collections.Generic;
using UnityEngine;

namespace Easy
{
    /// <summary>
    /// 主界面陈列柜
    /// </summary>
    public class DisplayCabinet : MonoBehaviour
    {
        /// <summary>
        /// 所有的展示
        /// </summary>
        [SerializeField] public List<GameObject> DisplayGameObjects;

        /// <summary>
        /// 单个角色位置
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector3 DisplayPosition(int index)
        {
            if (DisplayGameObjects != null && DisplayGameObjects.Count > index)
            {
                return DisplayGameObjects[index].transform.position;
            }

            return Vector3.zero;
        }
    }
}