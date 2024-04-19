/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Collections.Generic;
using System.Linq;
using InfinityCode.RunDesiredScene.UnityTypes;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.RunDesiredScene
{
    public abstract class SelectSceneWindowBase : EditorWindow
    {
        public static bool isDirty;
        
        [NonSerialized] 
        public bool focusOnTextField = true;
        
        [NonSerialized] 
        public string filter;
        
        [SerializeField] 
        public bool showHiddenScenes;
        
        [NonSerialized]
        protected SceneAsset[] buildScenes;

        [NonSerialized]
        protected SceneAsset[] favoriteScenes;

        [NonSerialized]
        protected SceneAsset[] otherScenes;

        [NonSerialized]
        protected SceneAsset[] filteredScenes;

        [NonSerialized]
        protected Vector2 scrollPosition;

        public virtual bool canOpenStandalone => false;

        protected virtual void DrawContent(int heightOffset)
        {
            if (filteredScenes != null)
            {
                DrawGroup(filteredScenes);
            }
            else
            {
                SceneLineDrawer.DrawCurrentItem(this);
                DrawGroup(favoriteScenes, "Favorites");
                DrawGroup(buildScenes, "In Build");
                if (!SceneManager.hideOtherScenes || showHiddenScenes) DrawGroup(otherScenes, "Other Scenes", true);
            }
        }

        private void DrawGroup(SceneAsset[] scenes, string label = null, bool isOtherScenes = false)
        {
            if (scenes == null || scenes.Length == 0) return;
            if (!string.IsNullOrEmpty(label)) DrawGroupLabel(label, isOtherScenes);

            foreach (SceneAsset scene in scenes)
            {
                SceneLineDrawer.DrawItem(this, scene);
            }
        }

        private void DrawGroupLabel(string label, bool isOtherScenes)
        {
            EditorGUILayout.BeginHorizontal();

            DrawShowOtherScenesButton(isOtherScenes);

            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);

            EditorGUILayout.EndHorizontal();

            Rect rect = EditorGUILayout.GetControlRect(false, 1);
            rect.xMin -= 14;
            rect.xMax += 14;
            EditorGUI.DrawRect(rect, EditorGUIUtility.isProSkin ? new Color32(32, 32, 32, 255) : new Color32(127, 127, 127, 255));
        }

        private static void DrawNewVersionNotification(ref int heightOffset)
        {
            if (!Updater.hasNewVersion) return;

            Color color = GUI.backgroundColor;
            GUI.backgroundColor = Color.red;
            GUIContent updateContent = TempContent.Get(Icons.updateAvailable, "Update Available. Click to open the updater.");
            updateContent.text = "Update Available. Click to open the updater.";
            if (GUILayout.Button(updateContent, EditorStyles.toolbarButton)) Updater.OpenWindow();
            GUI.backgroundColor = color;
            heightOffset += 20;
        }

        private void DrawShowOtherScenesButton(bool isOtherScenes)
        {
            if (!isOtherScenes || !showHiddenScenes) return;
            
            if (SceneManager.hideOtherScenes)
            {
                GUIContent content = TempContent.Get(Icons.hidden, "Show Other Scenes");
                if (GUILayout.Button(content, EditorStyles.label, GUILayout.Width(16)))
                {
                    SceneManager.hideOtherScenes = false;
                    SceneManager.Save();
                }
            }
            else
            {
                GUIContent content = TempContent.Get(Icons.visible, "Hide Other Scenes");
                if (GUILayout.Button(content, EditorStyles.label, GUILayout.Width(16)))
                {
                    SceneManager.hideOtherScenes = true;
                    SceneManager.Save();
                }
            }
        }
        
        private void OnDisable()
        {
            buildScenes = null;
            favoriteScenes = null;
            otherScenes = null;
        }

        private void OnEnable()
        {
            Updater.CheckNewVersionAvailable();
            UpdateScenes();
        }

        protected virtual void OnGUI()
        {
            SceneLineDrawer.Initialize();

            if (isDirty) UpdateScenes();

            int heightOffset = 25;
            DrawNewVersionNotification(ref heightOffset);
            WindowToolbarDrawer.DrawToolbar(this);

            scrollPosition = EditorGUILayoutRef.BeginVerticalScrollView(scrollPosition);
            DrawContent(heightOffset);
            EditorGUILayout.EndScrollView();

            if (isDirty) UpdateScenes();
        }

        public virtual void SoftClose()
        {
            
        }
        
        public virtual void UpdateFilteredScenes()
        {
            if (string.IsNullOrEmpty(filter))
            {
                filteredScenes = null;
                return;
            }

            string pattern = SearchableItem.GetPattern(filter);

            IEnumerable<SceneAsset> query = buildScenes.Where(s => s != null && SearchableItem.Match(pattern, s.name))
                .Concat(favoriteScenes.Where(s => s != null && SearchableItem.Match(pattern, s.name)));
            if (!SceneManager.hideOtherScenes)
            {
                query = query.Concat(otherScenes.Where(s => s != null && SearchableItem.Match(pattern, s.name)));
            }

            filteredScenes = query.ToArray();
        }

        public virtual void UpdateScenes()
        {
            SceneManager.ClearCache();

            favoriteScenes = SceneManager.GetFavoriteScenes();
            buildScenes = SceneManager.GetBuildScenes();
            otherScenes = SceneManager.GetOtherScenes();

            if (!showHiddenScenes)
            {
                favoriteScenes = favoriteScenes.Where(s => !SceneManager.IsHidden(s)).ToArray();
                buildScenes = buildScenes.Where(s => !SceneManager.IsHidden(s)).ToArray();
                otherScenes = otherScenes.Where(s => !SceneManager.IsHidden(s)).ToArray();
            }

            isDirty = false;
        }
    }
}