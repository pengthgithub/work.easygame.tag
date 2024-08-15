using UnityEngine;

namespace Easy
{
    [RequireComponent(typeof(Renderer))]
    [ExecuteInEditMode]
    public class TextureSheet : MonoBehaviour
    {
        /// <summary>
        /// 编辑器模式启用
        /// </summary>
        [Rename("编辑器更新")] public bool editorEnable = true;

        /// <summary>
        /// 生命周期 单位秒
        /// </summary>
        [Rename("生命周期 单位秒")] public float lifetime = 1f;

        /// <summary>
        /// 序列图方向
        /// </summary>
        [Rename("序列图方向")] public LAYOUT_ENUM Layout = LAYOUT_ENUM.FORWARD;

        /// <summary>
        /// 序列图结构 ?x?
        /// </summary>
         public Vector4 tilingOffset = new(1, 1, 0, 0);

        /// <summary>
        /// 循环次数
        /// </summary>
        [Rename("循环次数")] public int circle = 1;

        /// <summary>
        /// 是否循环
        /// </summary>
        [Rename("是否循环")] public bool loop = true;

        /// <summary>
        /// 绑定的材质ID
        /// </summary>
        [Rename("绑定的材质ID")] public int BindIndex = 0;

        /// <summary>
        /// UV 流动方向
        /// </summary>
        [Rename("UV 流动方向, 正负控制方向，大小控制速度")] public Vector2 UVDir = new(0, 0);

        /// <summary>
        /// alpha 控制
        /// </summary>
        [Rename("alpha 控制, x:最小值，y:最大值，z:大小控制速度，正负控制方向")]
        public Vector3 alpha = new(0, 1, 0.2f);


        private void OnEnable()
        {
            Init();
            leftFrame = totalFrame;
        }


        private void Update()
        {

#if UNITY_EDITOR
            if (editorEnable == false)
            {
                return;
            }
#endif
            if (totalFrame > 1)
            {
                if (leftFrame < 1)
                {
                    if (loop)
                    {
                        leftFrame = totalFrame;
                    }
                    else
                    {
                        enabled = false;
                        mRenderer.enabled = false;
                    }

                }

                float delta = Time.deltaTime;
                float frame = lifetime / totalFrame;
                if (currentTime >= frame)
                {
                    currentTime = 0;
                }
                else
                {
                    currentTime += delta;
                    return;
                }

                Play();
                leftFrame--;
            }
            else
            {
                Play();
            }
        }

        protected int leftFrame = 0;

        private void Awake()
        {
            Init();
            leftFrame = totalFrame;
        }


        public int totalFrame => frameCount * circle;

        public int frameCount => (int)(tilingOffset.x * tilingOffset.y);

        public float lengthU => broderU.y - broderU.x;

        public float lengthV => broderV.y - broderV.x;

        public float deltaX => lengthU / tilingOffset.x;

        public float deltaY => lengthV / tilingOffset.y;

        //------------------------------------------------------------------------------------------->>>>>
        public enum LAYOUT_ENUM
        {
            FORWARD,
            BACKWARD,
            RANDOM
        }

        private float currentTime = 0f;
        private float OffsetU = 0f;
        private float OffsetV = 0f;

        protected Vector4 _tilingOffset = new();

        private Vector2 broderU = new(0, 1);
        private Vector2 broderV = new(0, 1);


        private Material mBindMateral = null;

        private Renderer mRenderer = null;

        //-------------------------------------------------------------------------------------------<<<
        public void Init()
        {
            Renderer r = gameObject.GetComponent<Renderer>();
            r.enabled = true;
            mRenderer = r;
            if (mBindMateral == null)
            {
                if (r.sharedMaterials.Length <= BindIndex)
                {
                    BindIndex = r.sharedMaterials.Length - 1;
                }

                mBindMateral = r.sharedMaterials[BindIndex];
            }

            _tilingOffset = mBindMateral.GetVector("_MainTex_ST");
            if (totalFrame > 1)
            {
                _tilingOffset.x = deltaX;
                _tilingOffset.y = deltaY;
                _tilingOffset.z = broderU.x;
                _tilingOffset.w = 1f - broderV.x - deltaY;
            }

            _tintColor = mBindMateral.GetVector("_TintColor");

            if (mBindMateral != null)
            {
                mBindMateral.SetVector("_MainTex_ST", _tilingOffset);
            }
        }

        private Vector4 _tintColor;

        public void playForward()
        {
            if (totalFrame > 1)
            {
                _tilingOffset.z += deltaX;
                if (_tilingOffset.z >= broderU.y)
                {
                    _tilingOffset.z = broderU.x;
                    _tilingOffset.w -= deltaY;
                }

                if (_tilingOffset.w <= broderV.x)
                {
                    _tilingOffset.w = broderV.y;
                }
            }
            else
            {
                if (UVDir.x != 0)
                {
                    _tilingOffset.z += UVDir.x * Time.deltaTime;
                    if (_tilingOffset.z is > 1 or < (-1))
                    {
                        _tilingOffset.z = 0;
                    }
                }

                if (UVDir.y != 0)
                {
                    _tilingOffset.w += UVDir.y * Time.deltaTime;
                    if (_tilingOffset.w is > 1 or < (-1))
                    {
                        _tilingOffset.w = 0;
                    }
                }
            }

            if (alpha.z != 0)
            {
                _tintColor.w += alpha.z * Time.deltaTime;
                if (_tintColor.w > alpha.y)
                {
                    _tintColor.w = alpha.x;
                }

                if (_tintColor.w < alpha.x)
                {
                    _tintColor.w = alpha.y;
                }

                if (mBindMateral != null)
                {
                    mBindMateral.SetVector("_TintColor", _tintColor);
                }
            }


            if (mBindMateral != null)
            {
                mBindMateral.SetVector("_MainTex_ST", _tilingOffset);
            }
        }

        public void playBackward()
        {

        }

        public void playRandom()
        {

        }

        public void Play()
        {
            playForward();
        }
    }

}