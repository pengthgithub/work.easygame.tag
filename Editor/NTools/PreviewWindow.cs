using UnityEngine;
using UnityEditor;

namespace Easy
{
    public class PreviewWindow
    {
        public static PreviewWindow Instance;

        private ECharacter previewCharacter;
        
        private Object unityAsset;
        private Editor assetEditor;
        private bool isAnimationClip;
        public void OnGUI()
        {
            EditorGUI.BeginChangeCheck(); //开始检查编辑器资源是否发生变动
            unityAsset = EditorGUILayout.ObjectField(unityAsset, typeof(Object), true); //显示Object
            if (EditorGUI.EndChangeCheck()) //如果发生变动（比如拖拽新的资源）
            {
                isAnimationClip = false;
                var go = unityAsset as GameObject; //如果是物体
                if (go != null)
                {
                    assetEditor = PrevieEditor.CreateEditor(go); //创建新的Editor
                    
                    assetEditor.OnInspectorGUI(); //Editor初始化
                    return;
                }
            }

            // EditorGUI.BeginChangeCheck(); //开始检查编辑器资源是否发生变动
            // if (GUILayout.Button("播放"))
            // {
            //     if (!previewCharacter)
            //     {
            //         previewCharacter = ECharacter.Create("np_wugui");
            //         assetEditor = PrevieEditor.CreateEditor(previewCharacter.gameObject); //创建新的Editor
            //         assetEditor.OnInspectorGUI(); //Editor初始化
            //     }
            //     previewCharacter.CreateSkill("sfx_common_death");
            // }

            //CheckAsset();
            ShowPreview();
        }

        private void CheckAsset()
        {
            EditorGUI.BeginChangeCheck(); //开始检查编辑器资源是否发生变动
            unityAsset = EditorGUILayout.ObjectField(unityAsset, typeof(Object), true); //显示Object
            if (EditorGUI.EndChangeCheck()) //如果发生变动（比如拖拽新的资源）
            {
                isAnimationClip = false;
                var go = unityAsset as GameObject; //如果是物体
                if (go != null)
                {
                    assetEditor = Editor.CreateEditor(go); //创建新的Editor
                    assetEditor.OnInspectorGUI(); //Editor初始化
                    return;
                }

                var clip = unityAsset as AnimationClip; //如果是动画片
                if (clip != null)
                {
                    assetEditor = Editor.CreateEditor(clip);
                    assetEditor.OnInspectorGUI();
                    isAnimationClip = true;
                    return;
                }

                var mesh = unityAsset as Mesh; //如果是网格
                if (mesh != null)
                {
                    assetEditor = Editor.CreateEditor(mesh);
                    assetEditor.OnInspectorGUI();
                    return;
                }

                var tex = unityAsset as Texture; //如果是纹理资源
                if (tex != null)
                {
                    assetEditor = Editor.CreateEditor(tex);
                    assetEditor.OnInspectorGUI();
                    return;
                }

                var mat = unityAsset as Material;
                if (mat != null)
                {
                    assetEditor = Editor.CreateEditor(mat);
                    assetEditor.OnInspectorGUI();
                    return;
                }
            }
        }

