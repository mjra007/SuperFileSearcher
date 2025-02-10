using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SuperFileSearcher
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<RepoSource> RepoSources { get; } = new ObservableCollection<RepoSource>();
        public ObservableCollection<Repo> RepoPaths { get; set; } = new ObservableCollection<Repo>();
        public ObservableCollection<Filter> Filters { get;  } = new();
        public ObservableCollection<Occurrence> Occurrences { get; } = new();
        public ObservableCollection<Filter> FolderFilters { get; } = new ObservableCollection<Filter>(); // New collection for folder filters

        public void LoadRepoSources()
        {
            string repoSourceDirectory = "RepoSources"; // Directory containing CSV files
            if (Directory.Exists(Path.Combine(Environment.CurrentDirectory,repoSourceDirectory)))
            {
                foreach (var file in Directory.GetFiles(repoSourceDirectory, "*.txt"))
                {
                    RepoSources.Add(new RepoSource { FilePath = file, FileName = Path.GetFileName(file) });
                }
            }
        }

        public void LoadRepoPathsFromSource(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                RepoPaths.Clear();
                var lines = System.IO.File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        string value = line.Trim();
                        value = value.Replace("{ODATE}", DateTime.Today.ToString("dd-MM-yyyy"));
                        RepoPaths.Add(new Repo(value));
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RepoSource
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
    }
}
