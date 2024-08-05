using UnityEngine;
using System.Collections;

public class Utils : MonoBehaviour
{
    public bool Low_ShowLogs = true;
    public bool Medium_ShowLogs = true;
    public bool High_ShowLogs = true;
    public bool Dev_ShowLogs = true;

    public static bool low_logs;
    public static bool medium_logs;
    public static bool high_logs;
    public static bool dev_logs;

    public enum LogLevel { LOW, MEDIUM, HIGH, DEV };

    private void Awake()
    {
        low_logs = Low_ShowLogs;
        medium_logs = Medium_ShowLogs;
        high_logs = High_ShowLogs;
        dev_logs = Dev_ShowLogs;
    }

    public static void Log(string logMessage, LogLevel logLevel = LogLevel.LOW)
    {
        switch (logLevel)
        {
            case LogLevel.LOW:
                if (low_logs) Debug.Log(logMessage);
                break;
            case LogLevel.MEDIUM:
                if (medium_logs) Debug.Log(logMessage);
                break;
            case LogLevel.HIGH:
                if (high_logs) Debug.Log(logMessage);
                break;
            case LogLevel.DEV:
                if (dev_logs) Debug.Log(logMessage);
                break;
        }
    }
}

