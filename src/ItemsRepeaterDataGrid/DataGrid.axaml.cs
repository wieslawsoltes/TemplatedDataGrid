#define USE_LISTBOX
using System.Collections;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;

namespace ItemsRepeaterDataGrid
{
    public class DataGrid : TemplatedControl
    {
        private Grid? _root;
        private List<Control> _rootChildren = new List<Control>();
        private Grid? _columnHeaders;
        private List<Control> _columnHeadersChildren = new List<Control>();
#if !USE_LISTBOX
        private ScrollViewer? _scrollViewer;
        private ItemsRepeater? _itemsRepeater;
#else
        private ListBox? _listBox;
#endif
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
            _columnHeaders = e.NameScope.Find<Grid>("PART_ColumnHeaders");
#if !USE_LISTBOX
            _scrollViewer = e.NameScope.Find<ScrollViewer>("PART_ScrollViewer");
            _itemsRepeater = e.NameScope.Find<ItemsRepeater>("PART_ItemsRepeater");
#else
            _listBox = e.NameScope.Find<ListBox>("PART_ListBox");
#endif
            InvalidateRoot();
            InvalidateColumnHeaders();
            InvalidateScrollViewer();
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == ColumnsProperty)
            {
                InvalidateRoot();
                InvalidateColumnHeaders();
                InvalidateScrollViewer();
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
 
            //  Generate Root RowDefinitions

            var rowDefinitions = new List<RowDefinition>();

            rowDefinitions.Add(new RowDefinition(GridLength.Auto));
            rowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Pixel)));
            rowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Star)));

            // Generate Root ColumnDefinitions

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

            // Generate Root Vertical Separator's
            var horizontalSeparator = new Separator()
            {
                Height = 1,
                [Grid.RowProperty] = 1,
                [Grid.ColumnProperty] = 0,
                [Grid.ColumnSpanProperty] = columns.Count + (columns.Count - 1)
            };
            _rootChildren.Add(horizontalSeparator);

            // Set ItemsRepeater template
#if !USE_LISTBOX
            if (_itemsRepeater is { })
            {
                _itemsRepeater.ItemTemplate = new FuncDataTemplate<object>(
                    (_, _) => new DataGridRow()
                    {
                        [!DataGridRow.ColumnsProperty] = this[!DataGrid.ColumnsProperty]
                    });
            }
#else
            if (_listBox is { })
            {
                _listBox.ItemTemplate = new FuncDataTemplate<object>(
                    (_, _) => new DataGridRow()
                    {
                        [!DataGridRow.ColumnsProperty] = this[!DataGrid.ColumnsProperty]
                    });
            }
#endif
            foreach (var columnIndex in splitterColumnIndexes)
            {
                // Generate Root Horizontal Separator's
                var verticalSeparator = new Separator()
                {
                    Width = 1,
                    [Grid.RowProperty] = 0,
                    [Grid.RowSpanProperty] = 3,
                    [Grid.ColumnProperty] = columnIndex
                };
                _rootChildren.Add(verticalSeparator);

                // Generate Root GridSplitter's
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

            foreach (var child in _rootChildren)
            {
                _root.Children.Add(child);
            }
        }

        private void InvalidateColumnHeaders()
        {
            if (_columnHeaders is null)
            {
                return;
            }

            foreach (var child in _columnHeadersChildren)
            {
                _columnHeaders.Children.Remove(child);
            }

            var columns = Columns;
 
            // Generate ColumnHeaders ColumnDefinitions

            var columnDefinitions = new List<ColumnDefinition>();

            for (var i = 0; i < columns.Count; i++)
            {
                var column = columns[i];

                var columnDefinition = new ColumnDefinition()
                {
                    [!ColumnDefinition.WidthProperty] = column[!DataGridColumn.WidthProperty]
                };
                columnDefinitions.Add(columnDefinition);

                // Generate ColumnHeaders DataGridColumnHeader's
                var columnHeader = new DataGridColumnHeader()
                {
                    [Grid.ColumnProperty] = columnDefinitions.Count - 1,
                    [!DataGridColumnHeader.HeaderProperty] = column[!DataGridColumn.HeaderProperty]
                };
                _columnHeadersChildren.Add(columnHeader);
 
                if (i < columns.Count - 1)
                {
                    columnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Pixel)));
                }
            }

            _columnHeaders.SetColumnDefinitions(columnDefinitions);

            _columnHeaders.SetValue(Grid.RowProperty, 0);
            _columnHeaders.SetValue(Grid.ColumnProperty, 0);
            _columnHeaders.SetValue(Grid.ColumnSpanProperty,  columns.Count + (columns.Count - 1));

            foreach (var child in _columnHeadersChildren)
            {
                _columnHeaders.Children.Add(child);
            }
        }

        private void InvalidateScrollViewer()
        {
#if !USE_LISTBOX
            if (_scrollViewer is null)
            {
                return;
            }
 
            var columns = Columns;

            _scrollViewer.SetValue(Grid.RowProperty, 2);
            _scrollViewer.SetValue(Grid.ColumnProperty, 0);
            _scrollViewer.SetValue(Grid.ColumnSpanProperty,  columns.Count + (columns.Count - 1));
#else
            if (_listBox is null)
            {
                return;
            }
 
            var columns = Columns;

            _listBox.SetValue(Grid.RowProperty, 2);
            _listBox.SetValue(Grid.ColumnProperty, 0);
            _listBox.SetValue(Grid.ColumnSpanProperty,  columns.Count + (columns.Count - 1));
#endif
        }
    }
}
