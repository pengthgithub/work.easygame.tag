// #if UNITY_EDITOR
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using UnityEditor;
// using UnityEngine;
//
//     /// <summary>
//     /// 资源导入修正 加入需要的组件 整理需要的资源
//     /// </summary>
//     public class FBXImport : AssetPostprocessor {
//         //执行顺序 OnPreprocessModel ---> OnPostprocessModel ---> OnPostprocessAllAssets
//
//         public void OnPreprocessModel( ) {
//
//             string path = assetPath;
//             ArchieConfig.InitData( assetPath );
//             if (!path.ToLower( ).Contains( ".fbx" ))
//                 return;
//             //FBX Setting
//             ModelImporter modelImporter = assetImporter as ModelImporter;
//             if (modelImporter == null)
//                 return;
//             // Model
//             modelImporter.globalScale = 1.0f;
//             modelImporter.meshCompression = ModelImporterMeshCompression.Off;
//             modelImporter.importCameras = false;
//             modelImporter.importLights = false;
//             modelImporter.importVisibility = false;
//             modelImporter.preserveHierarchy = false;
//             modelImporter.isReadable = true;
//             modelImporter.optimizeMesh = true;
//             //Rig
//
//             if (FBXSetting.Instance.bImportWorking == false) {
//                 //modelImporter.importMaterials = false;
//                 return;
//             }
//
//             //Animation
//             FBXSetting.Instance.importAnimation = false;
//             modelImporter.animationCompression = ModelImporterAnimationCompression.Optimal;
//             if (modelImporter.importAnimation == false) {
//                 modelImporter.animationType = ModelImporterAnimationType.None;
//             } else {
//                 modelImporter.animationType = ModelImporterAnimationType.Generic;
//                 modelImporter.optimizeGameObjects = false;
//                 FBXSetting.Instance.importAnimation = true;
//                 string dir = Path.GetDirectoryName( path ).Replace( "\\", "/" );
//                 string[] files = Directory.GetFiles( dir, "*.txt", SearchOption.TopDirectoryOnly );
//                 if (files.Length != 0) {
//                     for (int i = 0; i < files.Length; i++) {
//                         string filePath = files[i];
//                         string fileName = Path.GetFileNameWithoutExtension( path );
//                         if (filePath.Contains( fileName )) {
//                             List<ModelImporterClipAnimation> list = new List<ModelImporterClipAnimation>( );
//                             FBXClip.ParseAnimationClipTextFile( filePath, ref list );
//                             if (list.Count > 0)
//                             {
//                                 modelImporter.clipAnimations = list.ToArray();
//                             }
//                                 
//                         }
//                     }
//                 }else {
//                     modelImporter.animationType = ModelImporterAnimationType.None;
//                 }
//             }
//             //Materials
//             //modelImporter.importMaterials = false;
//         }
//
//         public void OnPostprocessModel(GameObject g) {
//             if (FBXSetting.Instance.bImportWorking == false) {
//                 return;
//             }
//             //剔除资源asset里不需要的组件和节点
//             doKickUnnecessaryGameObjectOrComponent( g );
//
//             //替换材质
//             bool bScene = (ArchieConfig.export_type == ArchieConfig.ExportType.et_scene);
//             bool bTinyMesh = (ArchieConfig.export_type == ArchieConfig.ExportType.et_element);
//             string shadername = "";
//             if (bScene || bTinyMesh) {
//                 shadername = "LayaAir3D/Mesh/Unlit";
//             } else {
//                 shadername = "Fancy/Chess/CharacterShading";
//             }
//
//             Shader shader = Shader.Find( shadername );
//             doReplaceMaterials( g, shader );
//         }
//
//         static void OnPostprocessAllAssets(string[] importedAssets,
//             string[] deletedAssets,
//             string[] movedAssets,
//             string[] movedFromAssetPaths) {
//             if (FBXSetting.Instance.bImportWorking == false) {
//                 return;
//             }
//             for (int i = 0; i < importedAssets.Length; i++) {
//                 string path = importedAssets[i].ToLower( );
//
//                 if (!path.Contains( ".fbx" ))
//                     continue;
//
//                 //if (ArchieConfig.export_type == ArchieConfig.ExportType.et_effect || ArchieConfig.export_type == ArchieConfig.ExportType.et_scene)
//                 //    continue;
//
//                 GameObject assets = AssetDatabase.LoadAssetAtPath<GameObject>( path );
//                 bool hasItem = false;
//                 GameObject clone = null;
//                 UnityEngine.SceneManagement.Scene scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene( );
//                 GameObject[] gos = scene.GetRootGameObjects( );
//                 for (int j = 0; j < gos.Length; j++) {
//                     GameObject item = gos[j];
//                     if (item && item.name == assets.name) {
//                         hasItem = true;
//                         clone = item;
//                         break;
//                     }
//                 }
//
//                 UnityEngine.Object parentObject = null;
//                 if (clone != null)
//                     parentObject = PrefabUtility.GetCorrespondingObjectFromSource( clone );
//                 string _prefabPath = "";
//                 if (parentObject == null) {
//                     _prefabPath = AssetDatabase.GetAssetPath( clone );
//                 } else {
//                     _prefabPath = AssetDatabase.GetAssetPath( parentObject );
//                 }
//                 if (_prefabPath == "") {
//                     GameObject newClone = GameObject.Instantiate( assets ) as GameObject;
//                     if (clone != null)
//                     {
//                         newClone.transform.position = clone.transform.position;
//                         newClone.transform.rotation = clone.transform.rotation;
//                         newClone.transform.localScale = clone.transform.localScale;
//                         GameObject.DestroyImmediate(clone);
//                     }
//
//                     clone = newClone;
//                 }
//                 if (hasItem == false) {
//                     if(clone)
//                     {
//                         GameObject.DestroyImmediate(clone);
//                     }
//                     clone = GameObject.Instantiate( assets ) as GameObject;
//                     doAddPrefabComponentType( clone );
//                 }
//                 clone.SetActive(true);
//                 clone.name = clone.name.Replace( "(Clone)", string.Empty );
//                 if (FBXSetting.Instance.resUsage == FBXSetting.RESOURCE_USAGE.ELEMENT) {
//                     Vector3 euler = default( Vector3 );
//                     euler.x = -90;
//                     euler.y = 0;
//                     euler.z = 180;
//                     clone.transform.eulerAngles = euler;
//                 }
//
//                 string controllPath = path.ToLower( ).Replace( ".fbx", ".controller" );
//                 if (FBXSetting.Instance.importAnimation) {
//
//                     UnityEditor.Animations.AnimatorController controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath( controllPath );
//                     //----------------------------------------------------------//
//                     if (FBXSetting.Instance.resUsage == FBXSetting.RESOURCE_USAGE.AVATAR) {
//                         List<AnimationClip> clips = new List<AnimationClip>( );
//                         doGetAnimaitonClipFronAsset( assets, ref clips );
//
//                         UnityEditor.Animations.AnimatorControllerLayer layer = controller.layers[0];
//                         UnityEditor.Animations.AnimatorState defaultStat = null;
//                         foreach (AnimationClip clip in clips) {
//
//                             UnityEditor.Animations.AnimatorState stat = layer.stateMachine.AddState( clip.name );
//                             if (clip.name == "idle") {
//                                 defaultStat = stat;
//                             }
//                             stat.motion = clip;
//                         }
//                         if (defaultStat) {
//                             layer.stateMachine.defaultState = defaultStat;
//                         }
//                     }
//                     if (FBXSetting.Instance.resUsage == FBXSetting.RESOURCE_USAGE.AVATAR_WITH_LAYER) {
//                         controller.RemoveLayer( 0 );
//                         controller.AddLayer( "up" );
//                         controller.AddLayer( "down" );
//                         doBuildAnimatorController( controller, path );
//                     }
//
//                     AssetDatabase.SaveAssets( );
//
//                     Animator animator = clone.GetComponent<Animator>();
//                     if (animator)
//                     {
//                         animator.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(controllPath);
//                     }
//
//                 }
//
//                 string prefabPath = path.ToLower( ).Replace( ".fbx", ".prefab" ).Replace("\\", "/");
//                 if(prefabPath.Contains("/scene/") || prefabPath.Contains("/Scene/"))
//                 {
//                     string fileName = Path.GetFileName(prefabPath);
//                     int k = prefabPath.LastIndexOf("/");
//                     prefabPath = prefabPath.Substring(0, k) + "/wujian/";
//                     if(Directory.Exists(prefabPath) == false)
//                     {
//                         Directory.CreateDirectory(prefabPath);
//                     }
//
//                     prefabPath += fileName;
//                 }
//
//
//                 PrefabUtility.SaveAsPrefabAssetAndConnect( clone, prefabPath, InteractionMode.UserAction );
//             }
//         }
//
//         //======================================================================================================
//         //======================================================================================================
//         //======================================================================================================
//         void doKickUnnecessaryGameObjectOrComponent(GameObject g) {
//             Animation animation = g.GetComponent<Animation>( );
//             if (animation != null) {
//                 if (animation.clip == null && FBXSetting.Instance.resUsage == FBXSetting.RESOURCE_USAGE.STATIC) {
//                     Object.DestroyImmediate( animation );
//                 }
//             }
//
//             Animator animator = g.GetComponent<Animator>( );
//             if (animator != null) {
//                 if (animator.runtimeAnimatorController == null && FBXSetting.Instance.resUsage == FBXSetting.RESOURCE_USAGE.STATIC) {
//                     Object.DestroyImmediate( animator );
//                 }
//             }
//
//             MeshFilter[] mfs = g.GetComponentsInChildren<MeshFilter>( );
//             for (int i = 0; i < mfs.Length; ++i) {
//                 if (mfs[i].sharedMesh != null && mfs[i].sharedMesh.name.ToLower( ).Contains( "coll_box" )) {
//                     Object.DestroyImmediate( mfs[i].gameObject );
//                 }
//             }
//         }
//
//         void doReplaceMaterials(GameObject g, Shader shader) {
//             string path = Path.GetDirectoryName( assetPath ).Replace( "\\", "/" );
//             if (shader == null)
//                 return;
//
//             Renderer[] renderers = g.GetComponentsInChildren<Renderer>( );
//             for (int i = 0; i < renderers.Length; i++) {
//                 renderers[i].receiveShadows = false;
//                 renderers[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
//
//                 Mesh sharedMesh = null;
//                 if (renderers[i] is SkinnedMeshRenderer)
//                     sharedMesh = (renderers[i] as SkinnedMeshRenderer).sharedMesh;
//                 else if (renderers[i] is MeshRenderer) {
//                     MeshFilter mf = renderers[i].gameObject.GetComponent<MeshFilter>( );
//                     if (mf != null)
//                         sharedMesh = mf.sharedMesh;
//                 }
//                 if (sharedMesh == null || ArchieConfig.export_type == ArchieConfig.ExportType.et_effect ||
//                     ArchieConfig.export_type == ArchieConfig.ExportType.et_scene)
//                     continue;
//
//                 Material[] oldMaterials = renderers[i].sharedMaterials;
//                 Material[] destMats = new Material[sharedMesh.subMeshCount];
//                 Texture2D[] texs = new Texture2D[sharedMesh.subMeshCount];
//
//                 var texFiles = Directory.GetFiles( path, "*.*", SearchOption.TopDirectoryOnly ).Where( s => s.EndsWith( ".tga" ) || s.EndsWith( ".png" ) || s.EndsWith( ".jpg" ) );//.Where(s => s.ToLower().Contains(sharedMesh.name.ToLower()));
//
//                 string[] texAssets = texFiles.ToArray<string>( );
//                 int len = System.Math.Min( texAssets.Length, sharedMesh.subMeshCount );
//                 for (int k = 0; k < len; k++) {
//                     texs[k] = AssetDatabase.LoadAssetAtPath<Texture2D>( texAssets[k] );
//                 }
//                 for (int k = 0; k < oldMaterials.Length; k++) {
//                     string oriPath = AssetDatabase.GetAssetPath( oldMaterials[k] );
//                     bool inAssetPackage = this.assetPath == oriPath;
//                     Texture tex = texs[k];
//                     {
//                         destMats[k] = new Material( shader );
//
//                         if (tex != null) {
//                             destMats[k].name = tex.name;
//                             destMats[k].SetTexture( "_MainTex", tex );
//                         } else {
//                             destMats[k].name = Path.GetFileNameWithoutExtension( this.assetPath );
//                             if (oldMaterials.Length > 1) {
//                                 destMats[k].name += "_" + k.ToString( );
//                             }
//                         }
//                     }
//                     string matPath = path.TrimEnd( '/' ) + "/" + destMats[k].name + ".mat";
//                     Material assetMat = AssetDatabase.LoadAssetAtPath<Material>( matPath );
//                     if (assetMat == null) {
//                         Material clone = Material.Instantiate<Material>( destMats[k] );
//                         clone.name = clone.name.Replace( "(Clone)", string.Empty );
//                         AssetDatabase.CreateAsset( clone, matPath );
//                         AssetDatabase.ImportAsset( matPath );
//                         destMats[k] = AssetDatabase.LoadAssetAtPath<Material>( matPath );
//                     } else {
//                         destMats[k] = assetMat;
//                     }
//                 }
//                 renderers[i].sharedMaterials = destMats;
//             }
//         }
//
//         static void doAddPrefabComponentType(GameObject go) {
//             bool bH5 = FBXSetting.Instance.resType == FBXSetting.RESOURCE_TYPE.H5;
//             bool bAvatar = FBXSetting.Instance.resUsage == FBXSetting.RESOURCE_USAGE.AVATAR || FBXSetting.Instance.resUsage == FBXSetting.RESOURCE_USAGE.AVATAR_WITH_LAYER;
//             bool bTinyMesh = FBXSetting.Instance.resUsage == FBXSetting.RESOURCE_USAGE.ELEMENT;
//         }
//
//         static void doGetAnimaitonClipFronAsset(GameObject asset, ref List<AnimationClip> clips) {
//             if (clips == null)
//                 return;
//             clips.Clear( );
//
//             string path = AssetDatabase.GetAssetPath( asset );
//             Object[] assets = AssetDatabase.LoadAllAssetsAtPath( path );
//             foreach (Object obj in assets) {
//                 if (obj is AnimationClip) {
//                     if (obj.name.ToLower( ).Contains( "preview" ))
//                         continue;
//                     clips.AddExclusive( obj as AnimationClip );
//                 }
//             }
//         }
//
//         static void doBuildAnimatorController(UnityEditor.Animations.AnimatorController controller, string mainPath) {
//             if (controller == null)
//                 return;
//
//             string mainName = Path.GetFileNameWithoutExtension( mainPath ).ToLower( );
//             string dir = Path.GetDirectoryName( mainPath ).Replace( "\\", "/" );
//
//             string[] files = Directory.GetFiles( dir );
//
//             foreach (string file in files) {
//                 string path = file.Replace( "\\", "/" ).ToLower( );
//                 //不是max导出档案
//                 if (!path.EndsWith( ".fbx" ))
//                     continue;
//                 //不是动作文件
//                 if (!path.Contains( "@" ))
//                     continue;
//
//                 string shorname = Path.GetFileNameWithoutExtension( path );
//                 string[] mainNameAndAniName = shorname.Split( '@' );
//                 int len = mainNameAndAniName.Length;
//
//                 if (mainNameAndAniName[0] != mainName)
//                     continue;
//
//                 string aniName = mainNameAndAniName[len - 1];
//
//                 //抽取clip
//                 AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>( path );
//                 if (clip == null)
//                     continue;
//
//                 string[] aniNameAndLayer = aniName.Split( '_' );
//                 //up 0 down 1
//                 string layerName = aniNameAndLayer[aniNameAndLayer.Length - 1];
//                 int layerIndex = -1;
//                 if (layerName.Contains( "shang" ) || layerName.Contains( "s" ))
//                     layerIndex = 0;
//
//                 if (layerName.Contains( "xia" ) || layerName.Contains( "x" ))
//                     layerIndex = 1;
//
//                 if (layerIndex < 0)
//                     continue;
//                 if (layerIndex > controller.layers.Length - 1)
//                     continue;
//
//                 UnityEditor.Animations.AnimatorControllerLayer layer = controller.layers[layerIndex];
//                 layer.stateMachine.AddState( aniNameAndLayer[0] ).motion = clip;
//             }
//         }
//
//
//     }
//
// #endif