        private void ShowPreview()
        {
            if (assetEditor != null) //如果资源Editor非空
            {
                assetEditor.OnPreviewSettings(); //显示预览设置项
                assetEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(512, 512), EditorStyles.whiteLabel);
                
                // using (new EditorGUILayout.HorizontalScope()) //水平布局
                // {
                //     GUILayout.FlexibleSpace(); //填充间隔
                //     assetEditor.OnPreviewSettings(); //显示预览设置项
                // }
                //
                // if (isAnimationClip)
                // {
                //     AnimationMode.StartAnimationMode(); //为了播放正常播放预览动画而进行的设置
                //     AnimationMode.BeginSampling();
                //     assetEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(512, 512), EditorStyles.whiteLabel);
                //     AnimationMode.EndSampling();
                //     AnimationMode.StopAnimationMode();
                // }
                // else
                // {
                //     assetEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(512, 512), EditorStyles.whiteLabel);
                // }
            }
        }
        
        
    }

    public class PrevieEditor : Editor
    {
        public void OnInspectorGUI()
        {
            base.OnInspectorGUI();
           

        }
        private int sliderHash = "Slider".GetHashCode();

         private Vector2 Drag2D(Vector2 scrollPosition, Rect position)
         {
             // 每次获得独一无二的 controlID
             int controlID = GUIUtility.GetControlID(sliderHash, FocusType.Passive);
             Event current = Event.current;
             // 获取对应 controlID 的事件
             switch (current.GetTypeForControl(controlID))
             {
                 case EventType.MouseDown:
                 {
                     bool flag = position.Contains(current.mousePosition) && position.width > 50f;
                     if (flag)
                     {
                         // 鼠标摁住拖出预览窗口外，预览物体任然能够旋转
                         GUIUtility.hotControl = controlID;
                         // 采用事件
                         current.Use();
                         // 让鼠标可以拖动到屏幕外后，从另一边出来
                         EditorGUIUtility.SetWantsMouseJumping(1);
                     }

                     break;
                 }
                 case EventType.MouseUp:
                 {
                     bool flag2 = GUIUtility.hotControl == controlID;
                     if (flag2)
                     {
                         GUIUtility.hotControl = 0;
                     }

                     EditorGUIUtility.SetWantsMouseJumping(0);
                     break;
                 }
                 case EventType.MouseDrag:
                 {
                     bool flag3 = GUIUtility.hotControl == controlID;
                     if (flag3)
                     {
                         // shift 加速
                         scrollPosition -= current.delta * (float) (current.shift ? 3 : 1) /
                             Mathf.Min(position.width, position.height) * 140f;
                         // 以下两条缺少任意一个，会导致延迟更新,拖动过程中无法实时更新
                         // 直到 repaint事件触发才重新绘制
                         current.Use();
                         GUI.changed = true;
                     }

                     break;
                 }
             }

             return scrollPosition;
         }
        public override bool HasPreviewGUI()
        {
            return false;
        }
        public override GUIContent GetPreviewTitle()
        {
            return new GUIContent("特效标签预览");
        }
         
        private GameObject _lastGameObj;
        private bool _canRefreshPreviewGo = true;
        public override void OnPreviewSettings()
        {
        }
        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            InitPreview();
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            _previewRenderUtility.BeginPreview(r, background);
            Camera camera = _previewRenderUtility.camera;
            if (_previewInstance)
            {
                camera.transform.position = _previewInstance.transform.position + new Vector3(0, 5f, 3f);
                camera.transform.LookAt(_previewInstance.transform);    
            }
            camera.Render();
            _previewRenderUtility.EndAndDrawPreview(r);
        }
        private PreviewRenderUtility _previewRenderUtility;
        private GameObject _previewInstance;
        private void InitPreview()
        {
            if (_previewRenderUtility == null)
            {
                // 参数true代表绘制场景内的游戏对象
                _previewRenderUtility = new PreviewRenderUtility(true);
                // 设置摄像机的一些参数
                _previewRenderUtility.cameraFieldOfView = 30f;
            }

            if (_canRefreshPreviewGo)
            {
                _canRefreshPreviewGo = false;
                // 创建预览的游戏对象
                CreatePreviewInstances();
            }
        }

        private void DestroyPreview()
        {
            if (_previewRenderUtility != null)
            {
                // 务必要进行清理，才不会残留生成的摄像机对象等
                _previewRenderUtility.Cleanup();
                _previewRenderUtility = null;
            }
        }

        private void CreatePreviewInstances()
        {
            DestroyPreviewInstances();
        
            // 绘制预览的游戏对象
            if (_previewInstance == null)
            {
                _previewRenderUtility.AddSingleGO(_previewInstance);
            }
        }

        private void DestroyPreviewInstances()
        {
            if (_previewInstance)
            {
                DestroyImmediate(_previewInstance);
            }
            _previewInstance = null;
        }
    }
}