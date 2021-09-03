using Avalonia;
using Avalonia.Data;

namespace TemplatedDataGrid
{
    public abstract class TemplatedDataGridBoundColumn : TemplatedDataGridColumn
    {
        public static readonly StyledProperty<IBinding?> BindingProperty = 
            AvaloniaProperty.Register<TemplatedDataGridBoundColumn, IBinding?>(nameof(Binding));

        [AssignBinding]
        public IBinding? Binding
        {
            get => GetValue(BindingProperty);
            set => SetValue(BindingProperty, value);
        }
    }
}
