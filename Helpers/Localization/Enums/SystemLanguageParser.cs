using UnityEngine;

namespace UniversalUnity.Helpers.Localization.Enums
{
    public static class SystemLanguageParser
    {
        /// <summary>
        /// All supported game text languages can be parsed by this method.
        /// </summary>
        public static ETextLanguage ParseText(SystemLanguage systemLanguage)
        {
            switch (systemLanguage)
            {
                case SystemLanguage.English:
                    return ETextLanguage.English;
                case SystemLanguage.Russian:
                    return ETextLanguage.Russian;
                default:
                    return ETextLanguage.English;
            }
        }
        
        /// <summary>
        /// All supported game voice languages can be parsed by this method.
        /// </summary>
        public static EVoiceLanguage ParseVoice(SystemLanguage systemLanguage)
        {
            switch (systemLanguage)
            {
                case SystemLanguage.English:
                    return EVoiceLanguage.English;
                case SystemLanguage.Russian:
                    return EVoiceLanguage.Russian;
                default:
                    return EVoiceLanguage.English;
            }
        }
    }
}