namespace Easy
{
    public abstract class ITag
    {
        /// <summary>
        /// 绑定时间
        /// </summary>
        protected float BindTime;

        /// <summary>
        /// 生命周期
        /// </summary>
        protected float LifeTime;

        /// <summary>
        /// 特效主体
        /// </summary>
        protected ESfx Sfx { get; set; }

        /// <summary>
        /// 是否绑定完成
        /// </summary>
        private bool BindEnd { get; set; }

        /// <summary>
        /// 生命是否结束
        /// </summary>
        private bool LifeEnd { get; set; }

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="sfx"></param>
        public void Init(ESfx sfx)
        {
            Sfx = sfx;
            BindEnd = false;
            LifeEnd = false;
        }

        /// <summary>
        /// 更新逻辑
        /// </summary>
        /// <param name="updatedTime"></param>
        public void Update(float updatedTime)
        {
            if (LifeEnd) return;

            if (updatedTime >= BindTime && !BindEnd)
            {
                BindEnd = true;
                OnBind();
            }

            if (!LifeEnd && BindEnd) OnUpdate(updatedTime);

            if (LifeTime != 0 && !LifeEnd && updatedTime >= (LifeTime + BindTime))
            {
                LifeEnd = true;
                OnDispose();
            }
        }

        public float Death()
        {
            return OnDeath();
        }

        public void Dispose()
        {
            LifeEnd = true;
            OnDispose();
        }

        public void Destory()
        {
            OnDestroy();
        }

        /// <summary>
        /// 绑定时间
        /// </summary>
        protected abstract void OnBind();

        /// <summary>
        /// 释放回收
        /// </summary>
        protected abstract void OnDispose();

        /// <summary>
        /// 彻底销毁
        /// </summary>
        protected abstract void OnDestroy();

        /// <summary>
        /// 更新逻辑
        /// </summary>
        protected virtual void OnUpdate(float updatedTime)
        {
        }

        /// <summary>
        /// 死亡逻辑
        /// </summary>
        /// <returns></returns>
        protected virtual float OnDeath()
        {
            return 0;
        }
    }
}