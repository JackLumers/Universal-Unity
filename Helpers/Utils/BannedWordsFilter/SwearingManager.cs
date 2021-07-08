using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FileHelpers;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UniversalUnity.Helpers.MonoBehaviourExtenders;
using UniversalUnity.Helpers.Parsing.UsingFileHelpers;

namespace UniversalUnity.Helpers.Utils.BannedWordsFilter
{
    public class SwearingManager : GenericSingleton<SwearingManager>
    {
        [SerializeField] private AssetReference _swearingAsset;
        private HashSet<string> _swearings;

        private void Start()
        {
            _swearingAsset.LoadAssetAsync<TextAsset>().Completed += handle => { TryParse<SwearingsParsedEntity>(handle.Result); };
        }

        /// <summary>
        /// Return true if text contains swearing.
        /// </summary>
        public bool CheckSwearing(string text)
        {
            if (_swearings == null)
                return false;

            text = FormatText(text);
            for (int i = 0; i < _swearings.Count; i++)
            {
                if (text.IndexOf(FormatText(_swearings.ElementAt(i), true)) != -1)
                {
                    return true;
                }
            }

            return false;
        }

        public bool CheckSwearing(string text, string swearing, out int startSwearingPosition)
        {
            startSwearingPosition = FormatText(text).IndexOf(FormatText(swearing, true));
            if (startSwearingPosition != -1)
                return true;

            return false;
        }

        public bool CheckSwearing(string text, out List<int[]> swearingsPosition)
        {
            swearingsPosition = new List<int[]>();

            if (_swearings == null)
                return false;

            if (CheckSwearing(text))
            {
                for (int i = 0; i < _swearings.Count; i++)
                {
                    int index = 0;
                    string swearing = FormatText(_swearings.ElementAt(i), true);
                    while (CheckSwearing(text.Substring(index), swearing, out int startPosition))
                    {
                        if (index >= text.Length)
                            break;

                        index += startPosition + swearing.Length;
                        swearingsPosition.Add(new int[] { index - swearing.Length, index });
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private string FormatText(string text, bool isSweiring = false)
        {
            //в некоторых словах из csv на конце был пробел и из-за этого не всегда находило мат
            if (isSweiring)
            {
                Regex regex = new Regex(" ", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                var count = regex.Matches(text).Count;

                return text.Substring(0, text.Length - count).ToLower();
            }

            return text.ToLower();
        }

        private void TryParse<TDataEntity> (TextAsset csvTextAsset, bool skipHeader = true) where TDataEntity : SwearingsParsedEntity
        {
            try
            {
                _swearings = Parse<TDataEntity>(csvTextAsset.text, skipHeader);

                _swearings.Remove("");
                _swearings.Remove(" ");
            }
            catch (Exception e)
            {
                Debug.LogError("Parse failed!\n" + $"File: {csvTextAsset.name}\n" + $"Error: {e}");
                throw;
            }
        }

        private HashSet<string> Parse<TDataEntity> ([NotNull] string stringToRead, bool skipHeader) where TDataEntity : SwearingsParsedEntity
        {
            if (stringToRead == null) throw new ArgumentNullException(nameof(stringToRead));

            var engine = new FileHelperEngine<TDataEntity>(Encoding.ASCII);
            var parsedData = new HashSet<string>();

            if (skipHeader)
            {
                engine.Options.IgnoreFirstLines = 1;
            }

            try
            {
                foreach (var entity in engine.ReadString(stringToRead))
                {
                    // LogHelper.LogHelper.Log(
                    //     $"Adding parsed entity: {entityFactory.MakeLightweight(entity)}", 
                    //     MethodBase.GetCurrentMethod());
                    parsedData.Add(entity.RussainSwearingValue);
                    parsedData.Add(entity.EnglishSwearingValue);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Parse failed!\n Error: {e}");
                throw;
            }

            return parsedData;
        }
    }
}
