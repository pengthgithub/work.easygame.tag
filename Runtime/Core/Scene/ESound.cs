using System.Collections.Generic;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

namespace Easy
{
    public class ESound : MonoBehaviour
    {
        [SerializeField] [Range(0, 1)] [Rename("背景音量")]
        private float bgmVolume = 1;

        [SerializeField] [Range(0, 1)] [Rename("音效音量")]
        private float soundVolume = 1;

        [SerializeField] [Rename("静音")] private bool mute;
        [SerializeField] private List<AudioSource> audioSources;

        public bool uiMute;

        private readonly string _AudioUrl = "AudioSource";

        /// <summary>
        ///     声音map，默认表示同一个声音只能播放一次
        /// </summary>
        private Dictionary<string, AudioSource> audioMap;

        private AudioSource bgmSource;

        private int soundCount;

        /// <summary>
        ///     声音缓存池
        /// </summary>
        private EPool<AudioSource> soundPool;

        public static ESound Instance { get; private set; }

        public float BGMVolume
        {
            get => bgmVolume;
            set
            {
                bgmVolume = value;
                if (bgmSource) bgmSource.volume = bgmVolume;
            }
        }

        public float SoundVolume
        {
            get => soundVolume;
            set => soundVolume = value;
        }

        public bool MUTE
        {
            get => mute;
            set
            {
                mute = value;
                if (value == false)
                    bgmSource.Stop();
                else
                    bgmSource.Play();
            }
        }

        private void Awake()
        {
            Instance = this;
            Init();
        }

        public void Init()
        {
            soundPool = new EPool<AudioSource>();
            bgmSource = GetSource();
            audioMap = new Dictionary<string, AudioSource>();
        }

        private AudioSource GetSource()
        {
            return soundPool.Get(_AudioUrl, () =>
            {
                var audioGO = new GameObject("audio");
                audioGO.transform.SetParent(transform, false);
                var audioSource = audioGO.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                return audioSource;
            }, Destroy);
        }

        private void ReleaseSource(AudioSource source)
        {
            soundPool.Release(_AudioUrl, source);
        }

        public void UnInit()
        {
            soundPool.Clear();
        }

        /// <summary>
        ///     播放背景音
        /// </summary>
        /// <param name="url"></param>
        /// <param name="loop"></param>
        public async Task PlayBGM(string url, bool loop)
        {
            if (mute || bgmVolume == 0) return;
            var clip = await ELoader.LoadAsset<AudioClip>(url);
            bgmSource.clip = clip;
            bgmSource.loop = loop;
            bgmSource.volume = bgmVolume;
            bgmSource.Play();
        }

        /// <summary>
        ///     播放音效
        /// </summary>
        /// <param name="url"></param>
        public async Task PlaySound(string url)
        {
            if (uiMute || mute || soundVolume == 0) return;

            var clip = await ELoader.LoadAsset<AudioClip>(url);
            if (clip)
            {
                var source = GetSource();
                source.clip = clip;
                source.volume = soundVolume;
                source.Play();
                audioMap[url] = source;

                //延迟回收
                await Task.Delay((int)(clip.length * 1000));
                ReleaseSource(source);
                audioMap.Remove(url);
            }
        }

        /// <summary>
        ///     播放音效
        /// </summary>
        /// <param name="clip"></param>
        public AudioSource PlaySound(AudioClip clip, float _volume, bool loop)
        {
            if (mute || soundVolume == 0) return null;

            if (soundCount > 10) return null;
            soundCount++;
            var source = GetSource();
            source.clip = clip;
            source.loop = loop;
            source.volume = soundVolume * _volume;
            source.Play();
            return source;
        }

        public void ReleaseSound(AudioSource source)
        {
            soundCount--;
            //if (audioMap != null && source.clip) audioMap.Remove(source.clip.name);
            ReleaseSource(source);
            source = null;
        }

        public void StopSound(string url)
        {
            if (audioMap != null)
            {
                audioMap.TryGetValue(url, out var source);
                if (source) source.Stop();
            }
        }
    }
}