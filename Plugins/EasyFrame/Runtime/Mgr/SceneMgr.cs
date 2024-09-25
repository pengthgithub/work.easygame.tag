using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Easy
{
    /// <summary>
    /// 场景管理器
    /// </summary>
    public abstract class SceneMgr
    {
        private static SceneInstance _sceneHandle;
        
        /// <summary>
        /// 上次加载的场景名
        /// </summary>
        [CanBeNull] private static string _lastSceneUrl;

        /// <summary>
        /// 场景加载，同一场景不重复加载
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<bool> Load(string url)
        {
            if (_lastSceneUrl == url) return false;
            
            UnLoad();
            _lastSceneUrl = url;
            
            return await _LoadScene(url);
        }
        
        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="url">场景路径</param>
        /// <returns></returns>
        private static async Task<bool> _LoadScene(string url)
        {
            AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(url);
            await handle.Task;
            _sceneHandle = handle.Result;

            var result = _sceneHandle.Scene.IsValid();
            if (result)
            {
               Represent.PoolInit(200);
            }
            return result;
        }

        private static void UnLoad()
        {
            if(string.IsNullOrEmpty(_lastSceneUrl)) return;
            
            if (_sceneHandle.Scene.IsValid())
            {
                // 清楚掉所有的缓存
                Addressables.UnloadSceneAsync(_sceneHandle, true);
            }
            
            // 清除掉所有的缓存
            Represent.PoolDispose();
        }
    }
}