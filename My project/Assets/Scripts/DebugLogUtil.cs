using System;
using System.IO;
using UnityEngine;

public static class DebugLogUtil
{
    private static readonly string LogPath = Path.Combine(Application.dataPath, "..", ".cursor", "debug.log");

    private static string Escape(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        return s.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }

    public static void Log(string location, string message, string dataJson, string hypothesisId)
    {
        try
        {
            long ts = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
            string line = "{\"location\":\"" + Escape(location) + "\",\"message\":\"" + Escape(message) + "\",\"data\":" + dataJson + ",\"hypothesisId\":\"" + Escape(hypothesisId) + "\",\"timestamp\":" + ts + ",\"sessionId\":\"debug-session\"}\n";
            string path = Path.GetFullPath(LogPath);
            File.AppendAllText(path, line);
        }
        catch { }
    }
}
