/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.RunDesiredScene
{
    public class SelectSceneWindow : SelectSceneWindowBase
    {
        [NonSerialized] 
        private AdjustSize adjustSize = AdjustSize.HeightAndWidth; // 0 Not Adjust, 1 Adjust Only Height, 2 Adjust Height and Width

        public override bool canOpenStandalone => true;

        private void AdjustWindowSize(float h)
        {
            int widthOffset = h > 400 ? 20 : 0;
            h = Mathf.Min(h, 400);
            if (Math.Abs(position.height - h) > float.Epsilon || adjustSize == RunDesiredScene.AdjustSize.HeightAndWidth)
            {
                Rect r = position;
                if (adjustSize == RunDesiredScene.AdjustSize.HeightAndWidth)
                {
                    float w = CalculateWindowWidth() + widthOffset;
                    r.width = w;
                    adjustSize = RunDesiredScene.AdjustSize.Height;
                }

                r.height = h;
                position = r;
            }
        }

        private void CalculateAllItemsWidth(GUIStyle style, ref float w, int extraWidth)
        {
            foreach (SceneAsset scene in favoriteScenes)
            {
                Vector2 size = style.CalcSize(TempContent.Get(scene.name));
                w = Mathf.Max(w, size.x + extraWidth);
            }

            foreach (SceneAsset scene in buildScenes)
            {
                Vector2 size = style.CalcSize(TempContent.Get(scene.name));
                w = Mathf.Max(w, size.x + extraWidth);
            }

            if (showHiddenScenes || !SceneManager.hideOtherScenes)
            {
                foreach (SceneAsset scene in otherScenes)
                {
                    if (SceneManager.IsHidden(scene) && !showHiddenScenes) continue;

                    Vector2 size = style.CalcSize(TempContent.Get(scene.name));
                    w = Mathf.Max(w, size.x + extraWidth);
                }
            }
        }

        private void CalculateFilteredItemsWidth(GUIStyle style, ref float w, int extraWidth)
        {
            foreach (SceneAsset scene in filteredScenes)
            {
                if (SceneManager.IsHidden(scene) && !showHiddenScenes) continue;

                Vector2 size = style.CalcSize(TempContent.Get(scene.name));
                w = Mathf.Max(w, size.x + extraWidth);
            }
        }

        private float CalculateWindowWidth()
        {
            float w = 300;
            GUIStyle style = EditorStyles.label;
            const int MARGIN_WIDTH = 15;
            int extraWidth = style.margin.horizontal + 
                             SceneLineDrawer.PlayWidth +
                             SceneLineDrawer.SetStartWidth +
                             SceneLineDrawer.BookmarkWidth +
                             SceneLineDrawer.AdditiveWidth +
                             SceneLineDrawer.OpenWidth + 
                             MARGIN_WIDTH;
            
            if (showHiddenScenes) extraWidth += SceneLineDrawer.VisibilityWidth + EditorStyles.toolbarButton.margin.horizontal;

            if (filteredScenes != null) CalculateFilteredItemsWidth(style, ref w, extraWidth);
            else CalculateAllItemsWidth(style, ref w, extraWidth);

            return w;
        }

        protected override void DrawContent(int heightOffset)
        {
            base.DrawContent(heightOffset);
            
            Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(0));
            float h = rect.yMin + heightOffset;
            if (adjustSize > 0 && Event.current.type == EventType.Repaint) AdjustWindowSize(h);
        }

        public static void ShowWindow(Vector2 position)
        {
            SelectSceneWindow wnd = CreateInstance<SelectSceneWindow>();
            wnd.titleContent = new GUIContent("Select Scene");
            position = GUIUtility.GUIToScreenPoint(position);
            Vector2 size = new Vector2(300, 400);
            Rect rect = new Rect(position, size);
            wnd.minSize = new Vector2(1, 1);
            wnd.position = rect;
            wnd.ShowPopup();
            wnd.Focus();
        }

        protected override void OnGUI()
        {
            if (focusedWindow != this)
            {
                Close();
                return;
            }
            
            base.OnGUI();
        }

        public override void SoftClose()
        {
            base.SoftClose();
            
            Close();
        }

        public override void UpdateFilteredScenes()
        {
            adjustSize = AdjustSize.HeightAndWidth;
            
            base.UpdateFilteredScenes();
        }


        public override void UpdateScenes()
        {
            base.UpdateScenes();
            
            adjustSize = AdjustSize.HeightAndWidth;
        }
    }
}