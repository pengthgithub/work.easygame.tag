using System;
using UnityEngine;

namespace Easy
{
    public partial class Represent
    {
        [SerializeField] [Rename("更新时间")] public float _durationTime = 0;
        internal bool LockDirection = false;
        
        //_0表示变量数据不重置
        private SfxParticle _0SfxParticle;
        private void InitTag(SfxParticle sfx)
        {
            _durationTime = 0;
            LockDirection = false;
            _0SfxParticle = sfx;
           
            foreach (var sfxItem in sfx.sfxPrefab)
            {
                //0. 没有对象直接报错返回
                if (sfxItem.prefab == null) //预制件引用为空 不做任何事情
                {
                    Debug.LogError($"{url}标签中的SFXPrefab引用的特效预制件丢失。");
                    return;
                } 
            
                sfxItem.self = this;
                var item = GameObject.Instantiate(sfxItem.prefab, Vector3.zero, Quaternion.identity);
                item.transform.SetParent(transform, false);
                item.transform.localScale = Vector3.one;
                item.SetActive(false);
                sfxItem.display = item.GetComponent<SfxControl>();
                if (sfxItem.display == null)
                {
                    Debug.LogError($"{sfxItem.prefab.name} 预制件上没有挂载SfxControl组件");
                }
#if UNITY_EDITOR
                sfxItem.display.hideChild = true;
#endif
            }

            SetOwner();
            sfx.Init();
        }
        private void SetOwner()
        {
            if(!_0SfxParticle) return;
            foreach (var sfxItem in _0SfxParticle.sfxOwner)
            {
                sfxItem.control = Owner._0Control;
            }
        }
        private void UpdateTag()
        {
            if(isDisposed || !_0LoadEnd || !_0SfxParticle) return;
            
            _durationTime += Time.deltaTime * Speed;
            if (_0SfxParticle.lifeTime != 0 && _durationTime > _0SfxParticle.lifeTime)
            {
                Dispose();
                return;
            }
            _0SfxParticle.OnUpdate(_durationTime);
            
#if UNITY_EDITOR
            UpdateMoveBullet();
#endif
        }
        private void DisposeTag()
        {
            LockDirection = false;
            _durationTime = 0;
            if(_0SfxParticle) _0SfxParticle.Dispose();

            DeathEffect();
        }

        /// <summary>
        /// 死亡特效
        /// </summary>
        private void DeathEffect()
        {
            if(!_0SfxParticle || !_0SfxParticle.deathSfx) return;
            
            var deathSfxName = _0SfxParticle.deathSfx.name;
            
            var rep = Represent.Create(deathSfxName);
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
            if(_speed == 0) return;
            var speed = (float)(_speed / 2) * (_durationTime - lastTime);
            // 计算方向向量
            Vector3 direction = (_tage.position - transform.position);
            if (direction.magnitude < 0.01f)
            {
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