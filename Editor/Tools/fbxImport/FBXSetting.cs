#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class FBXSetting {
        public static FBXSetting Instance = new FBXSetting( );
        private FBXSetting( ) {
        } 

        public enum EDITOR_RESOURCE_COLOR {
            RED,
            GREEN,
            BLUE
        }

        public static Color[] editor_color_map =
        {
            Color.red,
            Color.green,
            Color.blue
        };

        public static string[] editor_material_names =
        {
            "red",
            "green",
            "blue"
        };

        public enum RESOURCE_TYPE {
            INVALID = -1,
            NATIVE,
            H5,

            //--------------------------------------------------//
            DEFAULT = H5
        }

        public enum RESOURCE_USAGE {
            INVALUE = -1,
            AVATAR,    // with animation
            AVATAR_WITH_LAYER,
            STATIC,     // no animation, used for scene mesh, sort of...
            ELEMENT,    // no animation, tiny mesh, vertex count less than 256, used for scene dynamic element
            DEFAULT = AVATAR

        }
        public RESOURCE_TYPE resType = RESOURCE_TYPE.DEFAULT;
        public RESOURCE_USAGE resUsage = RESOURCE_USAGE.DEFAULT;
        public bool bImportWorking = false;

        public bool importAnimation = false;
    }


#endif