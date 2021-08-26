using System.Reactive.Linq;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using TemplatedDataGrid.Primitives;

namespace TemplatedDataGrid
{
    [PseudoClasses(":selected")]
    public class DataGridRow : TemplatedControl
    {
        internal static readonly StyledProperty<object?> SelectedItemProperty = 
            AvaloniaProperty.Register<DataGridRow, object?>(nameof(SelectedItem));

        internal static readonly DirectProperty<DataGridRow, AvaloniaList<DataGridColumn>?> ColumnsProperty =
            AvaloniaProperty.RegisterDirect<DataGridRow, AvaloniaList<DataGridColumn>?>(
                nameof(Columns), 
                o => o.Columns, 
                (o, v) => o.Columns = v);

        internal static readonly StyledProperty<DataGridGridLinesVisibility> GridLinesVisibilityProperty = 
            AvaloniaProperty.Register<DataGridRow, DataGridGridLinesVisibility>(nameof(GridLinesVisibility));

        private AvaloniaList<DataGridColumn>? _columns;
        private DataGridCellsPresenter? _cellsPresenter;
        private Visual? _bottomGridLine;

        internal object? SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
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

        public DataGridRow()
        {
            UpdatePseudoClassesSelectedItem(SelectedItem);
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

            if (change.Property == SelectedItemProperty)
            {
                UpdatePseudoClassesSelectedItem(change.NewValue.GetValueOrDefault<object?>());
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
                _cellsPresenter[!DataGridCellsPresenter.ColumnsProperty] = this[!DataGridRow.ColumnsProperty];
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
