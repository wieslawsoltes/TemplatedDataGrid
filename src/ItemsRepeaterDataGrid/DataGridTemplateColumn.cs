using Avalonia;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;

namespace ItemsRepeaterDataGrid
{
    public class DataGridTemplateColumn : DataGridColumn
    {
        public static readonly StyledProperty<IDataTemplate?> CellTemplateProperty = 
            AvaloniaProperty.Register<DataGridTemplateColumn, IDataTemplate?>(nameof(CellTemplate));

        [Content]
        public IDataTemplate? CellTemplate
        {
            get => GetValue(CellTemplateProperty);
            set => SetValue(CellTemplateProperty, value);
        }
    }
}
