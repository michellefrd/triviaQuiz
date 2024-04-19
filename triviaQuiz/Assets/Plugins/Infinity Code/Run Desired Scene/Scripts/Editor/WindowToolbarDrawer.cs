/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using UnityEditor;
using UnityEngine;

namespace InfinityCode.RunDesiredScene
{
    public static class WindowToolbarDrawer
    {
        private static GUIContent showHiddenContent;
        
        private static void DrawHelp()
        {
            if (!GUILayout.Button("?", EditorStyles.toolbarButton, GUILayout.Width(20))) return;

            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Welcome"), false, Welcome.OpenWindow);
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Product Page"), false, Links.OpenHomepage);
            menu.AddItem(new GUIContent("Documentation"), false, Links.OpenDocumentation);
            menu.AddItem(new GUIContent("Videos"), false, Links.OpenYouTube);
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Support"), false, Links.OpenSupport);
            menu.AddItem(new GUIContent("Discord"), false, Links.OpenDiscord);
            menu.AddItem(new GUIContent("Forum"), false, Links.OpenForum);
            menu.AddItem(new GUIContent("Check Updates"), false, Updater.OpenWindow);
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Rate and Review"), false, Welcome.RateAndReview);
            menu.AddItem(new GUIContent("About"), false, About.OpenWindow);

            menu.ShowAsContext();
        }
        
        private static void DrawHidden(SelectSceneWindowBase window)
        {
            EditorGUI.BeginChangeCheck();
            window.showHiddenScenes = GUILayout.Toggle(window.showHiddenScenes, showHiddenContent, EditorStyles.toolbarButton, GUILayout.Width(20));
            if (EditorGUI.EndChangeCheck())
            {
                window.UpdateScenes();
                if (!string.IsNullOrEmpty(window.filter)) window.UpdateFilteredScenes();
            }
        }

        private static void DrawOpenStandalone(SelectSceneWindowBase window)
        {
            GUIContent content = TempContent.Get(Icons.pin, "Open in Standalone Window");
            if (!GUILayout.Button(content, EditorStyles.toolbarButton, GUILayout.Width(20))) return;
            StandaloneSceneWindow.ShowWindow(window.position.position);
        }

        private static void DrawSettings(SelectSceneWindowBase window)
        {
            if (!GUILayout.Button(EditorIconContent.settings, EditorStyles.toolbarButton, GUILayout.Width(20))) return;

            GenericMenu menu = new GenericMenu();
            DrawStylesMenu(menu);

            menu.AddItem(new GUIContent("Hide Other Scenes"), SceneManager.hideOtherScenes, () =>
            {
                SceneManager.hideOtherScenes = !SceneManager.hideOtherScenes;
                SceneManager.Save();
                window.UpdateScenes();
            });
            
            menu.AddItem(new GUIContent("Refresh Items"), false, window.UpdateScenes);

            menu.ShowAsContext();
        }

        private static void DrawStylesMenu(GenericMenu menu)
        {
            ToolbarButtonStyle style = SceneManager.toolbarButtonStyle;

            menu.AddItem(new GUIContent("Toolbar Button Style/Auto Detect"), style == ToolbarButtonStyle.AutoDetect, () => SelectStyle(ToolbarButtonStyle.AutoDetect));
            menu.AddItem(new GUIContent("Toolbar Button Style/ToolbarDropDown"), style == ToolbarButtonStyle.ToolbarDropDown, () => SelectStyle(ToolbarButtonStyle.ToolbarDropDown));
            menu.AddItem(new GUIContent("Toolbar Button Style/DropDown"), style == ToolbarButtonStyle.DropDown, () => SelectStyle(ToolbarButtonStyle.DropDown));
            menu.AddItem(new GUIContent("Toolbar Button Style/ToolbarButton"), style == ToolbarButtonStyle.ToolbarButton, () => SelectStyle(ToolbarButtonStyle.ToolbarButton));
        }

        public static void DrawToolbar(SelectSceneWindowBase window)
        {
            if (showHiddenContent == null) showHiddenContent = new GUIContent(Icons.hidden, "Show Hidden Scenes");
            showHiddenContent.tooltip = window.showHiddenScenes ? "Hide Hidden Scenes" : "Show Hidden Scenes";
            
            GUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            GUI.SetNextControlName("SSFilterTextField");
            window.filter = EditorGUILayout.TextField(window.filter, EditorStyles.toolbarSearchField);
            if (EditorGUI.EndChangeCheck()) window.UpdateFilteredScenes();

            if (window.focusOnTextField && Event.current.type == EventType.Repaint)
            {
                GUI.FocusControl("SSFilterTextField");
                window.focusOnTextField = false;
            }

            DrawHidden(window);
            DrawSettings(window);

            if (window.canOpenStandalone) DrawOpenStandalone(window);

            DrawHelp();

            GUILayout.EndHorizontal();
        }

        private static void SelectStyle(ToolbarButtonStyle style)
        {
            SceneManager.toolbarButtonStyle = style;
            Styles.ResetToolbarButtonStyle();
            SceneManager.Save();
        }
    }
}