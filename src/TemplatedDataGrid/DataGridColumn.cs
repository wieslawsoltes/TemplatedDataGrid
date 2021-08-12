using Avalonia;
using Avalonia.Controls;

namespace TemplatedDataGrid
{
    public class DataGridColumn : AvaloniaObject
    {
        public static readonly StyledProperty<object?> HeaderProperty = 
            AvaloniaProperty.Register<DataGridColumn, object?>(nameof(Header));

        public static readonly StyledProperty<GridLength> WidthProperty = 
            AvaloniaProperty.Register<DataGridColumn, GridLength>(nameof(Width), new GridLength(1, GridUnitType.Star));

        public static readonly StyledProperty<double> MinWidthProperty = 
            AvaloniaProperty.Register<DataGridColumn, double>(nameof(MinWidth), 0.0);

        public static readonly StyledProperty<double> MaxWidthProperty = 
            AvaloniaProperty.Register<DataGridColumn, double>(nameof(MaxWidth), double.PositiveInfinity);

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

        public double MinWidth
        {
            get => GetValue(MinWidthProperty);
            set => SetValue(MinWidthProperty, value);
        }

        public double MaxWidth
        {
            get => GetValue(MaxWidthProperty);
            set => SetValue(MaxWidthProperty, value);
        }
    }
}
