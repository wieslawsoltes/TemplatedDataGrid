using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;

namespace TemplatedDataGridDemo.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ReadOnlyObservableCollection<ItemViewModel> _items;
        private ItemViewModel? _selectedItem;
        private ListSortDirection? _sortingStateColumn1;
        private ListSortDirection? _sortingStateColumn2;
        private ListSortDirection? _sortingStateColumn3;
        private ListSortDirection? _sortingStateColumn4;
        private ListSortDirection? _sortingStateColumn5;

        public ReadOnlyObservableCollection<ItemViewModel> Items => _items;

        public ItemViewModel? SelectedItem
        {
            get => _selectedItem;
            set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
        }

        public ListSortDirection? SortingStateColumn1
        {
            get => _sortingStateColumn1;
            set => this.RaiseAndSetIfChanged(ref _sortingStateColumn1, value);
        }
        
        public ListSortDirection? SortingStateColumn2
        {
            get => _sortingStateColumn2;
            set => this.RaiseAndSetIfChanged(ref _sortingStateColumn2, value);
        }

        public ListSortDirection? SortingStateColumn3
        {
            get => _sortingStateColumn3;
            set => this.RaiseAndSetIfChanged(ref _sortingStateColumn3, value);
        }

        public ListSortDirection? SortingStateColumn4
        {
            get => _sortingStateColumn4;
            set => this.RaiseAndSetIfChanged(ref _sortingStateColumn4, value);
        }

        public ListSortDirection? SortingStateColumn5
        {
            get => _sortingStateColumn5;
            set => this.RaiseAndSetIfChanged(ref _sortingStateColumn5, value);
        }

        public ICommand SortCommand { get; }

        public ICommand AddItemCommand { get; }

        public ICommand InsertItemCommand { get; }

        public ICommand RemoveItemCommand { get; }

        public ICommand SelectFirstItemCommand { get; }

        public MainWindowViewModel()
        {
            var itemsSourceList = new SourceList<ItemViewModel>();

            var comparer = SortExpressionComparer<ItemViewModel>.Ascending(x => x.Column1);

            var comparerSubject = new Subject<IComparer<ItemViewModel>>();

            var disableSorting = itemsSourceList
                .Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _items);

            var enableSorting = itemsSourceList
                .Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Sort(comparer, comparerChanged: comparerSubject)
                .Bind(out _items);

            var subscription = enableSorting.Subscribe();
            var isSortingEnabled = true;

            int totalItems = 10_000;
            bool enableRandom = false;
            int randomSize = 100;

            var rand = new Random();
            var items = new List<ItemViewModel>();

            for (var i = 0; i < totalItems; i++)
            {
                var item = CreateItem(i);

                items.Add(item);
            }

            itemsSourceList.AddRange(items);

            SortingStateColumn1 = ListSortDirection.Ascending;

            void Sort(string? sortMemberPath)
            {
                switch (sortMemberPath)
                {
                    case null:
                        if (isSortingEnabled)
                        {
                            subscription?.Dispose();
                            subscription = disableSorting?.Subscribe();
                            isSortingEnabled = false;
                        }
                        break;
                    case "Column1":
                        if (!isSortingEnabled)
                        {
                            subscription?.Dispose();
                            subscription = enableSorting?.Subscribe();
                            isSortingEnabled = true;
                        }
                        comparerSubject?.OnNext(
                            SortingStateColumn1 == ListSortDirection.Ascending
                                ? SortExpressionComparer<ItemViewModel>.Ascending(x => x.Column1)
                                : SortExpressionComparer<ItemViewModel>.Descending(x => x.Column1));
                        break;
                    case "Column2":
                        if (!isSortingEnabled)
                        {
                            subscription?.Dispose();
                            subscription = enableSorting?.Subscribe();
                            isSortingEnabled = true;
                        }
                        comparerSubject?.OnNext(
                            SortingStateColumn2 == ListSortDirection.Ascending
                                ? SortExpressionComparer<ItemViewModel>.Ascending(x => x.Column2)
                                : SortExpressionComparer<ItemViewModel>.Descending(x => x.Column2));
                        break;
                    case "Column3":
                        if (!isSortingEnabled)
                        {
                            subscription?.Dispose();
                            subscription = enableSorting?.Subscribe();
                            isSortingEnabled = true;
                        }
                        comparerSubject?.OnNext(
                            SortingStateColumn3 == ListSortDirection.Ascending
                                ? SortExpressionComparer<ItemViewModel>.Ascending(x => x.Column3)
                                : SortExpressionComparer<ItemViewModel>.Descending(x => x.Column3));
                        break;
                    case "Column4":
                        if (!isSortingEnabled)
                        {
                            subscription?.Dispose();
                            subscription = enableSorting?.Subscribe();
                            isSortingEnabled = true;
                        }
                        comparerSubject?.OnNext(
                            SortingStateColumn4 == ListSortDirection.Ascending
                                ? SortExpressionComparer<ItemViewModel>.Ascending(x => x.Column4)
                                : SortExpressionComparer<ItemViewModel>.Descending(x => x.Column4));
                        break;
                    case "Column5":
                        if (!isSortingEnabled)
                        {
                            subscription?.Dispose();
                            subscription = enableSorting?.Subscribe();
                            isSortingEnabled = true;
                        }
                        comparerSubject?.OnNext(
                            SortingStateColumn5 == ListSortDirection.Ascending
                                ? SortExpressionComparer<ItemViewModel>.Ascending(x => x.Column5)
                                : SortExpressionComparer<ItemViewModel>.Descending(x => x.Column5));
                        break;
                }
            }
    
            SortCommand = ReactiveCommand.CreateFromTask<string?>(async sortMemberPath =>
            {
                await Task.Run(() =>
                {
                    Sort(sortMemberPath);
                });
            });

            InsertItemCommand = ReactiveCommand.Create(() =>
            {
                var index = Items.Count;
                var item = CreateItem(index);
                itemsSourceList.Insert(0, item);
            });

            AddItemCommand = ReactiveCommand.Create(() =>
            {
                var index = Items.Count;
                var item = CreateItem(index);
                itemsSourceList.Add(item);
            });

            RemoveItemCommand = ReactiveCommand.Create<ItemViewModel?>((item) =>
            {
                if (item is not null)
                {
                    itemsSourceList.Remove(item);
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
                    $"{index}",
                    enableRandom
                        ? new Thickness(0, rand.NextDouble() * randomSize, 0, rand.NextDouble() * randomSize)
                        : new Thickness(0));
            }
        }
    }
}
