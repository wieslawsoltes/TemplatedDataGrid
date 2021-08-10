using Avalonia;
using Avalonia.Controls;

namespace Demo.Controls
{
    public class DataGridColumn : AvaloniaObject
    {
        public static readonly StyledProperty<object?> HeaderProperty = 
            AvaloniaProperty.Register<DataGridColumn, object?>(nameof(Header));

        public static readonly StyledProperty<GridLength> WidthProperty = 
            AvaloniaProperty.Register<DataGridColumn, GridLength>(nameof(Width));

        public object? Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public GridLength Width
        {
            get => GetValue(WidthProperty);
            set => SetValue(WidthProperty, value);
        }
    }
}
