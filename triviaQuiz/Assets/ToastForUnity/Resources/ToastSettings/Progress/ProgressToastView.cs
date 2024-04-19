using System;
using System.Collections;
using ToastForUnity.Script.Core;
using UnityEngine;
using UnityEngine.UI;

namespace ToastForUnity.Resources.ToastSettings.Progress
{
    public class ProgressToastView : ToastPrefabBase
    {
        public Text Title;
        public Image Icon;
        public Image SliderColor;
        public Slider ProgressSlider;
        public Button CloseBtn;

        public bool ProgressRunOnStart = true;
        public bool DestroyWhenProgressComplete = false;

        public Sprite[] SpritesStatus;
        public Color[] ColorsStatus;

        void Start()
        {
            CloseBtn.onClick.AddListener(() => { Destroy(gameObject); });
        }

        public override void Initialize(ToastModelBase toastModel)
        {
            if (!(toastModel is ProgressToastModel progressModel)) return;

            ProgressRunOnStart = progressModel.ProgressRunOnStart;
            DestroyWhenProgressComplete = progressModel.DestroyWhenProgressComplete;
            
            Title.text = progressModel.Title;
            ProgressSlider.value = 0;
            SettingStatus(progressModel.Status);

            if (progressModel.ProgressValueChanged != null)
                ProgressSlider.onValueChanged.AddListener(progressModel.ProgressValueChanged);

            if (ProgressRunOnStart)
                StartCoroutine(RunProgress(progressModel.ProgressDone));
        }

        private void SettingStatus(ProgressToastStatus status)
        {
            var statusIndex = (int)status;
            Icon.sprite = SpritesStatus[statusIndex];
            Icon.color = ColorsStatus[statusIndex];
            SliderColor.color = ColorsStatus[statusIndex];
        }

        IEnumerator RunProgress(Action doneAction)
        {
            while (ProgressSlider.value < 1)
            {
                ProgressSlider.value += 0.1f;
                yield return new WaitForSeconds(0.1f);
            }

            doneAction?.Invoke();

            if (DestroyWhenProgressComplete)
                Destroy(gameObject);
        }
    }
}