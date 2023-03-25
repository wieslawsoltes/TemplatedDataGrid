using System;
using System.Collections;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using TemplatedDataGrid.Controls;
using TemplatedDataGrid.Internal;

namespace TemplatedDataGrid.Primitives
{
    public class TemplatedDataGridRowsPresenter : TemplatedControl
    {
        internal static readonly StyledProperty<IDataTemplate> ItemTemplateProperty =
            AvaloniaProperty.Register<TemplatedDataGridRowsPresenter, IDataTemplate>(nameof(ItemTemplate));

        internal static readonly DirectProperty<TemplatedDataGridRowsPresenter, IEnumerable?> ItemsProperty =
            AvaloniaProperty.RegisterDirect<TemplatedDataGridRowsPresenter, IEnumerable?>(
                nameof(Items), 
                o => o.Items, 
                (o, v) => o.Items = v);

        internal static readonly StyledProperty<bool> AutoScrollToSelectedItemProperty = 
            AvaloniaProperty.Register<TemplatedDataGridRowsPresenter, bool>(nameof(AutoScrollToSelectedItem));

        internal static readonly DirectProperty<TemplatedDataGridRowsPresenter, object?> SelectedItemProperty =
            AvaloniaProperty.RegisterDirect<TemplatedDataGridRowsPresenter, object?>(
                nameof(SelectedItem), 
                o => o.SelectedItem, 
                (o, v) => o.SelectedItem = v,
                defaultBindingMode: BindingMode.TwoWay);

        internal static readonly DirectProperty<TemplatedDataGridRowsPresenter, object?> SelectedCellProperty =
            AvaloniaProperty.RegisterDirect<TemplatedDataGridRowsPresenter, object?>(
                nameof(SelectedCell), 
                o => o.SelectedCell, 
                (o, v) => o.SelectedCell = v,
                defaultBindingMode: BindingMode.TwoWay);

        internal static readonly DirectProperty<TemplatedDataGridRowsPresenter, IScrollable?> ScrollProperty =
            AvaloniaProperty.RegisterDirect<TemplatedDataGridRowsPresenter, IScrollable?>(
                nameof(Scroll), 
                o => o.Scroll, 
                (o, v) => o.Scroll = v);

        internal static readonly DirectProperty<TemplatedDataGridRowsPresenter, AvaloniaList<TemplatedDataGridColumn>?> ColumnsProperty =
            AvaloniaProperty.RegisterDirect<TemplatedDataGridRowsPresenter, AvaloniaList<TemplatedDataGridColumn>?>(
                nameof(Columns), 
                o => o.Columns, 
                (o, v) => o.Columns = v);

        internal static readonly DirectProperty<TemplatedDataGridRowsPresenter, AvaloniaList<TemplatedDataGridRow>> RowsProperty =
            AvaloniaProperty.RegisterDirect<TemplatedDataGridRowsPresenter, AvaloniaList<TemplatedDataGridRow>>(
                nameof(Rows), 
                o => o.Rows, 
                (o, v) => o.Rows = v);

        internal static readonly StyledProperty<TemplatedDataGridGridLinesVisibility> GridLinesVisibilityProperty = 
            AvaloniaProperty.Register<TemplatedDataGridRowsPresenter, TemplatedDataGridGridLinesVisibility>(nameof(GridLinesVisibility));

        private IScrollable? _scroll;
        private AvaloniaList<TemplatedDataGridColumn>? _columns;
        private IEnumerable? _items;
        private object? _selectedItem;
        private object? _selectedCell;
        private AvaloniaList<TemplatedDataGridRow> _rows = new ();
        private ListBox? _listBox;

