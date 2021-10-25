using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UniversalUnity.Helpers.MonoBehaviourExtenders;
using UniversalUnity.Helpers.UI.BaseUiElements;
using UniversalUnity.Helpers.UI.CommonPatterns.Timers;

namespace UniversalUnity.Helpers.SceneLoading
{
    public class LoadingScreen : GenericSingleton<LoadingScreen>
    {
        [SerializeField] private BaseUiElement ui;
        [SerializeField] private BaseTextUiElement textElement;
        [SerializeField] private BaseUiTimer timer;
        
        private CancellationTokenSource _textChangingCancellationTokenSource = new CancellationTokenSource();

        public async UniTask Enable()
        {
            await ui.Enable();
        }

        public async UniTask Disable()
        {
            await ui.Disable();
        }
        
        public void ForceEnable()
        {
            ui.ForceEnable();
        }

        public void ForceDisable()
        {
            ui.ForceDisable();
        }

        public void StartTimer(float durationInMillis)
        {
            ui.Enable();
            timer.StartTimer(durationInMillis);
        }
        
        public void StopTimer()
        {
            timer.StopTimer();
        }
        
        public async UniTask ShowText(string text)
        {
            _textChangingCancellationTokenSource.Cancel();
            _textChangingCancellationTokenSource = new CancellationTokenSource();
            UniTask.Run(
                () => ui.Enable(),
                cancellationToken: _textChangingCancellationTokenSource.Token
            );
            await TextChanging(text);
        }

        private async UniTask TextChanging(string text)
        {
            if(textElement.IsEnabled) await textElement.Disable();
            textElement.Text = text;
            await textElement.Enable();
        }
        
        public void SetText(string text)
        {
            textElement.gameObject.SetActive(true);
            textElement.Text = text;
        }
    }
}