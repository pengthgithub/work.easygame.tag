using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

namespace ArchieEditor {
    class GameObjectSort {

        public static void Sort(GameObject go) {
            List<Transform> shortList = new List<Transform>( );
            int childCount = go.transform.childCount;
            for (int i = 0; i < childCount; i++) {
                Transform child = go.transform.GetChild( i );
                shortList.Add( child );
            }
            shortList.Sort(
                delegate (Transform x, Transform y) {
                    return x.name.CompareTo( y.name );
                }
                );

            for (int i = 0; i < shortList.Count; i++) {
                Transform child = shortList[i];
                child.SetSiblingIndex( i );
            }
        }

        public static void SortScene( ) {
            List<Transform> shortList = new List<Transform>( );

            UnityEngine.SceneManagement.Scene scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene( );
            GameObject[] gos = scene.GetRootGameObjects( );
            for (int i = 0; i < gos.Length; i++) {
                GameObject go = gos[i];
                shortList.Add( go.transform );
            }
            shortList.Sort(
                delegate (Transform x, Transform y) {
                    return x.name.CompareTo( y.name );
                }
                );

            for (int i = 0; i < shortList.Count; i++) {
                Transform child = shortList[i];
                child.SetSiblingIndex( i );
            }

            //GameObject rootGo = Utils.GetExportObjRootNode( );
            //if (rootGo) {
            //    rootGo.transform.SetAsFirstSibling( );
            //}
        }
    }
}
