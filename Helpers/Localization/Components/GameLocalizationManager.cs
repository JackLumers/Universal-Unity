using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UniversalUnity.Helpers.MonoBehaviourExtenders;
using UniversalUnity.Helpers.SceneLoading;

namespace UniversalUnity.Helpers.Localization.Components
{
    public class GameLocalizationManager : GenericSingleton<GameLocalizationManager>
    {
        /// <summary>
        /// Localization that is always in memory with commonly used entries.
        /// </summary>
        [SerializeField] private AssetReference[] alwaysLoaded;
        
        /// <summary>
        /// Localization that is used in specific game state, and will be loaded dynamically.
        /// It can depends on scene or another 
        /// </summary>
        [FormerlySerializedAs("sceneDependentLocalizationDictionary")] 
        [SerializeField] private SceneDependentLocalization[] sceneDependentLocalizationList;

        [Serializable]
        private class SceneDependentLocalization
        {
            [SerializeField] internal string sceneName = null;
            [SerializeField] internal AssetReference[] localizationAssetReferences;
        }
        
        private readonly Dictionary<string, SceneDependentLocalization> _sceneDependentLocalizations = new Dictionary<string, SceneDependentLocalization>();

        protected override void InheritAwake()
        {
            foreach (var assetReference in alwaysLoaded)
            {
                LocalizationManager.AddFileToParse(assetReference);
            }

            foreach (var sceneDependentLocalization in sceneDependentLocalizationList)
            {
                _sceneDependentLocalizations.Add(sceneDependentLocalization.sceneName, sceneDependentLocalization);
            }
            
            SceneLoader.OnSceneLoadingStarted += (scene, loadMode) => AddInParseByScene(scene.ToString());
            SceneManager.sceneUnloaded += (scene) => RemoveFromParseByScene(scene.name);
            LocalizationManager.OnTextLanguageChanged += (language) => LocalizationManager.StartParse();
            LocalizationManager.StartParse();
        }

        [UsedImplicitly]
        private void AddInParseByScene(string sceneName)
        {
            if (_sceneDependentLocalizations.ContainsKey(sceneName))
            {
                foreach (var assetReference in _sceneDependentLocalizations[sceneName].localizationAssetReferences)
                {
                    LocalizationManager.AddFileToParse(assetReference);
                }
            }
            LocalizationManager.StartParse();
        }
        
        [UsedImplicitly]
        private void RemoveFromParseByScene(string sceneName)
        {
            if (_sceneDependentLocalizations.ContainsKey(sceneName))
            {
                foreach (var assetReference in _sceneDependentLocalizations[sceneName].localizationAssetReferences)
                {
                    LocalizationManager.RemoveFileFromParse(assetReference);
                }
            }
        }

        protected override void InheritOnDestroy()
        {
            base.InheritOnDestroy();
            LocalizationManager.ReleaseAll();
        }
    }
}