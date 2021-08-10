using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using Demo.ViewModels.DataGrid;
using ReactiveUI;

namespace Demo.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<ItemViewModel> _items;
        private DataGridViewModel _dataGrid;
 
        public ObservableCollection<ItemViewModel> Items
        {
            get => _items;
            set => this.RaiseAndSetIfChanged(ref _items, value);
        }

        public DataGridViewModel DataGrid
        {
            get => _dataGrid;
            set => this.RaiseAndSetIfChanged(ref _dataGrid, value);
        }

        public MainWindowViewModel()
        {
            _items = new ObservableCollection<ItemViewModel>();

            _dataGrid = new DataGridViewModel();

            _dataGrid.Columns = new ObservableCollection<DataGridColumnViewModel>()
            {
                new DataGridColumnViewModel() { Width = GridLength.Parse("150") },
                new DataGridColumnViewModel() { Width = GridLength.Parse("*") },
                new DataGridColumnViewModel() { Width = GridLength.Parse("200") },
            };

            int totalItems = 100_000;

            Task.Run(() =>
            {
                var items = new List<ItemViewModel>();

                for (var i = 0; i < totalItems; i++)
                {
                    var item = new ItemViewModel()
                    {
                        Column1 = $"Test {i}-1",
                        Column2 = $"Test {i}-2",
                        Column3 = $"Test {i}-3"
                    };

                    items.Add(item);
                }

                Items = new ObservableCollection<ItemViewModel>(items);

                _dataGrid.Items = Items;
            });
        }
    }
}
