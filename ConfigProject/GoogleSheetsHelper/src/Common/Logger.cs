using System;
using System.IO;

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
        public static LogLevel Level = LogLevel.Log;

        public static void Log (object obj, params object[] args)
        {
            if (Level >= LogLevel.Log)
            {
                var str = obj == null ? "NULL" : obj.ToString();
                Console.WriteLine("[I]> " + convert_log_string(str, args));
            }
        }

        public static void Warning (object obj, params object[] args)
        {
            if (Level >= LogLevel.Warning)
            {
                var str = obj == null ? "NULL" : obj.ToString();
                Console.WriteLine("[W]> " + convert_log_string(str, args));
            }
        }

        public static void Error (object obj, params object[] args)
        {
            if (Level >= LogLevel.Error)
            {
                var str = obj == null ? "NULL" : obj.ToString();
                Console.WriteLine("[E]> " + convert_log_string(str, args));
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

            var stack_trace = new System.Diagnostics.StackTrace(true);
            var frame_list = stack_trace.GetFrames();
            foreach (var frame in frame_list)
            {
                builder.Append(string.Format("\r\n    {0}.{1} in {2}:{3}", frame.GetMethod().DeclaringType.FullName, frame.GetMethod().Name, Path.GetFileName(frame.GetFileName()), frame.GetFileLineNumber()) );
            }

            return builder.ToString();
        }
    }
}
