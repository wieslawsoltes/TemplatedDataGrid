using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Controls;
using ReactiveUI;

namespace Demo.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<ItemViewModel> _items;
        private GridLength _column1Width;
        private GridLength _column2Width;
        private GridLength _column3Width;

        public ObservableCollection<ItemViewModel> Items
        {
            get => _items;
            set => this.RaiseAndSetIfChanged(ref _items, value);
        }

        public GridLength Column1Width
        {
            get => _column1Width;
            set => this.RaiseAndSetIfChanged(ref _column1Width, value);
        }

        public GridLength Column2Width
        {
            get => _column2Width;
            set => this.RaiseAndSetIfChanged(ref _column2Width, value);
        }

        public GridLength Column3Width
        {
            get => _column3Width;
            set => this.RaiseAndSetIfChanged(ref _column3Width, value);
        }

        public MainWindowViewModel()
        {
            _items = new ObservableCollection<ItemViewModel>();

            _column1Width = GridLength.Parse("150");
            _column2Width = GridLength.Parse("*");
            _column3Width = GridLength.Parse("200");

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
            });

            this.WhenAnyValue(x => x.Column1Width)
                .Subscribe(x =>
                {
                    Debug.WriteLine($"Column1: {x}");
                });
        }
    }
}
