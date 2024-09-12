using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Easy
{
    /// <summary>
    /// 类的时间更新，比如buff 标签 等功能，主要是不能异步
    /// </summary>
    public class ETime
    {
        /// <summary>
        /// 禁止 new
        /// </summary>
        private ETime()
        {
            disposed = false;
        }

        public int ID { get; private set; }
        private float _maxTime;
        private float _updateTime;
        private bool _enableUpdate;

        internal void Update()
        {
            if (!_enableUpdate) return;
            _updateTime += Time.deltaTime;
            if (_updateTime >= _maxTime)
            {
                _enableUpdate = false;
                delayEnd?.Invoke();
                Cancle();
            }
        }

        /// <summary>
        /// 延迟时间
        /// </summary>
        public float DelayTime
        {
            get => _maxTime;
            set
            {
                _maxTime = value;
                _updateTime = 0;
                _enableUpdate = true;
            }
        }

        /// <summary>
        /// 经过的时间
        /// </summary>
        public float elapsedTime
        {
            get => _updateTime;
        }

        public Action delayEnd;

        private bool disposed;

        /// <summary>
        /// 取消延迟
        /// </summary>
        public ETime Cancle()
        {
            disposed = true;
            _maxTime = 0;
            _enableUpdate = false;
            delayEnd = null;
            _updateTime = 0;
            ID = 0;
            return null;
        }

        private static List<ETime> timeMap = new();

        internal static void Refesh()
        {
            for (int n = timeMap.Count - 1; n >= 0; n--)
            {
                var time = timeMap[n];
                if (time.disposed == false)
                {
                    time.Update();
                }
            }
        }

        /// <summary>
        /// 对象ID 每次自增加 1
        /// </summary>
        private static int _instanceID = 1;

        /// <summary>
        /// 可以保留ID，用于中途停止
        /// </summary>
        /// <returns></returns>
        public static ETime Get()
        {
            foreach (var time in timeMap)
            {
                if (time.disposed)
                {
                    time.Cancle();
                    time.disposed = false;
                    time.ID = _instanceID++;
                    return time;
                }
            }

            ETime _time = new ETime();
            _time.ID = _instanceID++;
            timeMap.Add(_time);
            return _time;
        }

        public static void Canacle(int id)
        {
            foreach (var time in timeMap)
            {
                if (time.ID == id)
                {
                    time.Cancle();
                    return;
                }
            }
        }
    }
}