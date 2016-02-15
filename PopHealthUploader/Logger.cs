using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PopHealthUploader
{
    public class Logger
    {
        public const string Extension = ".log";

        public string Identifier { get; set; }
        public string LogPath { get; set; }

        protected string LogFilePath { get; set; }

        public Logger(string identifier, string path)
        {
            Identifier = identifier;
            LogPath = path;
            InitializeFilePath();
        }

        public void InitializeFilePath()
        {
            LogFilePath = Path.Combine(LogPath, Identifier, Extension);
        }

        public void Write(string text, bool timestamp = true)
        {
            var builder = new StringBuilder();
            if (timestamp)
            {
                builder.AppendFormat("{0} - ", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            builder.AppendFormat("{0}\r\n", text);
            File.AppendAllText(LogFilePath, text);
        }

        public void WriteException(Exception exception)
        {
            var builder = new StringBuilder();
            builder.AppendFormat("Caught the following exception:\r\n{0}\r\n{1}", exception.Message, exception.StackTrace);
        }
    }
}
