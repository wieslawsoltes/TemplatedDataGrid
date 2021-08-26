using System.ComponentModel;
using System.Linq;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace TemplatedDataGrid
{
    [PseudoClasses(":pressed", ":sortascending", ":sortdescending")]
    public class DataGridColumnHeader : TemplatedControl
    {
        public static readonly StyledProperty<object?> HeaderProperty = 
            AvaloniaProperty.Register<DataGridColumnHeader, object?>(nameof(Header));

        internal static readonly StyledProperty<bool> IsPressedProperty =
            AvaloniaProperty.Register<DataGridColumnHeader, bool>(nameof(IsPressed));

        internal static readonly DirectProperty<DataGridColumnHeader, AvaloniaList<DataGridColumnHeader>?> ColumnHeadersProperty =
            AvaloniaProperty.RegisterDirect<DataGridColumnHeader, AvaloniaList<DataGridColumnHeader>?>(
                nameof(ColumnHeaders), 
                o => o.ColumnHeaders, 
                (o, v) => o.ColumnHeaders = v);

        internal static readonly DirectProperty<DataGridColumnHeader, DataGridColumn?> ColumnProperty =
            AvaloniaProperty.RegisterDirect<DataGridColumnHeader, DataGridColumn?>(
                nameof(Column), 
                o => o.Column, 
                (o, v) => o.Column = v);

        private AvaloniaList<DataGridColumnHeader>? _columnHeaders;
        private DataGridColumn? _column;

        public DataGridColumnHeader()
        {
            UpdatePseudoClassesIsPressed(IsPressed);
        }

        public object? Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        internal bool IsPressed
        {
            get => GetValue(IsPressedProperty);
            private set => SetValue(IsPressedProperty, value);
        }
        
        internal AvaloniaList<DataGridColumnHeader>? ColumnHeaders
        {
            get => _columnHeaders;
            set => SetAndRaise(ColumnHeadersProperty, ref _columnHeaders, value);
        }

        internal DataGridColumn? Column
        {
            get => _column;
            set => SetAndRaise(ColumnProperty, ref _column, value);
        }

        private ListSortDirection? CurrentSortingState { get; set; }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            UpdatePseudoClassesSortingState(CurrentSortingState);
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);
            
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                IsPressed = true;
                e.Handled = true;
            }
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);

            if (IsPressed && e.InitialPressMouseButton == MouseButton.Left)
            {
                IsPressed = false;
                e.Handled = true;

                if (this.GetVisualsAt(e.GetPosition(this)).Any(c => this == c || this.IsVisualAncestorOf(c)))
                {
                    OnClick();
                }
            }
        }

        protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
        {
            IsPressed = false;
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == IsPressedProperty)
            {
                UpdatePseudoClassesIsPressed(change.NewValue.GetValueOrDefault<bool>());
            }
        }

        private void OnClick()
        {
            if (_column is null || _columnHeaders is null)
            {
                return;
            }

            foreach (var columnHeader in _columnHeaders)
            {
                if (!Equals(columnHeader, this))
                {
                    columnHeader.CurrentSortingState = null;
                    columnHeader.UpdatePseudoClassesSortingState(columnHeader.CurrentSortingState);
                }
            }

            CurrentSortingState = CurrentSortingState == ListSortDirection.Ascending
                ? ListSortDirection.Descending
                : ListSortDirection.Ascending;

            UpdatePseudoClassesSortingState(CurrentSortingState);
        }

        private void UpdatePseudoClassesIsPressed(bool isPressed)
        {
            PseudoClasses.Set(":pressed", isPressed);
        }

        private void UpdatePseudoClassesSortingState(ListSortDirection? sortingState)
        {
            PseudoClasses.Set(":sortascending", sortingState == ListSortDirection.Ascending);
            PseudoClasses.Set(":sortdescending", sortingState == ListSortDirection.Descending);
        }
    }
}
