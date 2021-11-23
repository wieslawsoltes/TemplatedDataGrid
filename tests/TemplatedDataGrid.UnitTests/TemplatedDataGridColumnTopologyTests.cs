using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace TemplatedDataGrid.UnitTests
{

    public class TemplatedDataGridColumnTopologyTests
    {
        [Fact]
        public void Generate_Topology_Column_Sequence()
        {
            var columns = GenerateColumns();

            var expectedColumns = new[] { "A", "B", "C.1.1", "C.1.2", "C.2" };

            var topology = TemplatedDataGridColumnTopology.GetTopology(columns);

            Assert.Equal(2, topology.MaxDeep);

            Assert.Equal(expectedColumns, topology.Items.Select(i=>i.Column?.Header?.ToString()));
        }

        private static IEnumerable<TemplatedDataGridColumn> GenerateColumns()
        {
            const int indexCColumn = 2;
            var columns = new List<TemplatedDataGridColumn>()
            {
                new TemplatedDataGridTextColumn(){Header = "A"},
                new TemplatedDataGridTextColumn(){Header = "B"},
                new TemplatedDataGridGroupedColumn()
                {
                    Header = "C"
                },
            };
            var subGroup = new TemplatedDataGridGroupedColumn()
            {
                Header = "C.1",
            };
            subGroup.Columns.AddRange(new[]
            {
                new TemplatedDataGridTextColumn(){Header = "C.1.1"},
                new TemplatedDataGridTextColumn(){Header = "C.1.2"},
            });
            ((TemplatedDataGridGroupedColumn)columns[indexCColumn]).Columns
                .AddRange(new TemplatedDataGridColumn[]
                    {
                        subGroup,
                        new TemplatedDataGridCheckBoxColumn() { Header = "C.2" },
                    });
            return columns.AsReadOnly();
        }
    }
}
