using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Metadata;

namespace TemplatedDataGrid
{
    [PseudoClasses(":pressed", ":selected")]
    public class DataGridCell : TemplatedControl
    {
        internal static readonly DirectProperty<DataGridCell, object?> SelectedItemProperty =
            AvaloniaProperty.RegisterDirect<DataGridCell, object?>(
                nameof(SelectedItem), 
                o => o.SelectedItem, 
                (o, v) => o.SelectedItem = v,
                defaultBindingMode: BindingMode.TwoWay);

        internal static readonly DirectProperty<DataGridCell, object?> SelectedCellProperty =
            AvaloniaProperty.RegisterDirect<DataGridCell, object?>(
                nameof(SelectedCell), 
                o => o.SelectedCell, 
                (o, v) => o.SelectedCell = v,
                defaultBindingMode: BindingMode.TwoWay);

        public static readonly StyledProperty<IDataTemplate?> CellTemplateProperty = 
            AvaloniaProperty.Register<DataGridCell, IDataTemplate?>(nameof(CellTemplate));

        public static readonly StyledProperty<object?> ContentProperty =
            AvaloniaProperty.Register<DataGridCell, object?>(nameof(Content));

        public static readonly StyledProperty<HorizontalAlignment> HorizontalContentAlignmentProperty =
            AvaloniaProperty.Register<DataGridCell, HorizontalAlignment>(nameof(HorizontalContentAlignment));

        public static readonly StyledProperty<VerticalAlignment> VerticalContentAlignmentProperty =
            AvaloniaProperty.Register<DataGridCell, VerticalAlignment>(nameof(VerticalContentAlignment));

        private object? _selectedItem;
        private object? _selectedCell;

        public DataGridCell()
        {
            UpdatePseudoClassesSelectedCell(SelectedCell);
        }

        internal object? SelectedItem
        {
            get => _selectedItem;
            set => SetAndRaise(SelectedItemProperty, ref _selectedItem, value);
        }

        internal object? SelectedCell
        {
            get => _selectedCell;
            set => SetAndRaise(SelectedCellProperty, ref _selectedCell, value);
        }

        [Content]
        public IDataTemplate? CellTemplate
        {
            get => GetValue(CellTemplateProperty);
            set => SetValue(CellTemplateProperty, value);
        }

        [Content]
        [DependsOn(nameof(CellTemplate))]
        public object? Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public HorizontalAlignment HorizontalContentAlignment
        {
            get => GetValue(HorizontalContentAlignmentProperty);
            set => SetValue(HorizontalContentAlignmentProperty, value);
        }

        public VerticalAlignment VerticalContentAlignment
        {
            get => GetValue(VerticalContentAlignmentProperty);
            set => SetValue(VerticalContentAlignmentProperty, value);
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);
            
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                SelectedCell = this;
            }
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == DataContextProperty)
            {
                UpdatePseudoClassesSelectedItem(SelectedItem);
            }

            if (change.Property == SelectedItemProperty)
            {
                UpdatePseudoClassesSelectedItem(change.NewValue.GetValueOrDefault<object?>());
            }

            if (change.Property == SelectedCellProperty)
            {
                UpdatePseudoClassesSelectedCell(change.NewValue.GetValueOrDefault<object?>());
            }
        }

        private void UpdatePseudoClassesSelectedItem(object? selectedItem)
        {
            if (DataContext == null || selectedItem != DataContext)
            {
                PseudoClasses.Set(":selected", false);
            }
        }

        private void UpdatePseudoClassesSelectedCell(object? selectedCell)
        {
            PseudoClasses.Set(":selected", selectedCell != null && Equals(selectedCell, this));
        }
    }
}
