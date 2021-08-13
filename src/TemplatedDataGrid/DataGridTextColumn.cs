using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace TemplatedDataGrid
{
    public class DataGridTextColumn : DataGridBoundColumn
    {
        public DataGridTextColumn()
        {
            CellTemplate = new FuncDataTemplate(
                _ => true,
                (_, _) =>
                {
                    var textBlock = new TextBlock();

                    if (Binding is { })
                    {
                        textBlock.Bind(TextBlock.TextProperty, Binding);
                    }

                    return textBlock;
                },
                supportsRecycling: true);
        }
    }
}
