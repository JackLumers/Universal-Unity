using System;
using UnityEngine;

namespace UniversalUnity.Helpers.Logs
{
    public static class LogHelper
    {
        public static void LogInfo(object message, string methodName)
        {
            Log(message, methodName, LogType.Info);
        }
        
        public static void LogWarning(object message, string methodName)
        {
            Log(message, methodName, LogType.Warning);
        }
        
        public static void LogError(object message, string methodName)
        {
            Log(message, methodName, LogType.Error);
        }
        
        private enum LogType
        {
            Info,
            Warning,
            Error
        }

        private static void Log(object message, string methodName, LogType logType = LogType.Info)
        {
            var prefix = $"[{methodName}]";
            switch (logType)
            {
                case LogType.Info:
                    Debug.Log($"{prefix} {message}");
                    break;
                case LogType.Warning:
                    Debug.LogWarning($"{prefix} {message}");
                    break;
                case LogType.Error:
                    Debug.LogError($"{prefix} {message}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logType), logType, null);
            }
        }
    }
}