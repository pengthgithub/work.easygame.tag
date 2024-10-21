
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Easy
{
    [CustomEditor(typeof(SfxControl))]
    public class SfxControlEditor : UnityEditor.Editor
    {
        private SfxControl _sfxControl;
        private void OnEnable()
        {
            _sfxControl = target as SfxControl;
   
            if (_sfxControl.animator == null)
            {
                _sfxControl.animator = _sfxControl.GetComponentInChildren<Animator>();
                EditorUtility.SetDirty(_sfxControl);
                AssetDatabase.SaveAssets();
            }
            
            Init();
        }

        
        /// <summary>
        /// 加倍
        /// </summary>
        public void Add()
        {
            var ps = _sfxControl.particleSystems;
            foreach (var ren in ps)
            {
                var main = ren.main;
                main.maxParticles += main.maxParticles;
            }
        }
        /// <summary>
        /// 减半
        /// </summary>
        public void Sub()
        {
            var ps = _sfxControl.particleSystems;
            foreach (var ren in ps)
            {
                var main = ren.main;
                if (main.maxParticles > 1)
                {
                    main.maxParticles = (int)(main.maxParticles * 0.5f);
                }
            }
        }

        public void CalSize()
        {
            var path = UnityEditor.AssetDatabase.GetAssetPath(this);
            if (!string.IsNullOrEmpty(path))
            {
                FileInfo fileInfo = new FileInfo(path);
                // 检查文件是否存在
                if (fileInfo.Exists)
                {
                    _sfxControl.fileSize = (int)(fileInfo.Length/1024); // 文件大小（以字节为单位）
                    fileInfo = null;
                }
            }
        }
        private void Init()
        {

            if (_sfxControl.transform.childCount != 0)
            {
                _sfxControl.transform.GetChild(0).hideFlags = _sfxControl.hideChild ? HideFlags.HideInHierarchy : HideFlags.None;
            }
            _sfxControl.nodeCount = _sfxControl.gameObject.GetComponentsInChildren<Transform>().Length;
            
            _sfxControl.particleCount = 0;
            _sfxControl.particleSystems.Clear();
            var ps = _sfxControl.gameObject.GetComponentsInChildren<ParticleSystem>();
            foreach (var ren in ps)
            {
                _sfxControl.particleSystems.Add(ren);
                _sfxControl. particleCount += ren.main.maxParticles;
            }
            
            _sfxControl. trailRenders.Clear();
            var renders = _sfxControl.gameObject.GetComponentsInChildren<TrailRenderer>();
            foreach (var ren in renders)
            {
                _sfxControl.trailRenders.Add(ren);
            } 
            
            _sfxControl.lineRenders.Clear();
            var lineRender = _sfxControl.gameObject.GetComponentsInChildren<LineRenderer>();
            foreach (var ren in lineRender)
            {
                _sfxControl. lineRenders.Add(ren);
            }

            _sfxControl.renderCount = 0;
            var renderArray = _sfxControl.gameObject.GetComponentsInChildren<Renderer>();
            _sfxControl.renderCount = renderArray.Length;
            CalAnimationList();
           
            CalSize();

        }

        private List<ClipData> animEditorLists = new List<ClipData>();
        private void CalAnimationList()
        {
            if (_sfxControl.animator)
            {
                // 获取 Animator Controller
                UnityEditor.Animations.AnimatorController controller = _sfxControl.animator.runtimeAnimatorController as  UnityEditor.Animations.AnimatorController;
                if (controller != null)
                {
                    foreach (var layer in controller.layers)
                    {
                        // 遍历每个状态机
                        foreach (var state in layer.stateMachine.states)
                        {
                            float len = 0;
                            AnimationClip clip = state.state.motion as AnimationClip;
                            if (clip != null) len = clip.length;
                            
                            ClipData data = new ClipData();
                            data.name = state.state.name;
                            data.length = len;
                            data.clip = clip;
                            animEditorLists.Add(data);
                        }
                    }
                }

                // 添加 a 中的项到 b 中（如果 b 中没有相应的项）
                foreach (var clipA in animEditorLists)
                {
                    if (! _sfxControl.animLists.Exists(clipB => clipB.name == clipA.name))
                    {
                        _sfxControl.animLists.Add(clipA);
                    }
                }
                // 删除 b 中不在 a 中的项
                _sfxControl.animLists.RemoveAll(clipB => !animEditorLists.Exists(clipA => clipA.name == clipB.name));
            }
            else
            {
                _sfxControl.animLists.Clear();
            }
        }
        
        private void OnDisable()
        {
            
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.BeginHorizontal();
            // if (GUILayout.Button("粒子数加倍"))
            // {
            //     _sfxControl.Add();
            // }
            // if (GUILayout.Button("粒子数减半"))
            // {
            //     _sfxControl.Sub(); 
            // }
            EditorGUILayout.EndHorizontal();
            
            
            if (GUILayout.Button("播放"))
            {
                _sfxControl.Play("");
            }
        }
    }
}
