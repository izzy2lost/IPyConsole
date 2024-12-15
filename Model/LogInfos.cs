using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class LogInfos : INotifyPropertyChanged
    {
        public List<LogInfo> Items { get; set; }

        public object this[int index]
        {
            get => Items[index];
            set
            {
                Items[index] = (LogInfo)value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Items)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void AA()
        {
            Logger.WriteMessage += LoggingMethods.LogToConsole;

            Logger.WriteMessage("AA");

            Logger.WriteMessage -= LoggingMethods.LogToConsole;
        }
    }

    public class LogInfo
    {
        [Required]
        private int _id;

        [Required]
        private string _info;

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
            }
        }

        public string Info
        {
            get => _info;
            set
            {
                _info = value;
            }
        }

        public LogInfo(int id, string info)
        {
            Id = id;
            Info = info;
        }
    }

    public static class Logger
    {
        public static Action<string>? WriteMessage;

        public static Severity LogLevel { get; set; } = Severity.Warning;

        public static void LogMessage(string msg)
        {
            //if (WriteMessage is not null)
            //{
            //    WriteMessage(msg);
            //}

            WriteMessage?.Invoke(msg);
        }

        public static void LogMessage(Severity s, string component, string msg)
        {
            if (s < LogLevel)
            {
                return;
            }

            var outputMsg = $"{DateTime.Now}\t{s}\t{component}\t{msg}";
            if (WriteMessage is not null)
            {
                WriteMessage(outputMsg);
            }
        }
    }

    public static class LoggingMethods
    {
        public static void LogToConsole(string message)
        {
            Console.Error.WriteLine(message);
        }
    }

    public enum Severity
    {
        Verbose,
        Trace,
        Information,
        Warning,
        Error,
        Critical
    }

    public class FileLogger
    {
        private readonly string logPath;

        public FileLogger(string path)
        {
            logPath = path;
            Logger.WriteMessage += LogMessage;
        }

        public void DetachLog() => Logger.WriteMessage -= LogMessage;

        private void LogMessage(string msg)
        {
            try
            {
                using (var log = File.AppendText(logPath))
                {
                    log.WriteLine(msg);
                    log.Flush();
                }
            }
            catch (Exception e)
            {

            }
        }
    }

    public class FileSearcher
    {
        public void Search(string directory, string searchPattern, bool searchSubDirs = false)
        {
            if (searchSubDirs)
            {
                var allDirectories = Directory.GetDirectories(directory, "*.*", SearchOption.AllDirectories);
                var completedDirs = 0;
                var totalDirs = allDirectories.Length + 1;
                foreach (var dir in allDirectories)
                {
                    RaiseSearchDirectoryChanged(dir, totalDirs, completedDirs++);

                    SearchDirectory(directory, searchPattern);
                }
            }
            else
            {
                SearchDirectory(directory, searchPattern);
            }
        }

        private void RaiseSearchDirectoryChanged(string directory, int totalDirs, int completedDirs) =>
            _directoryChanged?.Invoke(this, new SearchDirectoryArgs(directory, totalDirs, completedDirs));

        private EventHandler<SearchDirectoryArgs>? _directoryChanged;

        internal event EventHandler<SearchDirectoryArgs> DirectoryChanged
        {
            add { _directoryChanged += value; }
            remove { _directoryChanged -= value; }
        }

        public event EventHandler<FileFoundArgs>? FileFound;

        private void SearchDirectory(string directory, string searchPattern)
        {
            foreach (var file in Directory.EnumerateFiles(directory, searchPattern))
            {
                FileFoundArgs args = RaiseFileFound(file);

                if (args.CancelRequested)
                {
                    break;
                }
            }
        }

        private FileFoundArgs RaiseFileFound(string file)
        {
            var args = new FileFoundArgs(file);

            FileFound?.Invoke(this, args);

            return args;
        }

    }

    public class FileFoundArgs:EventArgs
    {
        public string FoundFile { get; }

        public bool CancelRequested { get; set; }

        public FileFoundArgs(string fileName) => FoundFile = fileName;
    }

    internal class SearchDirectoryArgs : EventArgs
    {
        internal string CurrentSearchDirectory { get; }
        internal int TotalDirs { get; }
        internal int CompletedDirs { get; }

        internal SearchDirectoryArgs(string dir, int totalDirs, int completedDirs)
        {
            CurrentSearchDirectory = dir;
            TotalDirs = totalDirs;
            CompletedDirs = completedDirs;
        }
    }
}