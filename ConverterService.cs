using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf.Logging;

namespace TopshelfWindowsService
{
    public class ConverterService
    {
        private FileSystemWatcher _watcher;
        private static readonly LogWriter _logWriter = HostLogger.Get<ConverterService>();

        public bool Start()
        {
            _watcher = new FileSystemWatcher(@"C:\temp", "*.txt");
            _watcher.Created += _watcher_Created;
            _watcher.IncludeSubdirectories = false;
            _watcher.EnableRaisingEvents = true;
            return true;
        }

        private void _watcher_Created(object sender, FileSystemEventArgs e)
        {
            _logWriter.InfoFormat("Started conversion of {0}", e.FullPath);
            if (e.FullPath.Contains("badfile"))
            {
                throw new NotSupportedException("This file conversion is not supported");
            }
            string content = File.ReadAllText(e.FullPath);
            string upperContent = content.ToUpperInvariant();
            var convertedFilePath = Path.Combine(Path.GetDirectoryName(e.FullPath), Path.GetFileName(e.FullPath) + "_converted");
            File.WriteAllText(convertedFilePath, upperContent);
        }

        public bool Stop()
        {
            _watcher.Dispose();
            return true;
        }

        public bool Pause()
        {
            _watcher.EnableRaisingEvents = false;
            return true;
        }

        public bool Continue()
        {
            _watcher.EnableRaisingEvents = true;
            return true;
        }

        public void CustomCommand(int commandNumber)
        {
            _logWriter.InfoFormat("Got command {0}", commandNumber);
        }
    }
}
