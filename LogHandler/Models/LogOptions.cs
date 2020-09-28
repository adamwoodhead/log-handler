using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LogHandler.Models
{
    public class LogOptions
    {
        public LogType AllowedLogLevel { get; set; } = LogType.VERBOSE;

        public bool ShowTimestamps { get; set; } = true;

        public bool ShowLogIDs { get; set; } = true;

        public string HardLogFilePath { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "log.log");
    }
}
