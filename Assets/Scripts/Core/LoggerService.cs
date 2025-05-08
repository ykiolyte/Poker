using UnityEngine;

public enum LogLevel { Info, Warning, Error }

public class LoggerService
{
    public static LogLevel CurrentLogLevel = LogLevel.Info;

    public static void Log(string message, LogLevel level = LogLevel.Info)
    {
        if (level < CurrentLogLevel) return;

        switch (level)
        {
            case LogLevel.Info:
                Debug.Log("[INFO] " + message);
                break;
            case LogLevel.Warning:
                Debug.LogWarning("[WARNING] " + message);
                break;
            case LogLevel.Error:
                Debug.LogError("[ERROR] " + message);
                break;
        }
    }
}
