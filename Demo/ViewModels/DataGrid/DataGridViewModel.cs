using System.Collections;
using System.Collections.ObjectModel;
using ReactiveUI;

namespace Demo.ViewModels.DataGrid
{
    public class DataGridViewModel : ViewModelBase
    {
        private ObservableCollection<DataGridColumnViewModel>? _columns;
        private IEnumerable? _items;

        public ObservableCollection<DataGridColumnViewModel>? Columns
        {
            get => _columns;
            set => this.RaiseAndSetIfChanged(ref _columns, value);
        }

        public IEnumerable? Items
        {
            get => _items;
            set => this.RaiseAndSetIfChanged(ref _items, value);
        }
    }
}
