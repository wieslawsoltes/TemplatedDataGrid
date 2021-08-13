using System.Collections;
using System.Collections.Generic;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using TemplatedDataGrid.Primitives;

namespace TemplatedDataGrid
{
    public class DataGrid : TemplatedControl
    {
        public static readonly StyledProperty<AvaloniaList<DataGridColumn>> ColumnsProperty = 
            AvaloniaProperty.Register<DataGrid, AvaloniaList<DataGridColumn>>(nameof(Columns), new AvaloniaList<DataGridColumn>());

        public static readonly StyledProperty<IEnumerable?> ItemsProperty = 
            AvaloniaProperty.Register<DataGrid, IEnumerable?>(nameof(Items));

        public static readonly StyledProperty<object?> SelectedItemProperty = 
            AvaloniaProperty.Register<DataGrid, object?>(nameof(SelectedItem));

        public static readonly StyledProperty<bool> CanUserResizeColumnsProperty = 
            AvaloniaProperty.Register<DataGrid, bool>(nameof(CanUserResizeColumns));

        public static readonly StyledProperty<DataGridGridLinesVisibility> GridLinesVisibilityProperty = 
            AvaloniaProperty.Register<DataGrid, DataGridGridLinesVisibility>(nameof(GridLinesVisibility));

        public static readonly StyledProperty<bool> IsReadOnlyProperty = 
            AvaloniaProperty.Register<DataGrid, bool>(nameof(IsReadOnly));

        private Grid? _root;
        private readonly List<Control> _rootChildren = new List<Control>();
        private DataGridColumnHeadersPresenter? _columnHeadersPresenter;
        private DataGridRowsPresenter? _rowsPresenter;

        public AvaloniaList<DataGridColumn> Columns
        {
            get => GetValue(ColumnsProperty);
            set => SetValue(ColumnsProperty, value);
        }

        public IEnumerable? Items
        {
            get => GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        public object? SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public bool CanUserResizeColumns
        {
            get => GetValue(CanUserResizeColumnsProperty);
            set => SetValue(CanUserResizeColumnsProperty, value);
        }

        public DataGridGridLinesVisibility GridLinesVisibility
        {
            get => GetValue(GridLinesVisibilityProperty);
            set => SetValue(GridLinesVisibilityProperty, value);
        }

        public bool IsReadOnly
        {
            get => GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _root = e.NameScope.Find<Grid>("PART_Root");
            _columnHeadersPresenter = e.NameScope.Find<DataGridColumnHeadersPresenter>("PART_ColumnHeadersPresenter");
            _rowsPresenter = e.NameScope.Find<DataGridRowsPresenter>("PART_RowsPresenter");

            InvalidateRoot();
            InvalidateColumnHeadersPresenter();
            InvalidateRowsPresenter();
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

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
        }

        private void InvalidateRoot()
        {
            if (_root is null)
            {
                return;
            }

            foreach (var child in _rootChildren)
            {
                _root.Children.Remove(child);
            }

            var columns = Columns;

            var isVerticalGridLineVisible = this
                .GetObservable(DataGrid.GridLinesVisibilityProperty)
                .Select(x => x.HasFlag(DataGridGridLinesVisibility.Vertical));

            //  Generate RowDefinitions

            var rowDefinitions = new List<RowDefinition>();

            rowDefinitions.Add(new RowDefinition(GridLength.Auto));
            rowDefinitions.Add(new RowDefinition(new GridLength(0, GridUnitType.Pixel)));
            rowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Star)));

            // Generate ColumnDefinitions

            var columnDefinitions = new List<ColumnDefinition>();
            var splitterColumnIndexes = new List<int>();

            for (var i = 0; i < columns.Count; i++)
            {
                var column = columns[i];

                var columnDefinition = new ColumnDefinition()
                {
                    [!!ColumnDefinition.WidthProperty] = column[!!DataGridColumn.WidthProperty],
                    [!!ColumnDefinition.MinWidthProperty] = column[!!DataGridColumn.MinWidthProperty],
                    [!!ColumnDefinition.MaxWidthProperty] = column[!!DataGridColumn.MaxWidthProperty]
                };

                columnDefinitions.Add(columnDefinition);

                if (i < columns.Count - 1)
                {
                    columnDefinitions.Add(new ColumnDefinition(new GridLength(0, GridUnitType.Pixel)));
                    splitterColumnIndexes.Add(columnDefinitions.Count - 1);
                }
            }

            _root.RowDefinitions.Clear();
            _root.RowDefinitions.AddRange(rowDefinitions);

            _root.ColumnDefinitions.Clear();
            _root.ColumnDefinitions.AddRange(columnDefinitions);

            // Generate Horizontal Grid Lines

            var horizontalColumnsGridLine = new Rectangle()
            {
                [Grid.RowProperty] = 1,
                [Grid.ColumnProperty] = 0,
                [Grid.ColumnSpanProperty] = columns.Count + (columns.Count - 1)
            };
            ((IPseudoClasses)horizontalColumnsGridLine.Classes).Add(":horizontal");
            _rootChildren.Add(horizontalColumnsGridLine);

            // Generate Vertical Grid Lines
            // Generate GridSplitter's

            foreach (var columnIndex in splitterColumnIndexes)
            {
                var verticalColumnGridLine = new Rectangle()
                {
                    [Grid.RowProperty] = 0,
                    [Grid.RowSpanProperty] = 1,
                    [Grid.ColumnProperty] = columnIndex
                };
                ((IPseudoClasses)verticalColumnGridLine.Classes).Add(":vertical");
                _rootChildren.Add(verticalColumnGridLine);

                var verticalRowGridLine = new Rectangle()
                {
                    [Grid.RowProperty] = 1,
                    [Grid.RowSpanProperty] = 2,
                    [Grid.ColumnProperty] = columnIndex,
                    [!Rectangle.IsVisibleProperty] = isVerticalGridLineVisible.ToBinding()
                };
                ((IPseudoClasses)verticalRowGridLine.Classes).Add(":vertical");
                _rootChildren.Add(verticalRowGridLine);

                var verticalGridSplitter = new GridSplitter()
                {
                    [Grid.RowProperty] = 0,
                    [Grid.RowSpanProperty] = 3,
                    [Grid.ColumnProperty] = columnIndex,
                    [!GridSplitter.IsEnabledProperty] = this[!DataGrid.CanUserResizeColumnsProperty]
                };
                _rootChildren.Add(verticalGridSplitter);
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
            if (_columnHeadersPresenter is { })
            {
                _columnHeadersPresenter[!DataGridColumnHeadersPresenter.ColumnsProperty] = this[!DataGrid.ColumnsProperty];
            }
        }

        private void InvalidateRowsPresenter()
        {
            if (_rowsPresenter is { })
            {
                _rowsPresenter[!DataGridRowsPresenter.ColumnsProperty] = this[!DataGrid.ColumnsProperty];
                _rowsPresenter[!DataGridRowsPresenter.ItemsProperty] = this[!DataGrid.ItemsProperty];
                _rowsPresenter[!DataGridRowsPresenter.SelectedItemProperty] = this[!DataGrid.SelectedItemProperty];
                _rowsPresenter[!DataGridRowsPresenter.GridLinesVisibilityProperty] = this[!DataGrid.GridLinesVisibilityProperty];
            }
        }
    }
}
