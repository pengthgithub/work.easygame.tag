using System;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

    public class ArchieConfig {
        /// <summary>
        /// 导出类型
        /// </summary>
        public enum ExportType {
            et_character,
            et_doodad,
            et_npc,
            et_scene,
            et_element,
            et_effect,
            et_logic
        }
        /// <summary>
        /// 序列需要和 ExportType 一一对应
        /// </summary>
        public string[] ExportDir = new string[] { "player", "doodad", "npc", "scene", "element", "skilleffect", "logic" };

        /// <summary>
        /// 导出时的资源类型
        /// </summary>
        public static ExportType export_type;
        /// <summary>
        /// 导出的资源路径
        /// </summary>
        public static string export_path;
        public static string correction_dir;
        public static bool bLocal;

        public static void InitData(string path) {
            if (path.Contains( "/skilleffect/" )) {
                export_path = "../Products/res/";
                export_type = ExportType.et_effect;
                correction_dir = "skilleffect";
            } else if (path.Contains( "/scene/" )) {
                if (path.Contains( "wujian/" )) {
                    export_path = "../Products/res/";
                    export_type = ExportType.et_element;
                    correction_dir = "element";
                } else {
                    export_path = "../Products/res/";
                    export_type = ExportType.et_scene;
                    correction_dir = "scene";
                }
            } else if (path.Contains( "/character/" )) {
                export_path = "../Products/res/character/";
                export_type = ExportType.et_character;
                correction_dir = "player";
            } else if (path.Contains( "/doodad/" )) {
                export_path = "../Products/res/character/";
                export_type = ExportType.et_doodad;
                correction_dir = "doodad";
            } else if (path.Contains( "/npc/" )) {
                export_path = "../Products/res/character/";
                export_type = ExportType.et_npc;
                correction_dir = "npc";
            }
            export_path = Path.GetFullPath( export_path ).Replace( "\\", "/" );
        }
        public void Init(bool _bCorr = false) {
            Scene scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene( );
            string path = scene.path;
            InitData( path );

            if (export_type != ExportType.et_scene) {
                GameObject[] gos = scene.GetRootGameObjects( );
                for (int i = 0; i < gos.Length; i++) {
                    GameObject go = gos[i];
                    Light light = go.GetComponent<Light>( );
                    if (light) {
                        go.SetActive( false );
                    }

                    Camera camera = go.GetComponent<Camera>( );
                    if (camera) {
                        go.SetActive( false );
                    }
                }
            }
        }
    }