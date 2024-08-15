using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Easy
{
    public static class EditorTools
    {
        private static bool _bShader = false;
        [MenuItem("Tools/保存场景中的所有根节点预制件")]
        public static void SaveAllPrefab()
        {
             GameObject[] goes = SceneManager.GetActiveScene().GetRootGameObjects();
            int index = 0;
            foreach (GameObject go in goes)
            {
                Selection.activeGameObject = go;
                index++;
                PrefabUtility.ApplyPrefabInstance(go, InteractionMode.UserAction);
            }
        }

        [MenuItem("Tools/获取只有声音的标签")]
        public static void NextPrefab()
        {
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(SfxParticle)}");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                SfxParticle asset = AssetDatabase.LoadAssetAtPath<SfxParticle>(path);
                if (asset != null)
                {
                   var a= asset.sfxShark.Count == 0;
                    var b = asset.sfxPrefab.Count == 0;
                    var c = asset.sfxLines.Count == 0;
                    var d = asset.sfxOwner.Count == 0;
                    if(a && b && c  && d && asset.lifeTime == 0)
                    {
                        Selection.activeObject = asset;
                        // 聚焦到 Project 面板上
                        EditorGUIUtility.PingObject(asset);
                        return;
                    }

                }
            }
        }
        
        [MenuItem("Tools/获取无Collider的Mesh")]
        public static void NextColliderPrefab()
        {
            GameObject[] goes = SceneManager.GetActiveScene().GetRootGameObjects();
            int index = 0;
            foreach (GameObject go in goes)
            {  
                    var result = false;
                   var meshs = go.GetComponentsInChildren<SkinnedMeshRenderer>();
                   foreach (var mesh in meshs)
                   {
                      var boxCollider = mesh.GetComponent<BoxCollider>();
                      if (boxCollider == null)
                      {
                          result = true;
                      }
                   }

                   if (result)
                   {
                       Selection.activeObject = go;
                       return;
                   }
            }
        }
        
        [MenuItem("GameObject/场景/节点数据")]
        public static void ChildData()
        {
           var go =  Selection.activeGameObject;
           if (go)
           {
              var childCount = go.transform.childCount;
              Debug.LogWarning("ChildCount:" + childCount);
           }
        }

        
        [MenuItem("GameObject/场景/切换Shader")]
        public static void ChangeShader()
        {
            string changePath = "Universal Render Pipeline/Lit";
            if (_bShader)
            {
                changePath = "Custom/scene/hsqb";
            }

            GameObject select = Selection.activeGameObject;
            MeshRenderer[] renders = select.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer item in renders)
            {
                foreach (var mat in item.sharedMaterials)
                {
                    mat.shader = Shader.Find(changePath);
                }
            }

            _bShader = !_bShader;
        }

        [MenuItem("GameObject/场景/节点排序")]
        public static void SortChildren()
        {
            GameObject gameObject = Selection.activeGameObject;
            if (gameObject == null) return;

            List<Transform> children = new List<Transform>();
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                children.Add(gameObject.transform.GetChild(i));
            }

            children.Sort((a, b) => a.name.CompareTo(b.name));

            for (int i = 0; i < children.Count; i++)
            {
                children[i].SetSiblingIndex(i);
            }
        }

        [MenuItem("GameObject/场景/移除Miss脚本")]
        public static void RemoveMissing()
        {
            GameObject gameObject = Selection.activeGameObject;
            if (gameObject == null) return;
            gameObject.RemoveMissingComponent();
        }

        [MenuItem("Tools/获取角色信息")]
        public static void CharacterPrefab()
        {
            Dictionary<string, int> boneData = new ();
            Dictionary<string, int> vectorData = new ();
            boneData.Clear();
            vectorData.Clear();
            GameObject[] goes = SceneManager.GetActiveScene().GetRootGameObjects();
            int index = 0;
            foreach (GameObject go in goes)
            {  
                var result = false;
                var renders = go.GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (var render in renders)
                {
                    boneData[render.name] = render.bones.Length;
                    vectorData[render.name] = render.sharedMesh.vertexCount;
                }
            }
            
            var sortedDict = boneData.OrderBy(pair => pair.Value);
            string boneDataStr = "骨骼数据:\n";
            foreach (var pair in sortedDict)
            {
                boneDataStr += $"Key: {pair.Key}, Value: {pair.Value}\n";
            }
            Debug.Log(boneDataStr);
            
            sortedDict = vectorData.OrderBy(pair => pair.Value);
            string vectorDataStr = "顶点数据:\n";
            foreach (var pair in sortedDict)
            {
                vectorDataStr += $"Key: {pair.Key}, Value: {pair.Value}\n";
            }
            Debug.Log(vectorDataStr);
        }

        [MenuItem("Tools/获取特效信息")]
        public static void SfxPrefab()
        {
           //  string sfxDir = "Assets/Art/sfxTag/";
           //
           // var list = Directory.GetFiles(sfxDir, "*.asset", SearchOption.AllDirectories).ToList();
           //      
           // foreach (var s in list)
           // {
           //     var sfx = AssetDatabase.LoadAssetAtPath<SfxParticle>(s);
           //     foreach (var sfxPrefab in sfx.sfxPrefab)
           //     {
           //        var path = AssetDatabase.GetAssetPath(sfxPrefab.prefab);
           //        var targetPath = "Assets/Art/skilleffect/SfxPrefab/" + Path.GetFileName(path);
           //        AssetDatabase.MoveAsset(path, targetPath);
           //     }
           // } 
           Dictionary<string, int> vectorData = new ();
           Dictionary<string, int> nodeData = new ();
           Dictionary<string, float> particleData = new ();
           GameObject select = Selection.activeGameObject;
           if(select == null) return;
           
           var result = false;
           var psArray = select.GetComponentsInChildren<ParticleSystem>();
           int index = 0;
           foreach (var ps in psArray)
           {
               if(ps.gameObject.activeSelf == false) continue;
               nodeData.TryGetValue(ps.name, out int nodeCount);
               nodeCount++;
               nodeData[ps.name] = nodeCount;
           }

           Dictionary<string, List<ESfx>> characterSfsList = new Dictionary<string, List<ESfx>>();
           var sfxArray = select.GetComponentsInChildren<ESfx>();
           index = 0;
           foreach (var ps in sfxArray)
           {
               particleData[ps.URL + index] = ps.updatedTime;
               vectorData.TryGetValue(ps.URL, out int nodeCount);
               nodeCount++;
               vectorData[ps.URL] = nodeCount;

                var _cName = "";
                if(ps.Owner) _cName = ps.Owner.URL + "-" +ps.Owner.ID;
                var resultList = characterSfsList.TryGetValue(_cName, out List<ESfx> sfxList);
                if (resultList == false)
                {
                    characterSfsList[_cName] = new List<ESfx>();
                }
                characterSfsList[_cName].Add(ps);
                index++;
           }
           
           var sortedDict = nodeData.OrderBy(pair => pair.Value);
           string nodeDataStr = "节点数据:\n";
           foreach (var pair in sortedDict)
           {
               nodeDataStr += $"Key: {pair.Key}, Value: {pair.Value}\n";
           }
           Debug.Log(nodeDataStr);
            
           var sortDict = particleData.OrderBy(pair => pair.Value);
           string vectorDataStr = "SFX更新数据:\n";
           foreach (var pair in sortDict)
           {
               vectorDataStr += $"Key: {pair.Key}, Value: {pair.Value}\n";
           }
           Debug.Log(vectorDataStr);
           
           sortedDict = vectorData.OrderBy(pair => pair.Value);
           
            vectorDataStr = "特效数据:\n";
            foreach (var pair in sortedDict)
            {
                vectorDataStr += $"Key: {pair.Key}, Value: {pair.Value}\n";
            }
            Debug.Log(vectorDataStr);

            vectorDataStr = "角色拥有的特效:\n";
            foreach (var pair in characterSfsList)
            {
                vectorDataStr += $"Key: {pair.Key}, Value: {pair.Value.Count}\n";
                foreach (var esfx in pair.Value)
                {
                    vectorDataStr += $"\t {esfx.URL}\n";
                }
            }
            Debug.Log(vectorDataStr);
        }


        [MenuItem("Tools/修改材质")]
        public static void ModifyMaterial()
        {
           var url = "Assets/Art/character";
           var mats = Directory.GetFiles(url, "*.mat");
           var shader = Shader.Find("Custom/hsqb");
           foreach (var path in mats)
           {
              var mat =  AssetDatabase.LoadAssetAtPath<Material>(path);
              if (mat.shader == shader)
              {
                  //mat.SetFloat();
                  
              }
           }

        }
