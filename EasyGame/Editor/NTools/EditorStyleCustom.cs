using UnityEngine;

namespace Easy
{
    public abstract class EditorStyleCustom
    {
        private static GUIStyle _sliderStyle = null;
        public static GUIStyle SliderStyle
        {
            get
            {
                if (_sliderStyle == null)
                {
                    _sliderStyle = new GUIStyle(GUI.skin.horizontalSlider);
                    _sliderStyle.fixedHeight = 20f; // 设置slider高度
                    _sliderStyle.normal.textColor = Color.white; // 设置文本颜色
                    //_sliderStyle.active.background = CustomTexture; // 设置活动状态下的背景纹理
                    //_sliderStyle.hover.background = CustomHoverTexture; // 设置悬停状态下的背景纹理
                }

                return _sliderStyle;
            }
        }
        
        
    }
}