using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniversalUnity.Helpers._ProjectDependent;
using UniversalUnity.Helpers.MonoBehaviourExtenders;

namespace UniversalUnity.Helpers.SceneLoading
{
    public class SceneLoader : GenericSingleton<SceneLoader>
    {
        [SerializeField] private LoadingScreen loadingScreen = null;

        public static Action<ESceneName, LoadSceneMode> OnSceneLoadingStarted;
        public static Action<ESceneName, LoadSceneMode> OnSceneLoadingEnded;

        private CancellationTokenSource _loadingCancellationTokenSource;

        protected override void InheritAwake()
        {
            OnSceneLoadingEnded += (scene, mode) =>
            {
                loadingScreen.Disable().Forget();
            };
        }

        public async UniTask LoadSceneAsync(ESceneName sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            try
            {
                _loadingCancellationTokenSource?.Dispose();
                _loadingCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
                
                await loadingScreen.Enable();
                await LoadProcess(sceneName, loadSceneMode);
            }
            catch (Exception e)
            {
                Debug.LogError($"[SceneLoader.StartSceneLoading] Error while loading scene. Message: {e.Message}");
                throw;
            }
        }

        private async UniTask LoadProcess(ESceneName sceneName, LoadSceneMode loadSceneMode)
        {
            OnSceneLoadingStarted?.Invoke(sceneName, loadSceneMode);
            await SceneManager.LoadSceneAsync(sceneName.ToString(), loadSceneMode)
                .WithCancellation(_loadingCancellationTokenSource.Token);
            OnSceneLoadingEnded?.Invoke(sceneName, loadSceneMode);
        }
    }
}
