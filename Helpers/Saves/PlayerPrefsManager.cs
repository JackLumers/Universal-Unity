using System;
using UnityEngine;

namespace Common.Helpers.PlayerPreferencesManager
{
    public static class PlayerPrefsManager
    {
        private const string TextLanguage = "TextLanguage";
        private const string VoiceLanguage = "VoiceLanguage";
        private const string MusicVolume = "MusicVolume";
        private const string SoundEffectsVolume = "SoundEffectsVolume";
        private const string VoiceVolume = "VoiceVolume";
        private const string NotificationEnable = "NotificationEnable";

        // Temporary values. Remove when it was be done on server.
        private const string LastCinemaWatchYear = "LastCinemaWatchYear";
        private const string LastCinemaWatchMonth = "LastCinemaWatchMonth";
        private const string LastCinemaWatchDay = "LastCinemaWatchDay";
        private const string LastCinemaWatchHour = "LastCinemaWatchHour";
        private const string LastCinemaWatchMinute = "LastCinemaWatchMinute";
        private const string LastCinemaWatchSecond = "LastCinemaWatchSecond";

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

        // Temporary methods. Remove when it was be done on server.
        public static int SavedLastCinemaWatchYear
        {
            get => PlayerPrefs.GetInt(LastCinemaWatchYear, 2021);
            set => PlayerPrefs.SetInt(LastCinemaWatchYear, value);
        }
        public static int SavedLastCinemaWatchMonth
        {
            get => PlayerPrefs.GetInt(LastCinemaWatchMonth, 2);
            set => PlayerPrefs.SetInt(LastCinemaWatchMonth, value);
        }
        public static int SavedLastCinemaWatchDay
        {
            get => PlayerPrefs.GetInt(LastCinemaWatchDay, 1);
            set => PlayerPrefs.SetInt(LastCinemaWatchDay, value);
        }
        public static int SavedLastCinemaWatchHour
        {
            get => PlayerPrefs.GetInt(LastCinemaWatchHour, 0);
            set => PlayerPrefs.SetInt(LastCinemaWatchHour, value);
        }
        public static int SavedLastCinemaWatchMinute
        {
            get => PlayerPrefs.GetInt(LastCinemaWatchMinute, 0);
            set => PlayerPrefs.SetInt(LastCinemaWatchMinute, value);
        }
        public static int SavedLastCinemaWatchSecond
        {
            get => PlayerPrefs.GetInt(LastCinemaWatchSecond, 0);
            set => PlayerPrefs.SetInt(LastCinemaWatchSecond, value);
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

        public static void Save()
        {
            PlayerPrefs.Save();
        }
    }
}