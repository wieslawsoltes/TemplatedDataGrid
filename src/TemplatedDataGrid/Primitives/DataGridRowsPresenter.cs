using System.Collections;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;

namespace TemplatedDataGrid.Primitives
{
    public class DataGridRowsPresenter : TemplatedControl
    {
        public static readonly StyledProperty<IDataTemplate> ItemTemplateProperty =
            AvaloniaProperty.Register<DataGridRowsPresenter, IDataTemplate>(nameof(ItemTemplate));

        public static readonly StyledProperty<IEnumerable?> ItemsProperty = 
            AvaloniaProperty.Register<DataGridRowsPresenter, IEnumerable?>(nameof(Items));

        internal static readonly StyledProperty<object?> SelectedItemProperty = 
            AvaloniaProperty.Register<DataGridRowsPresenter, object?>(nameof(SelectedItem));

        internal static readonly StyledProperty<object?> SelectedCellProperty = 
            AvaloniaProperty.Register<DataGridRowsPresenter, object?>(nameof(SelectedCell));

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
        private AvaloniaList<DataGridRow> _rows = new AvaloniaList<DataGridRow>();
        private ListBox? _listBox;

        public IDataTemplate ItemTemplate
        {
            get => GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public IEnumerable? Items
        {
            get => GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        internal object? SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        internal object? SelectedCell
        {
            get => GetValue(SelectedCellProperty);
            set => SetValue(SelectedCellProperty, value);
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
                this[!ScrollProperty] = _listBox[!ListBox.ScrollProperty];
                this[!!SelectedItemProperty] = _listBox[!!SelectingItemsControl.SelectedItemProperty];
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

        private void InvalidateItemTemplate()
        {
            _rows.Clear();

            ItemTemplate = new FuncDataTemplate(
                _ => true,
                (_, _) =>
                {
                    var row = new DataGridRow()
                    {
                        [!DataGridRow.SelectedItemProperty] = this[!DataGridRowsPresenter.SelectedItemProperty],
                        [!!DataGridRow.SelectedCellProperty] = this[!!DataGridRowsPresenter.SelectedCellProperty],
                        [!DataGridRow.ColumnsProperty] = this[!DataGridRowsPresenter.ColumnsProperty],
                        [!DataGridRow.GridLinesVisibilityProperty] = this[!DataGridRowsPresenter.GridLinesVisibilityProperty]
                    };
                    _rows.Add(row);
                    return row;
                },
                supportsRecycling: true);
        }
    }
}
