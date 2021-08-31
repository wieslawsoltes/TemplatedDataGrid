using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;

namespace TemplatedDataGrid
{
    public abstract class DataGridColumn : AvaloniaObject
    {
        public static readonly StyledProperty<IDataTemplate?> CellTemplateProperty = 
            AvaloniaProperty.Register<DataGridColumn, IDataTemplate?>(nameof(CellTemplate));

        public static readonly StyledProperty<object?> HeaderProperty = 
            AvaloniaProperty.Register<DataGridColumn, object?>(nameof(Header));

        public static readonly StyledProperty<GridLength> WidthProperty = 
            AvaloniaProperty.Register<DataGridColumn, GridLength>(nameof(Width), new GridLength(1, GridUnitType.Star));

        public static readonly StyledProperty<double> MinWidthProperty = 
            AvaloniaProperty.Register<DataGridColumn, double>(nameof(MinWidth), 0.0);

        public static readonly StyledProperty<double> MaxWidthProperty = 
            AvaloniaProperty.Register<DataGridColumn, double>(nameof(MaxWidth), double.PositiveInfinity);

        public static readonly StyledProperty<bool> CanUserSortProperty = 
            AvaloniaProperty.Register<DataGridColumn, bool>(nameof(CanUserSort), true);

        public static readonly StyledProperty<string?> SortMemberPathProperty = 
            AvaloniaProperty.Register<DataGridColumn, string?>(nameof(SortMemberPath));

        public static readonly StyledProperty<bool> CanUserResizeProperty = 
            AvaloniaProperty.Register<DataGridColumn, bool>(nameof(CanUserResize));

        public static readonly StyledProperty<bool> CanUserReorderProperty = 
            AvaloniaProperty.Register<DataGridColumn, bool>(nameof(CanUserReorder));

        [Content]
        public IDataTemplate? CellTemplate
        {
            get => GetValue(CellTemplateProperty);
            set => SetValue(CellTemplateProperty, value);
        }

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

        public bool CanUserSort
        {
            get => GetValue(CanUserSortProperty);
            set => SetValue(CanUserSortProperty, value);
        }

        public string? SortMemberPath
        {
            get => GetValue(SortMemberPathProperty);
            set => SetValue(SortMemberPathProperty, value);
        }

        public bool CanUserResize
        {
            get => GetValue(CanUserSortProperty);
            set => SetValue(CanUserSortProperty, value);
        }

        public bool CanUserReorder
        {
            get => GetValue(CanUserSortProperty);
            set => SetValue(CanUserSortProperty, value);
        }
    }
}
