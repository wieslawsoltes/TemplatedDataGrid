using System.ComponentModel;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace TemplatedDataGrid
{
    [PseudoClasses(":sortascending", ":sortdescending")]
    public class DataGridColumnHeader : TemplatedControl
    {
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

        public static readonly StyledProperty<object?> HeaderProperty = 
            AvaloniaProperty.Register<DataGridColumnHeader, object?>(nameof(Header));

        private AvaloniaList<DataGridColumnHeader>? _columnHeaders;
        private DataGridColumn? _column;
        private Button? _button;
        
        public object? Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
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
            
            _button = e.NameScope.Find<Button>("PART_Button");

            if (_button is { })
            {
                _button.Click += ButtonOnClick;
            }

            UpdatePseudoClasses();
        }

        private void ButtonOnClick(object? sender, RoutedEventArgs e)
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
                    columnHeader.UpdatePseudoClasses();
                }
            }

            CurrentSortingState = CurrentSortingState == ListSortDirection.Ascending
                ? ListSortDirection.Descending
                : ListSortDirection.Ascending;

            UpdatePseudoClasses();
        }

        private void UpdatePseudoClasses()
        {
            PseudoClasses.Set(":sortascending", CurrentSortingState == ListSortDirection.Ascending);
            PseudoClasses.Set(":sortdescending", CurrentSortingState == ListSortDirection.Descending);
        }
    }
}
