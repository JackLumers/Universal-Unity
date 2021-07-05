using System;
using System.Collections;
using Common.Helpers.Coroutines;
using Common.Helpers.UI.BaseUiElements;
using UnityEngine;

namespace Common.Helpers.UI.CommonPatterns.Timers
{
    public class UiClassicTimer : BaseUiTimer
    {
        [SerializeField] protected BaseTextUiElement timerText = null;
        
        public bool animateNumberChanging = true;
        public string timeFormat = @"mm\:ss";
        
        protected Coroutine TimerCoroutine;
        
        protected override void InheritAwake()
        {
            timerText.Text = new TimeSpan(0).ToString(timeFormat);
        }

        protected override Coroutine UiHandleTimer(float durationInMillis)
        {
            Enable();
            timerText.Enable();

            timerText.Text = TimeSpan.FromMilliseconds(durationInMillis).ToString(timeFormat);
            
            return CoroutineHelper.RestartCoroutine(ref TimerCoroutine, 
                TimerProcess(durationInMillis - durationInMillis % 1000), 
                this);
        }
        
        protected virtual IEnumerator TimerProcess(float timerStartMillis)
        {
            while (timerStartMillis > 0)
            {
                yield return new WaitForSecondsRealtime(1f);
                timerStartMillis -= 1000f;
                if (timerStartMillis < 0) timerStartMillis = 0;
                
                if (animateNumberChanging)
                {
                    timerText.ShowText(TimeSpan.FromMilliseconds(timerStartMillis).ToString(timeFormat));
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