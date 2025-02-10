using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SuperFileSearcher
{
    public class Filter : IFilter, INotifyPropertyChanged
    {
        private readonly Guid _ID;
        public enum FilterType { StartsWith, Contains, EndsWith };
        public ObservableCollection<Filter> ChildFilters { get; } = new();
        private FilterType _filterType;
        private string _searchTerm;

        public event PropertyChangedEventHandler? PropertyChanged;

        public FilterType Type
        {
            get => _filterType;
            set
            {
                _filterType = value;
                NotifyPropertyChanged(nameof(Type));
            }
        }

        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                _searchTerm = value;
                NotifyPropertyChanged(nameof(SearchTerm));
            }
        }


        public Filter(string searchTerm, FilterType filterType)
        {
            _ID = Guid.NewGuid();
            Type= filterType;
            _searchTerm = searchTerm;
        }
         
        public Guid ID()
        {
            return _ID;
        }

        public bool DoesMatch(string line)
        {
            bool isTrue = false;
            if (Type == FilterType.StartsWith)
            {
                 isTrue=line.StartsWith(SearchTerm);
            }
            else if (Type == FilterType.Contains)
            {
                 isTrue=line.Contains(SearchTerm);
            }
            else
            {
                 isTrue=line.EndsWith(SearchTerm);
            }

            //if first filter is true we need to check the child filters
            if (isTrue && ChildFilters.Any())
            {
                return CheckChildFilters(line);
            }
            else
            {
                return isTrue;
            }
        }

        private bool CheckChildFilters(string line)
        {
            foreach (IFilter filter in ChildFilters)
            {
                if (filter.DoesMatch(line))
                {
                    //if one of the child filters is true then we return true
                    return true;
                }
            }
            return false;
        }

        public void AppendChildFilter(Filter filter)
        {
            ChildFilters.Append(filter);
        }

        public void RemoveChildFilter(Guid id)
        {
            ChildFilters.Remove(ChildFilters.First(x => x.ID() == id));
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}