/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using UnityEditor;
using UnityEngine;

namespace InfinityCode.RunDesiredScene
{
    public static class EditorIconContent
    {
        private static GUIContent _playButton;
        private static GUIContent _refresh;
        private static GUIContent _sceneLoadIn;
        private static GUIContent _settings;

        public static GUIContent playButton
        {
            get
            {
                if (_playButton == null) _playButton = EditorGUIUtility.IconContent("PlayButton");
                return _playButton;
            }
        }

        public static GUIContent refresh
        {
            get
            {
                if (_refresh == null) _refresh = EditorGUIUtility.IconContent("Refresh");
                return _refresh;
            }
        }

        public static GUIContent sceneLoadIn
        {
            get
            {
                if (_sceneLoadIn == null) _sceneLoadIn = EditorGUIUtility.IconContent("SceneLoadIn");
                return _sceneLoadIn;
            }
        }

        public static GUIContent settings
        {
            get
            {
                if (_settings == null) _settings = EditorGUIUtility.IconContent("SettingsIcon");
                return _settings;
            }
        }
    }
}