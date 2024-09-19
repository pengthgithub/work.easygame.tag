using System;
using System.Collections.Generic;
using UnityEngine;

namespace Easy
{

    [Serializable] public struct LocatorData
    {
        public LocatorType Type;
        public Transform Locator;
    }
    
    public partial class Control
    {
        [Header("插槽")]
        /// <summary>
        /// 离线插槽
        /// </summary>
        [SerializeField] public List<LocatorData> locators;
        
        public void RemoveLocator(LocatorType type)
        {
            if(locators == null || locators.Count == 0) return;
            
            for (int i = 0,n= locators.Count-1; i >= 0; i--)
            {
                if (locators[i].Type == type)
                {
                    locators.RemoveAt(i);
                    return;
                }
            }
        }
        
        /// <summary>
        /// 获取插槽
        /// </summary>
        /// <param name="type">插槽类型</param>
        /// <returns></returns>
        public Transform GetLocator(LocatorType type)
        {
            foreach (var loc in locators)
            {
                if (loc.Type == type)
                {
                    if (!loc.Locator) return transform;
                    return loc.Locator;
                }
            }

            return transform;
        }
    }
}