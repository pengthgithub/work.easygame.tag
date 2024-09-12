using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace Easy
{
    /// <summary>
    /// 角色相关工具
    /// </summary>
    public abstract class CharacterTools
    {
        private static CharacterToolsData _toolsData;
        [MenuItem("Assets/工具/移除01")]
        public static void RemoveParticle()
        {
            var files = Directory.GetFiles("Assets/Art/skilleffect", "*.prefab", SearchOption.AllDirectories);
            foreach (var _file in files)
            {
                  var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(_file);
                  for (int i = 0; i < prefab.transform.childCount; i++)
                  {
                      var ts = prefab.transform.GetChild(i);
                      if (ts && ts.childCount != 0)
                      {
                          var ps = ts.GetComponent<ParticleSystem>();
                          if(ps) GameObject.DestroyImmediate(ps, true);
                      }
                  }
                  EditorUtility.SetDirty(prefab);
            }
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// 自动生成角色的预制件
        /// 准备工作，需要在 Character 下 创建 【np_角色名】 的文件夹。列子： np_haigui
        /// 在 【np_角色名】 的文件夹下 需要创建文件夹【角色名】。列子：haigui
        /// 然后在 【角色名】的文件夹下，把美术资源拷贝过来，其中需要：带【_skin.fbx】的蒙皮fbx文件  列子：角色名_skin.fbx
        /// 需要带有动画的fbx文件。 列子：角色名@动画名.fbx
        /// 需要角色的贴图文件。列子：角色名.tga
        /// </summary>
        [MenuItem("Assets/工具/生成角色")]
        public static void AutoGeneralCharacter()
        {
            var dataPath = "Assets/Editor/CharacterTools.asset";
            if (File.Exists(dataPath) == false) return;
            _toolsData = AssetDatabase.LoadAssetAtPath<CharacterToolsData>(dataPath);

            bool hasDir = false;
            Object[] selectedObjects = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
            foreach (Object obj in selectedObjects)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                int k = path.LastIndexOf("/") + 1;
                //1、检测文件命名
                var dirName = path.Substring(k, path.Length - k);

                if (path.Contains("/character/"))
                {
                    hasDir = true;

                    if (Directory.Exists(path) == false)
                    {
                        path = Path.GetDirectoryName(path);
                    }
                    
                    CheckAndGeneral(path, dirName);
                }
            }

            if (hasDir == false)
            {
                Debug.Log("没有可以执行的目录，需要是np_的文件夹");
            }
        }

        [MenuItem("Assets/工具/生成Spine")]
        public static void AutoGeneralSpine()
        {
            bool hasDir = false;
            Object[] selectedObjects = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
            foreach (Object obj in selectedObjects)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                if (File.Exists(path))
                {
                    path = Path.GetDirectoryName(path).Replace("\\", "/");
                }

                int k = path.LastIndexOf("/") + 1;
                //1、检测文件命名
                var dirName = path.Substring(k, path.Length - k);

                if (path.Contains("Art/Spine/") && path.Contains("sp_"))
                {
                    hasDir = true;
                    _GenerlaSpine(dirName, path);
                }
            }

            if (hasDir == false)
            {
                Debug.LogError("所选目录没有一个目录包涵 sp_ 命名，生成失败.");
            }
        }

        private static void _GenerlaSpine(string dirName, string path)
        {
            //1、检测文件夹名字是否正确
            var childDirName = dirName.Replace("sp_", "");
            var prefabPath = path + "/sp_" + childDirName + ".prefab";
            if (File.Exists(prefabPath))
            {
                Debug.LogError($"文件夹{path}下已经生成好了预制件，如果要重新生成，删除后在次点击即可.");
                return;
            }

            var childDir = $"{path}/{childDirName}";
            bool hasExit = Directory.Exists(childDir);
            if (!hasExit)
            {
                Debug.LogError("文件夹的名字错误，应该取名为：" + childDirName);
                return;
            }

            var atlasPath = childDir + "/" + childDirName + ".atlas.txt";
            var skelPath = childDir + "/" + childDirName + ".skel.txt";
            if (File.Exists(atlasPath) == false || File.Exists(skelPath) == false)
            {
                Debug.LogError($"文件夹{childDir}下需要有对应的spine文件：{childDirName}.atlas.txt 和 {childDirName}.skel.txt");
                return;
            }

            var atlasAssetPath = childDir + "/" + childDirName + "_Atlas.asset";
            var skeletonDataPath = childDir + "/" + childDirName + ".asset";
            if (File.Exists(atlasAssetPath) == false)
            {
                Debug.LogError($"需要对当前文件夹{childDir}，右键，重新导入 Reimport");
                return;
            }

            // 编译不通过 注释
            //bool save = false;
            //Spine.Unity.SkeletonDataAsset skeletonDataAsset =
            //    AssetDatabase.LoadAssetAtPath<SkeletonDataAsset>(skeletonDataPath);
            //if (skeletonDataAsset == null)
            //{
            //    save = true;
            //    skeletonDataAsset = ScriptableObject.CreateInstance<SkeletonDataAsset>();
            //}

            //var skeletonJson = AssetDatabase.LoadAssetAtPath<TextAsset>(skelPath);
            //skeletonDataAsset.skeletonJSON = skeletonJson;

            //if (skeletonDataAsset.atlasAssets == null || skeletonDataAsset.atlasAssets.Length == 0)
            //{
            //    skeletonDataAsset.atlasAssets = new AtlasAssetBase[1];
            //    var atlasAsset = AssetDatabase.LoadAssetAtPath<SpineAtlasAsset>(atlasAssetPath);
            //    skeletonDataAsset.atlasAssets[0] = atlasAsset;
            //}

            //if (save) AssetDatabase.CreateAsset(skeletonDataAsset, skeletonDataPath);
            ////创建spine的预制件
            //GameObject prefab = new GameObject();
            //GameObject center = new GameObject("center");
            //center.transform.SetParent(prefab.transform);

            //GameObject spine = new GameObject(childDirName);
            //spine.transform.SetParent(center.transform);
            //spine.transform.localEulerAngles = new Vector3(0, 180, 0);
            //var skeletonAnimation = spine.AddComponent<SkeletonAnimation>();
            //skeletonAnimation.skeletonDataAsset = skeletonDataAsset;
            //skeletonAnimation.AnimationName = "idle";
            //skeletonAnimation.loop = true;
            //PrefabUtility.SaveAsPrefabAsset(prefab, prefabPath);

            //GameObject.DestroyImmediate(prefab);
        }

        [MenuItem("Assets/工具/生成平滑法线模型")]
        public static void GeneralNormalSmoothMesh()
        {
            bool hasDir = false;
            Object[] selectedObjects = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
            foreach (Object obj in selectedObjects)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                int k = path.LastIndexOf("/") + 1;
                string dir = path.Substring(0, k);
                //1、检测文件命名
                var dirName = path.Substring(k, path.Length - k);

                if (path.ToLower().Contains(".fbx"))
                {
                    GameObject originPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (originPrefab == null)
                    {
                        Debug.LogError("资源：" + path + " 加载失败.");
                        return;
                    }

                    var meshFilter = originPrefab.GetComponentsInChildren<MeshFilter>();
                    foreach (var _filter in meshFilter)
                    {
                        var _cloneMesh = GameObject.Instantiate<Mesh>(_filter.sharedMesh);
                        var _path = dir + _filter.sharedMesh.name + "_smooth.mesh";
                        var _newMesh = MeshNormalAverage(_cloneMesh);
                        AssetDatabase.CreateAsset(_newMesh, _path);
                    }

                    var skinMeshs = originPrefab.GetComponentsInChildren<SkinnedMeshRenderer>();
                    foreach (var meshRender in skinMeshs)
                    {
                        var _cloneMesh = GameObject.Instantiate<Mesh>(meshRender.sharedMesh);
                        var _path = dir + meshRender.sharedMesh.name + "_smooth.mesh";
                        var _newMesh = MeshNormalAverage(_cloneMesh);
                        AssetDatabase.CreateAsset(_newMesh, _path);
                    }
                }
            }
        }

        /// <summary>
        /// 如果没有就创建材质,在材质上添加贴图的引用
        /// </summary>
        private static int ModifyMaterial(string path, string dirName, GameObject skin)
        {
            var childDirName = dirName;
            string childPath = path;
            var boneCount = 0;
            var renders = skin.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var render in renders)
            {
                render.gameObject.layer = LayerMask.NameToLayer(_toolsData.defaultLayer);
                Material[] mats = render.sharedMaterials;
                for (int i = 0; i < mats.Length; i++)
                {
                    Material mat = mats[i];
                    //材质路径
                    var matPath = path + "/" + dirName + ".mat";
                    if (!File.Exists(matPath))
                    {
                        Shader shader = _toolsData.defaultShaderName;
                        if (shader == null)
                        {
                            Debug.LogError($"Shader{_toolsData.defaultShaderName}已经丢失,需要咋Editor/CharacterTools重新设置值");
                        }

                        Material newMat = new Material(shader);
                        var diffuseTexture =
                            AssetDatabase.LoadAssetAtPath<Texture2D>(childPath + "/" + dirName + ".tga");
                        Debug.LogWarning(path + "/" + dirName + ".tga");
                        newMat.mainTexture = diffuseTexture;
                        AssetDatabase.CreateAsset(newMat, matPath);
                        AssetDatabase.SaveAssets();
                    }

                    mats[i] = AssetDatabase.LoadAssetAtPath<Material>(matPath);
                }

                boneCount += render.bones.Length;
                render.sharedMaterials = mats;
            }

            return boneCount;
        }

        /// <summary>
        /// 修改动画，
        /// 自动在控制器上添加动画节点，并连线
        /// </summary>
        private static AnimatorController ModifyAnimator(string path, string dirName, GameObject origin)
        {
            var clipPath = path;

            var controllerPath = $"{path}/{dirName}.controller";

            AnimatorController controller;
            if (!File.Exists(controllerPath))
            {
                controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
            }
            else
            {
               
                controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
            }
            
            ChildAnimatorState[] states = controller.layers[0].stateMachine.states;
            foreach (var state in states)
            {
                if (state.state.name == _toolsData.defaultAniName)
                {
                    controller.layers[0].stateMachine.defaultState = state.state;
                }
            }

            foreach (var state in states)
            {
                if (_toolsData.needLineAniName.Contains(state.state.name))
                {
                    AnimatorStateTransition transition = new AnimatorStateTransition();
                    transition.duration = 0.25f; // 设置过渡持续时间
                    transition.interruptionSource = TransitionInterruptionSource.Destination;
                    transition.canTransitionToSelf = true;
                    transition.destinationState = controller.layers[0].stateMachine.defaultState;
                    state.state.AddTransition(transition);
                }
            }

            // 保存控制器
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return controller;
        }

        static Dictionary<string, GameObject> locatorMap = new();

        /// <summary>
        /// 获取已经在预制件上添加的插槽
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="dirName"></param>
        private static void GetExitLocator(GameObject prefab, string dirName)
        {
            var centerLocator = prefab.GetComponent<ELocator>();
            if (!centerLocator)
            {
                centerLocator = prefab.AddComponent<ELocator>();
                centerLocator.socketList = new List<GameObject>();
            }
            prefab.RemoveMissingComponent();
            foreach (var locatorName in _toolsData.defaultLocatorName)
            {
                var hasLoc = false;
                foreach (var locat in centerLocator.socketList)
                {
                    if (locat.gameObject.name == locatorName) hasLoc = true;
                }

                if (hasLoc == false)
                {
                  var locChild =  prefab.transform.FindChildByName(locatorName);
                  if (locChild)
                  {
                      centerLocator.socketList.Add(locChild.gameObject);
                  }
                }
            }
        }

        /// <summary>
        /// 保存插槽信息
        /// </summary>
        /// <param name="node"></param>
        /// <param name="path"></param>
        /// <param name="dirName"></param>
        private static void SaveSocket(GameObject node, string path, string dirName)
        {
            bool hasError = false;
            string errorLog = "没有找到插槽\n";
            //将之前的插槽 重新赋予节点
            foreach (var ketStr in locatorMap.Keys)
            {
                var names = ketStr.Split(">");
                var parentName = names[0];
                var childName = names[1];

                var parentTs = node.transform.FindChildByName(parentName);
                if (parentTs)
                {
                    locatorMap[ketStr].transform.SetParent(parentTs, false);
                }
                else
                {
                    hasError = true;
                    errorLog += $"      {childName} 对应的骨骼名 {parentName} \n";
                    locatorMap[ketStr].transform.SetParent(node.transform);
                }
            }

            errorLog += "检查后修改骨骼名。";
            if (hasError)
            {
                Debug.LogError(errorLog);
            }
        }

        /// <summary>
        /// 生成预制件，并保留预制件上已经存在的数据
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dirName"></param>
        /// <returns></returns>
        private static GameObject ModifyPrefab(string path, string dirName)
        {
            var prefabPath = $"{path}/{dirName}.prefab";
            //=========================================================
            //2、生成预制件
            //=========================================================
            GameObject originPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            Debug.Log("加载:" + prefabPath);
            GameObject prefab = null;
            if (originPrefab)
            {
                prefab = GameObject.Instantiate(originPrefab);
                prefab.name = dirName;
            }
            else
            {
                prefab = new GameObject(dirName);
            }

            var childDirName = dirName;
            string childPath = path;

            //=========================================================
            //添加center 节点，用于控制角色绕中心旋转, 并且可以添加默认击飞动画，同时也可以制作一些别的动画
            //=========================================================
            Transform center = prefab.transform.FindChildByName("center");
            if (!center)
            {
                center = new GameObject("center").transform;
                center.SetParent(prefab.transform, false);
                center.localPosition = new Vector3(0, 0.64f, 0);
            }

            var boxCollider = center.GetComponent<BoxCollider>();
            if (!boxCollider) boxCollider = center.gameObject.AddComponent<BoxCollider>();
            boxCollider.gameObject.layer = LayerMask.NameToLayer(_toolsData.defaultLayer);
            boxCollider.isTrigger = true;
            //=========================================================
            //添加 Animator 组件
            //=========================================================
            var skinClone = prefab.transform.FindChildByName(dirName, false);
            if (skinClone) 
            {
                skinClone.SetParent(center);  
            }
            else
            {
                //删除已经添加了的 skin，然后重新绑定skin
                var skinTs = center.FindChildByName(dirName);
                if (skinTs)
                {
                    GameObject.DestroyImmediate(skinTs.gameObject);
                }

                var skin = AssetDatabase.LoadAssetAtPath<GameObject>(childPath + "/" + dirName + ".FBX");
                if (skin == null) return prefab;
                skinClone = GameObject.Instantiate(skin, center.transform, true).transform;
                skinClone.name = dirName;
            }
            
            GetExitLocator(center.gameObject, dirName); 
            var animator = skinClone.GetComponent<Animator>();
            if (animator == null)
            {
                animator = skinClone.gameObject.AddComponent<Animator>();
            }
            
            var controllerPath = $"{path}/{dirName}.controller";
            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
            ChildAnimatorState[] states = controller.layers[0].stateMachine.states;
            foreach (var state in states)
            {
                if (_toolsData.needLineAniName.Contains(state.state.name))
                {
                    AnimatorStateTransition transition = new AnimatorStateTransition();
                    transition.duration = 0.25f; // 设置过渡持续时间
                    transition.interruptionSource = TransitionInterruptionSource.Destination;
                    transition.canTransitionToSelf = true;
                    transition.destinationState = controller.layers[0].stateMachine.defaultState;
                    state.state.AddTransition(transition);
                }

                if (state.state.transitions != null)
                {
                    foreach (var st in state.state.transitions)
                    {
                        state.state.RemoveTransition(st);
                    } 
                }
            }
            animator.runtimeAnimatorController = controller;

            var animation = center.GetComponent<Animation>();
            if (!animation)
            {
                animation = center.gameObject.AddComponent<Animation>();
                foreach (var _clip in _toolsData.defaultNodeClip)
                {
                    animation.AddClip(_clip, _clip.name);
                }

                animation.clip = _toolsData.defaultNodeClip[0];
            }

            animation.playAutomatically = false;

            var eAniSta = center.GetComponent<EAnimation>();
            if (!eAniSta)
            {
                eAniSta = center.gameObject.AddComponent<EAnimation>();
                eAniSta.animation = animation;
            }

            eAniSta.enabled = true;
            eAniSta.animator = animator;

            var boneCount = ModifyMaterial(path, dirName, skinClone.gameObject);
            // var nodeCount = skin.GetComponentsInChildren<Transform>();
            // eAniSta.boneCount = boneCount;
            // eAniSta.nodeCount = nodeCount.Length;
            // Debug.LogWarning($"{dirName}角色的节点数量,{nodeCount.Length},骨骼数量{boneCount}.");
            //=========================================================
            //添加插槽
            //=========================================================

            SaveSocket(skinClone.gameObject, path, dirName);
            //GeneralNewNormalSmoothMesh(skinClone.gameObject, path, dirName);
            //=========================================================
            // 添加影子组件
            //=========================================================
            // var shadowTs = prefab.transform.FindChildByName("shadowplane");
            // if (!shadowTs)
            // {
            //     var shadowClone = GameObject.Instantiate(_toolsData.shadowPrefab);
            //     shadowClone.name = "shadowplane";
            //     shadowClone.transform.SetParent(prefab.transform, false);
            // }

            return prefab;
        }

        /// <summary>
        /// 修改预制件
        /// </summary>
        private static void GeneralPrefab(string path, string dirName)
        {
            //1、检测Prefab文件是否存在
            var prefabPath = $"{path}/{dirName}.prefab";
            GameObject prefab = ModifyPrefab(path, dirName);
            //=========================================================
            //保存预制件
            //=========================================================
            PrefabUtility.SaveAsPrefabAsset(prefab, prefabPath);
            //删除节点
            GameObject.DestroyImmediate(prefab);
            //=========================================================
            // 添加资产到 addressabel
            //=========================================================
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            AddressableAssetGroup group = settings.FindGroup("np");
            if (group == null)
            {
                group = settings.CreateGroup("np", false, false, true, null, typeof(BundledAssetGroupSchema));
            }

            var guid = AssetDatabase.GUIDFromAssetPath(prefabPath).ToString();
            AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, group, false, false);
            if (entry != null)
            {
                entry.address = dirName;
            }
        }

        /// <summary>
        /// 检测准备工作是否完毕
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dirName"></param>
        private static void CheckAndGeneral(string path, string dirName)
        {
            //1、检测文件夹名字是否正确
            GeneralPrefab(path, dirName);
        }

        /// <summary>
        /// 将模型的切线 平滑后保存，用于处理模型的描边一致
        /// </summary>
        /// <param name="root"></param>
        /// <param name="path"></param>
        /// <param name="dirName"></param>
        private static void GeneralNewNormalSmoothMesh(GameObject root, string path, string dirName)
        {
            var locator = root.transform.parent.GetComponent<ELocator>();
            var skinMesh = root.GetComponentsInChildren<SkinnedMeshRenderer>();
            var meshFilter = root.GetComponentsInChildren<MeshFilter>();
            var meshPath = path + "/" + dirName + ".mesh";
            if (File.Exists(meshPath))
            {
                File.Delete(meshPath);
            }

            foreach (var _skinMesh in skinMesh)
            {
                var _cloneMesh = GameObject.Instantiate<Mesh>(_skinMesh.sharedMesh);
                var _newMesh = MeshNormalAverage(_cloneMesh);
                var _path = path + "/" + _skinMesh.gameObject.name + "_smooth.mesh";

                AssetDatabase.CreateAsset(_newMesh, _path);
                _skinMesh.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(_path);
                _skinMesh.shadowCastingMode = ShadowCastingMode.Off;
            }

            foreach (var _filter in meshFilter)
            {
                var _cloneMesh = GameObject.Instantiate<Mesh>(_filter.sharedMesh);
                var _newMesh = MeshNormalAverage(_cloneMesh);
                var _path = path + "/" + _filter.gameObject.name + "_smooth.mesh";
                AssetDatabase.CreateAsset(_newMesh, _path);
                _filter.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(_path);
            }

            if (locator == null)
                locator.renderArrays = root.GetComponentsInChildren<Renderer>();
        }

        /// <summary>
        /// 模型切线平滑
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        private static Mesh MeshNormalAverage(Mesh mesh)
        {
            Dictionary<Vector3, List<int>> map = new();
            for (int v = 0; v < mesh.vertexCount; ++v)
            {
                if (!map.ContainsKey(mesh.vertices[v]))
                {
                    map.Add(mesh.vertices[v], new List<int>());
                }

                map[mesh.vertices[v]].Add(v);
            }

            //Vector3[] normals = mesh.normals;
            Vector4[] tangents = mesh.tangents;
            foreach (KeyValuePair<Vector3, List<int>> p in map)
            {
                Vector3 normal = Vector3.zero;
                foreach (int n in p.Value)
                {
                    normal += mesh.normals[n];
                }

                normal /= p.Value.Count;
                foreach (int n in p.Value)
                {
                    //normals[n] = normal;
                    tangents[n].Set(normal.x, normal.y, normal.z, 1);
                }
            }

            //mesh.normals = normals;
            mesh.tangents = tangents;
            return mesh;
        }
    }

    /// <summary>
    /// FBX 导入
    /// 并自动添加数据到对应的 addsable的节点上去
    /// </summary>
    public class ModifyFBXImportSettings : AssetPostprocessor
    {
        void OnPreprocessModel()
        {
            ModelImporter modelImporter = assetImporter as ModelImporter;
            if (modelImporter == null) return;
            string assetPath = modelImporter.assetPath;
            if (!assetPath.EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase)) return;

            // 检查是否是导入的FBX文件
            // skin 文件修改
            if (assetPath.Contains("_skin."))
            {
                //modelImporter.isReadable = true;
            }

            // int k = assetPath.LastIndexOf("@");
            // if (k != -1)
            // {
            //     string fileName = assetPath.Substring(k + 1, assetPath.Length - 5 - k);
            //     if (fileName == "idle")
            //     {
            //         // 设置动画的循环模式为循环
            //         //modelImporter.animationType = ModelImporterAnimationType.Generic;
            //         var clips = modelImporter.clipAnimations;
            //         foreach (ModelImporterClipAnimation clipAnimation in clips)
            //         {
            //             clipAnimation.loopTime = true; // 将动画设置为循环播放
            //         }
            //
            //         modelImporter.clipAnimations = clips;
            //     }
            // }
        }

        private void OnPostprocessTexture(Texture2D texture)
        {
            TextureImporter modelImporter = assetImporter as TextureImporter;
            if (modelImporter == null) return;
            
            //if(modelImporter.textureShape != TextureImporterShape.TextureCube) modelImporter.mipmapEnabled = false;
        }

        /// <summary>
        /// 修改文件后缀 如果不是spine，改为spine
        /// </summary>
        /// <param name="assetPath"></param>
        private void AutoGeneralSpinePrefab(string assetPath)
        {
            var importer = AssetImporter.GetAtPath(assetPath);
            if (importer == null) return;
            bool result = false;
            //1、检查文件后缀
            var atlasPath = assetPath.Replace(".skel.txt", ".atlas.txt").Replace(".skel", ".atlas.txt");
            if (File.Exists(atlasPath) == false)
            {
                var oldPath = atlasPath.Replace(".txt", "");
                File.Copy(oldPath, atlasPath);
                File.Delete(oldPath);
            }

            if (assetPath.Contains(".skel.txt") == false)
            {
                File.Copy(assetPath, assetPath + ".txt");
                File.Delete(assetPath);
            }
        }
        
    }
}