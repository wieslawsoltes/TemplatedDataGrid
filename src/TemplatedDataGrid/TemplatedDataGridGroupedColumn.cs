using Avalonia;
using Avalonia.Collections;

namespace TemplatedDataGrid
{
    public class TemplatedDataGridGroupedColumn : TemplatedDataGridColumn
    {
        private AvaloniaList<TemplatedDataGridColumn> _columns;

        public static readonly DirectProperty<TemplatedDataGrid, AvaloniaList<TemplatedDataGridColumn>> ColumnsProperty =
            AvaloniaProperty.RegisterDirect<TemplatedDataGrid, AvaloniaList<TemplatedDataGridColumn>>(nameof(Columns),
                o => o.Columns);

        public TemplatedDataGridGroupedColumn()
        {
            _columns = new AvaloniaList<TemplatedDataGridColumn>();
        }

        /// <summary>
        /// Child columns
        /// </summary>
        public AvaloniaList<TemplatedDataGridColumn> Columns
        {
            get => _columns;
            private set => SetAndRaise(ColumnsProperty, ref _columns, value);
        }
    }
}
