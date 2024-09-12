using System;

namespace Easy
{
    public interface IEDisposable
    {
        void Dispose(float delayTime = 0, bool destroy = false);
    }
}