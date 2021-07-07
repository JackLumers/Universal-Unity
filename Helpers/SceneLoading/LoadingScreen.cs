using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniversalUnity.Helpers.MonoBehaviourExtenders;
using UniversalUnity.Helpers.UI.BaseUiElements;

namespace UniversalUnity.Helpers.SceneLoading
{
    public class LoadingScreen : GenericSingleton<LoadingScreen>
    {
        [SerializeField] private BaseUiElement ui;
        [SerializeField] private BaseTextUiElement textElement;

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
    }
}