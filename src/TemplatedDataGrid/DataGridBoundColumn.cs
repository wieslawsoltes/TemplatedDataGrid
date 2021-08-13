using Avalonia;
using Avalonia.Data;

namespace TemplatedDataGrid
{
    public abstract class DataGridBoundColumn : DataGridColumn
    {
        public static readonly StyledProperty<IBinding?> BindingProperty = 
            AvaloniaProperty.Register<DataGridBoundColumn, IBinding?>(nameof(Binding));

        [AssignBinding]
        public IBinding? Binding
        {
            get => GetValue(BindingProperty);
            set => SetValue(BindingProperty, value);
        }
    }
}
