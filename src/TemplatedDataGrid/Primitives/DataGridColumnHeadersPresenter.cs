using System.Collections.Generic;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;

namespace TemplatedDataGrid.Primitives
{
    public class DataGridColumnHeadersPresenter : TemplatedControl
    {
        internal static readonly DirectProperty<DataGridColumnHeadersPresenter, AvaloniaList<DataGridColumn>?> ColumnsProperty =
            AvaloniaProperty.RegisterDirect<DataGridColumnHeadersPresenter, AvaloniaList<DataGridColumn>?>(
                nameof(Columns), 
                o => o.Columns, 
                (o, v) => o.Columns = v);

        private AvaloniaList<DataGridColumn>? _columns;
        private Grid? _root;
        private readonly List<Control> _rootChildren = new List<Control>();

        internal AvaloniaList<DataGridColumn>? Columns
        {
            get => _columns;
            set => SetAndRaise(ColumnsProperty, ref _columns, value);
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
            if (columns is null)
            {
                return;
            }

            //  Generate RowDefinitions

            var rowDefinitions = new List<RowDefinition>();

            rowDefinitions.Add(new RowDefinition(GridLength.Auto));
            rowDefinitions.Add(new RowDefinition(new GridLength(0, GridUnitType.Pixel)));

            // Generate Horizontal Grid Lines

            var horizontalColumnsGridLine = new Rectangle()
            {
                [Grid.RowProperty] = 1,
                [Grid.ColumnProperty] = 0,
                [Grid.ColumnSpanProperty] = columns.Count + (columns.Count - 1)
            };
            ((IPseudoClasses)horizontalColumnsGridLine.Classes).Add(":horizontal");
            _rootChildren.Add(horizontalColumnsGridLine);

            // Generate ColumnDefinitions

            var columnDefinitions = new List<ColumnDefinition>();
            var splitterColumnIndexes = new List<int>();
            var isSharedSizeScope = false;

            for (var i = 0; i < columns.Count; i++)
            {
                var column = columns[i];
                var isAutoWidth = column.Width == GridLength.Auto;

                var columnDefinition = new ColumnDefinition()
                {
                    [!ColumnDefinition.MinWidthProperty] = column[!DataGridColumn.MinWidthProperty],
                    [!ColumnDefinition.MaxWidthProperty] = column[!DataGridColumn.MaxWidthProperty]
                };

                if (isAutoWidth)
                {
                    columnDefinition.Width = column.Width;
                    columnDefinition.SetValue(DefinitionBase.SharedSizeGroupProperty, $"Column{i}");
                    isSharedSizeScope = true;
                }
                else
                {
                    columnDefinition[!ColumnDefinition.WidthProperty] = column[!DataGridColumn.WidthProperty];
                }

                columnDefinitions.Add(columnDefinition);

                // Generate DataGridColumnHeader's
                var columnHeader = new DataGridColumnHeader()
                {
                    [Grid.RowProperty] = 0,
                    [Grid.ColumnProperty] = columnDefinitions.Count - 1,
                    [!DataGridColumnHeader.HeaderProperty] = column[!DataGridColumn.HeaderProperty]
                };
                _rootChildren.Add(columnHeader);
 
                if (i < columns.Count - 1)
                {
                    columnDefinitions.Add(new ColumnDefinition(new GridLength(0, GridUnitType.Pixel)));
                    splitterColumnIndexes.Add(columnDefinitions.Count - 1);
                }
            }

            // Generate Vertical Grid Lines

            foreach (var columnIndex in splitterColumnIndexes)
            {
                var verticalColumnGridLine = new Rectangle()
                {
                    [Grid.RowProperty] = 0,
                    [Grid.RowSpanProperty] = 1,
                    [Grid.ColumnProperty] = columnIndex
                };
                ((IPseudoClasses)verticalColumnGridLine.Classes).Add(":vertical");
                _rootChildren.Add(verticalColumnGridLine);
            }

            _root.RowDefinitions.Clear();
            _root.RowDefinitions.AddRange(rowDefinitions);

            _root.ColumnDefinitions.Clear();
            _root.ColumnDefinitions.AddRange(columnDefinitions);

            foreach (var child in _rootChildren)
            {
                _root.Children.Add(child);
            }
        }
    }
}
