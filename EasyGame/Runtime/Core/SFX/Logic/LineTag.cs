using UnityEngine;

namespace Easy
{
    public class LineTag : ITag
    {
        private SfxLineTag _lineTag;

        public LineTag(SfxLineTag lineTag)
        {
            _lineTag = lineTag;
            BindTime = _lineTag.bindTime;
            LifeTime = _lineTag.lifeTime;
        }

        /// <summary>
        /// 表现对象
        /// </summary>
        private GameObject _instanced;

        /// <summary>
        /// 线的起始点位置
        /// </summary>
        private Transform _lineStartTs;

        /// <summary>
        /// 线的结束点位置
        /// </summary>
        private Transform _lineEndTs;

        /// <summary>
        /// 连线的所有节点
        /// </summary>
        private LineRenderer[] _lineRenders;

        /// <summary>
        /// 所有的粒子节点
        /// </summary>
        private ParticleSystem[] _particleSystems;

        protected void GetStartAndEndTs()
        {
            //0、 获取起始点的位置
            // 直接获取插槽位置，获取不到，则使用 施法者的位置，如果没有施法者，直接使用特效本身的位置
            if (_lineTag.locator != "none" && Sfx.Owner)
            {
                _lineStartTs = Sfx.Owner.GetLocator(_lineTag.locator);
                if (_lineStartTs == null)
                {
                    _lineStartTs = Sfx.Owner.transform;
                }
            }

            if (Sfx.Owner == null)
            {
                _lineStartTs = Sfx.transform;
            }

            //1、获取结束点的位置
            // 直接获取插槽位置，获取不到，则使用 目标点的位置
            if (_lineTag.targetLocator != "none")
            {
                _lineEndTs = Sfx.target.GetLocator(_lineTag.targetLocator);
            }

            if (!_lineEndTs)
            {
                _lineEndTs = Sfx.target.transform;
            }
        }

        protected override void OnBind()
        {
            //连线必须要有目标，没有目标，连线不显示
            if (Sfx.target == null) return;
            //0. 没有对象直接报错返回
            if (_lineTag != null && _lineTag.prefab == null) //预制件引用为空 不做任何事情
            {
                Debug.LogError($"{Sfx.name}标签中的SFXLine引用的特效预制件丢失。");
                return;
            }

            //0、 创建表现，只创建一次
            if (_instanced == null) _instanced = Object.Instantiate(_lineTag.prefab, Sfx.transform, false);

            //1、 获取线需要的起始和 结束位置
            GetStartAndEndTs();

            //3、获取所有的线节点
            if (_lineRenders == null || _lineRenders.Length == 0)
            {
                _lineRenders = _instanced.GetComponentsInChildren<LineRenderer>();
            }

            //4、获取所有的粒子节点
            if (_particleSystems == null || _particleSystems.Length == 0)
            {
                _particleSystems = _instanced.GetComponentsInChildren<ParticleSystem>();
            }

            _instanced.SetActive(true);
        }

        protected override void OnUpdate(float updatedTime)
        {
            base.OnUpdate(updatedTime);

            //连线必须要有目标，没有目标，连线不显示
            if (!Sfx.target) return;

            Vector3 distance = _lineEndTs.position - _lineStartTs.position;
            Vector3 directionAb = distance.normalized;
            var endPos = _lineEndTs.position;
            foreach (var line in _lineRenders)
            {
                if (!line) continue;
                //连线特效需要当前节点 不做任何旋转和位移处理
                line.transform.position = Vector3.zero;
                line.transform.rotation = UnityEngine.Quaternion.identity;
                line.transform.localScale = Vector3.one;

                line.SetPosition(0, _lineStartTs.position);
                line.SetPosition(1, endPos);
            }

            // 将粒子特效按照目标方向缩放
            if (_particleSystems != null)
            {
                foreach (var ps in _particleSystems)
                {
                    var psTs = ps.transform;
                    psTs.position = _lineStartTs.position;
                    psTs.rotation = UnityEngine.Quaternion.FromToRotation(Vector3.forward, directionAb);

                    var localScale = psTs.localScale;
                    localScale.z = distance.z;
                    psTs.localScale = localScale;
                }
            }
        }

        protected override void OnDispose()
        {
            if (_instanced) _instanced.SetActive(false);
        }

        protected override void OnDestroy()
        {
            OnDispose();
            _lineTag = null;
            _particleSystems = null;
            _lineRenders = null;
            _lineEndTs = null;
            _lineStartTs = null;
            if (_instanced)
            {
                GameObject.Destroy(_instanced);
                _instanced = null;
            }
        }
    }
}