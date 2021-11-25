using System;
using UnityEngine;
using UniversalUnity.Helpers.Localization.Enums;

namespace UniversalUnity.Helpers.Utils
{
    public static class PlayerPrefsManager
    {
        private const string TEXT_LANGUAGE = "TextLanguage";
        private const string VOICE_LANGUAGE = "VoiceLanguage";
        private const string MUSIC_VOLUME = "MusicVolume";
        private const string SOUND_EFFECTS_VOLUME = "SoundEffectsVolume";
        private const string VOICE_VOLUME = "VoiceVolume";
        private const string NOTIFICATION_ENABLE = "NotificationEnable";

        public static string SavedTextLanguage
        {
            get => PlayerPrefs.GetString(TEXT_LANGUAGE, ETextLanguage.Undefined.ToString());
            set
            {
                PlayerPrefs.SetString(TEXT_LANGUAGE, value);
                PlayerPrefs.Save();
            }
        }

        public static string SavedVoiceLanguage
        {
            get => PlayerPrefs.GetString(VOICE_LANGUAGE, EVoiceLanguage.Undefined.ToString());
            set
            {
                PlayerPrefs.SetString(VOICE_LANGUAGE, value);
                PlayerPrefs.Save();
            }
        }

        public static int SavedMusicVolume
        {
            get => PlayerPrefs.GetInt(MUSIC_VOLUME, 1);
            set
            {
                PlayerPrefs.SetInt(MUSIC_VOLUME, value);
                PlayerPrefs.Save();
            }
        }

        public static int SavedSoundEffectsVolume
        {
            get => PlayerPrefs.GetInt(SOUND_EFFECTS_VOLUME, 1);
            set
            {
                PlayerPrefs.SetInt(SOUND_EFFECTS_VOLUME, value);
                PlayerPrefs.Save();
            }
        }

        public static int SavedNotificationEnable
        {
            get => PlayerPrefs.GetInt(NOTIFICATION_ENABLE, 1);
            set
            {
                PlayerPrefs.SetInt(NOTIFICATION_ENABLE, value);
                PlayerPrefs.Save();
            }
        }

        public static float SavedVoiceVolume
        {
            get => PlayerPrefs.GetFloat(VOICE_VOLUME, 0.9f);
            set
            {
                PlayerPrefs.SetFloat(VOICE_VOLUME, value);
                PlayerPrefs.Save();
            }
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