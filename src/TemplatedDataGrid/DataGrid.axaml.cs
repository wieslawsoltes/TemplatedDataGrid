using System.Collections;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace ItemsRepeaterDataGrid
{
    public class DataGrid : TemplatedControl
    {
        private Grid? _root;
        private List<Control> _rootChildren = new List<Control>();
        private DataGridColumnHeadersPresenter? _columnHeadersPresenter;
        private DataGridRowsPresenter? _rowsPresenter;

        public static readonly StyledProperty<AvaloniaList<DataGridColumn>> ColumnsProperty = 
            AvaloniaProperty.Register<DataGrid, AvaloniaList<DataGridColumn>>(nameof(Columns), new AvaloniaList<DataGridColumn>());

        public static readonly StyledProperty<IEnumerable?> ItemsProperty = 
            AvaloniaProperty.Register<DataGrid, IEnumerable?>(nameof(Items));

        public static readonly StyledProperty<object?> SelectedItemProperty = 
            AvaloniaProperty.Register<DataGrid, object?>(nameof(SelectedItem));

        public static readonly StyledProperty<bool> CanUserResizeColumnsProperty = 
            AvaloniaProperty.Register<DataGrid, bool>(nameof(CanUserResizeColumns));

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
 
            //  Generate RowDefinitions

            var rowDefinitions = new List<RowDefinition>();

            rowDefinitions.Add(new RowDefinition(GridLength.Auto));
            rowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Pixel)));
            rowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Star)));

            // Generate ColumnDefinitions

            var columnDefinitions = new List<ColumnDefinition>();
            var splitterColumnIndexes = new List<int>();

            for (var i = 0; i < columns.Count; i++)
            {
                var column = columns[i];

                var columnDefinition = new ColumnDefinition()
                {
                    [!ColumnDefinition.WidthProperty] = column[!!DataGridColumn.WidthProperty]
                };

                columnDefinitions.Add(columnDefinition);

                if (i < columns.Count - 1)
                {
                    columnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Pixel)));
                    splitterColumnIndexes.Add(columnDefinitions.Count - 1);
                }
            }

            _root.SetRowDefinitions(rowDefinitions);
            _root.SetColumnDefinitions(columnDefinitions);

            // Generate Horizontal Separator's

            var horizontalSeparator = new Separator()
            {
                Height = 1,
                [Grid.RowProperty] = 1,
                [Grid.ColumnProperty] = 0,
                [Grid.ColumnSpanProperty] = columns.Count + (columns.Count - 1)
            };
            _rootChildren.Add(horizontalSeparator);

            // Generate Vertical Separator's
            // Generate GridSplitter's

            foreach (var columnIndex in splitterColumnIndexes)
            {
                var verticalSeparator = new Separator()
                {
                    Width = 1,
                    [Grid.RowProperty] = 0,
                    [Grid.RowSpanProperty] = 3,
                    [Grid.ColumnProperty] = columnIndex
                };
                _rootChildren.Add(verticalSeparator);

                var verticalGridSplitter = new GridSplitter()
                {
                    Width = 1,
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
            }
        }
    }
}
