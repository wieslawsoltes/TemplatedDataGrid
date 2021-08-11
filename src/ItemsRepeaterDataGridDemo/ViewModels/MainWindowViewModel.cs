using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia;
using ReactiveUI;

namespace ItemsRepeaterDataGridDemo.ViewModels
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

            int totalItems = 100_000;

            Task.Run(() =>
            {
                //var rand = new Random();
                var items = new List<ItemViewModel>();

                for (var i = 0; i < totalItems; i++)
                {
                    var item = new ItemViewModel()
                    {
                        Column1 = $"Test {i}-1",
                        Column2 = $"Test {i}-2",
                        Column3 = $"Test {i}-3",
                        //Margin = new Thickness(0, rand.NextDouble() * 100, 0, rand.NextDouble() * 100)
                        Margin = new Thickness(0)
                    };

                    items.Add(item);
                }

                Items = new ObservableCollection<ItemViewModel>(items);
            });
        }
    }
}
