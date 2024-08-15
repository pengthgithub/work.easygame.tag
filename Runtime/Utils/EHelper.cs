using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Easy
{
    public static class EHelper
    {
        /// <summary>
        /// 获取粒子曲线上的最大值或者最小值
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="maxVale"></param>
        /// <returns></returns>
        public static float GetMinOrMaxValue(this ParticleSystem.MinMaxCurve curve, bool maxVale = true)
        {
            float _value = 0;
            if (curve.mode == ParticleSystemCurveMode.Constant ||
                curve.mode == ParticleSystemCurveMode.TwoConstants)
            {
                _value = Mathf.Max(curve.constantMin, curve.constantMax);
            }
            else
            {
                float curveMax = 0;
                var keys = curve.curveMax.keys;
                foreach (var key in keys)
                {
                    if (curveMax < key.value) curveMax = key.value * curve.curveMultiplier;
                }

                if (curve.curveMin != null)
                {
                    keys = curve.curveMin.keys;
                    foreach (var key in keys)
                    {
                        if (curveMax < key.value) curveMax = key.value * curve.curveMultiplier;
                    }
                }


                _value = curveMax;
            }

            return _value;
        }

        /// <summary>
        ///     获取节点下 粒子的最大生命周期
        ///     Duration + StartDelay + MaxLifeTime
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static float GetMaxParticleLifetime(this Component transform, out float particleLifeTime)
        {
            float startLifeTime = 0;
            float resultMaxLifeTime = 0;
            var psArray = transform.GetComponentsInChildren<ParticleSystem>();
            var hasLoop = false;
            for (int i = 0; i < psArray.Length; i++)
            {
                var ps = psArray[i];
                float maxLifeTime = 0;
                if (ps.main.loop)
                {
                    hasLoop = true;
                    break;
                }
                maxLifeTime += ps.main.startLifetime.GetMinOrMaxValue(true);
                if (startLifeTime < maxLifeTime)
                {
                    startLifeTime = maxLifeTime;
                }

                maxLifeTime += ps.main.startDelay.GetMinOrMaxValue(true);

                if (ps.emission.enabled)
                {
                    float val = ps.emission.rateOverTime.GetMinOrMaxValue(true);
                    if (val != 0)
                    {
                        maxLifeTime += ps.main.duration;
                    }
                }

                if (resultMaxLifeTime < maxLifeTime)
                {
                    resultMaxLifeTime = maxLifeTime;
                }
            }

            // 粒子消散的时间
            particleLifeTime = startLifeTime;
            if (hasLoop) return 0; 
            //最大生命周期  Duration + StartDelay + MaxLifeTime
            return resultMaxLifeTime;
        }

        public static Transform Reset(this Transform trans)
        {
            trans.localPosition = new Vector3(0, 0, 0);
            trans.localEulerAngles = new Vector3(0, 0, 0);
            trans.localScale = new Vector3(1, 1, 1);

            return trans;
        }

        public static Transform FindRoot(this Transform g)
        {
            Transform root = null;
            if (g == null) return root;
            if (g.parent == null) return g;
            return g.parent.FindRoot();
        }

        public static void RemoveMissingComponent(this GameObject gameObject)
        {
            #if UNITY_EDITOR
                        UnityEditor.GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gameObject);
            #endif
            var tsArray =  gameObject.GetComponentsInChildren<Transform>();
            foreach (var ts in tsArray)
            {
                #if UNITY_EDITOR
                UnityEditor.GameObjectUtility.RemoveMonoBehavioursWithMissingScript(ts.gameObject);
                #endif
            }
        }

        public static Transform FindChildByName(this Transform g, string name, bool includeSelf = true)
        {
            if (includeSelf)
            {
                if (g.name == name) return g;
            }
            
            Transform ret = null;
            ret = g.Find(name);
            if (ret == null)
            {
                foreach (Transform trans in g)
                {
                    ret = FindChildByName(trans, name);
                    if (ret != null) break;
                }
            }

            return ret;
        }

        public static void Clone(this Transform trans, Transform other)
        {
            if (!other || !trans) return;
            trans.localPosition = other.localPosition;
            trans.localEulerAngles = other.localEulerAngles;

            trans.position = other.position;
            trans.eulerAngles = other.eulerAngles;

            trans.localScale = other.localScale;
        }

        public static void CloneLocal(this Transform trans, Transform other)
        {
            if (!other || !trans) return;
            trans.localPosition = other.localPosition;
            trans.localEulerAngles = other.localEulerAngles;
            trans.localScale = other.localScale;
        }

        public static Transform RemoveAllChild(this Transform trans)
        {
            Transform parent = trans;
            Transform child = null;

            if (parent.childCount > 0)
                child = parent.GetChild(0);

            while (child)
            {
                Object.DestroyImmediate(child.gameObject);
                if (parent.childCount > 0)
                    child = parent.GetChild(0);
            }

            return trans;
        }

        public static Transform AlianTransform(this Transform trans)
        {
            float x, y, z;
            int ix = (int)System.Math.Floor(trans.position.x);
            int iz = (int)System.Math.Floor(trans.position.z);
            x = ix + 0.5f;
            y = 0;
            z = iz + 0.5f;

            trans.position = new Vector3(x, y, z);
            return trans;
        }

        public static Transform AlianTransformCeil(this Transform trans)
        {
            float x, y, z;
            int ix = Mathf.CeilToInt(trans.position.x);
            int iz = Mathf.CeilToInt(trans.position.z);
            x = ix;
            y = 0.05f;
            z = iz;

            trans.position = new Vector3(x, y, z);
            return trans;
        }

        public static Vector3 Parse(this Vector3 vec, string txt)
        {
            if (string.IsNullOrEmpty(txt)) return vec;

            string[] texArray = txt.Replace("(", "").Replace(")", "").Trim().Split(",");
            if (texArray.Length < 3) return vec;
            float x = float.Parse(texArray[0]);
            float y = float.Parse(texArray[1]);
            float z = float.Parse(texArray[2]);
            vec.Set(x, y, z);
            return vec;
        }

        public static bool Equal(this float val, float tag)
        {
            var result = Math.Abs(val - tag) > 0.0000001f;
            return result;
        }
    }
}