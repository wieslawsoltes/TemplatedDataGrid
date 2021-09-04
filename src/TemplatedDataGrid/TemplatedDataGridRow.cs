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
    public class TemplatedDataGridRow : TemplatedControl
    {
        internal static readonly DirectProperty<TemplatedDataGridRow, object?> SelectedItemProperty =
            AvaloniaProperty.RegisterDirect<TemplatedDataGridRow, object?>(
                nameof(SelectedItem), 
                o => o.SelectedItem, 
                (o, v) => o.SelectedItem = v,
                defaultBindingMode: BindingMode.TwoWay);

        internal static readonly DirectProperty<TemplatedDataGridRow, object?> SelectedCellProperty =
            AvaloniaProperty.RegisterDirect<TemplatedDataGridRow, object?>(
                nameof(SelectedCell), 
                o => o.SelectedCell, 
                (o, v) => o.SelectedCell = v,
                defaultBindingMode: BindingMode.TwoWay);

        internal static readonly DirectProperty<TemplatedDataGridRow, AvaloniaList<TemplatedDataGridColumn>?> ColumnsProperty =
            AvaloniaProperty.RegisterDirect<TemplatedDataGridRow, AvaloniaList<TemplatedDataGridColumn>?>(
                nameof(Columns), 
                o => o.Columns, 
                (o, v) => o.Columns = v);

        internal static readonly StyledProperty<TemplatedDataGridGridLinesVisibility> GridLinesVisibilityProperty = 
            AvaloniaProperty.Register<TemplatedDataGridRow, TemplatedDataGridGridLinesVisibility>(nameof(GridLinesVisibility));

        private object? _selectedItem;
        private object? _selectedCell;
        private AvaloniaList<TemplatedDataGridColumn>? _columns;
        private TemplatedDataGridCellsPresenter? _cellsPresenter;
        private Visual? _bottomGridLine;

        public TemplatedDataGridRow()
        {
            UpdatePseudoClassesSelectedItem(SelectedItem, DataContext);
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

        internal AvaloniaList<TemplatedDataGridColumn>? Columns
        {
            get => _columns;
            set => SetAndRaise(ColumnsProperty, ref _columns, value);
        }

        internal TemplatedDataGridGridLinesVisibility GridLinesVisibility
        {
            get => GetValue(GridLinesVisibilityProperty);
            set => SetValue(GridLinesVisibilityProperty, value);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _cellsPresenter = e.NameScope.Find<TemplatedDataGridCellsPresenter>("PART_CellsPresenter");
            _bottomGridLine = e.NameScope.Find<Visual>("PART_BottomGridLine");

            InvalidateCellsPresenter();
            InvalidateBottomGridLine();

            UpdatePseudoClassesSelectedItem(SelectedItem, DataContext);
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == DataContextProperty)
            {
                UpdatePseudoClassesSelectedItem(SelectedItem, change.NewValue.GetValueOrDefault<object?>());
            }

            if (change.Property == SelectedItemProperty)
            {
                UpdatePseudoClassesSelectedItem(change.NewValue.GetValueOrDefault<object?>(), DataContext);
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
                _cellsPresenter[!!TemplatedDataGridCellsPresenter.SelectedItemProperty] = this[!!TemplatedDataGridRow.SelectedItemProperty];
                _cellsPresenter[!!TemplatedDataGridCellsPresenter.SelectedCellProperty] = this[!!TemplatedDataGridRow.SelectedCellProperty];
                _cellsPresenter[!TemplatedDataGridCellsPresenter.ColumnsProperty] = this[!TemplatedDataGridRow.ColumnsProperty];
            }
        }

        private void InvalidateBottomGridLine()
        {
            if (_bottomGridLine is null)
            {
                return;
            }

            var isHorizontalGridLineVisible = this
                .GetObservable(TemplatedDataGridRow.GridLinesVisibilityProperty)
                .Select(x => x.HasFlag(TemplatedDataGridGridLinesVisibility.Horizontal));

            _bottomGridLine[!Visual.IsVisibleProperty] = isHorizontalGridLineVisible.ToBinding();
        }

        private void UpdatePseudoClassesSelectedItem(object? selectedItem, object? dataContext)
        {
            PseudoClasses.Set(":selected", dataContext is { } && selectedItem == dataContext);
        }
    }
}
