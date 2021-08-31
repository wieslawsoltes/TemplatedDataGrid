using System.Collections;
using System.Diagnostics;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;

namespace TemplatedDataGrid.Primitives
{
    public class DataGridRowsPresenter : TemplatedControl
    {
        internal static readonly StyledProperty<IDataTemplate> ItemTemplateProperty =
            AvaloniaProperty.Register<DataGridRowsPresenter, IDataTemplate>(nameof(ItemTemplate));

        internal static readonly DirectProperty<DataGridRowsPresenter, IEnumerable?> ItemsProperty =
            AvaloniaProperty.RegisterDirect<DataGridRowsPresenter, IEnumerable?>(
                nameof(Items), 
                o => o.Items, 
                (o, v) => o.Items = v);

        internal static readonly DirectProperty<DataGridRowsPresenter, object?> SelectedItemProperty =
            AvaloniaProperty.RegisterDirect<DataGridRowsPresenter, object?>(
                nameof(SelectedItem), 
                o => o.SelectedItem, 
                (o, v) => o.SelectedItem = v,
                defaultBindingMode: BindingMode.TwoWay);

        internal static readonly DirectProperty<DataGridRowsPresenter, object?> SelectedCellProperty =
            AvaloniaProperty.RegisterDirect<DataGridRowsPresenter, object?>(
                nameof(SelectedCell), 
                o => o.SelectedCell, 
                (o, v) => o.SelectedCell = v,
                defaultBindingMode: BindingMode.TwoWay);

        internal static readonly DirectProperty<DataGridRowsPresenter, IScrollable?> ScrollProperty =
            AvaloniaProperty.RegisterDirect<DataGridRowsPresenter, IScrollable?>(
                nameof(Scroll), 
                o => o.Scroll, 
                (o, v) => o.Scroll = v);

        internal static readonly DirectProperty<DataGridRowsPresenter, AvaloniaList<DataGridColumn>?> ColumnsProperty =
            AvaloniaProperty.RegisterDirect<DataGridRowsPresenter, AvaloniaList<DataGridColumn>?>(
                nameof(Columns), 
                o => o.Columns, 
                (o, v) => o.Columns = v);

        internal static readonly DirectProperty<DataGridRowsPresenter, AvaloniaList<DataGridRow>> RowsProperty =
            AvaloniaProperty.RegisterDirect<DataGridRowsPresenter, AvaloniaList<DataGridRow>>(
                nameof(Rows), 
                o => o.Rows, 
                (o, v) => o.Rows = v);

        internal static readonly StyledProperty<DataGridGridLinesVisibility> GridLinesVisibilityProperty = 
            AvaloniaProperty.Register<DataGridRowsPresenter, DataGridGridLinesVisibility>(nameof(GridLinesVisibility));

        private IScrollable? _scroll;
        private AvaloniaList<DataGridColumn>? _columns;
        private IEnumerable? _items;
        private object? _selectedItem;
        private object? _selectedCell;
        private AvaloniaList<DataGridRow> _rows = new ();
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

        internal AvaloniaList<DataGridColumn>? Columns
        {
            get => _columns;
            set => SetAndRaise(ColumnsProperty, ref _columns, value);
        }

        internal AvaloniaList<DataGridRow> Rows
        {
            get => _rows;
            set => SetAndRaise(RowsProperty, ref _rows, value);
        }

        internal DataGridGridLinesVisibility GridLinesVisibility
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
                _listBox[!ItemsControl.ItemsProperty] = this[!ItemsProperty];
                _listBox[!ItemsControl.ItemTemplateProperty] = this[!ItemTemplateProperty];

                this[!!SelectedItemProperty] = _listBox[!!SelectingItemsControl.SelectedItemProperty];
                this[!ScrollProperty] = _listBox[!ListBox.ScrollProperty];

#if DEBUG
                _listBox.ItemContainerGenerator.Materialized += (sender, args) =>
                {
                    Debug.WriteLine($"[ItemContainerGenerator.Materialized] Containers.Count='{args.Containers.Count}' StartingIndex='{args.StartingIndex}'");
                };

                _listBox.ItemContainerGenerator.Dematerialized += (sender, args) =>
                {
                    Debug.WriteLine($"[ItemContainerGenerator.Dematerialized] Containers.Count='{args.Containers.Count}' StartingIndex='{args.StartingIndex}'");
                };

                _listBox.ItemContainerGenerator.Recycled += (sender, args) =>
                {
                    Debug.WriteLine($"[ItemContainerGenerator.Recycled] Containers.Count='{args.Containers.Count}' StartingIndex='{args.StartingIndex}'");
                };
#endif

            }

            InvalidateItemTemplate();
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
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

        private int rowId = 0;

        private void InvalidateItemTemplate()
        {
            _rows.Clear();

            ItemTemplate = new FuncDataTemplate(
                _ => true,
                (_, _) =>
                {
                    var row = new DataGridRow()
                    {
#if DEBUG
                        Tag = rowId++,
#endif
                        [!!DataGridRow.SelectedItemProperty] = this[!!DataGridRowsPresenter.SelectedItemProperty],
                        [!!DataGridRow.SelectedCellProperty] = this[!!DataGridRowsPresenter.SelectedCellProperty],
                        [!DataGridRow.ColumnsProperty] = this[!DataGridRowsPresenter.ColumnsProperty],
                        [!DataGridRow.GridLinesVisibilityProperty] = this[!DataGridRowsPresenter.GridLinesVisibilityProperty]
                    };

#if DEBUG
                    row.AttachedToVisualTree += (_, _) =>
                    {
                        Debug.WriteLine($"[AttachedToVisualTree] Row='{row.Tag}'");
                    };

                    row.DetachedFromVisualTree += (_, _) =>
                    {
                        Debug.WriteLine($"[DetachedFromVisualTree] Row='{row.Tag}'");
                    };

                    row.AttachedToLogicalTree += (_, _) =>
                    {
                        Debug.WriteLine($"[AttachedToLogicalTree] Row='{row.Tag}'");
                    };

                    row.DetachedFromLogicalTree += (_, _) =>
                    {
                        Debug.WriteLine($"[DetachedFromLogicalTree] Row='{row.Tag}'");
                    };
#endif

                    _rows.Add(row);
                    return row;
                },
                supportsRecycling: true);
        }
    }
}
