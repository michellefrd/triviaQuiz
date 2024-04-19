/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using UnityEngine;

namespace InfinityCode.RunDesiredScene
{
    public class StandaloneSceneWindow : SelectSceneWindowBase
    {
        public static void ShowWindow(Vector2 position)
        {
            StandaloneSceneWindow wnd = CreateInstance<StandaloneSceneWindow>();
            wnd.titleContent = new GUIContent("Run Scene");
            position = GUIUtility.GUIToScreenPoint(position);
            Vector2 size = new Vector2(300, 400);
            Rect rect = new Rect(position, size);
            wnd.minSize = new Vector2(1, 1);
            wnd.position = rect;
            wnd.Show();
            wnd.Focus();
        }
    }
}