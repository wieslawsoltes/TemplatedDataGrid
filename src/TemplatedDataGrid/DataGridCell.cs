using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Metadata;

namespace TemplatedDataGrid
{
    [PseudoClasses(":pressed", ":selected")]
    public class DataGridCell : TemplatedControl
    {
        internal static readonly StyledProperty<object?> SelectedItemProperty = 
            AvaloniaProperty.Register<DataGridCell, object?>(nameof(SelectedItem));

        internal static readonly StyledProperty<object?> SelectedCellProperty = 
            AvaloniaProperty.Register<DataGridCell, object?>(nameof(SelectedCell));

        public static readonly StyledProperty<IDataTemplate?> CellTemplateProperty = 
            AvaloniaProperty.Register<DataGridCell, IDataTemplate?>(nameof(CellTemplate));

        public static readonly StyledProperty<object?> ContentProperty =
            AvaloniaProperty.Register<DataGridCell, object?>(nameof(Content));

        public DataGridCell()
        {
            UpdatePseudoClassesSelectedCell(SelectedCell);
        }

        internal object? SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        internal object? SelectedCell
        {
            get => GetValue(SelectedCellProperty);
            set => SetValue(SelectedCellProperty, value);
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
