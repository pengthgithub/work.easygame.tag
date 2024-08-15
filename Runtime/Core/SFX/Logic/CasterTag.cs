using UnityEngine;

namespace Easy
{
    /// <summary>
    /// 施法者标签
    /// 用于对设法者控制，实现一些在播放标签时出现的表现
    /// </summary>
    public class CasterTag : ITag
    {
        private SfxOwner _casterTag;

        private static int _classID = 0;
        private int _sfxId;

        public CasterTag(SfxOwner casterTag)
        {
            _casterTag = casterTag;
            BindTime = _casterTag.bindTime;
            LifeTime = _casterTag.lifeTime;
        }

        private float _lastScale;
        protected override void OnBind()
        {
            _sfxId = _classID++;
            ECharacter owner = Sfx.Owner as ECharacter;
            if (!owner) return;
            //播放节点动画
            if (_casterTag.animationClip != null)
            {
#if UNITY_DEBUG
                    var path = UnityEditor.AssetDatabase.GetAssetPath(_casterTag.animationClip);
                    if (path.Contains("character/np_common") == false)
                    {
                        Debug.LogError($"动画{Sfx.name}路径错误:SfxOwner 上的动画，只针对角色有效哦，动画路径在Character/np_common/ani 目录下");
                        return;
                    }
#endif

                owner.PlayNodeAnimation(_casterTag.animationClip, _casterTag.clipName, _casterTag.animationPriority,
                    _casterTag.repeatPlay, _sfxId);
            }

            // if (owner)
            // {
            //     owner.Active = !_casterTag.hide;
            // }
            _lastScale = owner.transform.localScale.x;
            
            if (_casterTag.enableMaskTexture && _casterTag.maskTexture != null)
            {
                if (owner.Locator)
                {
                    owner.Locator.MaskTexture = _casterTag.maskTexture;
                }
            }
        }

        protected override void OnUpdate(float updatedTime)
        {
            var caster = Sfx.Owner;
            if (!Sfx || !caster) return;
            var locator = caster.Locator;
            if (!locator) return;

            if (_casterTag.rimEnable)
            {
                //qingdu
                var rimPower = _casterTag.rimIntensityCure.Evaluate(updatedTime - _casterTag.bindTime);
                //范围
                var rimInten = _casterTag.rimPowerCure.Evaluate(updatedTime - _casterTag.bindTime);
                //区域
                var rimArea = _casterTag.rimAreaCure.Evaluate(updatedTime - _casterTag.bindTime);

                locator.RimColor = _casterTag.rimColor;
                locator.RimPower = rimPower;
                locator.RimRange = rimInten;
                locator.RimArea = rimArea;
            }

            if (_casterTag.enableAlpha && _casterTag.alphaCure != null && _casterTag.alphaCure.length != 0)
            {
                locator.Alpha = _casterTag.alphaCure.Evaluate(updatedTime - _casterTag.bindTime);
            }

            if (_casterTag.enableOutLine && _casterTag.outLineWidth != null && _casterTag.outLineWidth.length != 0)
            {
                var outLineWidth = _casterTag.outLineWidth.Evaluate(updatedTime - _casterTag.bindTime);
                if (Sfx.Owner == null || Sfx.Owner.Locator == null) return;
                locator.OutLineColor = _casterTag.outLineColor;
                locator.OutLineWidth = outLineWidth;
            }

            if (_casterTag.enableScale && _casterTag.scaleCure != null && _casterTag.scaleCure.length != 0)
            {
                var scale = _casterTag.scaleCure.Evaluate(updatedTime - _casterTag.bindTime);
                locator.transform.localScale = new Vector3(scale, scale, scale);
            }

            if (_casterTag.enableMaskTexture && _casterTag.maskTexture != null)
            {
                var power = _casterTag.maskCure.Evaluate(updatedTime - _casterTag.bindTime);
                locator.MaskPower = power;
            }

            locator.ModifyParam(_casterTag.enableAlpha, _casterTag.enableOutLine,_casterTag.rimEnable,_casterTag.enableMaskTexture);
        }

        protected override void OnDispose()
        {      
            if (!Sfx) return;
            ECharacter owner = Sfx.Owner;
            if (owner == null) return;

            if (owner) owner.Active = true;
            owner.AniPriority = 0;
            if (owner && _casterTag.animationClip != null && !_casterTag.repeatPlay)
            {
                //owner.StopNodeAnimation(_sfxId); 
            }

            var locator = owner.Locator;
            if (!locator) return;
            if (_casterTag.enableScale)
            {
                if (_lastScale == 0) _lastScale = 1;
                locator.transform.localScale = new Vector3(_lastScale, _lastScale, _lastScale);
            }
            if (_casterTag.enableMaskTexture && _casterTag.maskTexture != null)
            {
                locator.MaskPower = 0;
            }
            
            locator.Alpha = 1;
            locator.RimPower = 0;
            locator.EnableOutLine = false;
            locator.Show(true, true);
            locator.ModifyParam(_casterTag.enableAlpha, _casterTag.enableOutLine,_casterTag.rimEnable,_casterTag.enableMaskTexture);
        }

        protected override void OnDestroy()
        {
            OnDispose();
            _casterTag = null;
        }
    }
}