using System.Collections.Generic;
using ToastForUnity.Script.Utility;
using UnityEngine;

namespace ToastForUnity.Script.Core
{
    public partial class Toast : MonoBehaviour
    {
        private static Transform DefaultParent = null;
        
        private static readonly string GlobalToastSetupPath = "ToastSettings/GlobalToastSetup";

        private static readonly Dictionary<string, ToastSetting>
            ToastPrefabDic = new Dictionary<string, ToastSetting>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void StartToast()
        {
            var globalSetup = (ToastSetup)UnityEngine.Resources.Load(GlobalToastSetupPath);
            AddGlobalSetupToastDictionary(globalSetup);
            FindDefaultParent();
        }

        private static void FindDefaultParent()
        {
            if (DefaultParent != null) return;
            
            var monoList = GameObject.FindObjectsOfType<MonoBehaviour>();
            foreach (var mono in monoList)
            {
                if (!mono.TryGetComponent(typeof(Canvas), out var canvas)) continue;
                
                DefaultParent = mono.transform;
                return;
            }
        }


        public static ToastSetting GetToast(string toastName)
        {
            ToastPrefabDic.TryGetValue(toastName, out var toastSetting);
            return toastSetting;
        }

        public static void SetDefaultParent(Transform parent)
        {
            DefaultParent = parent;
        }

        public static bool UpdateToastSetting(string toastName, ToastSetting newToastSetting)
        {
            if (ToastPrefabDic.TryGetValue(toastName, out _))
            {
                ToastPrefabDic[toastName] = newToastSetting;
                return true;
            }

            Debug.LogError("Toast Name Not Found");
            return false;
        }


        public static ToastSetting PopOut<TToastView>(string toastName, ToastModelBase model, Transform parent = null)
            where TToastView : ToastPrefabBase
        {
            ToastPrefabDic.TryGetValue(toastName, out var toastSetting);
            if (toastSetting == null) return null;

            if (parent == null)
            {
                parent = DefaultParent;
            }

            if (parent == null)
            {
                Debug.LogWarning("Toast Create Error: Can't Find Spawn Parent");
                return toastSetting;
            }

            var toast = Instantiate(toastSetting.ToastPrefab, parent.transform);
            toast.GetComponent<TToastView>().Initialize(model);

            if (toastSetting.DragAble)
                toast.AddComponent<DraggableObj>();

            if (toastSetting.DestroyWhenClicked)
                toast.AddComponent<DestroyObjWhenClicked>();

            if (toastSetting.DestroyTime >= 0)
                Destroy(toast, toastSetting.DestroyTime);

            return toastSetting;
        }

        private static void AddGlobalSetupToastDictionary(ToastSetup toastSetup)
        {
            foreach (var toastSettingList in toastSetup.ToastSettingList)
            {
                foreach (var toastSetting in toastSettingList.ToastSettings)
                {
                    ToastPrefabDic.Add(toastSetting.ToastName, toastSetting);
                }
            }
        }
    }
}