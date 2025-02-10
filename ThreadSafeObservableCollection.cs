using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SuperFileSearcher
{
    public class ThreadSafeObservableCollection<T> : ObservableCollection<T>
    {
        private readonly object _lock = new object();

        public ThreadSafeObservableCollection(IEnumerable<T> collection) : base(collection) {
        }

        public ThreadSafeObservableCollection() : base() { }

        protected override void InsertItem(int index, T item)
        {
            lock (_lock)
            {
                Application.Current.Dispatcher.Invoke(() => base.InsertItem(index, item));
            }
        }

        protected override void RemoveItem(int index)
        {
            lock (_lock)
            {
                Application.Current.Dispatcher.Invoke(() => base.RemoveItem(index));
            }
        }

        protected override void ClearItems()
        {
            lock (_lock)
            {
                Application.Current.Dispatcher.Invoke(() => base.ClearItems());
            }
        }

        protected override void SetItem(int index, T item)
        {
            lock (_lock)
            {
                Application.Current.Dispatcher.Invoke(() => base.SetItem(index, item));
            }
        }
    }
}
