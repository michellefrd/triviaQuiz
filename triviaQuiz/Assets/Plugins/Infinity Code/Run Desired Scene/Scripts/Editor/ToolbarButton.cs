/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using UnityEditor;
using UnityEngine;

namespace InfinityCode.RunDesiredScene
{
    [InitializeOnLoad]
    public static class ToolbarButton
    {
        static ToolbarButton()
        {
            ToolbarManager.AddLeftToolbar("RunDesiredScene", DrawButton, 1000);
        }

        private static void DrawButton()
        {
            DrawEditorButton();
            
            if (EditorApplication.isPlaying) DrawRuntimeButton();
        }

        private static void DrawRuntimeButton()
        {
            GUIContent content = TempContent.Get(EditorIconContent.refresh);
            if (GUILayoutUtils.Button(content, EditorStyles.toolbarButton, false) != ButtonEvent.click) return;

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorApplication.isPlaying = false;
        }

        private static void DrawEditorButton()
        {
            GUIContent content = TempContent.Get(SceneManager.GetStartSceneName());
            if (GUILayoutUtils.Button(content, Styles.toolbarButtonStyle, false) != ButtonEvent.click) return;
            
            Rect rect = GUILayoutUtils.lastRect;
            SelectSceneWindow.ShowWindow(rect.position + new Vector2(0, rect.height + 5));
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
                EditorApplication.isPlaying = true;
            }
        }
    }
}
