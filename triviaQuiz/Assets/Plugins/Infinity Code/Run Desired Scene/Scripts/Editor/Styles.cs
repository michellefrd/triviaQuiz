/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using UnityEditor;
using UnityEngine;

namespace InfinityCode.RunDesiredScene
{
    public static class Styles
    {
        private static GUIStyle _centeredLabel;
        private static GUIStyle _dropdown;
        private static GUIStyle _toolbarButtonStyle;

        public static GUIStyle centeredLabel
        {
            get
            {
                if (_centeredLabel == null)
                {
                    _centeredLabel = new GUIStyle(EditorStyles.label)
                    {
                        alignment = TextAnchor.MiddleCenter
                    };
                }

                return _centeredLabel;
            }
        }
        
        public static GUIStyle dropdown
        {
            get
            {
                if (_dropdown == null)
                {
                    _dropdown = "Dropdown";
                    _dropdown.fixedHeight = 20;
                    _dropdown.margin = new RectOffset(0, 0, 0, 0);
                }
                return _dropdown;
            }
        }
        
        public static GUIStyle toolbarButtonStyle
        {
            get
            {
                if (_toolbarButtonStyle != null) return _toolbarButtonStyle;
                
                ToolbarButtonStyle style = SceneManager.toolbarButtonStyle;
                    
                if (style == ToolbarButtonStyle.AutoDetect)
                {
#if UEE
                    _toolbarButtonStyle = dropdown;
#else
                    _toolbarButtonStyle = EditorStyles.toolbarDropDown;
#endif
#if !UNITY_2021_3_OR_NEWER
                    _toolbarButtonStyle.fixedHeight = 22;
#endif
                }
                else if (style == ToolbarButtonStyle.DropDown)
                {
                    _toolbarButtonStyle = dropdown;
#if !UNITY_2021_3_OR_NEWER
                    _toolbarButtonStyle.fixedHeight = 22;
#endif
                }
                else if (style == ToolbarButtonStyle.ToolbarButton)
                {
                    _toolbarButtonStyle = new GUIStyle(EditorStyles.toolbarButton);
#if UNITY_2021_3_OR_NEWER
                    _toolbarButtonStyle.fixedHeight = 18;
#else 
                    _toolbarButtonStyle.fixedHeight = 20;
#endif
                    _toolbarButtonStyle.margin = new RectOffset(4, 4, 1, 1);
                }
                else 
                {
                    _toolbarButtonStyle = new GUIStyle(EditorStyles.toolbarDropDown);
#if !UNITY_2021_3_OR_NEWER
                    _toolbarButtonStyle.fixedHeight = 22;
#endif
                }

                return _toolbarButtonStyle;
            }
        }
        
        public static void ResetToolbarButtonStyle()
        {
            _toolbarButtonStyle = null;
        }
    }
}