using System.Collections;
using System.Collections.Generic;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using TemplatedDataGrid.Primitives;

namespace TemplatedDataGrid
{
    public class TemplatedDataGrid : TemplatedControl
    {
        public static readonly DirectProperty<TemplatedDataGrid, AvaloniaList<TemplatedDataGridColumn>> ColumnsProperty =
            AvaloniaProperty.RegisterDirect<TemplatedDataGrid, AvaloniaList<TemplatedDataGridColumn>>(
                nameof(Columns), 
                o => o.Columns);

        public static readonly DirectProperty<TemplatedDataGrid, IEnumerable?> ItemsProperty =
            AvaloniaProperty.RegisterDirect<TemplatedDataGrid, IEnumerable?>(
                nameof(Items), 
                o => o.Items, 
                (o, v) => o.Items = v);

        public static readonly DirectProperty<TemplatedDataGrid, object?> SelectedItemProperty =
            AvaloniaProperty.RegisterDirect<TemplatedDataGrid, object?>(
                nameof(SelectedItem), 
                o => o.SelectedItem, 
                (o, v) => o.SelectedItem = v,
                defaultBindingMode: BindingMode.TwoWay);

        public static readonly StyledProperty<bool> AutoScrollToSelectedItemProperty = 
            AvaloniaProperty.Register<TemplatedDataGrid, bool>(nameof(AutoScrollToSelectedItem));

        internal static readonly DirectProperty<TemplatedDataGrid, object?> SelectedCellProperty =
            AvaloniaProperty.RegisterDirect<TemplatedDataGrid, object?>(
                nameof(SelectedCell), 
                o => o.SelectedCell, 
                (o, v) => o.SelectedCell = v,
                defaultBindingMode: BindingMode.TwoWay);

        public static readonly StyledProperty<bool> CanUserSortColumnsProperty = 
            AvaloniaProperty.Register<TemplatedDataGrid, bool>(nameof(CanUserSortColumns), true);

        public static readonly StyledProperty<bool> CanUserResizeColumnsProperty = 
            AvaloniaProperty.Register<TemplatedDataGrid, bool>(nameof(CanUserResizeColumns));

        public static readonly StyledProperty<TemplatedDataGridGridLinesVisibility> GridLinesVisibilityProperty = 
            AvaloniaProperty.Register<TemplatedDataGrid, TemplatedDataGridGridLinesVisibility>(nameof(GridLinesVisibility));

        public static readonly StyledProperty<bool> IsReadOnlyProperty = 
            AvaloniaProperty.Register<TemplatedDataGrid, bool>(nameof(IsReadOnly));

        public static readonly StyledProperty<bool> AutoGenerateColumnsProperty = 
            AvaloniaProperty.Register<TemplatedDataGrid, bool>(nameof(AutoGenerateColumns));

        private AvaloniaList<TemplatedDataGridColumn> _columns;
        private IEnumerable? _items;
        private object? _selectedItem;
        private object? _selectedCell;
        private Panel? _panel;
        private Grid? _root;
        private readonly List<Control> _rootChildren = new List<Control>();
        private TemplatedDataGridColumnHeadersPresenter? _columnHeadersPresenter;
        private TemplatedDataGridRowsPresenter? _rowsPresenter;

        public TemplatedDataGrid()
        {
            _columns = new AvaloniaList<TemplatedDataGridColumn>();
        }

        public AvaloniaList<TemplatedDataGridColumn> Columns
        {
            get => _columns;
            private set => SetAndRaise(ColumnsProperty, ref _columns, value);
        }

        public IEnumerable? Items
        {
            get => _items;
            set => SetAndRaise(ItemsProperty, ref _items, value);
        }

        public bool AutoScrollToSelectedItem
        {
            get => GetValue(AutoScrollToSelectedItemProperty);
            set => SetValue(AutoScrollToSelectedItemProperty, value);
        }

        public object? SelectedItem
        {
            get => _selectedItem;
            set => SetAndRaise(SelectedItemProperty, ref _selectedItem, value);
        }

