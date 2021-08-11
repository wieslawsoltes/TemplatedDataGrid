using Avalonia;
using Avalonia.Controls.Primitives;

namespace TemplatedDataGrid
{
    public class DataGridColumnHeader : TemplatedControl
    {
        public static readonly StyledProperty<object?> HeaderProperty = 
            AvaloniaProperty.Register<DataGridColumnHeader, object?>(nameof(Header));

        public object? Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }
    }
}
