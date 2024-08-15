using System;
using System.Collections.Generic;
using UnityEngine;

namespace Easy
{
    public class AutoProfile : MonoBehaviour
    {
        /// <summary>
        /// 忽略需要清楚的资源
        /// </summary>
       [SerializeField] public List<string> ignore = new List<string>();
       [SerializeField] public int clearSeconds = 10;
    
       private int clearFrame = 0;
       private void Start()
       {
           clearFrame = Application.targetFrameRate * clearSeconds;
       }

       private void LateUpdate()
       {
           if (Time.frameCount % clearFrame == 0)
           {
               ESfx.AutoClear(clearFrame, ignore);
               ECharacter.AutoClear(clearFrame, ignore);
           }
       }

    }
}