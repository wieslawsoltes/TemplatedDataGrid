using System;

namespace TemplatedDataGrid
{
    [Flags]
    public enum TemplatedDataGridGridLinesVisibility
    {
        None = 0,
        Horizontal = 1,
        Vertical = 2,
        All = Vertical | Horizontal
    }
}
