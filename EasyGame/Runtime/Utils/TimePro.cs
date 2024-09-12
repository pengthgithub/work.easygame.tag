using System.Collections.Generic;
using UnityEngine;

namespace Easy
{
    public static class TimePro
    {
        private static Dictionary<string, float> _timeMap = new Dictionary<string, float>();

        /// <summary>
        /// 时间计算
        /// </summary>
        /// <param name="name"></param>
        public static void TimeBegin(string name)
        {
            _timeMap[name] = Time.realtimeSinceStartup;
        }

        /// <summary>
        /// 时间计算结束
        /// </summary>
        /// <param name="name"></param>
        public static void TimeEnd(string name)
        {
            _timeMap.TryGetValue(name, out float value);
            var _time = Time.realtimeSinceStartup - value;
            Debug.LogError(name + " 耗时 " + _time + "s.");
            _timeMap.Remove(name);
        }
    }
}