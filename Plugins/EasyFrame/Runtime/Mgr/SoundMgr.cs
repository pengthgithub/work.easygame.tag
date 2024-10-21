using System;
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

        [SerializeField] public List<AudioSource> audioSources;
        [SerializeField] public AudioSource bgmSource;

        [SerializeField] public List<int> groupCount;
        Dictionary<AudioPriorityGroup, List<int>> playingGroups;

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
            DontDestroyOnLoad(gameObject);

            playingGroups = new Dictionary<AudioPriorityGroup, List<int>>();

            Init();
        }

        public void Init()
        {
            if (audioSources == null) return;

            var count = audioSources.Count;
            _soundIndex = 0;

            for (int i = 0; i < 4; i++)
            {
                var c = groupCount[i];
                var playing = new List<int>();
                for (int j = 0; j < c; j++)
                {
                    playing.Add(_soundIndex);
                    _soundIndex++;
                }

                playingGroups[(AudioPriorityGroup)i] = playing;
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
        internal int PlaySound(SfxSound sfxSound)
        {
            if (mute || soundVolume == 0) return -1;

            playingGroups.TryGetValue(sfxSound.priorityGroup, out var list);

            if (list.Count == 0)
            {
                sfxSound.playingIndex = -1;
                return -1;
            }

            sfxSound.playingIndex = list[0];
            list.RemoveAt(0);
            var audioSource = audioSources[sfxSound.playingIndex];
            audioSource.clip = sfxSound._clip;
            audioSource.volume = sfxSound.volume * soundVolume;
            audioSource.loop = sfxSound.loop;
            audioSource.Play();
            return sfxSound.playingIndex;
        }

        /// <summary>
        /// 释放音效
        /// </summary>
        /// <param name="source"></param>
        internal void ReleaseSound(int playingIndex, AudioPriorityGroup priorityGroup)
        {
            if (playingIndex != -1)
            {
                var source = audioSources[playingIndex];
                source.loop = false;
                source.Stop();

                playingGroups.TryGetValue(priorityGroup, out var list);
                
                if(list.IndexOf(playingIndex) == -1 ) list.Add(playingIndex);
                playingGroups[priorityGroup] = list;
            }
        }

        /// <summary>
        /// 停止正在播放的音效
        /// </summary>
        internal void Stop()
        {
            foreach (AudioSource audio in audioSources)
            {
                audio.Stop();
            }
        }
    }
}