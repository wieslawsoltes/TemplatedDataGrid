using Avalonia.Controls;
using ReactiveUI;

namespace Demo.ViewModels.DataGrid
{
    public class DataGridColumnViewModel : ViewModelBase
    {
        private GridLength _width;

        public GridLength Width
        {
            get => _width;
            set => this.RaiseAndSetIfChanged(ref _width, value);
        }
    }
}
