using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UniversalUnity.Helpers.Extensions;
using UniversalUnity.Helpers.Localization.Enums;
using UniversalUnity.Helpers.Localization.UsingFileHelpers;
using UniversalUnity.Helpers.Logs;
using UniversalUnity.Helpers.Parsing.UsingFileHelpers;
using UniversalUnity.Helpers.Utils;
using CsvParser = UniversalUnity.Helpers.Parsing.UsingFileHelpers.CsvParser;

namespace UniversalUnity.Helpers.Localization
{
    public class LocalizationManager : ScriptableObject
    {
        /// <summary>
        ///     key - <see cref="AParsedLightweightEntity"/> id
        ///     value - <see cref="AParsedLightweightEntity"/>
        /// </summary>
        private static Dictionary<string, LightweightLocalizedEntityFactory.LocalizedLightweightEntity> _localizedEntities = 
            new Dictionary<string, LightweightLocalizedEntityFactory.LocalizedLightweightEntity>();
        private static readonly List<AssetReference> LocalizationAssetReferences = new List<AssetReference>();
        
        /// <summary>
        /// key - asset reference from <see cref="LocalizationAssetReferences"/>;
        /// <para></para>
        /// value - handle of the getting asset operation (See <see cref="AssetReference.LoadAssetAsync{Tobject}"/>)
        /// </summary>
        private static readonly Dictionary<AssetReference, AsyncOperationHandle<TextAsset>> Handles = new Dictionary<AssetReference, AsyncOperationHandle<TextAsset>>();
        private static int _parsedCounter;
        public static bool IsParsed { get; private set; }

        public static event Action<ETextLanguage> OnTextLanguageChanged;
        public static event Action<EVoiceLanguage> OnVoiceLanguageChanged;
        public static event Action OnParsed;

        public static ETextLanguage GameTextLanguage
        {
            get
            {
                var prefsString = PlayerPrefsManager.SavedTextLanguage;
                if (prefsString == "Undefined")
                {
                    return SystemLanguageParser.ParseText(Application.systemLanguage);
                }
                else
                {
                    if (Enum.TryParse(prefsString, true, out ETextLanguage language))
                    {
                        return language;
                    }
                    else
                    {
                        LogHelper.LogError($"Text language saved in players prefs not supported. Language in prefs: {prefsString}", nameof(GameTextLanguage));
                        return SystemLanguageParser.ParseText(Application.systemLanguage);
                    }
                }
            }
            set
            {
                Debug.Log($"[LocalizationManager.GameVoiceLang] Settled \"{value}\" text language for the game.");
                PlayerPrefsManager.SavedTextLanguage = value.ToString();
                OnTextLanguageChanged?.Invoke(value);
            }
        }

        public static EVoiceLanguage GameVoiceLanguage
        {
            get
            {
                var prefsString = PlayerPrefsManager.SavedVoiceLanguage;
                if (prefsString == "Undefined")
                {
                    return SystemLanguageParser.ParseVoice(Application.systemLanguage);
                }
                else
                {
                    if (Enum.TryParse(prefsString, true, out EVoiceLanguage language))
                    {
                        return language;
                    }
                    else
                    {
                        LogHelper.LogError($"Voice language saved in players prefs not supported. Language in prefs: {prefsString}", nameof(GameTextLanguage));
                        return SystemLanguageParser.ParseVoice(Application.systemLanguage);
                    }
                }
            }
            set
            {
                Debug.Log($"[LocalizationManager.GameVoiceLang] Settled \"{value}\" voice language for the game.");
                PlayerPrefsManager.SavedVoiceLanguage = value.ToString();
                OnVoiceLanguageChanged?.Invoke(value);
            }
        }

        public static string GetImageKey(string imageID)
        {
            return $"{imageID}_{GameTextLanguage.ToString()}";
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