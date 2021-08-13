﻿using System.Collections;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;

namespace TemplatedDataGrid.Primitives
{
    public class DataGridRowsPresenter : TemplatedControl
    {
        public static readonly StyledProperty<IDataTemplate> ItemTemplateProperty =
            AvaloniaProperty.Register<DataGridRowsPresenter, IDataTemplate>(nameof(ItemTemplate));

        public static readonly StyledProperty<IEnumerable?> ItemsProperty = 
            AvaloniaProperty.Register<DataGridRowsPresenter, IEnumerable?>(nameof(Items));

        public static readonly StyledProperty<object?> SelectedItemProperty = 
            AvaloniaProperty.Register<DataGridRowsPresenter, object?>(nameof(SelectedItem));

        internal static readonly DirectProperty<DataGridRowsPresenter, AvaloniaList<DataGridColumn>?> ColumnsProperty =
            AvaloniaProperty.RegisterDirect<DataGridRowsPresenter, AvaloniaList<DataGridColumn>?>(
                nameof(Columns), 
                o => o.Columns, 
                (o, v) => o.Columns = v);

        internal static readonly StyledProperty<DataGridGridLinesVisibility> GridLinesVisibilityProperty = 
            AvaloniaProperty.Register<DataGridRowsPresenter, DataGridGridLinesVisibility>(nameof(GridLinesVisibility));

        private AvaloniaList<DataGridColumn>? _columns;

        public IDataTemplate ItemTemplate
        {
            get => GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public IEnumerable? Items
        {
            get => GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        public object? SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        internal AvaloniaList<DataGridColumn>? Columns
        {
            get => _columns;
            set => SetAndRaise(ColumnsProperty, ref _columns, value);
        }

        internal DataGridGridLinesVisibility GridLinesVisibility
        {
            get => GetValue(GridLinesVisibilityProperty);
            set => SetValue(GridLinesVisibilityProperty, value);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            InvalidateItemTemplate();
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == ColumnsProperty)
            {
                InvalidateItemTemplate();
            }
        }

        private void InvalidateItemTemplate()
        {
            ItemTemplate = new FuncDataTemplate(
                _ => true,
                (_, _) => new DataGridRow()
                {
                    [!DataGridRow.ColumnsProperty] = this[!DataGridRowsPresenter.ColumnsProperty],
                    [!DataGridRow.GridLinesVisibilityProperty] = this[!DataGridRowsPresenter.GridLinesVisibilityProperty]
                },
                supportsRecycling: true);
        }
    }
}
