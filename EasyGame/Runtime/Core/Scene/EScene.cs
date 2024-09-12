using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Easy
{
    public abstract class EScene
    {
        private static SceneInstance _sceneInstance;

        [CanBeNull] private static string _lastSceneUrl;

        /// <summary>
        /// 场景加载，同一场景不重复加载
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<bool> Load(string url)
        {
            //默认加载 空场景 
            if (string.IsNullOrEmpty(url))
            {
                return await _Load("Empty");
            }

            if (_lastSceneUrl != url)
            {
                UnLoad();
                _lastSceneUrl = url;
            }

            return await _Load(url);
        }

        public static async Task<bool> _Load(string url)
        {
            AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(url);
            await handle.Task;
            _sceneInstance = handle.Result;

            var result = _sceneInstance.Scene.IsValid();
            if (result)
            {
                //初始化摄像机数据
                ECamera.Init();
                ECharacter.actorPool.Init(50);
                if(url == "s_void") ESfx.sfxPool.Init(50);
                else
                {
                    ESfx.sfxPool.Init(500);
                }
            }
            return result;
        }

        private static void UnLoad()
        {
            if (string.IsNullOrEmpty(_lastSceneUrl) && _sceneInstance.Scene.IsValid())
            {
                //清楚掉所有的缓存

                Addressables.UnloadSceneAsync(_sceneInstance, true);
            }
            
            ESfx.DestoryAll();
            ECharacter.DestoryAll();
        }
    }
}