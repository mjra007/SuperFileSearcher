using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace SuperFileSearcher
{
    public class SearchTemplate
    {
        public string Name { get; set; }
        public string FileFilter { get; set; }
        public ObservableCollection<Filter> Filters { get; set; }

        public ObservableCollection<Repo> Repos { get; set; }
        // Parameterless constructor for deserialization
        public SearchTemplate()
        {
            Filters = new ObservableCollection<Filter>();
            Repos = new ObservableCollection<Repo>();
        }

        [JsonConstructor]
        public SearchTemplate(string name, string fileFilter, ObservableCollection<Filter> filters, ObservableCollection<Repo> repos)
        {
            this.Name = name;
            this.FileFilter = fileFilter;
            this.Filters = filters ?? new ObservableCollection<Filter>();
            this.Repos = repos ?? new ObservableCollection<Repo>();
        }
    }
}
