using System;
using System.Collections.Generic;
using UnityEngine;

namespace Easy
{
    [CreateAssetMenu(menuName = "特效标签", fileName = "sfx_特效名字", order = 0)]
    public class SfxParticle : ScriptableObject
    {
        [Rename("自动计算生命周期", "red")] public bool autoCalLifeTime;

        [SerializeField] [Rename("预览角色")] [DropdownAsset("Assets/Art/character", "*.prefab")]
        public string previewName = "np_shizi";

        [SerializeField]
        [Rename("预览动作", "red")]
        public string previewAniName = "at01";

        [SerializeField] [Rename("移动速度", "red")] [Range(0, 128)]
        public int moveSpeed = 0;

        [SerializeField] [Rename("生命周期，0为永久")] [Range(0, 20)]
        public float lifeTime;

        [SerializeField] [Rename("播放速率")] [Range(0, 4)]
        public float speed = 1.0f;

        [Rename("死亡后显示的标签")] public SfxParticle deathSfx;
        [SerializeField] [Tooltip("特效表现")] public List<SfxPrefab> sfxPrefab = new();
        [SerializeField] [Tooltip("施法者身上的表现")] public List<SfxOwner> sfxOwner = new();
        [SerializeField] [Tooltip("镜头上的上的表现")] public List<SfxCameraShark> sfxShark = new();
        [SerializeField] [Tooltip("声音")] public List<SfxSound> sfxSound = new();
        [SerializeField] [Tooltip("连线表现")] public List<SfxLineTag> sfxLines = new();

#if UNITY_EDITOR
        private void OnValidate()
        {   
            foreach (var owner in sfxOwner)
            {
                if (owner.animationClip != null)
                {
                    owner.clipName = owner.animationClip.name;
                }
            }

            
            if (autoCalLifeTime == false)
            {
                return;
            }

            float maxLifeTime = 0;
            //特效的生命周期计算为：Duration + StartDelay + MaxLifeTime
            foreach (SfxPrefab sfx in sfxPrefab)
            {
                if (sfx.prefab)
                {
                    sfx.lifeTime = sfx.prefab.transform.GetMaxParticleLifetime(out float delayTime);
                    sfx.deleteNow = delayTime;

                    if (maxLifeTime < sfx.lifeTime + sfx.bindTime)
                    {
                        maxLifeTime = sfx.lifeTime + sfx.bindTime;
                    }
                }
            }
            foreach (SfxSound sfx in sfxSound)
            {
                if (sfx.audioClip)
                {
                    sfx.lifeTime = sfx.audioClip.length;
                }

                if (sfx.randomClips != null && sfx.randomClips.Count != 0)
                {
                    foreach (var _clip in sfx.randomClips)
                    {
                        if (sfx.lifeTime < _clip.length)
                        {
                            sfx.lifeTime = _clip.length;
                        }
                    }
                }
            }
            if (maxLifeTime == 0)
            {
                lifeTime = 0;
                return;
            }

            foreach (SfxLineTag sfx in sfxLines)
            {
                if (sfx.prefab)
                {
                    if (maxLifeTime < sfx.lifeTime + sfx.bindTime)
                    {
                        maxLifeTime = sfx.lifeTime + sfx.bindTime;
                    }
                }
            }

            foreach (var owner in sfxOwner)
            {
                if (owner.animationClip != null)
                {
                    owner.lifeTime = owner.animationClip.length;
                    owner.clipName = owner.animationClip.name;
                }
                
                if (maxLifeTime < owner.lifeTime + owner.bindTime)
                {
                    maxLifeTime = owner.lifeTime + owner.bindTime;
                }
            }

            foreach (var shark in sfxShark)
            {
                if (maxLifeTime < shark.lifeTime + shark.bindTime)
                {
                    maxLifeTime = shark.lifeTime + shark.bindTime;
                }
            }

            foreach (var sound in sfxSound)
            {
                if (maxLifeTime < sound.lifeTime + sound.bindTime)
                {
                    maxLifeTime = sound.lifeTime + sound.bindTime;
                }
            }

            foreach (var line in sfxLines)
            {
                if (maxLifeTime < line.lifeTime + line.bindTime)
                {
                    maxLifeTime = line.lifeTime + line.bindTime;
                }
            }

            lifeTime = maxLifeTime;
            if (lifeTime >= 20)
            {
                lifeTime = 20;
            }
        }
#endif
    }

//===================================================================================================================
// 编辑的节点
//===================================================================================================================
    [Serializable]
    public class BaseSfx
    {
        [SerializeField] public bool debug;

        [Rename("绑定时间点")] [Range(0, 20)] [SerializeField]
        public float bindTime;

        [Rename("生命周期")] [Range(0, 20)] [SerializeField]
        public float lifeTime;
    }

