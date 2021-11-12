using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniversalUnity.Helpers.MonoBehaviourExtenders;
using UniversalUnity.Helpers.UI.BaseUiElements.BaseElements;
using UniversalUnity.Helpers.UI.CommonPatterns.Timers;

namespace UniversalUnity.Helpers.SceneLoading
{
    public class LoadingScreen : GenericSingleton<LoadingScreen>
    {
        [SerializeField] private BaseUiElement ui;
        [SerializeField] private BaseTextUiElement textElement;
        [SerializeField] private BaseUiTimer timer;
        
        private CancellationTokenSource _textChangingCancellationTokenSource;

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
            ui.Enable().Forget();
            timer.StartTimer(durationInMillis).Forget();
        }
        
        public void StopTimer()
        {
            timer.StopTimer();
            if (ui.IsEnabled)
            {
                timer.Disable().Forget();
            }
            else
            {
                timer.ForceDisable();
            }
        }
        
        public async UniTask ShowText(string text)
        {
            _textChangingCancellationTokenSource?.Cancel();
            _textChangingCancellationTokenSource = new CancellationTokenSource();
            
            ui.Enable().Forget();
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