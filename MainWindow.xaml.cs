using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace SuperFileSearcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel _MainWindowContext = new MainWindowViewModel();
         
        public MainWindow()
        { 
            DataContext = _MainWindowContext;
            _MainWindowContext.LoadRepoSources();
            InitializeComponent();
        }

        private void OnAddFilterClick(object sender, RoutedEventArgs e)
        {
            Filter newFilter = new Filter(string.Empty, Filter.FilterType.Contains);
            _MainWindowContext.Filters.Add(newFilter); 
        }

        private TreeViewItem CreateFilterUI(Filter filter)
        {
            var filterPanel = new FrameworkElementFactory(typeof(StackPanel));
            filterPanel.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);

            // Create the ComboBox for filter type selection
            var filterTypeComboBox = new FrameworkElementFactory(typeof(ComboBox));
            filterTypeComboBox.SetValue(ComboBox.WidthProperty, 80.0);
            filterTypeComboBox.SetValue(ComboBox.MarginProperty, new Thickness(2, 2, 2, 2));
            filterTypeComboBox.SetBinding(ComboBox.SelectedValueProperty, new Binding("Type") { Mode = BindingMode.TwoWay });
            filterTypeComboBox.SetValue(ComboBox.SelectedValuePathProperty, "Content");

            var comboBoxItemContains= new FrameworkElementFactory(typeof(ComboBoxItem));
            var comboBoxItemStartsWith = new FrameworkElementFactory(typeof(ComboBoxItem));
            var comboBoxItemEndsWith = new FrameworkElementFactory(typeof(ComboBoxItem));
            comboBoxItemContains.SetValue(ComboBoxItem.ContentProperty, Filter.FilterType.Contains);
            comboBoxItemStartsWith.SetValue(ComboBoxItem.ContentProperty, Filter.FilterType.StartsWith);
            comboBoxItemEndsWith.SetValue(ComboBoxItem.ContentProperty, Filter.FilterType.EndsWith);
            // Add ComboBoxItems for the filter types
            filterTypeComboBox.AppendChild(comboBoxItemContains);
            filterTypeComboBox.AppendChild(comboBoxItemStartsWith);
            filterTypeComboBox.AppendChild(comboBoxItemEndsWith);

            var addButtonFactory = new FrameworkElementFactory(typeof(Button));
            addButtonFactory.SetValue(Button.ContentProperty, "+");
            addButtonFactory.SetValue(Button.WidthProperty, 30.0);
            addButtonFactory.SetValue(Button.MarginProperty, new Thickness(2,2,2,2));
            addButtonFactory.AddHandler(Button.ClickEvent, new RoutedEventHandler(OnAddChildFilterClick));

            // Create a TextBox for the search term
            var textBoxFactory = new FrameworkElementFactory(typeof(TextBox));
            textBoxFactory.SetValue(TextBox.MinWidthProperty, 200.0);
            textBoxFactory.SetValue(TextBox.MaxWidthProperty, 200.0);
            textBoxFactory.SetValue(TextBox.MarginProperty, new Thickness(5));
            textBoxFactory.SetBinding(TextBox.TextProperty, new Binding("SearchTerm") { Mode = BindingMode.TwoWay });

            // Add controls to the filter panel
            filterPanel.AppendChild(filterTypeComboBox);
            filterPanel.AppendChild(textBoxFactory); 
            filterPanel.AppendChild(addButtonFactory);
            // Create a new TreeViewItem and add the filter panel

            // Create a DataTemplate for the TreeViewItem header
            var headerTemplate = new DataTemplate();
            headerTemplate.VisualTree = filterPanel;

            TreeViewItem newFilterItem = new()
            {
                Header = filter,
                HeaderTemplate = headerTemplate,
                IsExpanded = true
            };  
            return newFilterItem;
        }

        private void OnSearchClick(object sender, RoutedEventArgs e)
        {
            _MainWindowContext.Occurrences.Clear();
            foreach (Repo repository in _MainWindowContext.RepoPaths)
            {
                IEnumerable<File> allFiles = repository.GetFiles(FilterFileBox.Text);
                foreach (var file in allFiles)
                {
                    foreach (var occurrence in file.FindOcurrence(_MainWindowContext.Filters))
                    {
                        _MainWindowContext.Occurrences.Add(occurrence);
                    }
                }
            } 
        }

        private void OnAddChildFilterClick(object sender, RoutedEventArgs e)
        {
            Button addButton = sender as Button;
            if (addButton == null) return;
            Filter parentFilter = addButton.DataContext as Filter;
            if (parentFilter == null) return;

            Filter childFilter = new Filter(string.Empty, Filter.FilterType.Contains);
            parentFilter.ChildFilters.Add(childFilter);

            // Find the parent TreeViewItem and expand it
            TreeViewItem parentTreeViewItem = FindTreeViewItem(FilterTree, parentFilter);
            if (parentTreeViewItem != null)
            {
                parentTreeViewItem.IsExpanded = true;
            }
        } 

        private void OnDeleteChildFilterClick(object sender, RoutedEventArgs e)
        {
            Button deleteButton = sender as Button;
            if (deleteButton == null) return;
            Filter filterToDelete = deleteButton.DataContext as Filter;
            if (filterToDelete == null) return;
            DeleteRecursevly(filterToDelete, _MainWindowContext.Filters);
             
        }

        public void DeleteRecursevly(Filter filter, ObservableCollection<Filter> filters)
        {
            foreach (Filter currentFilter in filters)
            {
                if (currentFilter.ID == filter.ID)
                {
                    filters.Remove(currentFilter);
                    return;
                }
                else
                {
                    DeleteRecursevly(filter, currentFilter.ChildFilters);
                }
            }
        }

        // Helper method to find the TreeViewItem for a given data item
        private TreeViewItem FindTreeViewItem(ItemsControl parentContainer, object dataItem)
        {
            if (parentContainer == null || dataItem == null) return null;

            // Try to get the TreeViewItem container for the data item
            TreeViewItem treeViewItem = parentContainer.ItemContainerGenerator.ContainerFromItem(dataItem) as TreeViewItem;
            if (treeViewItem != null)
            {
                return treeViewItem;
            }

            // If not found, recursively search in child items
            foreach (var item in parentContainer.Items)
            {
                var childContainer = parentContainer.ItemContainerGenerator.ContainerFromItem(item) as ItemsControl;
                if (childContainer != null)
                {
                    treeViewItem = FindTreeViewItem(childContainer, dataItem);
                    if (treeViewItem != null)
                    {
                        return treeViewItem;
                    }
                }
            }

            return null;
        }

        private void CopyCellValue_Click(object sender, RoutedEventArgs e)
        {
            if (ResultsListView.SelectedItem is Occurrence selectedOccurrence)
            {
                var column = GetColumnUnderMouse(ResultsListView);
                if (column != null)
                {
                    string cellValue = string.Empty;
                    switch (column.Header)
                    {
                        case "File Name":
                            cellValue = selectedOccurrence.Name;
                            break;
                        case "Path":
                            cellValue = selectedOccurrence.Path;
                            break;
                        case "Line Number":
                            cellValue = selectedOccurrence.LineNumber.ToString();
                            break;
                        case "Line":
                            cellValue = selectedOccurrence.Line;
                            break;
                        default:
                            return;
                    }
                   Clipboard.SetText(cellValue);
                }
            }
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            if (ResultsListView.SelectedItem is Occurrence selectedOccurrence)
            {
                string filePath = selectedOccurrence.Path;

                // Check if the file exists
                if (System.IO.File.Exists(filePath))
                {
                    try
                    {
                        // Open the file with the default application
                        Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to open file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("File does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void CopyFilePath_Click(object sender, RoutedEventArgs e)
        {
            if (ResultsListView.SelectedItem is Occurrence selectedOccurrence)
            {
                Clipboard.SetText(selectedOccurrence.Path);
            }
        }

        private void CopyRow_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected item
            if (ResultsListView.SelectedItem is Occurrence selectedOccurrence)
            {
                // Copy the entire row as a formatted string
                string rowText = $"{selectedOccurrence.Name}, {selectedOccurrence.Path}, {selectedOccurrence.LineNumber}, {selectedOccurrence.Line}";
                Clipboard.SetText(rowText);
            }
        }

        private GridViewColumn GetColumnUnderMouse(ListView listView)
        {
            // Get the mouse position relative to the ListView
            var mousePosition = Mouse.GetPosition(listView);

            // Perform a hit test to find the visual element under the mouse
            var hitTestResult = VisualTreeHelper.HitTest(listView, mousePosition);
            if (hitTestResult == null) return null;

            // Traverse up the visual tree to find the GridViewRowPresenter
            var gridViewRowPresenter = FindParent<GridViewRowPresenter>(hitTestResult.VisualHit);
            if (gridViewRowPresenter == null) return null;

            // Get the ListViewItem containing the row
            var listViewItem = FindParent<ListViewItem>(gridViewRowPresenter);
            if (listViewItem == null) return null;

            // Get the index of the cell under the mouse
            int cellIndex = GetCellIndexUnderMouse(gridViewRowPresenter, mousePosition);
            if (cellIndex < 0 || cellIndex >= gridViewRowPresenter.Columns.Count) return null;

            // Return the corresponding GridViewColumn
            return gridViewRowPresenter.Columns[cellIndex];
        }

        // Helper method to find the index of the cell under the mouse
        private int GetCellIndexUnderMouse(GridViewRowPresenter rowPresenter, Point mousePosition)
        {
            double x = mousePosition.X;
            double accumulatedWidth = 0;

            for (int i = 0; i < rowPresenter.Columns.Count; i++)
            {
                var column = rowPresenter.Columns[i];
                accumulatedWidth += column.ActualWidth;

                if (x <= accumulatedWidth)
                {
                    return i;
                }
            }

            return -1;
        } 
        private void OnAddRepoPathClick(object sender, RoutedEventArgs e)
        {
            string repoPath = RepoPathBox.Text.Trim();
            if (!string.IsNullOrEmpty(repoPath) && !((MainWindowViewModel)DataContext).RepoPaths.Any(rp => rp.RepoPath == repoPath))
            {
                ((MainWindowViewModel)DataContext).RepoPaths.Add(new Repo(repoPath));
                RepoPathBox.Clear();
            }
        }

        private void OnRemoveRepoPathClick(object sender, RoutedEventArgs e)
        {
            if (RepoPathsListBox.SelectedItem is Repo selectedRepoPath)
            {
                ((MainWindowViewModel)DataContext).RepoPaths.Remove(selectedRepoPath);
            }
        }

        // Helper method to find a parent of a specific type
        private static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);
            while (parent != null && !(parent is T))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent as T;
        }

        private void OnLoadRepoPathsClick(object sender, RoutedEventArgs e)
        {
            if (RepoSourceComboBox.SelectedItem is RepoSource selectedSource)
            {
                ((MainWindowViewModel)DataContext).LoadRepoPathsFromSource(selectedSource.FilePath);
            }
        }
        private void OnMassEditRepoPathsClick(object sender, RoutedEventArgs e)
        {
            string folderToAppend = MassEditFolderBox.Text.Trim();
            if (!string.IsNullOrEmpty(folderToAppend))
            {
                var viewModel = (MainWindowViewModel)DataContext;
                for (int i = 0; i < viewModel.RepoPaths.Count; i++)
                {
                    // Append the folder to each repo path
                    viewModel.RepoPaths[i].RepoPath = System.IO.Path.Combine(viewModel.RepoPaths[i].RepoPath, folderToAppend);
                }
            }
        }

        private void OnClearAllRepoPathsClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to clear all repo paths?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                if (DataContext is MainWindowViewModel viewModel)
                {
                    viewModel.RepoPaths.Clear();
                }
            }
        }
    }
}