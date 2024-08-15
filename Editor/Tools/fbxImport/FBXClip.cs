using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

    public class FBXClip {
        /// <summary>
        /// 解析Clip
        /// </summary>
        /// <param name="path"></param>
        /// <param name="List"></param>
        public static void ParseAnimationClipTextFile(string path, ref List<ModelImporterClipAnimation> List) {
            if (string.IsNullOrEmpty( path ) || !File.Exists( path ))
                return;

            string txt = File.ReadAllText( path );
            string[] lines = txt.Split( '\r', '\n' );
            for (int i = 0; i < lines.Length; i++) {
                if (string.IsNullOrEmpty( lines[i] ) || lines[i].Contains( "#" ))
                    continue;

                string str = lines[i].Trim( ).ToLower( );
                string[] keyvalue = str.Split( ' ' );
                if (keyvalue.Length < 3)
                    continue;
                try {
                    bool loop = false;
                    ModelImporterClipAnimation clip = new ModelImporterClipAnimation( );
                    clip.name = keyvalue[0].Trim( );
                    clip.firstFrame = System.Convert.ToInt32( keyvalue[1].Trim( ), 10 );
                    clip.lastFrame = System.Convert.ToInt32( keyvalue[2].Trim( ), 10 );
                    if (keyvalue.Length > 3) {
                        if (keyvalue[3].Trim( ) == "1")
                            loop = true;
                    }
                    clip.loop = loop;
                    clip.loopTime = loop;

                    if (List.IndexOf( clip ) < 0)
                        List.Add( clip );
                } catch (System.Exception e) {
                    UnityEngine.Debug.LogError( path + " 文件配置错误." );
                }
            }
        }

    }
