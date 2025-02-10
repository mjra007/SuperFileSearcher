using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SuperFileSearcher
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        public ThreadSafeObservableCollection<RepoSource> RepoSources { get; } = new();
        public ThreadSafeObservableCollection<Repo> RepoPaths { get; set; } = new();
        public ThreadSafeObservableCollection<Filter> Filters { get;  } = new();
        public ThreadSafeObservableCollection<Occurrence> Occurrences { get; } = new();
        public ThreadSafeObservableCollection<SearchTemplate> Templates { get; set; } = new();
        public SearchTemplate SelectedTemplate { get; set; }

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

        public void DeleteTemplate(SearchTemplate template)
        {
            if (template != null)
            {
                Templates.Remove(template);
            }
        }

    }

    public class RepoSource
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
    }
}
