using System.Collections.Generic;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace TemplatedDataGrid
{
    public class DataGridRow : TemplatedControl
    {
        private Grid? _root;
        private List<Control> _rootChildren = new List<Control>();

        public static readonly StyledProperty<AvaloniaList<DataGridColumn>> ColumnsProperty = 
            AvaloniaProperty.Register<DataGridRow, AvaloniaList<DataGridColumn>>(nameof(Columns), new AvaloniaList<DataGridColumn>());

        public AvaloniaList<DataGridColumn> Columns
        {
            get => GetValue(ColumnsProperty);
            set => SetValue(ColumnsProperty, value);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _root = e.NameScope.Find<Grid>("PART_Root");

            InvalidateRoot();
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == ColumnsProperty)
            {
                InvalidateRoot();
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
 
            // Generate ColumnDefinitions

            var columnDefinitions = new List<ColumnDefinition>();

            for (var i = 0; i < columns.Count; i++)
            {
                var column = columns[i];

                var columnDefinition = new ColumnDefinition()
                {
                    [!ColumnDefinition.WidthProperty] = column[!DataGridColumn.WidthProperty],
                    [!ColumnDefinition.MinWidthProperty] = column[!DataGridColumn.MinWidthProperty],
                    [!ColumnDefinition.MaxWidthProperty] = column[!DataGridColumn.MaxWidthProperty]
                };
                columnDefinitions.Add(columnDefinition);

                // Generate DataGridCell's

                if (column is DataGridTemplateColumn templateColumn)
                {
                    var cell = new DataGridCell()
                    {
                        [Grid.ColumnProperty] = columnDefinitions.Count - 1,
                        [!DataGridCell.ContentProperty] = this[!DataGridRow.DataContextProperty],
                        [!DataGridCell.CellTemplateProperty] = templateColumn[!DataGridTemplateColumn.CellTemplateProperty]
                    };
                    _rootChildren.Add(cell);
                }
                else
                {
                    // TODO: Add support of other column types.
                }

                if (i < columns.Count - 1)
                {
                    columnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Pixel)));
                }
            }

            _root.ColumnDefinitions.Clear();
            _root.ColumnDefinitions.AddRange(columnDefinitions);

            foreach (var child in _rootChildren)
            {
                _root.Children.Add(child);
            }
        }
    }
}
