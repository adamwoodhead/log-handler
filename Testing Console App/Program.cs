using LogHandler;
using LogHandler.Models;
using System;
using System.Threading.Tasks;

namespace Testing_Console_App
{
    class Program
    {
        static void Main(string[] args)
        {
            LogOptions logOptions = new LogOptions()
            {
                AllowedLogLevel = LogType.VERBOSE,
                HardLogFilePath = @"C:\Logs",
                ShowLogIDs      = true,
                ShowTimestamps  = true
            };

            Log.Initialize(null, logOptions);

            Log.Verbose("Log Handler Initialised!");

            Log.Warning("Starting a few async tasks for logging...");

            for (int i = 0; i < 5; i++)
            {
                Task.Run(() => {
                    Log.Info($"Uninterupted, linearly recorded log text - Line 1");
                    Log.Info($"Uninterupted, linearly recorded log text - Line 2");
                    Log.Info($"Uninterupted, linearly recorded log text - Line 3");
                });
            }

            Log.Verbose("Finished those async logs...");

            int number  = 10;
            int zero    = 0;

            try
            {
                int result = number / zero;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);

                Log.Exception(ex);
            }

            Console.ReadKey();
        }
    }
}
