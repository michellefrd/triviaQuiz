using UnityEditor;

// ReSharper disable once CheckNamespace
namespace ToastForUnity.Script.Core
{
    public class ToastMenuItem : EditorWindow
    {
        private Editor _editor;
        private static ToastSetup _globalToastSetup;
        private const string GlobalToastSetupPath = "ToastSettings/GlobalToastSetup";


        [MenuItem("Window/Toast For Unity/Set-up Window")]
        public static void ShowToastWindow()
        {
            var window = GetWindow<ToastMenuItem>(true, "ToastForUnity Set-Up Window", true);
            window._editor = Editor.CreateEditor(_globalToastSetup);
        }

        private void OnEnable()
        {
            _globalToastSetup = (ToastSetup)UnityEngine.Resources.Load(GlobalToastSetupPath);
        }

        private void OnGUI()
        {
            _editor.OnInspectorGUI();
        }
    }
}