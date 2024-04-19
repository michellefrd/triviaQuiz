using System;
using ToastForUnity.Script.Enum;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class ParentController : MonoBehaviour
{
    public bool ShowChildImage =true;
    private bool ShowChildImagePrev =true;

    private void Update()
    {
        if (ShowChildImage == ShowChildImagePrev) return;
        ChildImageToggle();
        ShowChildImagePrev = ShowChildImage;
    }

    private void ChildImageToggle()
    {
        var images = transform.GetComponentsInChildren(typeof(Image), true);

        foreach (var image in images) {
            image.GetComponent<Image>().enabled = ShowChildImage;
        }
    }

    public Transform GetParent(ToastPosition position)
    {
        switch (position)
        {
            case ToastPosition.Random:
                return transform.GetChild(Random.Range(0, transform.childCount));
            case ToastPosition.TopLeft:
                return transform.GetChild(0);
            case ToastPosition.TopCenter:
                return transform.GetChild(1);
            case ToastPosition.TopRight:
                return transform.GetChild(2);
            case ToastPosition.MiddleLeft:
                return transform.GetChild(3);
            case ToastPosition.MiddleCenter:
                return transform.GetChild(4);
            case ToastPosition.MiddleRight:
                return transform.GetChild(5);
            case ToastPosition.BottomLeft:
                return transform.GetChild(6);
            case ToastPosition.BottomCenter:
                return transform.GetChild(7);
            case ToastPosition.BottomRight:
                return transform.GetChild(8);
            default:
                throw new ArgumentOutOfRangeException(nameof(position), position, null);
        };
    }
}
