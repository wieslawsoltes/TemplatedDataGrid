﻿using System;
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
    public partial class MainWindowViewModel : ViewModelBase
    {
        private ReadOnlyObservableCollection<ItemViewModel>? _items;

        public ReadOnlyObservableCollection<ItemViewModel>? Items => _items;

        [Reactive]
        public partial ItemViewModel? SelectedItem { get; set; }
        
        [Reactive]
        public partial ListSortDirection? SortingStateColumn1 { get; set; }
        
        [Reactive]
        public partial ListSortDirection? SortingStateColumn2 { get; set; }
        
        [Reactive]
        public partial ListSortDirection? SortingStateColumn3 { get; set; }
        
        [Reactive]
        public partial ListSortDirection? SortingStateColumn4 { get; set; }
        
        [Reactive]
        public partial ListSortDirection? SortingStateColumn5 { get; set; }
        
        public ICommand SortCommand { get; }

        public ICommand AddItemCommand { get; }

        public ICommand InsertItemCommand { get; }

        public ICommand RemoveItemCommand { get; }

        public ICommand SelectFirstItemCommand { get; }

        public MainWindowViewModel()
        {
            var itemsSourceList = new SourceList<ItemViewModel>();
            var comparerSubject = new Subject<IComparer<ItemViewModel>>();
            var isSortingEnabled = false;
            var totalItems = 1_000;
            var enableRandom = false;
            var randomSize = 100;
            var rand = new Random();
            var items = new List<ItemViewModel>();

            for (var i = 0; i < totalItems; i++)
            {
                items.Add(CreateItem(i));
            }
            itemsSourceList.AddRange(items);

            IDisposable? subscription = null;
            SortingStateColumn5 = ListSortDirection.Ascending;
            EnableSort(x => x.Column5, SortingStateColumn5);

            ItemViewModel CreateItem(int index)
            {
                return new ItemViewModel(
                    $"Template1 {index}-1",
                    $"Template2 {index}-2",
                    $"Template3 {index}-3",
                    rand.NextDouble() > 0.5,
                    index,
                    enableRandom
                        ? new Thickness(0, rand.NextDouble() * randomSize, 0, rand.NextDouble() * randomSize)
                        : new Thickness(0));
            }

            IObservable<IChangeSet<ItemViewModel>> GetSortObservable(IComparer<ItemViewModel> comparer)
            {
                return itemsSourceList!
                    .Connect()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Sort(comparer, comparerChanged: comparerSubject)
                    .Bind(out _items);
            }

            IObservable<IChangeSet<ItemViewModel>> GetDefaultObservable()
            {
                return itemsSourceList!
                    .Connect()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(out _items);
            }

            void EnableSort(Func<ItemViewModel, IComparable> expression, ListSortDirection? listSortDirection)
            {
                var sortExpressionComparer = listSortDirection == ListSortDirection.Ascending
                    ? SortExpressionComparer<ItemViewModel>.Ascending(expression)
                    : SortExpressionComparer<ItemViewModel>.Descending(expression);

                if (!isSortingEnabled)
                {
                    subscription?.Dispose();
                    subscription = GetSortObservable(sortExpressionComparer).Subscribe();
                    isSortingEnabled = true;
                    this.RaisePropertyChanged(nameof(Items));
                }
                else
                {
                    comparerSubject.OnNext(sortExpressionComparer);
                }
            }

            void DisableSort()
            {
                if (isSortingEnabled)
                {
                    subscription?.Dispose();
                    subscription = GetDefaultObservable().Subscribe();
                    isSortingEnabled = false;
                    this.RaisePropertyChanged(nameof(Items));
                }
            }

            void Sort(string? sortMemberPath)
            {
                switch (sortMemberPath)
                {
                    case null:
                        DisableSort();
                        break;
                    case "Column1":
                        EnableSort(x => x.Column1, SortingStateColumn1);
                        break;
                    case "Column2":
                        EnableSort(x => x.Column2, SortingStateColumn2);
                        break;
                    case "Column3":
                        EnableSort(x => x.Column3, SortingStateColumn3);
                        break;
                    case "Column4":
                        EnableSort(x => x.Column4, SortingStateColumn4);
                        break;
                    case "Column5":
                        EnableSort(x => x.Column5, SortingStateColumn5);
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
                if (_items is null)
                {
                    return;
                }
                var index = _items.Count;
                var item = CreateItem(index);
                itemsSourceList.Insert(0, item);
            });

            AddItemCommand = ReactiveCommand.Create(() =>
            {
                if (_items is null)
                {
                    return;
                }
                var index = _items.Count;
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
                if (_items is null)
                {
                    return;
                }
                SelectedItem = _items.FirstOrDefault();
            });
        }
    }
}
