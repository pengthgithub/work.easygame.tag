using System;
using UnityEngine;

namespace Easy
{
    public partial class Represent
    {
        [SerializeField] [Rename("更新时间")] public float _durationTime = 0;
        internal bool LockDirection = false;

        private bool __IsSfxTag = false; //特效标签的标记
        
        //__表示变量数据不重置
        private SfxParticle __SfxParticle;
        private void InitTag(SfxParticle sfx)
        {
            _durationTime = 0;
            LockDirection = false;
            __SfxParticle = sfx;
            __IsSfxTag = true;
            __SfxParticle.Init(this);
            foreach (var sfxItem in sfx.sfxPrefab)
            {
                //0. 没有对象直接报错返回
                if (sfxItem.prefab == null) //预制件引用为空 不做任何事情
                {
                    Debug.LogError($"{url}标签中的SFXPrefab引用的特效预制件丢失。");
                    return;
                } 
            
               
                var item = GameObject.Instantiate(sfxItem.prefab, Vector3.zero, Quaternion.identity);
                item.transform.SetParent(transform, false);
                item.transform.localScale = Vector3.one;
                item.SetActive(false);
                sfxItem.__display = item.GetComponent<SfxControl>();
                sfxItem.__DisplayIsNotNull = (sfxItem.__display != null);
#if UNITY_EDITOR
                if (sfxItem.__DisplayIsNotNull == false)
                {
                    Debug.LogError($"{sfxItem.prefab.name} 预制件上没有挂载SfxControl组件");
                }
#endif
            }
        }

        public void ResetTag()
        {
            SetOwner();
            if(__SfxParticle) __SfxParticle.Init(this);
        }

        private void SetOwner()
        {
            if(!__SfxParticle || !Owner) return;
            foreach (var sfxItem in __SfxParticle.sfxOwner)
            {
                sfxItem.control = Owner.__0Control;
            }
        }
        private void UpdateTag()
        {
            //死亡后数据更新
            if (__stat == RepresentStat.RsDeathing)
            {
                _durationTime += Time.deltaTime * Speed;
                __SfxParticle.DeathUpdate(_durationTime);

                if (_durationTime > (__SfxParticle.deathTimeTotal + 0.1f))
                {
                    _durationTime = 0;
                    TagDeathEnd();
                }
            }
            
            //未死亡前数据更新
            if (__stat >= RepresentStat.RsLoadEnd && __stat < RepresentStat.RsDisposing)
            {
                _durationTime += Time.deltaTime * Speed;
                __SfxParticle.OnUpdate(_durationTime);
                
                //此处+0.1f 是 避免由于精度原因导致最后一帧的数据没有执行。
                if (__SfxParticle.lifeTime != 0 && _durationTime > (__SfxParticle.lifeTime + 0.1f))
                {
                    OnDispose();
                }
            }
            
            
#if UNITY_EDITOR
            UpdateMoveBullet();
#endif
        }
        private void DisposeTag()
        {
            LockDirection = false;
            _durationTime = 0;
            
            DeathEndPlaySfx();
            
            if (__SfxParticle.deathTimeTotal != 0)
            {
                __stat = RepresentStat.RsDeathing;
                __SfxParticle.Death();
            }
            else
            {
                TagDeathEnd();
            }
        }

        private void TagDeathEnd()
        {
            __stat = RepresentStat.RsDeathEnd;
            __SfxParticle.Dispose();
            Release(this);
        }

        /// <summary>
        /// 死亡特效
        /// </summary>
        private void DeathEndPlaySfx()
        {
            if(!__SfxParticle || !__SfxParticle.deathSfx) return;
            
            var rep = Represent.Create(__SfxParticle.deathSfx.url);
            if(rep == null) return;
            
            rep.transform.position = transform.position;
            rep.transform.rotation = transform.rotation;
            rep.transform.localScale = transform.localScale;
        }
        
        //======================================================================
        // 编辑器需要的功能
        //======================================================================
#if UNITY_EDITOR
        private Transform _tage;
        private float _speed;
        private float lastTime;
        private bool bMove = false;
        public void MoveBullet(float speed, Transform target)
        {
            _speed = speed;
            _tage = target;
            bMove = true;
        }
        internal void UpdateMoveBullet()
        {
            if(_speed == 0 || !bMove) return;
            var speed = (float)(_speed / 2) * (_durationTime - lastTime);
            // 计算方向向量
            Vector3 direction = (_tage.position - transform.position);
            if (direction.magnitude < 0.01f)
            {
                bMove = false;
                Dispose();
                return;
            }
            
            transform.position = Vector3.Lerp(transform.position, _tage.position, speed/direction.magnitude);
            Quaternion lookRotation = Quaternion.LookRotation(direction.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * speed);
            lastTime = _durationTime;
        }
#endif
        
    }
}