        internal object? SelectedCell
        {
            get => _selectedCell;
            set => SetAndRaise(SelectedCellProperty, ref _selectedCell, value);
        }

        public bool CanUserSortColumns
        {
            get => GetValue(CanUserSortColumnsProperty);
            set => SetValue(CanUserSortColumnsProperty, value);
        }

        public bool CanUserResizeColumns
        {
            get => GetValue(CanUserResizeColumnsProperty);
            set => SetValue(CanUserResizeColumnsProperty, value);
        }

        public TemplatedDataGridGridLinesVisibility GridLinesVisibility
        {
            get => GetValue(GridLinesVisibilityProperty);
            set => SetValue(GridLinesVisibilityProperty, value);
        }

        public bool IsReadOnly
        {
            get => GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        public bool AutoGenerateColumns
        {
            get => GetValue(AutoGenerateColumnsProperty);
            set => SetValue(AutoGenerateColumnsProperty, value);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _panel = e.NameScope.Find<Panel>("PART_Panel");
            _root = e.NameScope.Find<Grid>("PART_Root");
            _columnHeadersPresenter = e.NameScope.Find<TemplatedDataGridColumnHeadersPresenter>("PART_ColumnHeadersPresenter");
            _rowsPresenter = e.NameScope.Find<TemplatedDataGridRowsPresenter>("PART_RowsPresenter");

            InvalidateRoot();
            InvalidateColumnHeadersPresenter();
            InvalidateRowsPresenter();
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == SelectedItemProperty)
            {
                // TODO:
            }

            if (change.Property == ColumnsProperty)
            {
                InvalidateRoot();
                InvalidateColumnHeadersPresenter();
                InvalidateRowsPresenter();
            }

            if (change.Property == ItemsProperty)
            {
                InvalidateRowsPresenter();
            }

            if (change.Property == SelectedCellProperty)
            {
                // TODO:
            }
        }

