using UnityEngine;

namespace Easy
{
    public abstract class EUtils
    {
        private static Vector3 _screenTemp = new Vector3();

        public static int HitGameObject(float x, float y)
        {
            _screenTemp.Set(x, y, 0);
            Ray screenRay = ECamera.MainCamera.ScreenPointToRay(_screenTemp);

            // Bit shift the index of the layer (8) to get a bit mask
            int layerMask = 1 << LayerMask.NameToLayer("Default");
            //layerMask += 1 << 3; //增加层级使用 | 或者 + 都可以

            // This would cast rays only against colliders in layer 8.
            // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
            // layerMask = ~layerMask;

            // Does the ray intersect any objects excluding the player layer
            if (!Physics.Raycast(screenRay, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                return -1;
            }

            var cc = hit.collider.gameObject.GetComponentInParent<ECharacter>();
            if (cc)
            {
                return (int)cc.logicID;
            }

            return -1;
        }

        /// <summary>
        /// 根据屏幕坐标获取引擎坐标
        /// </summary>
        public static Vector3 ScreenToEngine(float screenX, float screenY)
        {
            _screenTemp.Set(screenX, screenY, 0);
            var pos = ECamera.MainCamera.ScreenToWorldPoint(_screenTemp);
            return pos;
        }

        /// <summary>
        /// 根据引擎坐标获取屏幕坐标
        /// </summary>
        public static Vector3 EngineToScreen(float x, float y, float z)
        {
            return EngineToScreen(new Vector3(x, y, z));
        }

        /// <summary>
        /// 世界坐标转换为屏幕坐标
        /// </summary>
        /// <param name="worldPos"></param>
        /// <returns></returns>
        public static Vector3 EngineToScreen(Vector3 worldPos)
        {
            return ECamera.MainCamera.WorldToScreenPoint(worldPos);
        }
    }
}