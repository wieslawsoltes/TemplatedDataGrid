using System;
using System.Collections.Generic;
using System.Linq;

namespace TemplatedDataGrid
{
    [System.Diagnostics.DebuggerDisplay("{Column} IsLast={IsLast}")]
    internal class TemplatedDataGridColumnTopology
    {
        public TemplatedDataGridColumnTopology(TemplatedDataGridColumn column, int row, TemplatedDataGridColumnTopology? parent = default)
        {
            Column = column;
            Row = row;
            Parent = parent;
        }

        /// <summary>
        /// Get TemplatedDataGridColumn Row
        /// </summary>
        public int Row { get; }
        /// <summary>
        /// Get TemplatedDataGridColumn Parent
        /// </summary>
        public TemplatedDataGridColumnTopology? Parent { get; }
        /// <summary>
        /// The Current TemplatedDataGridColumn
        /// </summary>
        public TemplatedDataGridColumn Column { get; }
        /// <summary>
        /// Get/Set link from Column and Header
        /// </summary>
        public TemplatedDataGridColumnHeader? Header { get; set; }
        /// <summary>
        /// If true the current column is last of column group
        /// </summary>
        public bool IsLastGroupColumn { get; private set; }

        /// <summary>
        /// Walk through the entire tree of the columns and return the maximum depth and the last leaf of each branch of the tree.
        /// </summary>
        /// <param name="columns">TemplatedDataGrid columns</param>
        /// <returns name="MaxDeep">maximum depth of tree</returns>
        /// <returns name="Items">last leaf of each branch of the tree</returns>
        /// <example>
        /// 
        /// Considering Columns Header like this:
        /// 
        /// ╔═══╦═══╦═════════════════════╗
        /// ║ A ║ B ║          C          ║
        /// ║   ║   ╠═══════════════╦═════╣
        /// ║   ║   ║ C.1           ║ C.2 ║
        /// ║   ║   ╠═══════╦═══════╣     ║
        /// ║   ║   ║ C.1.1 ║ C.1.2 ║     ║
        /// ╚═══╩═══╩═══════╩═══════╩═════╝
        /// 
        /// GetTopology will return:
        /// 
        /// MaxDeep: 2
        /// 
        /// Items:
        /// 
        /// ╔═══╦═══╦═══════╦═══════╦═════╗
        /// ║ A ║ B ║ C.1.1 ║ C.1.2 ║ C.2 ║
        /// ╚═══╩═══╩═══════╩═══════╩═════╝
        /// </example>
        public static (int MaxDeep, IReadOnlyList<TemplatedDataGridColumnTopology> Items) GetTopology(IEnumerable<TemplatedDataGridColumn> columns)
        {
            var maxDeep = 0;
            var store = new List<TemplatedDataGridColumnTopology>();
            var transformation = columns
                .Select((item, index) => new TemplatedDataGridColumnTopology(item, 0))
                .Walk(node =>
                {
                    maxDeep = Math.Max(maxDeep, node.Row);
                    IEnumerable<TemplatedDataGridColumnTopology> result = Array.Empty<TemplatedDataGridColumnTopology>();
                    var children = 0;
                    if (node.Column is TemplatedDataGridGroupedColumn groupedColumn)
                    {
                        children = groupedColumn.Columns.Count;

                        TemplatedDataGridColumnTopology? previus = default;
                        result = groupedColumn.Columns
                            .Select((child, ci) =>
                            {
                                var current = new TemplatedDataGridColumnTopology(child, node.Row + 1, node);
                                previus = current;
                                return current;
                            }).ToList(); // Force to walk
                        if (previus is { })
                        {
                            previus.IsLastGroupColumn = true;
                        }
                    }
                    else
                    {
                        store.Add(node);
                    }
                    return result;
                }).GetEnumerator();
            // Run transformation
            while (transformation.MoveNext());
            return (maxDeep, store.AsReadOnly());
        }
    }
}
