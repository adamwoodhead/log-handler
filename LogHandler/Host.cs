using LogHandler.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogHandler
{
    internal class Host
    {
        internal Task LoopTask { get; set; }

        internal List<Action> TaskFlow { get; } = new List<Action>();

        internal CancellationTokenSource CancellationToken { get; set; } = new CancellationTokenSource();
        
        internal LogOptions LogOptions { get; set; }

        internal int LogID { get; set; } = 1;

        internal async Task BeginTaskLoopAsync()
        {
            while (!CancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (TaskFlow.Count > 0 && TaskFlow[0] != null)
                    {
                        TaskFlow[0]();
                        TaskFlow.RemoveAt(0);
                    }
                    await Task.Delay(2);
                }
                catch (Exception) { }
            }
        }

        internal void WriteLog(string text, LogType type)
        {
            if ((int)type <= (int)LogOptions.AllowedLogLevel)
            {
                Console.WriteLine(text);
                Console.ResetColor();
            }
        }

        internal string FormatLog(string logText, DateTime stamp, LogType type)
        {
            string text = "";

            if (LogOptions.ShowLogIDs)
            {
                text += $"[{LogID:D6}] ";
            }

            if (LogOptions.ShowTimestamps)
            {
                text += $"{stamp:MM/dd/yy HH:mm:ss}] ";
            }
            
            switch (type)
            {
                case LogType.VERBOSE:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    return $"{text}[VERBOSE__] {logText}";

                case LogType.EXCEPTION:
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    return $"{text}[EXCEPTION] {logText}";

                case LogType.ERROR:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    return $"{text}[ERROR____] {logText}";

                case LogType.WARNING:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    return $"{text}[WARNING__] {logText}";

                case LogType.INFO:
                    Console.ForegroundColor = ConsoleColor.White;
                    return $"{text}[INFO_____] {logText}";

                default:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    return $"{text}[INVALID__] {logText}";
            }
        }

        internal void HardLog(string text)
        {
            Action action = new Action(() =>
            {
                Lock(LogOptions.HardLogFilePath, (fileStream) =>
                {
                    try
                    {
                        using (StreamWriter streamWriter = new StreamWriter(fileStream))
                        {
                            streamWriter.WriteLine(text);
                            streamWriter.Flush();
                        }
                    }
                    catch (IOException ioe)
                    {
                        Log.Exception(ioe);
                    }
                });
            });

            TaskFlow.Add(action);
        }

        internal void Lock(string path, Action<FileStream> action)
        {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);

            while (true)
            {
                try
                {
                    using (FileStream file = new FileStream(path, FileMode.Append, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        action(file);
                        break;
                    }
                }
                catch (IOException)
                {
                    FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(Path.GetDirectoryName(path))
                    {
                        EnableRaisingEvents = true
                    };

                    fileSystemWatcher.Changed += (o, e) =>
                    {
                        if (Path.GetFullPath(e.FullPath) == Path.GetFullPath(path))
                        {
                            autoResetEvent.Set();
                        }
                    };

                    autoResetEvent.WaitOne();
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                }
            }
        }

        internal Host(LogOptions options = null)
        {
            if (options != null)
            {
                LogOptions = options;
            }

            LoopTask = BeginTaskLoopAsync();
        }
    }
}
