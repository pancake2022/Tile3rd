using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSFramework
{
    public static class Logger
    {
        public enum LogLevel
        {
            None,
            Error,
            Warning,
            Log,
        }
#if DEBUG || DEVELOPMENT_BUILD
        public static LogLevel Level = LogLevel.Log;
#else
        public static LogLevel Level = LogLevel.Error;
#endif

        public static void Log (object obj, params object[] args)
        {
            if (Level >= LogLevel.Log)
            {
                var str = obj == null ? "NULL" : obj.ToString();
                str = convert_log_string(str, args);
                UnityEngine.Debug.Log("[I]> " + str);
            }
        }

        public static void Warning (object obj, params object[] args)
        {
            if (Level >= LogLevel.Warning)
            {
                var str = obj == null ? "NULL" : obj.ToString();
                str = convert_log_string(str, args);
                UnityEngine.Debug.LogWarning("[W]> " + str);
            }
        }

        public static void Error (object obj, params object[] args)
        {
            if (Level >= LogLevel.Error)
            {
                var str = obj == null ? "NULL" : obj.ToString();
                str = convert_log_string(str, args);
                UnityEngine.Debug.LogError("[E]> " + str);
            }
        }

        private static string convert_log_string (string str, params object[] args)
        {
            var builder = new System.Text.StringBuilder();

            var now = System.DateTime.Now;

            builder.Append(now.ToString("HH:mm:ss.fff", System.Globalization.CultureInfo.CreateSpecificCulture("en-US")));
            builder.Append(" ");

            if (args != null && args.Length > 0)
                builder.AppendFormat(str, args);
            else
                builder.Append(str);

            return builder.ToString();
        }
    }
}
