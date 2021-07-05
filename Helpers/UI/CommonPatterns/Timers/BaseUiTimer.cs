using System;
using System.Reflection;
using Common.Helpers.UI.BaseUiElements;
using JetBrains.Annotations;
using UnityEngine;

namespace Common.Helpers.UI.CommonPatterns.Timers
{
    public abstract class BaseUiTimer : BaseUiElement
    {
        private bool _destroyed;
        private System.Timers.Timer _timer;
        
        protected override void InheritAwake()
        {
            _destroyed = false;
        }

        private void OnDestroy()
        {
            _timer?.Dispose();
            _destroyed = true;
        }

        public Coroutine StartTimer(float durationInMillis, [CanBeNull] Action onDone = null)
        {
            if (durationInMillis < 0)
            {
                LogHelper.LogHelper.Log("Argument out of range! Must be >= 0.",
                    MethodBase.GetCurrentMethod(),
                    LogHelper.LogHelper.LogType.Error);
                return null;
            }

            _timer?.Dispose();
            _timer = new System.Timers.Timer(durationInMillis);
            _timer.Start();
            _timer.AutoReset = false;
            _timer.Elapsed += (sender, args) => HandleTimer(onDone);

            return UiHandleTimer(durationInMillis);
        }

        protected abstract Coroutine UiHandleTimer(float durationInMillis);

        private void HandleTimer([CanBeNull] Action onDone)
        {
            if (_destroyed) return;
            LogHelper.LogHelper.Log("Timer done!", MethodBase.GetCurrentMethod());
            onDone?.Invoke();
        }
        
        [ContextMenu("[RuntimeTest] -> Test timer")]
        public void TestTimer()
        {
            if (Application.isPlaying)
            {
                StartTimer(5000);
            }
        }
    }
}