#if UNITY_WEBGL
        [MenuItem("Tools/修改资源MD5")]
        public static void ModifyMD5()
        {
            var asset = "Assets\\Editor\\LoadOrder.txt";
            var orderPath = AssetDatabase.LoadAssetAtPath<TextAsset>(asset);
            
            WeChatWASM.WXEditorScriptObject config = WeChatWASM.UnityUtil.GetEditorConf();
            var dstDir = config.ProjectConf.DST;
            var files = Directory.GetFiles(dstDir, "*.bundle", SearchOption.AllDirectories);
            string preLoadPath = "";
            string[] lines = orderPath.text.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
               var _line = line.Replace("\r", "")+"_";
                for (int i = 0; i < files.Length; i++)
                {
                    var fileName = files[i];

                    if (fileName.IndexOf(_line) != -1)
                    { 
                        int k = fileName.IndexOf("\\webgl\\");
                        if (k == -1)
                        {
                            break;
                        }
                        var path = fileName.Substring(k, fileName.Length - k);
                        path = path.Replace("\\", "/");
                        preLoadPath += "'"+ path + "',\n";
                        break;
                    }
                }
            }
            
            var dir = dstDir + "\\minigame\\game.js";
            var gameJs =  File.ReadAllText(dir);
            int index = gameJs.IndexOf(" preloadDataList: [");
            var first =  gameJs.Substring(0, index);
            first += " preloadDataList: [\n" + preLoadPath;
            first += preLoadPath;
            first += "],\n";
            index = gameJs.IndexOf("contextConfig:");
            var second =  gameJs.Substring(index, gameJs.Length - index);
            first += second;
            File.WriteAllText(dir,first);
            
            EditorUtility.DisplayDialog("预下载资源","完成","确认");
        }
#endif   
    }
}