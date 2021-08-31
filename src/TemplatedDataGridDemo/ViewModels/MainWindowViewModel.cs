using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using ReactiveUI;

namespace TemplatedDataGridDemo.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<ItemViewModel> _items;
        private ItemViewModel? _selectedItem;

        public ObservableCollection<ItemViewModel> Items
        {
            get => _items;
            set => this.RaiseAndSetIfChanged(ref _items, value);
        }

        public ItemViewModel? SelectedItem
        {
            get => _selectedItem;
            set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
        }

        public ICommand AddItemCommand { get; }

        public ICommand InsertItemCommand { get; }

        public ICommand RemoveItemCommand { get; }

        public ICommand SelectFirstItemCommand { get; }

        public MainWindowViewModel()
        {
            _items = new ObservableCollection<ItemViewModel>();

            int totalItems = 100_000;
            bool enableRandom = false;
            int randomSize = 100;

            var rand = new Random();
            var items = new List<ItemViewModel>();

            for (var i = 0; i < totalItems; i++)
            {
                var item = CreateItem(i);

                items.Add(item);
            }

            Items = new ObservableCollection<ItemViewModel>(items);

            InsertItemCommand = ReactiveCommand.Create(() =>
            {
                var index = Items.Count;
                var item = CreateItem(index);
                Items.Insert(0, item);
            });

            AddItemCommand = ReactiveCommand.Create(() =>
            {
                var index = Items.Count;
                var item = CreateItem(index);
                Items.Add(item);
            });

            RemoveItemCommand = ReactiveCommand.Create<ItemViewModel?>((item) =>
            {
                if (item is not null)
                {
                    Items.Remove(item);
                }
            });

            SelectFirstItemCommand = ReactiveCommand.Create(() =>
            {
                SelectedItem = Items.FirstOrDefault();
            });

            ItemViewModel CreateItem(int index)
            {
                return new ItemViewModel(
                    $"Template1 {index}-1",
                    $"Template2 {index}-2",
                    $"Template3 {index}-3",
                    rand.NextDouble() > 0.5,
                    $"Text {index}-5",
                    enableRandom
                        ? new Thickness(0, rand.NextDouble() * randomSize, 0, rand.NextDouble() * randomSize)
                        : new Thickness(0));
            }
        }
    }
}
