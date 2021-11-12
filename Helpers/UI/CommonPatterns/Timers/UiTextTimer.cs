using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniversalUnity.Helpers.Logs;
using UniversalUnity.Helpers.UI.BaseUiElements.BaseElements;

namespace UniversalUnity.Helpers.UI.CommonPatterns.Timers
{
    public class UiTextTimer : BaseUiTimer
    {
        [SerializeField] protected BaseTextUiElement timerText = null;
        
        public bool animateNumberChanging = true;
        public string timeFormat = @"mm\:ss";

        protected override void InheritAwake()
        {
            timerText.Text = new TimeSpan(0).ToString(timeFormat);
        }
        
        protected override async UniTask UiHandleTimer(float durationInMillis, CancellationToken cancellationToken)
        {
            timerText.Enable().Forget();

            timerText.Text = TimeSpan.FromMilliseconds(durationInMillis).ToString(timeFormat);
            await TimerProcess(durationInMillis - durationInMillis % 1000, cancellationToken)
                .SuppressCancellationThrow();
        }
        
        protected virtual async UniTask TimerProcess(float timerStartMillis, CancellationToken cancellationToken)
        {
            while (timerStartMillis > 0)
            {
                await UniTask.Delay(1000, cancellationToken: cancellationToken);
                
                if (cancellationToken.IsCancellationRequested)
                {
                    LogHelper.LogInfo("UI timer process canceled.", nameof(TimerProcess));
                    return;
                }
                
                timerStartMillis -= 1000f;
                if (timerStartMillis < 0) timerStartMillis = 0;

                if (animateNumberChanging)
                {
                    timerText.ShowText(TimeSpan.FromMilliseconds(timerStartMillis).ToString(timeFormat))
                        .AttachExternalCancellation(cancellationToken)
                        .Forget();
                }
                else
                {
                    timerText.ForceEnable();
                    timerText.Text = TimeSpan.FromMilliseconds(timerStartMillis).ToString(timeFormat);
                }
            }
        }
    }
}