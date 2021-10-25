using System;
using UnityEngine;

namespace UniversalUnity.Helpers.Utils
{
    public static class PlayerPrefsManager
    {
        private const string TextLanguage = "TextLanguage";
        private const string VoiceLanguage = "VoiceLanguage";
        private const string MusicVolume = "MusicVolume";
        private const string SoundEffectsVolume = "SoundEffectsVolume";
        private const string VoiceVolume = "VoiceVolume";
        private const string NotificationEnable = "NotificationEnable";

        public static string SavedTextLanguage
        {
            get => PlayerPrefs.GetString(TextLanguage, string.Empty);
            set => PlayerPrefs.SetString(TextLanguage, value);
        }
        
        public static string SavedVoiceLanguage
        {
            get => PlayerPrefs.GetString(VoiceLanguage, string.Empty);
            set => PlayerPrefs.SetString(VoiceLanguage, value);
        }
        
        public static int SavedMusicVolume
        {
            get => PlayerPrefs.GetInt(MusicVolume, 1);
            set => PlayerPrefs.SetInt(MusicVolume, value);
        }
        
        public static int SavedSoundEffectsVolume
        {
            get => PlayerPrefs.GetInt(SoundEffectsVolume, 1);
            set => PlayerPrefs.SetInt(SoundEffectsVolume, value);
        }

        public static int SavedNotificationEnable
        {
            get => PlayerPrefs.GetInt(NotificationEnable, 1);
            set => PlayerPrefs.SetInt(NotificationEnable, value);
        }
        
        public static float SavedVoiceVolume
        {
            get => PlayerPrefs.GetFloat(VoiceVolume, 0.9f);
            set => PlayerPrefs.SetFloat(VoiceVolume, value);
        }

        public static float GetSavedVolumeForSource(AudioManager.AudioManager.EAudioSource sourceType)
        {
            switch (sourceType)
            {
                case AudioManager.AudioManager.EAudioSource.None:
                    return 0;
                case AudioManager.AudioManager.EAudioSource.UiElementResponse:
                    return SavedSoundEffectsVolume;
                case AudioManager.AudioManager.EAudioSource.GlobalSoundtrack:
                    return SavedMusicVolume;
                case AudioManager.AudioManager.EAudioSource.EffectsSource:
                    return SavedSoundEffectsVolume;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sourceType), sourceType, null);
            }
        }
    }
}