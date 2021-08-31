﻿using System.ComponentModel;
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

        internal static readonly StyledProperty<bool> CanUserSortColumnsProperty = 
            AvaloniaProperty.Register<DataGridColumnHeader, bool>(nameof(CanUserSortColumns));

        internal static readonly StyledProperty<bool> CanUserResizeColumnsProperty = 
            AvaloniaProperty.Register<DataGridColumnHeader, bool>(nameof(CanUserResizeColumns));

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
        
        internal bool CanUserSortColumns
        {
            get => GetValue(CanUserSortColumnsProperty);
            set => SetValue(CanUserSortColumnsProperty, value);
        }
        
        internal bool CanUserResizeColumns
        {
            get => GetValue(CanUserResizeColumnsProperty);
            set => SetValue(CanUserResizeColumnsProperty, value);
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

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            UpdatePseudoClassesSortingState(Column?.SortingState);
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
                    OnClick(e.KeyModifiers);
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

        private void OnClick(KeyModifiers keyModifiers)
        {
            if (_column is null || _columnHeaders is null)
            {
                return;
            }

            if (!_column.CanUserSort || !CanUserSortColumns)
            {
                return;
            }

            var ctrl = (keyModifiers & KeyModifiers.Control) == KeyModifiers.Control;
            var shift = (keyModifiers & KeyModifiers.Shift) == KeyModifiers.Shift;

            if (!shift)
            {
                foreach (var columnHeader in _columnHeaders)
                {
                    if (!Equals(columnHeader, this))
                    {
                        if (columnHeader.Column is { } column)
                        {
                            column.SortingState = null;
                            columnHeader.UpdatePseudoClassesSortingState(column.SortingState);
                        }
                    }
                }
            }

            var sortMemberPath = ctrl ? default : _column.SortMemberPath;
            var sortingState =  ctrl ? default : (_column.SortingState == ListSortDirection.Ascending
                ? ListSortDirection.Descending
                : ListSortDirection.Ascending);

            _column.SortingState = sortingState;

            UpdatePseudoClassesSortingState(sortingState);

            if (_column.SortCommand is { } command)
            {
                if (command.CanExecute(sortMemberPath))
                {
                    command.Execute(sortMemberPath);
                }
            }
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
