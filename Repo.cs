using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SuperFileSearcher
{
    public class Repo: INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;
        public string _RepoPath { get; set; } = string.Empty;
        public string _AppendedFolder { get; set; } = string.Empty;

        public string AppendedFolder
        {
            get => _AppendedFolder;
            set
            {
                _AppendedFolder = value;
                NotifyPropertyChanged(nameof(AppendedFolder));
                NotifyPropertyChanged(nameof(FullPath));
            }
        }

        public string FullPath { get => AppendedFolder is null ? RepoPath : Path.Combine(RepoPath, AppendedFolder); }

        public string RepoPath
        {
            get => _RepoPath;
            set
            {
                _RepoPath = value;
                NotifyPropertyChanged(nameof(RepoPath));
                NotifyPropertyChanged(nameof(FullPath));
            }
        }

        public Repo() {
            RepoPath = string.Empty;
            AppendedFolder = string.Empty;
        }

        public Repo(string path)
        {
            RepoPath = path;
            AppendedFolder = string.Empty;
        }

        public IEnumerable<File> GetFiles(string searchPattern)
        {
            string path = Path.Combine(RepoPath, AppendedFolder);
            if(Directory.Exists(path))
             return System.IO.Directory.EnumerateFiles(path, searchPattern).Select(x=>new File(x));
            else
                return Enumerable.Empty<File>();
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
