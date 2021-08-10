using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ReactiveUI;

namespace Demo.ViewModels
{

    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<ItemViewModel> _items;

        public ObservableCollection<ItemViewModel> Items
        {
            get => _items;
            set => this.RaiseAndSetIfChanged(ref _items, value);
        }

        public MainWindowViewModel()
        {
            _items = new ObservableCollection<ItemViewModel>();

            Task.Run(() =>
            {
                var items = new List<ItemViewModel>();

                for (var i = 0; i < 100_000; i++)
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
            });
        }
    }
}
