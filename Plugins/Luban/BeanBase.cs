using UnityEngine;

namespace Luban
{
    public abstract class BeanBase : Object, ITypeId
    {
        public abstract int GetTypeId();
    }
}