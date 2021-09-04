using System.Collections.Generic;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;

namespace TemplatedDataGrid.Primitives
{
    public class TemplatedDataGridCellsPresenter : TemplatedControl
    {
        internal static readonly DirectProperty<TemplatedDataGridCellsPresenter, object?> SelectedItemProperty =
            AvaloniaProperty.RegisterDirect<TemplatedDataGridCellsPresenter, object?>(
                nameof(SelectedItem), 
                o => o.SelectedItem, 
                (o, v) => o.SelectedItem = v,
                defaultBindingMode: BindingMode.TwoWay);

        internal static readonly DirectProperty<TemplatedDataGridCellsPresenter, object?> SelectedCellProperty =
            AvaloniaProperty.RegisterDirect<TemplatedDataGridCellsPresenter, object?>(
                nameof(SelectedCell), 
                o => o.SelectedCell, 
                (o, v) => o.SelectedCell = v,
                defaultBindingMode: BindingMode.TwoWay);

        internal static readonly DirectProperty<TemplatedDataGridCellsPresenter, AvaloniaList<TemplatedDataGridColumn>?> ColumnsProperty =
            AvaloniaProperty.RegisterDirect<TemplatedDataGridCellsPresenter, AvaloniaList<TemplatedDataGridColumn>?>(
                nameof(Columns), 
                o => o.Columns, 
                (o, v) => o.Columns = v);

        internal static readonly DirectProperty<TemplatedDataGridCellsPresenter, AvaloniaList<TemplatedDataGridCell>> CellsProperty =
            AvaloniaProperty.RegisterDirect<TemplatedDataGridCellsPresenter, AvaloniaList<TemplatedDataGridCell>>(
                nameof(Cells), 
                o => o.Cells, 
                (o, v) => o.Cells = v);

        private object? _selectedItem;
        private object? _selectedCell;
        private AvaloniaList<TemplatedDataGridColumn>? _columns;
        private AvaloniaList<TemplatedDataGridCell> _cells = new AvaloniaList<TemplatedDataGridCell>();
        private Grid? _root;
        private readonly List<Control> _rootChildren = new List<Control>();

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

        internal AvaloniaList<TemplatedDataGridCell> Cells
        {
            get => _cells;
            set => SetAndRaise(CellsProperty, ref _cells, value);
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

            _cells.Clear();

            var columns = Columns;
            if (columns is null)
            {
                return;
            }
 
            // Generate ColumnDefinitions

            var columnDefinitions = new List<ColumnDefinition>();

            for (var i = 0; i < columns.Count; i++)
            {
                var column = columns[i];
                var isAutoWidth = column.Width == GridLength.Auto;

                var columnDefinition = new ColumnDefinition
                {
                    [!ColumnDefinition.WidthProperty] = column[!TemplatedDataGridColumn.WidthProperty],
                    [!ColumnDefinition.MinWidthProperty] = column[!TemplatedDataGridColumn.MinWidthProperty],
                    [!ColumnDefinition.MaxWidthProperty] = column[!TemplatedDataGridColumn.MaxWidthProperty]
                };

                if (isAutoWidth)
                {
                    columnDefinition.SetValue(DefinitionBase.SharedSizeGroupProperty, $"Column{i}");
                }

                columnDefinitions.Add(columnDefinition);

                // Generate DataGridCell's

                TemplatedDataGridCell cell;

                if (column is TemplatedDataGridTemplateColumn templateColumn)
                {
                    cell = new TemplatedDataGridCell
                    {
                        [!!TemplatedDataGridCell.SelectedItemProperty] = this[!!TemplatedDataGridCellsPresenter.SelectedItemProperty],
                        [!!TemplatedDataGridCell.SelectedCellProperty] = this[!!TemplatedDataGridCellsPresenter.SelectedCellProperty],
                        [Grid.ColumnProperty] = columnDefinitions.Count - 1,
                        [!TemplatedDataGridCell.ContentProperty] = this[!TemplatedDataGridCellsPresenter.DataContextProperty],
                        [!TemplatedDataGridCell.CellTemplateProperty] = templateColumn[!TemplatedDataGridColumn.CellTemplateProperty]
                    };
                }
                else if (column is TemplatedDataGridTextColumn textColumn)
                {
                    cell = new TemplatedDataGridCell
                    {
                        [!!TemplatedDataGridCell.SelectedItemProperty] = this[!!TemplatedDataGridCellsPresenter.SelectedItemProperty],
                        [!!TemplatedDataGridCell.SelectedCellProperty] = this[!!TemplatedDataGridCellsPresenter.SelectedCellProperty],
                        [Grid.ColumnProperty] = columnDefinitions.Count - 1,
                        [!TemplatedDataGridCell.ContentProperty] = this[!TemplatedDataGridCellsPresenter.DataContextProperty],
                        [!TemplatedDataGridCell.CellTemplateProperty] = textColumn[!TemplatedDataGridColumn.CellTemplateProperty]
                    };
                }
                else if (column is TemplatedDataGridCheckBoxColumn checkBoxColumn)
                {
                    cell = new TemplatedDataGridCell
                    {
                        [!!TemplatedDataGridCell.SelectedItemProperty] = this[!!TemplatedDataGridCellsPresenter.SelectedItemProperty],
                        [!!TemplatedDataGridCell.SelectedCellProperty] = this[!!TemplatedDataGridCellsPresenter.SelectedCellProperty],
                        [Grid.ColumnProperty] = columnDefinitions.Count - 1,
                        [!TemplatedDataGridCell.ContentProperty] = this[!TemplatedDataGridCellsPresenter.DataContextProperty],
                        [!TemplatedDataGridCell.CellTemplateProperty] = checkBoxColumn[!TemplatedDataGridColumn.CellTemplateProperty]
                    };
                }
                else
                {
                    cell = new TemplatedDataGridCell
                    {
                        [!!TemplatedDataGridCell.SelectedItemProperty] = this[!!TemplatedDataGridCellsPresenter.SelectedItemProperty],
                        [!!TemplatedDataGridCell.SelectedCellProperty] = this[!!TemplatedDataGridCellsPresenter.SelectedCellProperty],
                        [Grid.ColumnProperty] = columnDefinitions.Count - 1,
                        [!TemplatedDataGridCell.ContentProperty] = this[!TemplatedDataGridCellsPresenter.DataContextProperty],
                        [!TemplatedDataGridCell.CellTemplateProperty] = column[!TemplatedDataGridColumn.CellTemplateProperty]
                    };
                }

                _cells.Add(cell);
                _rootChildren.Add(cell);

                if (i < columns.Count)
                {
                    columnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Pixel)));
                }
            }

            columnDefinitions.Add(new ColumnDefinition(GridLength.Auto));

            _root.ColumnDefinitions.Clear();
            _root.ColumnDefinitions.AddRange(columnDefinitions);

            foreach (var child in _rootChildren)
            {
                _root.Children.Add(child);
            }
        }
    }
}
