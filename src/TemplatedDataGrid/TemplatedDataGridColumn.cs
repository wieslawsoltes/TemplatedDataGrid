using System.ComponentModel;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Metadata;

namespace TemplatedDataGrid
{
    public abstract class TemplatedDataGridColumn : AvaloniaObject
    {
        public static readonly StyledProperty<IDataTemplate?> CellTemplateProperty = 
            AvaloniaProperty.Register<TemplatedDataGridColumn, IDataTemplate?>(nameof(CellTemplate));

        public static readonly StyledProperty<object?> HeaderProperty = 
            AvaloniaProperty.Register<TemplatedDataGridColumn, object?>(nameof(Header));

        public static readonly StyledProperty<GridLength> WidthProperty = 
            AvaloniaProperty.Register<TemplatedDataGridColumn, GridLength>(nameof(Width), new GridLength(1, GridUnitType.Star));

        public static readonly StyledProperty<double> MinWidthProperty = 
            AvaloniaProperty.Register<TemplatedDataGridColumn, double>(nameof(MinWidth), 0.0);

        public static readonly StyledProperty<double> MaxWidthProperty = 
            AvaloniaProperty.Register<TemplatedDataGridColumn, double>(nameof(MaxWidth), double.PositiveInfinity);

        public static readonly StyledProperty<bool> CanUserSortProperty = 
            AvaloniaProperty.Register<TemplatedDataGridColumn, bool>(nameof(CanUserSort), true);

        public static readonly StyledProperty<string?> SortMemberPathProperty = 
            AvaloniaProperty.Register<TemplatedDataGridColumn, string?>(nameof(SortMemberPath));

        public static readonly StyledProperty<bool> CanUserResizeProperty = 
            AvaloniaProperty.Register<TemplatedDataGridColumn, bool>(nameof(CanUserResize));

        public static readonly StyledProperty<bool> CanUserReorderProperty = 
            AvaloniaProperty.Register<TemplatedDataGridColumn, bool>(nameof(CanUserReorder));

        public static readonly StyledProperty<ListSortDirection?> SortingStateProperty = 
            AvaloniaProperty.Register<TemplatedDataGridColumn, ListSortDirection?>(nameof(SortingState), null, false, BindingMode.TwoWay);

        public static readonly StyledProperty<ICommand> SortCommandProperty = 
            AvaloniaProperty.Register<TemplatedDataGridColumn, ICommand>(nameof(SortCommand));

        internal static readonly DirectProperty<TemplatedDataGridColumn, double> ActualWidthProperty =
            AvaloniaProperty.RegisterDirect<TemplatedDataGridColumn, double>(
                nameof(ActualWidth), 
                o => o.ActualWidth, 
                (o, v) => o.ActualWidth = v);

        private double _actualWidth;

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

        public ListSortDirection? SortingState
        {
            get => GetValue(SortingStateProperty);
            set => SetValue(SortingStateProperty, value);
        }

        public ICommand SortCommand
        {
            get => GetValue(SortCommandProperty);
            set => SetValue(SortCommandProperty, value);
        }

        internal double ActualWidth
        {
            get => _actualWidth;
            set => SetAndRaise(ActualWidthProperty, ref _actualWidth, value);
        }
    }
}
