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
    public class TemplatedDataGridCell : TemplatedControl
    {
        internal static readonly DirectProperty<TemplatedDataGridCell, object?> SelectedItemProperty =
            AvaloniaProperty.RegisterDirect<TemplatedDataGridCell, object?>(
                nameof(SelectedItem), 
                o => o.SelectedItem, 
                (o, v) => o.SelectedItem = v,
                defaultBindingMode: BindingMode.TwoWay);

        internal static readonly DirectProperty<TemplatedDataGridCell, object?> SelectedCellProperty =
            AvaloniaProperty.RegisterDirect<TemplatedDataGridCell, object?>(
                nameof(SelectedCell), 
                o => o.SelectedCell, 
                (o, v) => o.SelectedCell = v,
                defaultBindingMode: BindingMode.TwoWay);

        public static readonly StyledProperty<IDataTemplate?> CellTemplateProperty = 
            AvaloniaProperty.Register<TemplatedDataGridCell, IDataTemplate?>(nameof(CellTemplate));

        public static readonly StyledProperty<object?> ContentProperty =
            AvaloniaProperty.Register<TemplatedDataGridCell, object?>(nameof(Content));

        public static readonly StyledProperty<HorizontalAlignment> HorizontalContentAlignmentProperty =
            AvaloniaProperty.Register<TemplatedDataGridCell, HorizontalAlignment>(nameof(HorizontalContentAlignment));

        public static readonly StyledProperty<VerticalAlignment> VerticalContentAlignmentProperty =
            AvaloniaProperty.Register<TemplatedDataGridCell, VerticalAlignment>(nameof(VerticalContentAlignment));

        private object? _selectedItem;
        private object? _selectedCell;

        public TemplatedDataGridCell()
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

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == DataContextProperty)
            {
                UpdatePseudoClassesSelectedItem(SelectedItem);
            }

            if (change.Property == SelectedItemProperty)
            {
                UpdatePseudoClassesSelectedItem(change.GetNewValue<object?>());
            }

            if (change.Property == SelectedCellProperty)
            {
                UpdatePseudoClassesSelectedCell(change.GetNewValue<object?>());
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
