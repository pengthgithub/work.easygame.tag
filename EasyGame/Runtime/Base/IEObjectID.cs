namespace Easy
{
    public class IEObjectID
    {
        /// <summary>
        /// 对象ID,每次回收后都会自增加1
        /// </summary>
        private int _id = -99999;

        public int ObjectID
        {
            get { return _id; }
            private set => _id = value;
        }

        protected void ObjectDispose()
        {
            _id++;
        }
    }
}