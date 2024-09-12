using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipboardHelper
{
    public static string clipboard
    {
        get
        {
            return GUIUtility.systemCopyBuffer;
        }

        set
        {
            GUIUtility.systemCopyBuffer = value;
        }
    }
}
