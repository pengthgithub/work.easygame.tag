namespace Easy
{
    /// <summary>
    /// 摄象机标签
    /// 用于控制标签对摄象机的控制，结束时需要重置
    /// </summary>
    public class CameraTag : ITag
    {
        private SfxCameraShark _cameraTag;

        public CameraTag(SfxCameraShark cameraTag)
        {
            _cameraTag = cameraTag;
            BindTime = _cameraTag.bindTime;
            LifeTime = _cameraTag.lifeTime;
        }

        protected override void OnBind()
        {
            if (_cameraTag == null || _cameraTag.animationClip == null) return;
            ECamera.Play(_cameraTag.animationClip);
        }

        protected override void OnDispose()
        {
            ECamera.Stop();
        }

        protected override void OnDestroy()
        {
            OnDispose();
            _cameraTag = null;
        }
    }
}