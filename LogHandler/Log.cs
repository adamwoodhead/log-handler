using LogHandler.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LogHandler
{

    public static class Log
    {
        internal static Host Host { get; set; }

        public static void Initialize(TextWriter _in = null, LogOptions options = null)
        {
            if (_in != null)
            {
                Console.SetOut(_in);
            }

            Host = new Host(options);
        }

        public static void Cancel()
        {
            Host.CancellationToken.Cancel();
        }

        public static void Exception(Exception exception, bool fileLog = false)
        {
            Exception(exception.ToString(), fileLog);
        }

        public static void Exception(string text, bool fileLog = false)
        {
            DateTime stamp = DateTime.UtcNow;
            Action action = new Action(() =>
            {
                Host.WriteLog(Host.FormatLog(text, stamp, LogType.EXCEPTION), LogType.EXCEPTION);
                if (fileLog) { Host.HardLog(text); }
            });

            Host.TaskFlow.Add(action);
        }

        public static void Error(string text, bool fileLog = false)
        {
            DateTime stamp = DateTime.UtcNow;
            Action action = new Action(() =>
            {
                Host.WriteLog(Host.FormatLog(text, stamp, LogType.ERROR), LogType.ERROR);
                if (fileLog) { Host.HardLog(text); }
            });

            Host.TaskFlow.Add(action);
        }

        public static void Warning(string text, bool fileLog = false)
        {
            DateTime stamp = DateTime.UtcNow;
            Action action = new Action(() =>
            {
                Host.WriteLog(Host.FormatLog(text, stamp, LogType.WARNING), LogType.WARNING);
                if (fileLog) { Host.HardLog(text); }
            });

            Host.TaskFlow.Add(action);
        }

        public static void Info(string text, bool fileLog = false)
        {
            DateTime stamp = DateTime.UtcNow;
            Action action = new Action(() =>
            {
                Host.WriteLog(Host.FormatLog(text, stamp, LogType.INFO), LogType.INFO);
                if (fileLog) { Host.HardLog(text); }
            });

            Host.TaskFlow.Add(action);
        }

        public static void Verbose(string text, bool fileLog = false)
        {
            DateTime stamp = DateTime.UtcNow;
            Action action = new Action(() =>
            {
                Host.WriteLog(Host.FormatLog(text, stamp, LogType.VERBOSE), LogType.VERBOSE);
                if (fileLog) { Host.HardLog(text); }
            });

            Host.TaskFlow.Add(action);
        }
    }
}