        private void InvalidateRoot()
        {
            if (_root is null || _panel is null)
            {
                return;
            }

            foreach (var child in _rootChildren)
            {
                _root.Children.Remove(child);
            }

            var columns = Columns;

            var isVerticalGridLineVisible = this
                .GetObservable(TemplatedDataGrid.GridLinesVisibilityProperty)
                .Select(x => x.HasFlag(TemplatedDataGridGridLinesVisibility.Vertical));

            //  Generate RowDefinitions

            var rowDefinitions = new List<RowDefinition>();

            rowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Star)));

            // Generate ColumnDefinitions

            var columnDefinitions = new List<ColumnDefinition>();
            var splitterColumnIndexes = new List<int>();
            var isSharedSizeScope = false;

            for (var i = 0; i < columns.Count; i++)
            {
                var column = columns[i];
                var isStarWidth = column.Width.IsStar;
                var isAutoWidth = column.Width.IsAuto;
                var isPixelWidth = column.Width.IsAbsolute;

                var columnDefinition = new ColumnDefinition
                {
                    [!ColumnDefinition.MinWidthProperty] = column[!TemplatedDataGridColumn.MinWidthProperty],
                    [!ColumnDefinition.MaxWidthProperty] = column[!TemplatedDataGridColumn.MaxWidthProperty]
                };

                if (isStarWidth)
                {
                    columnDefinition[!ColumnDefinition.WidthProperty] = 
                        column.GetObservable(TemplatedDataGridColumn.ActualWidthProperty)
                              .Select(x => new GridLength(x, GridUnitType.Pixel))
                              .ToBinding();
                }

                if (isAutoWidth)
                {
                    columnDefinition[!ColumnDefinition.WidthProperty] = column[!TemplatedDataGridColumn.WidthProperty];
                    columnDefinition.SetValue(DefinitionBase.SharedSizeGroupProperty, $"Column{i}");
                    isSharedSizeScope = true;
                }

                if (isPixelWidth)
                {
                    columnDefinition[!ColumnDefinition.WidthProperty] = column[!TemplatedDataGridColumn.WidthProperty];
                }

                columnDefinitions.Add(columnDefinition);

                if (i < columns.Count)
                {
                    columnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Pixel)));
                    splitterColumnIndexes.Add(columnDefinitions.Count - 1);
                }
            }

            columnDefinitions.Add(new ColumnDefinition(GridLength.Auto));

            Grid.SetIsSharedSizeScope(_panel, isSharedSizeScope);

            _root.RowDefinitions.Clear();
            _root.RowDefinitions.AddRange(rowDefinitions);

            _root.ColumnDefinitions.Clear();
            _root.ColumnDefinitions.AddRange(columnDefinitions);

            // Generate Vertical Grid Lines

            foreach (var columnIndex in splitterColumnIndexes)
            {
                var verticalRowGridLine = new Rectangle
                {
                    [Grid.RowProperty] = 1,
                    [Grid.RowSpanProperty] = 1,
                    [Grid.ColumnProperty] = columnIndex,
                    [Rectangle.IsHitTestVisibleProperty] = false,
                    [!Rectangle.IsVisibleProperty] = isVerticalGridLineVisible.ToBinding()
                };
                ((IPseudoClasses)verticalRowGridLine.Classes).Add(":vertical");
                _rootChildren.Add(verticalRowGridLine);
            }

            if (_columnHeadersPresenter is { })
            {
                _columnHeadersPresenter.SetValue(Grid.RowProperty, 0);
                _columnHeadersPresenter.SetValue(Grid.ColumnProperty, 0);
                _columnHeadersPresenter.SetValue(Grid.ColumnSpanProperty,  columns.Count + (columns.Count - 1));
            }

            if (_rowsPresenter is { })
            {
                _rowsPresenter.SetValue(Grid.RowProperty, 2);
                _rowsPresenter.SetValue(Grid.ColumnProperty, 0);
                _rowsPresenter.SetValue(Grid.ColumnSpanProperty,  columns.Count + (columns.Count - 1));
            }

            foreach (var child in _rootChildren)
            {
                _root.Children.Add(child);
            }
        }

        private void InvalidateColumnHeadersPresenter()
        {
            if (_columnHeadersPresenter is { } && _rowsPresenter is { })
            {
                _columnHeadersPresenter[!TemplatedDataGridColumnHeadersPresenter.ColumnsProperty] = this[!TemplatedDataGrid.ColumnsProperty];
                _columnHeadersPresenter[!TemplatedDataGridColumnHeadersPresenter.ScrollProperty] = _rowsPresenter[!TemplatedDataGridRowsPresenter.ScrollProperty];
                _columnHeadersPresenter[!TemplatedDataGridColumnHeadersPresenter.CanUserSortColumnsProperty] = this[!TemplatedDataGrid.CanUserSortColumnsProperty];
                _columnHeadersPresenter[!TemplatedDataGridColumnHeadersPresenter.CanUserResizeColumnsProperty] = this[!TemplatedDataGrid.CanUserResizeColumnsProperty];
            }
        }

        private void InvalidateRowsPresenter()
        {
            if (_rowsPresenter is { })
            {
                _rowsPresenter[!TemplatedDataGridRowsPresenter.ColumnsProperty] = this[!TemplatedDataGrid.ColumnsProperty];
                _rowsPresenter[!TemplatedDataGridRowsPresenter.ItemsProperty] = this[!TemplatedDataGrid.ItemsProperty];
                _rowsPresenter[!TemplatedDataGridRowsPresenter.AutoScrollToSelectedItemProperty] = this[!TemplatedDataGrid.AutoScrollToSelectedItemProperty];
                _rowsPresenter[!!TemplatedDataGridRowsPresenter.SelectedItemProperty] = this[!!TemplatedDataGrid.SelectedItemProperty];
                _rowsPresenter[!!TemplatedDataGridRowsPresenter.SelectedCellProperty] = this[!!TemplatedDataGrid.SelectedCellProperty];
                _rowsPresenter[!TemplatedDataGridRowsPresenter.GridLinesVisibilityProperty] = this[!TemplatedDataGrid.GridLinesVisibilityProperty];
            }
        }
    }
}
