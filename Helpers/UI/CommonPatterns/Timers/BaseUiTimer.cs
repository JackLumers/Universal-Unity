using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UniversalUnity.Helpers.Logs;
using UniversalUnity.Helpers.UI.BaseUiElements.BaseElements;

namespace UniversalUnity.Helpers.UI.CommonPatterns.Timers
{
    public abstract class BaseUiTimer : BaseUiElement
    {
        private bool _destroyed;
        private System.Timers.Timer _timer;

        private CancellationTokenSource _cancellationTokenSource;
        
        protected override void InheritAwake()
        {
            _destroyed = false;
        }

        private void OnDestroy()
        {
            StopTimer();
            _destroyed = true;
        }

        private void OnDisable()
        {
            StopTimer();
        }

        public async UniTask StartTimer(float durationInMillis, [CanBeNull] Action onDone = null)
        {
            Enable().Forget();
            
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            
            if (durationInMillis < 0)
            {
                LogHelper.LogError("Argument out of range! Must be >= 0.",
                    nameof(StartTimer));
                return;
            }

            _timer?.Dispose();
            _timer = new System.Timers.Timer(durationInMillis);
            _timer.Start();
            _timer.AutoReset = false;
            _timer.Elapsed += (sender, args) => HandleTimer(onDone);

            await UiHandleTimer(durationInMillis)
                .AttachExternalCancellation(_cancellationTokenSource.Token)
                .SuppressCancellationThrow();
        }
        
        public virtual void StopTimer()
        {
            _cancellationTokenSource?.Cancel();
            _timer?.Dispose();
        }
        
        protected abstract UniTask UiHandleTimer(float durationInMillis);

        private void HandleTimer([CanBeNull] Action onDone)
        {
            if (_destroyed) return;
            LogHelper.LogInfo("Timer done!", nameof(HandleTimer));
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