    [Serializable]
    public class SfxPrefab : BaseSfx
    {
        [Rename("挂载插槽")]
        [SerializeField]
        [CustomPop("none", "origin", "top", "body", "bip_l_hand", "bip_r_hand", "bip_l_weapon", "bip_r_weapon")]
        public string locator = "none";

        [Rename("特效表现")] [SerializeField] public GameObject prefab;

        [Rename("本地位置")] [Tooltip("播放时使用人物的位置，后续不在更新")] [SerializeField]
        public bool useSfxPosition; //使用本地位置

        [Rename("本地旋转")] [Tooltip("播放时使用人物的旋转，后续不在更新")] [SerializeField]
        public bool useSfxRotation; //本地旋转

        [Rename("本地缩放")] [Tooltip("播放时使用人物的缩放，后续不在更新")] [SerializeField]
        public bool useSfxScale; //本地缩放， 为true的时候 和

        [Rename("永无旋转")] [Tooltip("特效的旋转和插槽无任何关系")] [SerializeField]
        public bool noRotation; //本地旋转

        [Rename("永无缩放")] [Tooltip("特效的所发和插槽无任何关系")] [SerializeField]
        public bool noScale;

        [Tooltip("随机动画")] [Rename("随机动画")] [SerializeField]
        public bool randomAnimation;

        [Rename("动画名")] [SerializeField] public List<AnimationClip> randomClipList;

        /// <summary>
        /// 死亡动画
        /// </summary>
        [Rename("死亡消失动画")] public AnimationClip deathClip;

        [Rename("死亡消失时间")] [Range(0, 2)] public float deleteNow;
    }

    [Serializable]
    public class SfxLineTag : BaseSfx
    {
        [Rename("挂载插槽")]
        [SerializeField]
        [CustomPop("none", "origin", "top", "body", "bip_l_hand", "bip_r_hand", "bip_l_weapon", "bip_r_weapon")]
        public string locator = "none";

        [Rename("连线特效")] [SerializeField] public GameObject prefab;

        [Rename("目标挂载插槽")] [SerializeField] [CustomPop("none", "origin", "top", "body", "bip_l_hand", "bip_r_hand","bip_l_weapon", "bip_r_weapon")]
        public string targetLocator = "none";
    }

    [Serializable]
    public class SfxOwner : BaseSfx
    {
        [Rename("隐藏")] public bool hide;

        [Header("动画")] [SerializeField] [Rename("节点动画路径")] [DropdownAsset("Assets/Art/character/np_common/ani")]
        public AnimationClip animationClip;

        [SerializeField] public string clipName;

        [SerializeField] [Rename("打断播放")] [Tooltip("如果为true会立即停止当前动画，再次播放")]
        public bool repeatPlay;

        [Rename("动画优先级")] [Tooltip("优先级越高,越不会被顶掉")] [Range(0, 100)]
        public int animationPriority;

        [Header("半透")] [SerializeField] [Rename("启用半透")]
        public bool enableAlpha;

        [SerializeField] [Rename("半透曲线")] public AnimationCurve alphaCure;

        [Header("边缘光")] [SerializeField] [Rename("边缘光启用")]
        public bool rimEnable = false;

        [SerializeField] [Rename("边缘光颜色")] public Color rimColor;
        [SerializeField] [Rename("边缘光强度")] public AnimationCurve rimIntensityCure;
        [SerializeField] [Rename("边缘光范围")] public AnimationCurve rimPowerCure;
        [SerializeField] [Rename("边缘光区域")] public AnimationCurve rimAreaCure;

        [Header("描边")] [SerializeField] [Rename("启用描边")]
        public bool enableOutLine;

        [Rename("描边颜色")] [Tooltip("Alpha小于128,使用公共的描边颜色。")]
        public Color outLineColor;

        [Rename("描边宽度")] public AnimationCurve outLineWidth;
        [Header("缩放")]
        public bool enableScale;
        [Rename("缩放曲线")] public AnimationCurve scaleCure;
        [Header("石化冰冻等")]
        public bool enableMaskTexture;
        [Rename("效果贴图")] public Texture2D maskTexture;
        [Rename("效果曲线")] public AnimationCurve maskCure;
    }

    [Serializable]
    public class SfxCameraShark : BaseSfx
    {
        [SerializeField] [Rename("镜头动画")] [DropdownAsset("Assets/Art/Scene/common/ani", "Shark")]
        public AnimationClip animationClip;
    }

    [Serializable]
    public class SfxSound : BaseSfx
    {
        [SerializeField] [Rename("声音资源")] public AudioClip audioClip;

        [SerializeField] [Rename("音量")] [Range(0, 1)]
        public float volume = 1f;

        [SerializeField] [Rename("是否循环播放")] public bool loop;

        [SerializeField] [Tooltip("随机列表，有值就用随机列表里的数据")]
        public List<AudioClip> randomClips;

        [SerializeField] [ReadOnly] public int debugRandomIndex;
    }
}