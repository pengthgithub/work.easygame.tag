using System.Collections.Generic;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

namespace Easy
{
    public class SoundMgr : MonoBehaviour
    {
        public static SoundMgr Instance { get; private set; }
        //=================================================================
        [SerializeField] [Range(0, 1)] [Rename("背景音量")]
        private float bgmVolume = 1;
        [SerializeField] [Range(0, 1)] [Rename("音效音量")]
        private float soundVolume = 1;
        [SerializeField] [Rename("静音")] private bool mute;
        
        [SerializeField] private List<AudioSource> audioSources;
        [SerializeField] private AudioSource bgmSource;
        private Queue<int> _soundIndexQueue;
        private bool _mute;
        private int _soundIndex; //由于只播放10个音效，所以用队列来管理
        
        /// <summary>
        /// 背景音量
        /// </summary>
        public float BGMVolume
        {
            get => bgmVolume;
            set
            {
                bgmVolume = value;
                if (bgmSource) bgmSource.volume = bgmVolume;
            }
        }
        
        /// <summary>
        /// 音量
        /// </summary>
        public float SoundVolume
        {
            get => soundVolume;
            set => soundVolume = value;
        }
        
        /// <summary>
        /// 静音
        /// </summary>
        public bool MUTE
        {
            get => _mute;
            set
            {
                _mute = value;
                if (bgmSource)
                {
                    if (_mute) bgmSource.Stop();
                    else bgmSource.Play();
                }
            }
        }
        
        private void Awake()
        {
            Instance = this;
            
            var count = audioSources.Count;
            _soundIndexQueue = new Queue<int>(count);
            for (int i = 0; i < count; i++)
            {
                _soundIndexQueue.Enqueue(i);
            }

            _soundIndex = 0;
        }
        
        /// <summary>
        ///     播放背景音
        /// </summary>
        /// <param name="url"></param>
        /// <param name="loop"></param>
        public async Task PlayBGM(string url, bool loop)
        {
            if (mute || bgmVolume == 0) return;
            
            var clip = await LoaderMgr.LoadAsset<AudioClip>(url);
            bgmSource.clip = clip;
            bgmSource.loop = loop;
            bgmSource.volume = bgmVolume;
            bgmSource.Play();
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="volume"></param>
        /// <param name="loop"></param>
        /// <returns></returns>
        public int PlaySound(AudioClip clip, float volume, bool loop)
        {
            if (mute || soundVolume == 0 || !clip) return 0;

            if (_soundIndexQueue.Count == 0) return 0;
            _soundIndex = _soundIndexQueue.Dequeue();

            var source = audioSources[_soundIndex];
            source.clip = clip;
            source.loop = loop;
            source.volume = soundVolume * volume;
            source.Play();
                
            return _soundIndex;
        }
        
        /// <summary>
        /// 释放音效
        /// </summary>
        /// <param name="source"></param>
        public void ReleaseSound(int source)
        {
            if(source == 0) return;
            _soundIndexQueue.Enqueue(source);
        }
        
        /// <summary>
        /// 停止正在播放的音效
        /// </summary>
        public void Stop()
        {
            foreach (AudioSource audio in audioSources)
            {
                audio.Stop();
            } 
        }
    }
}