        internal IDataTemplate ItemTemplate
        {
            get => GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        internal IEnumerable? Items
        {
            get => _items;
            set => SetAndRaise(ItemsProperty, ref _items, value);
        }

        internal object? SelectedItem
        {
            get => _selectedItem;
            set => SetAndRaise(SelectedItemProperty, ref _selectedItem, value);
        }

        internal bool AutoScrollToSelectedItem
        {
            get => GetValue(AutoScrollToSelectedItemProperty);
            set => SetValue(AutoScrollToSelectedItemProperty, value);
        }

        internal object? SelectedCell
        {
            get => _selectedCell;
            set => SetAndRaise(SelectedCellProperty, ref _selectedCell, value);
        }

        internal IScrollable? Scroll
        {
            get => _scroll;
            set => SetAndRaise(ScrollProperty, ref _scroll, value);
        }

        internal AvaloniaList<TemplatedDataGridColumn>? Columns
        {
            get => _columns;
            set => SetAndRaise(ColumnsProperty, ref _columns, value);
        }

        internal AvaloniaList<TemplatedDataGridRow> Rows
        {
            get => _rows;
            set => SetAndRaise(RowsProperty, ref _rows, value);
        }

        internal TemplatedDataGridGridLinesVisibility GridLinesVisibility
        {
            get => GetValue(GridLinesVisibilityProperty);
            set => SetValue(GridLinesVisibilityProperty, value);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _listBox = e.NameScope.Find<ListBox>("PART_ListBox");

            if (_listBox is { })
            {
                var listBoxDisposables = new CompositeDisposable();

                _listBox.OneWayBind(ItemsControl.ItemsProperty, this, ItemsProperty, listBoxDisposables);
                _listBox.OneWayBind(SelectingItemsControl.AutoScrollToSelectedItemProperty, this, AutoScrollToSelectedItemProperty, listBoxDisposables);
                _listBox.OneWayBind(ItemsControl.ItemTemplateProperty, this, ItemTemplateProperty, listBoxDisposables);

                this.TwoWayBind(SelectedItemProperty, _listBox, SelectingItemsControl.SelectedItemProperty, listBoxDisposables);
                this.OneWayBind(ScrollProperty, _listBox, ListBox.ScrollProperty, listBoxDisposables);

                _listBox.ContainerPrepared += (sender, args) =>
                {
                    var container = args.Container;
                    var item = args.Container.DataContext;
                    var index = args.Index;
#if DEBUG
                    Console.WriteLine($"[ContainerPrepared] item='{item}', index='{index}'");
#endif
                    TemplatedDataGridRow.SetItem(container, container.DataContext);
                    TemplatedDataGridRow.SetIndex(container, index);
                };

                _listBox.ContainerClearing += (sender, args) =>
                {
                    var container = args.Container;
                    var item = args.Container.DataContext;
#if DEBUG
                    Console.WriteLine($"[ContainerPrepared] item='{item}'");
#endif
                    // var listBoxItem = container.ContainerControl as ListBoxItem;
                    // var presenter = listBoxItem?.Presenter;
                    // var row = presenter?.Child as TemplatedDataGridRow;
                    // row?.Detach();
                    TemplatedDataGridRow.SetItem(container, null);
                    TemplatedDataGridRow.SetIndex(container, -1);
                };

                _listBox.ContainerIndexChanged += (sender, args) =>
                {
                    var container = args.Container;
                    var item = args.Container.DataContext;
                    var index = args.NewIndex;
#if DEBUG
                    Console.WriteLine($"[ContainerIndexChanged] item='{item}', index='{index}'");
#endif
                    TemplatedDataGridRow.SetItem(container, item);
                    TemplatedDataGridRow.SetIndex(container, index);
                };
            }

            InvalidateItemTemplate();
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == SelectedItemProperty)
            {
                // TODO:
            }

            if (change.Property == SelectedCellProperty)
            {
                // TODO:
            }

            if (change.Property == ColumnsProperty)
            {
                InvalidateItemTemplate();
            }
        }
#if DEBUG
        private int _rowId;
#endif
        private void InvalidateItemTemplate()
        {
            _rows.Clear();

            ItemTemplate = new FuncDataTemplate(
                _ => true,
                (_, _) =>
                {
                    var row = new TemplatedDataGridRow();

                    var disposables = new CompositeDisposable();

                    row.TwoWayBind(TemplatedDataGridRow.SelectedItemProperty, this, TemplatedDataGridRowsPresenter.SelectedItemProperty, disposables);
                    row.TwoWayBind(TemplatedDataGridRow.SelectedCellProperty, this, TemplatedDataGridRowsPresenter.SelectedCellProperty, disposables);
                    row.OneWayBind(TemplatedDataGridRow.ColumnsProperty, this, TemplatedDataGridRowsPresenter.ColumnsProperty, disposables);
                    row.OneWayBind(TemplatedDataGridRow.GridLinesVisibilityProperty, this, TemplatedDataGridRowsPresenter.GridLinesVisibilityProperty, disposables);

                    row.TemplateDisposables = disposables;

#if DEBUG
                    row.Tag = _rowId++;

                    row.DataContextChanged += (_, _) =>
                    {
                        //Console.WriteLine($"[DataContextChanged] Row='{row.Tag}', DataContext='{row.DataContext}'");
                    };

                    row.AttachedToVisualTree += (_, _) =>
                    {
                        //Console.WriteLine($"[AttachedToVisualTree] Row='{row.Tag}'");
                    };

                    row.DetachedFromVisualTree += (_, _) =>
                    {
                        //Console.WriteLine($"[DetachedFromVisualTree] Row='{row.Tag}'");
                    };

                    row.AttachedToLogicalTree += (_, _) =>
                    {
                        //Console.WriteLine($"[AttachedToLogicalTree] Row='{row.Tag}'");
                    };

                    row.DetachedFromLogicalTree += (_, _) =>
                    {
                        //Console.WriteLine($"[DetachedFromLogicalTree] Row='{row.Tag}'");
                    };
#endif

                    _rows.Add(row);
                    return row;
                },
                supportsRecycling: true);
        }
    }
}
