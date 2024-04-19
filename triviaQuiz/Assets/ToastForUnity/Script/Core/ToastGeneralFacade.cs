using System;
using UnityEngine;
using ToastForUnity.Script.Enum;
using ToastForUnity.Resources.ToastSettings.General.DefaultToast;
using ToastForUnity.Resources.ToastSettings.General.DialogToast;
using ToastForUnity.Resources.ToastSettings.General.WindowPanel;

namespace ToastForUnity.Script.Core
{
    public partial class Toast
    {
        public static void PopOut(string text, Transform parent = null)
        {
            PopOut<DefaultToast>("DefaultToast", new DefaultToastModel()
            {
                PopText = text,
                Color = new Color(0.3843138f, 0.9333334f, 0.5960785f, 0.9568627f)
            }, parent);
        }

        public static void PopOut(string text, ToastStatus status, Transform parent = null)
        {
            Color color;
            switch (status)
            {
                case ToastStatus.Normal:
                    color = new Color(0.39f, 0.41f, 0.38f, 0.95f);
                    break;
                case ToastStatus.Success:
                    color = new Color(0.3843138f, 0.9333334f, 0.5960785f, 0.9568627f);
                    break;
                case ToastStatus.Warning:
                    color = new Color(0.93f, 0.88f, 0.29f, 0.95f);
                    break;
                case ToastStatus.Danger:
                    color = new Color(0.93f, 0.09f, 0.15f, 0.95f);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }

            PopOut<DefaultToast>("DefaultToast", new DefaultToastModel()
            {
                PopText = text,
                Color = color,
            }, parent);
        }

        public static void PopOut(string text, Color color, Transform parent = null)
        {
            PopOut<DefaultToast>("DefaultToast", new DefaultToastModel()
            {
                PopText = text,
                Color = color,
            }, parent);
        }

        public static void WindowPopOut(WindowToastModel windowToastModel, Transform parent = null)
        {
            PopOut<WindowToast>("WindowToast", windowToastModel, parent);
        }

        public static void DialogPopOut(string dialogText, Transform parent = null)
        {
            var toast = Toast.PopOut<DialogToastView>("DialogToast", new DialogToastModel()
            {
                DialogText = dialogText
            }, parent);
        }
    }
}