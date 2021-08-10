using System.Collections.ObjectModel;
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
            _items = new ObservableCollection<ItemViewModel>()
            {
                // new ItemViewModel()
                // {
                //     Column1 = "Test 1-1",
                //     Column2 = "Test 1-2",
                //     Column3 = "Test 1-3"
                // },
                // new ItemViewModel()
                // {
                //     Column1 = "Test 2-1",
                //     Column2 = "Test 2-2",
                //     Column3 = "Test 2-3"
                // },
                // new ItemViewModel()
                // {
                //     Column1 = "Test 3-1",
                //     Column2 = "Test 3-2",
                //     Column3 = "Test 3-3"
                // },
            };

            for (var i = 0; i < 100_000; i++)
            {
                var item = new ItemViewModel()
                {
                    Column1 = $"Test {i}-1",
                    Column2 = $"Test {i}-2",
                    Column3 = $"Test {i}-3"
                };

                _items.Add(item);
            }
        }
    }
}
