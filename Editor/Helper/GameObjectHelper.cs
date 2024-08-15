#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

    public static class GameObjectHelper
    {
        public static GameObject ApplyRendererSetting(this GameObject go,bool castShadow, bool recevieShadow)
        {
            Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
            int i = 0;
            int len = renderers.Length;

            for (; i < len; i++)
            {
                renderers[i].receiveShadows = recevieShadow;
                renderers[i].shadowCastingMode = castShadow ? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.Off;
            }
            return go;
        }

        /// <summary>
        /// 转换材质
        /// </summary>
        public static GameObject ApplyMaterial(this GameObject go, Material mat)
        {
            if(mat != null)
            {
                Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
                int i = 0;
                int len = renderers.Length;
                for (; i < len; i++)
                {
                    renderers[i].sharedMaterial = mat;
                }
            }
            return go;
        }

        /// <summary>
        /// 获取组件 如果没有就添加一个并返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <param name="addIfNotExist"></param>
        /// <returns></returns>
        public static T GetComponentExt<T>(this GameObject go, bool addIfNotExist = true) where T : Component
        {
            T comp = go.GetComponent<T>();

            if(comp == null && addIfNotExist)
            {
                comp = go.AddComponent<T>();
            }

            return comp;
        }

        public static int compare_x_axis<T>(T x, T y) where T : Component
        {
            Vector3 positonA = x.gameObject.transform.position;
            Vector3 postionB = y.gameObject.transform.position;

            float abs = Mathf.Abs(positonA.x - postionB.x);
            if (abs < 0.5f) return 0;

            if (positonA.x < postionB.x)
                return -1;
            if (positonA.x > postionB.x)
                return 1;

            return 0;
        }

        public static int compare_y_axis<T>(T x, T y) where T : Component
        {
            Vector3 positonA = x.gameObject.transform.position;
            Vector3 postionB = y.gameObject.transform.position;

            float abs = Mathf.Abs(positonA.z - postionB.z);
            if (abs < 0.5f) return 0;

            if (positonA.z < postionB.z)
                return -1;
            if (positonA.z > postionB.z)
                return 1;

            return 0;
        }
        
    }

#endif
