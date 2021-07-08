using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UniversalUnity.Helpers.Extensions;
using UniversalUnity.Helpers.Localization.Enums;
using UniversalUnity.Helpers.Localization.UsingFileHelpers;
using UniversalUnity.Helpers.Parsing.UsingFileHelpers;
using UniversalUnity.Helpers.Utils;
using CsvParser = UniversalUnity.Helpers.Parsing.UsingFileHelpers.CsvParser;

namespace UniversalUnity.Helpers.Localization
{
    public static class LocalizationManager
    {
        /// <summary>
        ///     key - <see cref="AParsedLightweightEntity"/> id
        ///     value - <see cref="AParsedLightweightEntity"/>
        /// </summary>
        private static Dictionary<string, LightweightLocalizedEntityFactory.LocalizedLightweightEntity> _localizedEntities = 
            new Dictionary<string, LightweightLocalizedEntityFactory.LocalizedLightweightEntity>();
        private static readonly List<AssetReference> LocalizationAssetReferences = new List<AssetReference>();
        
        private static EVoiceLang _gameVoiceLang;
        private static ETextLanguage _gameTextLanguage;

        /// <summary>
        /// key - asset reference from <see cref="LocalizationAssetReferences"/>;
        /// <para></para>
        /// value - handle of the getting asset operation (See <see cref="AssetReference.LoadAssetAsync{Tobject}"/>)
        /// </summary>
        private static readonly Dictionary<AssetReference, AsyncOperationHandle<TextAsset>> Handles = new Dictionary<AssetReference, AsyncOperationHandle<TextAsset>>();
        private static int _parsedCounter;
        public static bool IsParsed { get; private set; }

        public static event Action<ETextLanguage> OnTextLanguageChanged;
        public static event Action<EVoiceLang> OnVoiceLanguageChanged;
        public static event Action OnParsed;

        public static ETextLanguage GameTextLanguage
        {
            get
            {
                if (_gameTextLanguage == ETextLanguage.Undefined)
                {
                    return Enum.TryParse(PlayerPrefsManager.SavedTextLanguage, out ETextLanguage language)
                        ? language
                        : ETextLanguage.Russian;
                }
                return _gameTextLanguage;
            }
            set
            {
                Debug.Log($"[LocalizationManager.GameVoiceLang] Settled \"{value}\" text language for the game.");
                _gameTextLanguage = value;
                PlayerPrefs.SetString("language", _gameTextLanguage.ToString());
                OnTextLanguageChanged?.Invoke(value);
            }
        }

        public static EVoiceLang GameVoiceLang
        {
            get
            {
                if (_gameTextLanguage == ETextLanguage.Undefined)
                {
                    return Enum.TryParse(PlayerPrefsManager.SavedVoiceLanguage, out EVoiceLang language)
                        ? language
                        : EVoiceLang.Russian;
                }
                return _gameVoiceLang;
            }
            set
            {
                Debug.Log($"[LocalizationManager.GameVoiceLang] Settled \"{value}\" voice language for the game.");
                _gameVoiceLang = value;
                PlayerPrefs.SetString("voiceLanguage", _gameVoiceLang.ToString());
                OnVoiceLanguageChanged?.Invoke(value);
            }
        }

        public static string GetImageKey(string imageID)
        {
            return string.Format("{0}_{1}", imageID,GameTextLanguage.ToString());
        }

        public static string GetText(string textId)
        {
            if (!IsParsed)
            {
                Debug.LogWarning($"[LocalizationManager.GetText] Localization was not parsed yet!");
                return textId;
            }
            
            if (_localizedEntities.ContainsKey(textId))
            {
                return _localizedEntities[textId].Text;
            }

            Debug.LogWarning($"[LocalizationManager.GetText] There is no such object in parsed objects! id: {textId}");
            return textId;
        }

        public static bool HasText(string textId)
        {
            if (!IsParsed)
            {
                Debug.LogWarning($"[LocalizationManager.GetText] Localization was not parsed yet!");
                return false;
            }

            return _localizedEntities.ContainsKey(textId);
        }
        
        public static void AddFileToParse(AssetReference localizationAsset)
        {
            if (!LocalizationAssetReferences.Contains(localizationAsset))
            {
                LocalizationAssetReferences.Add(localizationAsset);
            }
            else
            {
                throw new ArgumentException($"This localization asset already added! Asset key: {localizationAsset.AssetGUID}");
            }
        }
        
        public static void RemoveFileFromParse(AssetReference localizationAsset)
        {
            if (LocalizationAssetReferences.Contains(localizationAsset))
            {
                LocalizationAssetReferences.Remove(localizationAsset);
                Handles.Remove(localizationAsset);
                localizationAsset.ReleaseAsset();
            }
            else
            {
                throw new ArgumentException($"There is no such localization asset to remove! Asset key: {localizationAsset}");
            }
        }

        public static void StartParse()
        {
            ClearBeforeParse();

            for (int i = 0; i < LocalizationAssetReferences.Count; i++)
            {
                // If asset is already loaded -> parse and go to another asset reference
                if (Handles.ContainsKey(LocalizationAssetReferences[i]) && Handles[LocalizationAssetReferences[i]].IsDone)
                {
                    var csvAsset = (TextAsset)LocalizationAssetReferences[i].Asset;
                    
                    _localizedEntities = DictionaryExtensions.Merge(_localizedEntities, 
                        CsvParser.ParseLightweight(csvAsset, new LightweightLocalizedEntityFactory(GameTextLanguage)));
                    
                    IncrementAndCheckParsing();
                    continue;
                }

                if(!Handles.ContainsKey(LocalizationAssetReferences[i]))
                {
                    Handles.Add(LocalizationAssetReferences[i], LocalizationAssetReferences[i].LoadAssetAsync<TextAsset>());
                }

                var i1 = i;
                Handles[LocalizationAssetReferences[i1]].Completed += (handle) =>
                {
                    var csvAsset = handle.Result;
                    _localizedEntities = DictionaryExtensions.Merge(_localizedEntities, 
                        CsvParser.ParseLightweight(csvAsset, new LightweightLocalizedEntityFactory(GameTextLanguage)));
                    IncrementAndCheckParsing();
                };
            }
        }

        public static void ReleaseAll()
        {
            Handles.Clear();
            ClearBeforeParse();
            foreach (var assetReference in LocalizationAssetReferences)
            {
                assetReference.ReleaseAsset();
            }
        }
        
        private static void IncrementAndCheckParsing()
        {
            _parsedCounter++;
            if (_parsedCounter == LocalizationAssetReferences.Count)
            {
                IsParsed = true;
                OnParsed?.Invoke();
            }
        }

        private static void ClearBeforeParse()
        {
            IsParsed = false;
            _parsedCounter = 0;
            _localizedEntities.Clear();
        }
    }
}