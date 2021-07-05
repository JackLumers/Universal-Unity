using System;
using System.Reflection;
using UnityEngine;

namespace UniversalUnity.Helpers.LogHelper
{
    public class LogHelper
    {
        public enum LogType
        {
            Info,
            Warning,
            Error
        }
        
        public static void Log(object message, MethodBase methodBase, LogType logType = LogType.Info)
        {
            string prefix = $"[{methodBase.Name}]";
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