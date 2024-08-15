using UnityEngine;

namespace Easy
{
    /// <summary>
    /// 声音标签
    /// 标签上对声音的控制
    /// </summary>
    public class SoundTag : ITag
    {
        private readonly SfxSound _soundTag;

        private AudioSource _audioSource;

        public SoundTag(SfxSound soundTag)
        {
            _soundTag = soundTag;
            LifeTime = _soundTag.lifeTime;
            BindTime = _soundTag.bindTime;
        }

        protected override void OnUpdate(float deltaTime)
        {
#if UNITY_EDITOR
            if (_audioSource)
            {
                _audioSource.pitch = Time.timeScale;
            }
#endif
        }

        protected override void OnBind()
        {
            if (_soundTag == null) return;
            
            if(Sfx.mute == true) return;
            
            AudioClip clip = _soundTag.audioClip;
            if (_soundTag.randomClips.Count != 0)
            {
                int index = Random.Range(0, _soundTag.randomClips.Count);
                clip = _soundTag.randomClips[index];
            }

            _audioSource = ESound.Instance.PlaySound(clip, _soundTag.volume, _soundTag.loop);
        }


        protected override void OnDestroy()
        {
            OnDispose();
        }

        protected override void OnDispose()
        {
            if(Sfx.mute == true) return;
            if (_audioSource)
            {
                _audioSource.Stop();
                ESound.Instance.ReleaseSound(_audioSource);
            }

            _audioSource = null;
        }
    }
}