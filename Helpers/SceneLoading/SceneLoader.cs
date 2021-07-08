using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniversalUnity.Helpers._ProjectDependent;
using UniversalUnity.Helpers.Coroutines;
using UniversalUnity.Helpers.MonoBehaviourExtenders;

namespace UniversalUnity.Helpers.SceneLoading
{
    public class SceneLoader : GenericSingleton<SceneLoader>
    {
        [SerializeField] private LoadingScreen loadingScreen = null;

        public static Action<ESceneName, LoadSceneMode> OnSceneLoadingStarted;
        public static Action<ESceneName, LoadSceneMode> OnSceneLoadingEnded;

        public static bool IsLoadingInProcess { get; private set; } = false;

        private Coroutine _loadingCoroutine;

        protected override void InheritAwake()
        {
            OnSceneLoadingEnded += (scene, mode) =>
            {
                loadingScreen.Disable();
                IsLoadingInProcess = false;
            };
        }

        public async UniTask LoadSceneAsync(ESceneName sceneName, [CanBeNull] Action onLoaded = null, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            await loadingScreen.Enable();
            StartSceneLoading(sceneName, loadSceneMode, onLoaded);
        }

        private void StartSceneLoading(ESceneName sceneName, LoadSceneMode loadSceneMode, [CanBeNull] Action onLoaded)
        {
            try
            {
                IsLoadingInProcess = true;
                CoroutineHelper.RestartCoroutine(
                    ref _loadingCoroutine,
                    LoadingProcess(sceneName, loadSceneMode, onLoaded), 
                    this);
            }
            catch (Exception e)
            {
                Debug.LogError($"[SceneLoader.StartSceneLoading] Error while loading scene. Message: {e.Message}");
                throw;
            }
        }

        private IEnumerator LoadingProcess(ESceneName sceneName, LoadSceneMode loadSceneMode, [CanBeNull] Action onLoaded)
        {
            OnSceneLoadingStarted?.Invoke(sceneName, loadSceneMode);
            yield return SceneManager.LoadSceneAsync(sceneName.ToString(), loadSceneMode);
            onLoaded?.Invoke();
            OnSceneLoadingEnded?.Invoke(sceneName, loadSceneMode);
        }
    }
}
