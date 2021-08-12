using System.Reactive.Linq;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using TemplatedDataGrid.Primitives;

namespace TemplatedDataGrid
{
    public class DataGridRow : TemplatedControl
    {
        internal static readonly StyledProperty<AvaloniaList<DataGridColumn>> ColumnsProperty = 
            AvaloniaProperty.Register<DataGridRow, AvaloniaList<DataGridColumn>>(nameof(Columns), new AvaloniaList<DataGridColumn>());

        internal static readonly StyledProperty<DataGridGridLinesVisibility> GridLinesVisibilityProperty = 
            AvaloniaProperty.Register<DataGridRow, DataGridGridLinesVisibility>(nameof(GridLinesVisibility));

        private DataGridCellsPresenter? _cellsPresenter;
        private Visual? _bottomGridLine;

        internal AvaloniaList<DataGridColumn> Columns
        {
            get => GetValue(ColumnsProperty);
            set => SetValue(ColumnsProperty, value);
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
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == ColumnsProperty)
            {
                InvalidateCellsPresenter();
            }
        }

        private void InvalidateCellsPresenter()
        {
            if (_cellsPresenter is { })
            {
                _cellsPresenter[!DataGridCellsPresenter.ColumnsProperty] = this[!DataGrid.ColumnsProperty];
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
    }
}
