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
        protected System.Timers.Timer Timer;

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

        private void OnEnable()
        {
            if (Timer is {Enabled: true})
            {
                UiHandleTimer((float) Timer.Interval, _cancellationTokenSource.Token)
                    .SuppressCancellationThrow().Forget();
            }
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

            Timer?.Dispose();
            Timer = new System.Timers.Timer(durationInMillis);
            Timer.Start();
            Timer.AutoReset = false;
            Timer.Elapsed += (sender, args) => HandleTimer(onDone);

            await UiHandleTimer(durationInMillis, _cancellationTokenSource.Token)
                .SuppressCancellationThrow();
        }
        
        public virtual void StopTimer()
        {
            _cancellationTokenSource?.Cancel();
            Timer?.Dispose();
        }
        
        protected abstract UniTask UiHandleTimer(float durationInMillis, CancellationToken cancellationToken);

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
                StartTimer(5000).Forget();
            }
        }
    }
}