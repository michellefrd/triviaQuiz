/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InfinityCode.RunDesiredScene
{
    public static class SceneLineDrawer
    {
        public const int AdditiveWidth = 20;
        public const int BookmarkWidth = 25;
        public const int OpenWidth = 30;
        public const int PlayWidth = 20;
        public const int SetStartWidth = 20;
        public const int VisibilityWidth = 20;
        
        private const double DoubleClickTime = 0.3;
        
        private static GUIContent openContent;
        private static GUIContent playButton;
        private static GUIContent starActiveContent;
        private static GUIContent starInactiveContent;
        private static GUIContent setStartSceneButton = new GUIContent("", "Set Start Scene");

        private static double lastClickTime;
        private static bool isInitialized;

        public static bool AskForSave(params Scene[] scenes)
        {
            if (scenes.Length == 0) return true;

            List<string> paths = new List<string>();

            for (int i = 0; i < scenes.Length; i++)
            {
                Scene scene = scenes[i];
                if (scene.isDirty) paths.Add(scene.path);
            }

            if (paths.Count == 0) return true;

            string pathStr = string.Join("\n", paths);
            if (pathStr.Length == 0) pathStr = "Untitled";

            int result = EditorUtility.DisplayDialogComplex("Scene(s) Have Been Modified", "Do you want to save the changes you made in the scenes:\n" + pathStr + "\n\nYour changes will be lost if you don't save them.", "Save", "Don't Save", "Cancel");
            if (result == 2) return false;

            if (result == 0)
            {
                for (int i = 0; i < scenes.Length; i++)
                {
                    Scene scene = scenes[i];
                    if (scene.isDirty) EditorSceneManager.SaveScene(scene);
                }
            }

            return true;
        }

        private static void DrawAdditiveButton(SceneAsset scene)
        {
            bool isAdditive = SceneManager.IsAdditive(scene);
            EditorGUI.BeginChangeCheck();
            isAdditive = GUILayout.Toggle(isAdditive, "A", EditorStyles.toolbarButton, GUILayout.Width(AdditiveWidth));
            if (EditorGUI.EndChangeCheck())
            {
                if (isAdditive) SceneManager.AddAdditive(scene);
                else SceneManager.RemoveAdditive(scene);
                SelectSceneWindow.isDirty = true;
            }
        }

        public static void DrawCurrentItem(SelectSceneWindowBase window)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (GUILayout.Button(playButton, EditorStyles.toolbarButton, GUILayout.Width(PlayWidth)))
            {
                EditorSceneManager.playModeStartScene = null;
                EditorApplication.isPlaying = true;
                window.SoftClose();
            }

            bool isStartScene = SceneManager.IsStartScene(null);
            EditorGUI.BeginChangeCheck();
            GUILayout.Toggle(isStartScene, setStartSceneButton, EditorStyles.toggle, GUILayout.Width(SetStartWidth));
            if (EditorGUI.EndChangeCheck())
            {
                SceneManager.SetStartScene(null);
                SelectSceneWindow.isDirty = true;
                GUI.changed = true;
            }

            GUILayout.Label("Current Scene", EditorStyles.label);

            EditorGUILayout.EndHorizontal();
        }

        private static void DrawFavoriteButton(SceneAsset scene)
        {
            EditorGUI.BeginChangeCheck();
            bool isFavorite = SceneManager.IsFavorite(scene);
            GUIContent starContent = isFavorite ? starActiveContent : starInactiveContent;
            bool v = GUILayout.Toggle(isFavorite, starContent, EditorStyles.toolbarButton, GUILayout.Width(BookmarkWidth));
            if (EditorGUI.EndChangeCheck())
            {
                if (v) SceneManager.AddFavorite(scene);
                else SceneManager.RemoveFavorite(scene);
                SelectSceneWindow.isDirty = true;
            }
        }

        private static void DrawHidden(SelectSceneWindowBase window, SceneAsset scene)
        {
            if (!window.showHiddenScenes) return;
            
            if (SceneManager.IsHidden(scene))
            {
                GUIContent content = TempContent.Get(Icons.hidden, "Make Unhidden");
                if (GUILayout.Button(content, EditorStyles.label, GUILayout.Width(VisibilityWidth)))
                {
                    SceneManager.ToggleHidden(scene);
                }
            }
            else
            {
                GUIContent content = TempContent.Get(Icons.visible, "Make Hidden");
                if (GUILayout.Button(content, EditorStyles.label, GUILayout.Width(VisibilityWidth)))
                {
                    SceneManager.ToggleHidden(scene);
                }
            }
        }

        public static void DrawItem(SelectSceneWindowBase window, SceneAsset scene)
        {
            if (scene == null) return;
            string label = scene.name;

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            DrawPlayButton(window, scene);
            DrawStartScene(scene);
            DrawHidden(window, scene);
            
            string scenePath = AssetDatabase.GetAssetPath(scene);
            DrawLabel(window, scene, scenePath, label);

            GUILayout.Space(5);

            DrawFavoriteButton(scene);
            DrawAdditiveButton(scene);
            DrawOpenButton(window, scenePath);

            EditorGUILayout.EndHorizontal();
        }

        private static void DrawLabel(SelectSceneWindowBase window, SceneAsset scene, string scenePath, string label)
        {
            StringBuilder sb = StaticStringBuilder.Start();
            sb.Append(scenePath);
            sb.Append("\n\n(Click to Ping)\n(Double Click to Open)\n(Shift+Double Click to Open Additive)");
            GUIContent content = TempContent.Get(label, sb.ToString());

            ButtonEvent buttonEvent = GUILayoutUtils.Button(content, EditorStyles.label);
            Event e = Event.current;

            if (buttonEvent == ButtonEvent.click) ProcessItemClick(window, scene, e, scenePath);
        }

        private static void DrawOpenButton(SelectSceneWindowBase window, string scenePath)
        {
            if (EditorApplication.isPlaying) return;
            
            ButtonEvent openButtonEvent = GUILayoutUtils.Button(openContent, EditorStyles.toolbarButton, GUILayout.Width(OpenWidth));
            if (openButtonEvent == ButtonEvent.click)
            {
                if (Event.current.modifiers == EventModifiers.None)
                {
                    window.SoftClose();

                    if (AskForSave(EditorSceneManager.GetActiveScene()))
                    {
                        EditorSceneManager.OpenScene(scenePath);
                    }
                }
                else
                {
                    EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                }
            }
        }

        private static void DrawPlayButton(SelectSceneWindowBase window, SceneAsset scene)
        {
            if (!GUILayout.Button(playButton, EditorStyles.toolbarButton, GUILayout.Width(PlayWidth))) return;

            EditorSceneManager.playModeStartScene = scene;
            
            if (!EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = true;
            }
            else
            {
                EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
                EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
                EditorApplication.isPlaying = false;
            }
            
            window.SoftClose();
        }

        private static void DrawStartScene(SceneAsset scene)
        {
            bool isStartScene = SceneManager.IsStartScene(scene);
            EditorGUI.BeginChangeCheck();
            isStartScene = GUILayout.Toggle(isStartScene, setStartSceneButton, EditorStyles.toggle, GUILayout.Width(SetStartWidth));
            if (EditorGUI.EndChangeCheck())
            {
                if (isStartScene) SceneManager.SetStartScene(scene);
                else SceneManager.SetStartScene(null);
            }
        }

        public static void Initialize()
        {
            if (isInitialized) return;
            
            isInitialized = true;
            
            playButton = new GUIContent(EditorIconContent.playButton.image, "Play Scene");
            starActiveContent = new GUIContent(Icons.starYellow, "Remove from Favorites");
            starInactiveContent = new GUIContent(EditorGUIUtility.isProSkin ? Icons.starWhite : Icons.starBlack, "Add to Favorites");
            openContent = new GUIContent(Icons.sceneLoadIn, "Click - Open Scene\nShift+Click - Open Additive");
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
                EditorApplication.isPlaying = true;
            }
        }

        private static void ProcessItemClick(SelectSceneWindowBase window, SceneAsset scene, Event e, string scenePath)
        {
            if (e.button == 0) ProcessItemLeftClick(window, scene, e, scenePath);
            else if (e.button == 1) ShowSceneContextMenu(window, scene);
        }

        private static void ProcessItemLeftClick(SelectSceneWindowBase window, SceneAsset scene, Event e, string scenePath)
        {
            if (EditorApplication.timeSinceStartup - lastClickTime < DoubleClickTime)
            {
                if (!e.shift)
                {
                    window.SoftClose();

                    if (AskForSave(EditorSceneManager.GetActiveScene()))
                    {
                        EditorSceneManager.OpenScene(scenePath);
                    }
                }
                else EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            }
            else
            {
                lastClickTime = EditorApplication.timeSinceStartup;
                EditorGUIUtility.PingObject(scene);
            }
        }

        private static void ShowSceneContextMenu(SelectSceneWindowBase window, SceneAsset scene)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Open"), false, () =>
            {
                window.SoftClose();
                
                if (AskForSave(EditorSceneManager.GetActiveScene()))
                {
                    EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(scene));
                }
            });
            menu.AddItem(new GUIContent("Open Additive"), false, () => EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(scene), OpenSceneMode.Additive));
            menu.AddItem(new GUIContent("Select"), false, () => Selection.activeObject = scene);
            menu.AddItem(new GUIContent("Hide"), SceneManager.IsHidden(scene), () => ToggleHidden(scene));
            menu.ShowAsContext();
        }

        private static void ToggleHidden(SceneAsset scene)
        {
            SceneManager.ToggleHidden(scene);
            SelectSceneWindow.isDirty = true;
        }
    }
}