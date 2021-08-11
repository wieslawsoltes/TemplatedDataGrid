using System.Collections;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;

namespace ItemsRepeaterDataGrid
{
    public class DataGridRowsPresenter : TemplatedControl
    {
        private ListBox? _listBox;

        public static readonly StyledProperty<AvaloniaList<DataGridColumn>> ColumnsProperty = 
            AvaloniaProperty.Register<DataGridRowsPresenter, AvaloniaList<DataGridColumn>>(nameof(Columns), new AvaloniaList<DataGridColumn>());

        public static readonly StyledProperty<IEnumerable?> ItemsProperty = 
            AvaloniaProperty.Register<DataGridRowsPresenter, IEnumerable?>(nameof(Items));

        public static readonly StyledProperty<object?> SelectedItemProperty = 
            AvaloniaProperty.Register<DataGridRowsPresenter, object?>(nameof(SelectedItem));

        public AvaloniaList<DataGridColumn> Columns
        {
            get => GetValue(ColumnsProperty);
            set => SetValue(ColumnsProperty, value);
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

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _listBox = e.NameScope.Find<ListBox>("PART_ListBox");

            InvalidatePanel();
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == ColumnsProperty)
            {
                InvalidatePanel();
            }
        }

        private void InvalidatePanel()
        {
            if (_listBox is null)
            {
                return;
            }

            _listBox.ItemTemplate = new FuncDataTemplate<object>(
                (_, _) => new DataGridRow()
                {
                    [!DataGridRow.ColumnsProperty] = this[!DataGrid.ColumnsProperty]
                });
        }
    }
}
