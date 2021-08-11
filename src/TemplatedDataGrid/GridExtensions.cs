using System.Collections.Generic;
using Avalonia.Controls;

namespace ItemsRepeaterDataGrid
{
    public static class GridExtensions
    {
        public static void SetRowDefinitions(this Grid grid, IEnumerable<RowDefinition> rowDefinitions)
        {
            grid.RowDefinitions.Clear();
            grid.RowDefinitions.AddRange(rowDefinitions);
        }

        public static void SetColumnDefinitions(this Grid grid, IEnumerable<ColumnDefinition> columnDefinitions)
        {
            grid.ColumnDefinitions.Clear();
            grid.ColumnDefinitions.AddRange(columnDefinitions);
        }
    }
}
