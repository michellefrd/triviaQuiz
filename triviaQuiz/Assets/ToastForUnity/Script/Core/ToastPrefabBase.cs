using UnityEngine;

namespace ToastForUnity.Script.Core
{
    public abstract class ToastPrefabBase :MonoBehaviour
    {
        public abstract void Initialize(ToastModelBase toastModel);

        public void DisableAnimator() {
            transform.gameObject.GetComponent<Animator>().enabled = false;
        }

    }
}
