using System;
using UnityEngine;

namespace ToastForUnity.Script.Core
{
    [Serializable]
    [CreateAssetMenu(fileName = "ToastSetting", menuName = "Toast/AddToast", order = 1)]
    public class ToastSetting : ScriptableObject
    {
        public string ToastName = "";
        public GameObject ToastPrefab;
        public float DestroyTime = -1;
        public bool DragAble;
        public bool DestroyWhenClicked;
    }
}