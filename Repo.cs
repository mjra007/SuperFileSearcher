using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SuperFileSearcher
{
    public class Repo: INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;
        public string _RepoPath { get; set; }
        public string RepoPath
        {
            get => _RepoPath;
            set
            {
                _RepoPath = value;
                NotifyPropertyChanged(nameof(RepoPath));
            }
        }

        public Repo(string path)
        {
            RepoPath = path;
        }

        public IEnumerable<File> GetFiles(string searchPattern)
        {
            return System.IO.Directory.EnumerateFiles(RepoPath, searchPattern).Select(x=>new File(x));
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
