using System.Reactive.Linq;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using TemplatedDataGrid.Primitives;

namespace TemplatedDataGrid
{
    [PseudoClasses(":selected")]
    public class DataGridRow : TemplatedControl
    {
        internal static readonly DirectProperty<DataGridRow, object?> SelectedItemProperty =
            AvaloniaProperty.RegisterDirect<DataGridRow, object?>(
                nameof(SelectedItem), 
                o => o.SelectedItem, 
                (o, v) => o.SelectedItem = v,
                defaultBindingMode: BindingMode.TwoWay);

        internal static readonly DirectProperty<DataGridRow, object?> SelectedCellProperty =
            AvaloniaProperty.RegisterDirect<DataGridRow, object?>(
                nameof(SelectedCell), 
                o => o.SelectedCell, 
                (o, v) => o.SelectedCell = v,
                defaultBindingMode: BindingMode.TwoWay);

        internal static readonly DirectProperty<DataGridRow, AvaloniaList<DataGridColumn>?> ColumnsProperty =
            AvaloniaProperty.RegisterDirect<DataGridRow, AvaloniaList<DataGridColumn>?>(
                nameof(Columns), 
                o => o.Columns, 
                (o, v) => o.Columns = v);

        internal static readonly StyledProperty<DataGridGridLinesVisibility> GridLinesVisibilityProperty = 
            AvaloniaProperty.Register<DataGridRow, DataGridGridLinesVisibility>(nameof(GridLinesVisibility));

        private object? _selectedItem;
        private object? _selectedCell;
        private AvaloniaList<DataGridColumn>? _columns;
        private DataGridCellsPresenter? _cellsPresenter;
        private Visual? _bottomGridLine;

        public DataGridRow()
        {
            UpdatePseudoClassesSelectedItem(SelectedItem);
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

        internal AvaloniaList<DataGridColumn>? Columns
        {
            get => _columns;
            set => SetAndRaise(ColumnsProperty, ref _columns, value);
        }

        internal DataGridGridLinesVisibility GridLinesVisibility
        {
            get => GetValue(GridLinesVisibilityProperty);
            set => SetValue(GridLinesVisibilityProperty, value);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _cellsPresenter = e.NameScope.Find<DataGridCellsPresenter>("PART_CellsPresenter");
            _bottomGridLine = e.NameScope.Find<Visual>("PART_BottomGridLine");

            InvalidateCellsPresenter();
            InvalidateBottomGridLine();

            UpdatePseudoClassesSelectedItem(SelectedItem);
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == DataContextProperty)
            {
                UpdatePseudoClassesSelectedItem(SelectedItem);
            }

            if (change.Property == SelectedItemProperty)
            {
                UpdatePseudoClassesSelectedItem(change.NewValue.GetValueOrDefault<object?>());
            }

            if (change.Property == SelectedCellProperty)
            {
                // TODO:
            }

            if (change.Property == ColumnsProperty)
            {
                InvalidateCellsPresenter();
            }
        }

        private void InvalidateCellsPresenter()
        {
            if (_cellsPresenter is { })
            {
                _cellsPresenter[!!DataGridCellsPresenter.SelectedItemProperty] = this[!!DataGridRow.SelectedItemProperty];
                _cellsPresenter[!!DataGridCellsPresenter.SelectedCellProperty] = this[!!DataGridRow.SelectedCellProperty];
                _cellsPresenter[!DataGridCellsPresenter.ColumnsProperty] = this[!DataGridRow.ColumnsProperty];
            }
        }

        private void InvalidateBottomGridLine()
        {
            if (_bottomGridLine is null)
            {
                return;
            }

            var isHorizontalGridLineVisible = this
                .GetObservable(DataGridRow.GridLinesVisibilityProperty)
                .Select(x => x.HasFlag(DataGridGridLinesVisibility.Horizontal));

            _bottomGridLine[!Visual.IsVisibleProperty] = isHorizontalGridLineVisible.ToBinding();
        }

        private void UpdatePseudoClassesSelectedItem(object? selectedItem)
        {
            PseudoClasses.Set(":selected", DataContext is { } && selectedItem == DataContext);
        }
    }
}
