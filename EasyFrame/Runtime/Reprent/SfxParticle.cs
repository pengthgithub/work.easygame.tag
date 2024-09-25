using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Easy
{
    [CreateAssetMenu(menuName = "特效标签", fileName = "sfx_特效名字", order = 0)]
    [Icon("Packages/EasyFrame/Editor/Icon/tag_icon.png")]public class SfxParticle : ScriptableObject
    {
        #if UNITY_EDITOR
        [SerializeField] [HideInInspector] public int aniIndex;
        [SerializeField] [HideInInspector] public int moveSpeed;
        [SerializeField] [HideInInspector] public string enemy;
        [SerializeField] [HideInInspector] public string preview;
        [SerializeField] [HideInInspector] public string defaultScene;
        [SerializeField] [HideInInspector] public Vector3 pos;
        #endif
        
        [SerializeField] [Rename("生命周期，0为永久")] [Range(0, 20)]
        public float lifeTime;
        [SerializeField] [Rename("播放速率")] [Range(0, 4)]
        public float speed = 1.0f;
        [Rename("死亡后显示的标签")] public SfxParticle deathSfx;
        
        [SerializeField] [Tooltip("特效表现")] public List<SfxPrefab> sfxPrefab = new();
        [SerializeField] [Tooltip("施法者身上的表现")] public List<SfxOwner> sfxOwner = new();
        [SerializeField][Tooltip("镜头上的上的表现")] public List<SfxCameraShark> sfxShark = new();
        [SerializeField] [Tooltip("声音")] public List<SfxSound> sfxSound = new();

        #region 逻辑处理
        private List<BaseSfx> needUpdate = new List<BaseSfx>();
        private bool bInited = false;
        internal void Init()
        {
            if (bInited)
            {
                foreach (var sfx in needUpdate)
                {
                    sfx.Init();
                }
                return;
            }
            bInited = true;
            needUpdate.Clear();
            foreach (var sfx in sfxPrefab)
            {
                needUpdate.Add(sfx);
            }
            foreach (var sfx in sfxOwner)
            {
                needUpdate.Add(sfx);
            }
            foreach (var sfx in sfxShark)
            {
                needUpdate.Add(sfx);
            }
            foreach (var sfx in sfxSound)
            {
                needUpdate.Add(sfx);
            }
        }
        internal void OnUpdate(float durationTime)
        {
            foreach (var sfx in needUpdate)
            {
                sfx.Update(durationTime);
            }
        }

        internal void Dispose()
        {
            foreach (var sfx in needUpdate)
            {
                sfx.Dispose();
            }
        }

        #endregion
    }
    
    //===================================================================================================================
    // 编辑的节点
    //===================================================================================================================
    #region 编辑节点
    [Serializable]
    public class BaseSfx
    {
        [Rename("绑定时间点")] [Range(0, 20)] [SerializeField]
        public float bindTime;

        [Rename("生命周期")] [Range(0, 20)] [SerializeField]
        public float lifeTime;

        private bool bBind = false;
        private bool sleep = false;
        protected float duaration = 0;

        internal void Init()
        {
            bBind = false;
            sleep = false;
            duaration = 0;
        }
        internal void Update(float duration)
        {
            duaration = duration;
            if(sleep) return;
            if (duaration >= (lifeTime + bindTime) && lifeTime != 0)
            {
                OnDeath();
                return;
            }
            
            if (duration >= bindTime && !bBind)
            {
                bBind = true;
                OnBind();
            }
            
            if(!bBind) return;
            OnUpdate();
        }

        internal void Dispose()
        {
            OnDispose();
            sleep = true;
            bBind = false;
        }

        protected virtual void OnDeath(){ }
        protected virtual void OnBind(){ }
        protected virtual void OnUpdate(){ }
        protected virtual void OnDispose(){ }
    }

    [Serializable]
    public class SfxPrefab : BaseSfx
    {
        [SerializeField][Rename("插槽")] public LocatorType locatorType;
        [SerializeField] [Rename("目标插槽")]public LocatorType targetlocatorType;
        [Rename("特效表现")] [SerializeField] public GameObject prefab;

        [Rename("只更新一次位置")] [Tooltip("播放时使用人物的位置，后续不在更新")] [SerializeField]
        public bool useSfxPosition; //使用本地位置

        [Rename("只更新一次旋转")] [Tooltip("播放时使用人物的旋转，后续不在更新")] [SerializeField]
        public bool useSfxRotation; //本地旋转

        [Rename("只更新一次缩放")] [Tooltip("播放时使用人物的缩放，后续不在更新")] [SerializeField]
        public bool useSfxScale; //本地缩放， 为true的时候 和

        [Rename("不更新旋转")] [Tooltip("特效的旋转和插槽无任何关系")] [SerializeField]
        public bool noRotation; //本地旋转

        [Rename("不更新缩放")] [Tooltip("特效的所发和插槽无任何关系")] [SerializeField]
        public bool noScale;
        
        [Rename("死亡消失时间")] [Range(0, 2)] public float deleteNow;
        [SerializeField] public List<ClipData> clipList;
        #region 逻辑控制
        internal Represent __self;
        internal SfxControl __display;
        
        private float deathTime = 0;
        private bool isLine = false;

        #region 绑定
        private Transform _locatorTs;
        private Transform _targetLocatorTs;
        private void InitLocator()
        {
            //3.1 如果有施法主体，则查找主体上的插槽
            if (__self.Owner)
            {
                _locatorTs = __self.Owner.GetLocator(locatorType);
                
                if (!_locatorTs)
                {
                    _locatorTs = __self.Owner.transform;
                }

                __display.transform.localPosition = _locatorTs.transform.localPosition;
                if (!noRotation)
                {
                    __self.transform.rotation = _locatorTs.rotation;
                }
                // 永无缩放，表示永远不设置缩放
                if (!noScale)
                {
                    __self.transform.localScale = _locatorTs.localScale;
                }
            }
        }
        protected void InitTargetLocator()
        {
            //1、获取结束点的位置
            // 直接获取插槽位置，获取不到，则使用 目标点的位置
            if (targetlocatorType != LocatorType.none)
            {
                _targetLocatorTs = __self.Target.GetLocator(targetlocatorType);
            }
            if (_targetLocatorTs == null)
            {
                _targetLocatorTs =  __self.Target.transform;
            }
        }
        protected override void OnBind()
        {
            if(!__display) return;
            __display.gameObject.SetActive(true);

            if (targetlocatorType != LocatorType.none && __self.Target)
            {
                isLine = true;
                InitTargetLocator();
            }
            InitLocator();
            
            //随机动画
            var count = clipList.Count;
            if (count != 0)
            {
                int index = Random.Range(0, count);
                __display.Play(clipList[index].name);
            }
        }
        #endregion
        
        private void UpdateLogic()
        {
            //0、 强制刷新位置，必须是要有插槽
            if (!__display && !_locatorTs) return;
            __display.Speed = __self.Speed;

            if (!_locatorTs) return;
            //1、如果勾选了本地位置，只会在创建的时候刷新一下位置
            if (!useSfxPosition)
            {
                __display.transform.position = _locatorTs.transform.position;
            }

            //2、如果勾选了本地旋转，则会在创建的时候刷新一下旋转。后续不在刷新旋转
            // 如果勾选了 无旋转，表示永远用自己的旋转
            // 死亡时创建的标签需要锁定位置
            if (!useSfxRotation && !noRotation && !__self.LockDirection)
            {
                __display.transform.rotation = _locatorTs.transform.rotation;
            }

            //3、如果勾选了本地缩放，则会在创建的时候刷新一下缩放。后续不在刷新缩放
            // 如果勾选了 无缩放，表示永远用自己的缩放
            if (!useSfxScale && !noScale)
            {
                __display.transform.localScale = _locatorTs.transform.localScale;
            }
        }
        protected override void OnUpdate()
        { 
            if (isLine)
            {
                __display.FlowTarget(_locatorTs, _targetLocatorTs);
            }
            else
            {
                UpdateLogic();
            }
        }
        
        protected override void OnDispose()
        {
            _locatorTs = null;
            _targetLocatorTs = null;
            deathTime = 0;
            isLine = false;
        }

        protected override void OnDeath()
        {
            if (__display == null)
            {
                Dispose();
                return;
            }
            var count = clipList.Count;
            deathTime += Time.deltaTime * __self.Speed;
            if (deathTime > deleteNow)
            {
                __display.gameObject.SetActive(false);
                __display.Dispose();
                Dispose();
                return;
            }
			__display.Dispose();
            if (count == 0)return;
            var index = Random.Range(0, count);
            var data = clipList[index];
            __display.Play(data.name);
            deleteNow = data.length;
        }
        #endregion
    }

    [Serializable]
    public class SfxOwner : BaseSfx
    {
        [Rename("隐藏")] public bool hide;

        [Header("动画")] [SerializeField] [Rename("节点动画路径")] [DropdownAsset("Assets/Art/Character/common/ani")]
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
        
        #region 逻辑控制
        internal Control control;

        protected override void OnUpdate()
        {
            if(enableAlpha && enableOutLine && rimEnable && enableMaskTexture) return;
            UpdateLogic(duaration);
        }
        protected override void OnBind()
        {
            if(!control) return;
            if (!string.IsNullOrEmpty(clipName) && animationClip)
            {
                control.PlayClip(clipName, animationClip);
            }
            if (enableMaskTexture && !maskTexture)
            {
                control.MaskTexture = maskTexture;
            }
        }

        private void UpdateLogic(float updatedTime)
        {
            if(!control) return;
            if(!enableAlpha && !enableOutLine&& !rimEnable && !enableMaskTexture) return;
            
            if (rimEnable)
            {
                //qingdu
                var rimPower = rimIntensityCure.Evaluate(updatedTime - bindTime);
                //范围
                var rimInten = rimPowerCure.Evaluate(updatedTime - bindTime);
                //区域
                var rimArea = rimAreaCure.Evaluate(updatedTime - bindTime);

                control.RimColor = rimColor;
                control.RimPower = rimPower;
                control.RimRange = rimInten;
                control.RimArea = rimArea;
            }
            
            if (enableAlpha && alphaCure != null && alphaCure.length != 0)
            {
                control.Alpha = alphaCure.Evaluate(updatedTime - bindTime);
            }

            if (enableOutLine && outLineWidth != null && outLineWidth.length != 0)
            {
                var _width = outLineWidth.Evaluate(updatedTime - bindTime);
                control.OutLineColor = outLineColor;
                control.OutLineWidth = _width;
            }

            if (enableScale && scaleCure != null && scaleCure.length != 0)
            {
                var scale = scaleCure.Evaluate(updatedTime - bindTime);
                control.transform.localScale = new Vector3(scale, scale, scale);
            }

            if (enableMaskTexture && maskTexture != null)
            {
                var power = maskCure.Evaluate(updatedTime - bindTime);
                control.MaskPower = power;
            }

            control.ModifyParam( enableAlpha, enableOutLine, rimEnable, enableMaskTexture);
        }

        protected override void OnDispose()
        {
            if(control) control.ResetMaterial();
            control = null;
        }

        #endregion
    }
    
    [Serializable]
    public class SfxCameraShark : BaseSfx
    {
        [SerializeField] [Rename("镜头动画")] [DropdownAsset("Assets/Art/Scene/common/ani", "Shark")]
        public AnimationClip animationClip;
        
        #region 逻辑控制
        internal Control control;
        protected override void OnBind()
        {
            if(!animationClip) return;
            
            if (CameraMgr.Instance)
            {
                CameraMgr.Instance.Play(animationClip);
            }
        }
        protected override void OnDispose()
        {
            if (CameraMgr.Instance)
            {
                CameraMgr.Instance.Stop();
            }

            control = null;
        }
        #endregion
    }

    [Serializable]
    public class SfxSound : BaseSfx
    {
        [SerializeField] [Rename("音量")] [Range(0, 1)]
        public float volume = 1f;
        [SerializeField] [Rename("是否循环播放")] public bool loop;
        [SerializeField] [Tooltip("随机列表，有值就用随机列表里的数据")]
        public List<AudioClip> randomClips;
        [SerializeField] [HideInInspector] public List<string> randomClipNames;
        
        #region 逻辑控制
        private int _index;
        protected override void OnBind()
        {
            var count = randomClips.Count;
            if (count != 0)
            {
                int index = Random.Range(0, randomClips.Count);
                AudioClip clip = randomClips[index];
                if (SoundMgr.Instance)
                {
                    _index = SoundMgr.Instance.PlaySound(clip, volume, loop);
                }
            }
        }
        protected override void OnDispose()
        {
            if(SoundMgr.Instance) SoundMgr.Instance.ReleaseSound(_index);
            _index = 0;
        }
        #endregion
    }
    #endregion
}