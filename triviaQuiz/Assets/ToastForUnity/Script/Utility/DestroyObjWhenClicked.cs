using UnityEngine;
using UnityEngine.EventSystems;

namespace ToastForUnity.Script.Utility
{
    public class DestroyObjWhenClicked : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            Destroy(gameObject);
        }
    }
}