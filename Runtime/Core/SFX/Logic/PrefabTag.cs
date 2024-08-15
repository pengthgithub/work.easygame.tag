using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Easy.Logic
{
    public class PrefabTag : ITag
    {
        private SfxPrefab _prefabTag;

        public PrefabTag(SfxPrefab prefabTag)
        {
            _prefabTag = prefabTag;
            LifeTime = _prefabTag.lifeTime;
            BindTime = _prefabTag.bindTime;
        }

        /// <summary>
        /// 创建好的表现
        /// </summary>
        private GameObject _instanced;

        /// <summary>
        /// 跟随插槽
        /// </summary>
        private Transform _locatorTs;

        /// <summary>
        /// 动画节点
        /// </summary>
        private Animator _animator = null;

        private ParticleSystem[] _particleSystems;
        private TrailRenderer[] _trailRenders;

        protected override void OnBind()
        {
            //0. 没有对象直接报错返回
            if (_prefabTag != null && _prefabTag.prefab == null) //预制件引用为空 不做任何事情
            {
                Debug.LogError($"{Sfx.name}标签中的SFXPrefab引用的特效预制件丢失。");
                return;
            }

            if (_instanced != null)
            {
                InitLocator();
                return;
            }

            //1、创建表现对象
            //如果对象有则不删除
            if (_instanced == null)
            {
                _instanced = Object.Instantiate(_prefabTag.prefab, Sfx.transform, false);
                _instanced.transform.Reset();
            }

            //2、读取动画，配置了随机动画，就随机播放。没有就是默认效果
            if (_animator == null) _animator = _instanced.GetComponentInChildren<Animator>();
            if (_animator && _prefabTag.randomAnimation) //随机动画
            {
                var _count = _prefabTag.randomClipList.Count;
                if (_count != 0)
                {
                    int index = Random.Range(0, _prefabTag.randomClipList.Count);
                    _animator.Play(_prefabTag.randomClipList[index].name);
                }
            }

            //3、获取所有的粒子节点
            if (_particleSystems == null || _particleSystems.Length == 0)
            {
                _particleSystems = _instanced.GetComponentsInChildren<ParticleSystem>();
            }

            if (_trailRenders == null || _trailRenders.Length == 0)
            {
                _trailRenders = _instanced.GetComponentsInChildren<TrailRenderer>();
            }

            //4、根据配置的插槽，读取插槽节点。
            InitLocator();
        }

        private float _lastSpeed;

        /// <summary>
        /// 切换速度
        /// </summary>
        /// <param name="speed"></param>
        private void ChangeSpeed(float speed)
        {
            if (speed.Equals(_lastSpeed)) return;
            _lastSpeed = speed;
            if (_particleSystems != null)
            {
                foreach (ParticleSystem ps in _particleSystems)
                {
                    ParticleSystem.MainModule main = ps.main;
                    main.simulationSpeed = speed;
                }
            }

            if (_animator) _animator.speed = speed;
        }

        private void InitLocator()
        {
            //3.1 如果有施法主体，则查找主体上的插槽
            if (_prefabTag.locator != "none" && Sfx.Owner)
            {
                _locatorTs = Sfx.Owner.GetLocator(_prefabTag.locator);
                if (!_locatorTs)
                {
                    _locatorTs = Sfx.Owner.transform;
                }

                //如果永无旋转，和 第一次设置位置
                if (_prefabTag.useSfxPosition && _prefabTag.noRotation)
                {
                    Sfx.transform.position = _locatorTs.position;
                }
                else
                {
                    _instanced.transform.position = _locatorTs.position;
                }

                // 如果勾选了本地旋转，则会在创建的时候刷新一下旋转。
                // 如果勾选了 永无旋转，表示永远用自己的旋转
                // 死亡时创建的标签需要锁定位置
                if (!_prefabTag.noRotation && !Sfx.LockDirection)
                    _instanced.transform.rotation = _locatorTs.rotation;
                // 永无缩放，表示永远不设置缩放
                if (!_prefabTag.noScale) _instanced.transform.localScale = _locatorTs.localScale;
            }

            //3.2 如果有施法主体，但是插槽为none，则直接使用施法主体的位置
            if (_prefabTag.locator == "none" && Sfx.Owner != null)
            {
                _locatorTs = null;
                Sfx.transform.position = Sfx.Owner.transform.position;
                // 如果勾选了本地旋转，则会在创建的时候刷新一下旋转。
                // 如果勾选了 永无旋转，表示永远用自己的旋转
                // 死亡时创建的标签需要锁定位置
                if (!_prefabTag.noRotation && !Sfx.LockDirection)
                    Sfx.transform.rotation = Sfx.Owner.transform.rotation;
                // 永无缩放，表示永远不设置缩放
                if (!_prefabTag.noScale) Sfx.transform.localScale = Sfx.Owner.transform.localScale;
            }

            //3.3 如果没有施法主体，则归零
            if (Sfx.Owner == null)
            {
                _locatorTs = null;
                if (_instanced) _instanced.transform.Reset();
            }

            if (_instanced.transform.position != Vector3.zero)
            {
                _instanced.SetActive(true);
            }
            
            if (_trailRenders != null)
            {
                foreach (var trail in _trailRenders)
                {
                    if (trail) trail.Clear();
                }
            }

        }

        protected override void OnUpdate(float deltaTime)
        {
            //0、 强制刷新位置，必须是要有插槽
            if (!_instanced && !_locatorTs) return;
            float speed = Sfx.PlaySpeed;
            if (Sfx.Owner) speed *= Sfx.Owner.PlaySpeed;
            ChangeSpeed(speed);

            if (!_locatorTs) return;
            //1、如果勾选了本地位置，只会在创建的时候刷新一下位置
            if (!_prefabTag.useSfxPosition)
            {
                _instanced.transform.position = _locatorTs.transform.position;
                if (_instanced.gameObject.activeSelf == false)
                {
                    _instanced.SetActive(true);
                }
            }

            //2、如果勾选了本地旋转，则会在创建的时候刷新一下旋转。后续不在刷新旋转
            // 如果勾选了 无旋转，表示永远用自己的旋转
            // 死亡时创建的标签需要锁定位置
            if (!_prefabTag.useSfxRotation && !_prefabTag.noRotation && !Sfx.LockDirection)
            {
                _instanced.transform.rotation = _locatorTs.transform.rotation;
            }

            //3、如果勾选了本地缩放，则会在创建的时候刷新一下缩放。后续不在刷新缩放
            // 如果勾选了 无缩放，表示永远用自己的缩放
            if (!_prefabTag.useSfxScale && !_prefabTag.noScale)
            {
                _instanced.transform.localScale = _locatorTs.transform.localScale;
            }
        }

        protected override void OnDispose()
        {
            if (_instanced)
            {
                //重置位置
                _instanced.transform.Reset();
                ChangeSpeed(Sfx.PlaySpeed);

                //隐藏
                _instanced.SetActive(false);
                _lastSpeed = 1;
            }

            if (_particleSystems != null)
            {
                foreach (var ps in _particleSystems)
                {
                    if (ps) ps.Stop();
                }
            }

            if (_trailRenders != null)
            {
                foreach (var trail in _trailRenders)
                {
                    if (trail) trail.Clear();
                }
            }

            _locatorTs = null;
        }

        protected override void OnDestroy()
        {
            _particleSystems = null;
            Object.Destroy(_instanced);
            _instanced = null;
            _animator = null;
            _locatorTs = null;
            _prefabTag = null;
        }

        private string deathName = "";
        private Dictionary<string, float> clipsTimes;

        protected override float OnDeath()
        {
            float deathTime = 0;
            //有死亡动画，就直接播放死亡动画
            if (_animator && _prefabTag.deathClip)
            {
                if (deathName == "")
                {
                    deathName = string.Intern(_prefabTag.deathClip.name);
                    RuntimeAnimatorController controller = _animator.runtimeAnimatorController;
                    if (!controller) return 0;

                    AnimationClip[] clips = controller.animationClips;
                    if (clips.Length == 0) return 0;
                    clipsTimes ??= new Dictionary<string, float>();
                    foreach (var clip in clips)
                    {
                        clipsTimes[clip.name] = clip.length;
                    }
                }

                deathTime = 0;
                bool result = clipsTimes.TryGetValue(deathName, out deathTime);

                if (result)
                {
                    _animator.Play(deathName); // gctodo
                }

                return deathTime;
            }
            else
            {
                if (_particleSystems != null && _prefabTag.deleteNow != 0)
                {
                    // 没有死亡动画，就播放粒子停止发射
                    foreach (var ps in _particleSystems)
                    {
                        if (ps) ps.Stop();
                    }
                }

                deathTime = _prefabTag.deleteNow;
                
                if (deathTime > 2) deathTime = 2;
            }

            return deathTime;
        }
    }
}