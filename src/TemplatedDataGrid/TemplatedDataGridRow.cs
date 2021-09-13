using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using TemplatedDataGrid.Internal;
using TemplatedDataGrid.Primitives;

namespace TemplatedDataGrid
{
    [PseudoClasses(":selected")]
    public class TemplatedDataGridRow : TemplatedControl
    {
        internal static readonly DirectProperty<TemplatedDataGridRow, object?> SelectedItemProperty =
            AvaloniaProperty.RegisterDirect<TemplatedDataGridRow, object?>(
                nameof(SelectedItem), 
                o => o.SelectedItem, 
                (o, v) => o.SelectedItem = v,
                defaultBindingMode: BindingMode.TwoWay);

        internal static readonly DirectProperty<TemplatedDataGridRow, object?> SelectedCellProperty =
            AvaloniaProperty.RegisterDirect<TemplatedDataGridRow, object?>(
                nameof(SelectedCell), 
                o => o.SelectedCell, 
                (o, v) => o.SelectedCell = v,
                defaultBindingMode: BindingMode.TwoWay);

        internal static readonly DirectProperty<TemplatedDataGridRow, AvaloniaList<TemplatedDataGridColumn>?> ColumnsProperty =
            AvaloniaProperty.RegisterDirect<TemplatedDataGridRow, AvaloniaList<TemplatedDataGridColumn>?>(
                nameof(Columns), 
                o => o.Columns, 
                (o, v) => o.Columns = v);

        internal static readonly StyledProperty<TemplatedDataGridGridLinesVisibility> GridLinesVisibilityProperty = 
            AvaloniaProperty.Register<TemplatedDataGridRow, TemplatedDataGridGridLinesVisibility>(nameof(GridLinesVisibility));

        internal static readonly AttachedProperty<object?> ItemProperty = 
            AvaloniaProperty.RegisterAttached<IControl, object?>("Item", typeof(TemplatedDataGridRowsPresenter), null, true);

        internal static readonly AttachedProperty<int> IndexProperty = 
            AvaloniaProperty.RegisterAttached<IControl, int>("Index", typeof(TemplatedDataGridRowsPresenter), -1, true);

        internal static object? GetItem(IControl control)
        {
            return control.GetValue(ItemProperty);
        }

        internal static void SetItem(IControl control, object? value)
        {
            control.SetValue(ItemProperty, value);
        }

        internal static int GetIndex(IControl control)
        {
            return control.GetValue(IndexProperty);
        }

        internal static void SetIndex(IControl control, int value)
        {
            control.SetValue(IndexProperty, value);
        }

        private object? _selectedItem;
        private object? _selectedCell;
        private AvaloniaList<TemplatedDataGridColumn>? _columns;
        private TemplatedDataGridCellsPresenter? _cellsPresenter;
        private Visual? _bottomGridLine;

        public TemplatedDataGridRow()
        {
            UpdatePseudoClassesSelectedItem(SelectedItem, DataContext);

            this.GetObservable(ItemProperty).Subscribe(item =>
            {
#if DEBUG
                //Console.WriteLine($"[TemplatedDataGridRow.Item] item='{item}',  DataContext='{DataContext}'");
#endif
                UpdatePseudoClassesSelectedItem(SelectedItem, DataContext);
            });
 
            this.GetObservable(IndexProperty).Subscribe(Index =>
            {
#if DEBUG
                //Console.WriteLine($"[TemplatedDataGridRow.Index] Index='{Index}',  DataContext='{DataContext}'");
#endif
                UpdatePseudoClassesSelectedItem(SelectedItem, DataContext);
            });

            this.GetObservable(SelectedItemProperty).Subscribe(item =>
            {
#if DEBUG
                //Console.WriteLine($"[TemplatedDataGridRow.SelectedItem] item='{item}', GetItem='{GetItem(this)}', GetIndex='{GetIndex(this)}', DataContext='{DataContext}'");
#endif
            });
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

        internal AvaloniaList<TemplatedDataGridColumn>? Columns
        {
            get => _columns;
            set => SetAndRaise(ColumnsProperty, ref _columns, value);
        }

        internal TemplatedDataGridGridLinesVisibility GridLinesVisibility
        {
            get => GetValue(GridLinesVisibilityProperty);
            set => SetValue(GridLinesVisibilityProperty, value);
        }

        internal CompositeDisposable? TemplateDisposables { get; set; }

        internal CompositeDisposable? CellsDisposables { get; set; }

        internal CompositeDisposable? BottomGridLineDisposables { get; set; }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _cellsPresenter = e.NameScope.Find<TemplatedDataGridCellsPresenter>("PART_CellsPresenter");
            _bottomGridLine = e.NameScope.Find<Visual>("PART_BottomGridLine");

            InvalidateCellsPresenter();
            InvalidateBottomGridLine();

            UpdatePseudoClassesSelectedItem(SelectedItem, DataContext);
            
#if DEBUG
            //Console.WriteLine($"[TemplatedDataGridRow.OnApplyTemplate] Item='{GetItem(this)}', Index='{GetIndex(this)}', DataContext='{DataContext}'");
#endif
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
#if DEBUG
            Console.WriteLine($"[TemplatedDataGridRow.Attach] {DataContext}");
#endif
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);
#if DEBUG
            Console.WriteLine($"[TemplatedDataGridRow.Detach] {DataContext}");
#endif
            Detach();
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == ItemProperty)
            {
#if DEBUG
                var oldValue = change.OldValue.GetValueOrDefault<object?>();
                var newValue = change.NewValue.GetValueOrDefault<object?>();
                //Console.WriteLine($"[TemplatedDataGridRow.Item] OldValue='{oldValue}' NewValue='{newValue}', DataContext='{DataContext}'");
#endif
            }
            
            if (change.Property == IndexProperty)
            {
#if DEBUG
                //Console.WriteLine($"[TemplatedDataGridRow.Index] old='{change.OldValue.GetValueOrDefault<int>()}' new='{change.NewValue.GetValueOrDefault<int>()}', DataContext='{DataContext}'");
#endif
            }          

            if (change.Property == DataContextProperty)
            {
                UpdatePseudoClassesSelectedItem(SelectedItem, change.NewValue.GetValueOrDefault<object?>());
            }

            if (change.Property == SelectedItemProperty)
            {
                UpdatePseudoClassesSelectedItem(change.NewValue.GetValueOrDefault<object?>(), DataContext);
            }

            if (change.Property == SelectedCellProperty)
            {
                // TODO:
            }

            if (change.Property == ColumnsProperty)
            {
                InvalidateCellsPresenter();
            }
        }

        internal void Detach()
        {
            TemplateDisposables?.Dispose();
            TemplateDisposables = null;

            CellsDisposables?.Dispose();
            CellsDisposables = null;

            BottomGridLineDisposables?.Dispose();
            BottomGridLineDisposables = null;
        }

        private void InvalidateCellsPresenter()
        {
            CellsDisposables?.Dispose();
            CellsDisposables = null;

            if (_cellsPresenter is { })
            {
                CellsDisposables = new CompositeDisposable();
                
                _cellsPresenter.TwoWayBind(TemplatedDataGridCellsPresenter.SelectedItemProperty, this, TemplatedDataGridRow.SelectedItemProperty, CellsDisposables);
                _cellsPresenter.TwoWayBind(TemplatedDataGridCellsPresenter.SelectedCellProperty, this, TemplatedDataGridRow.SelectedCellProperty, CellsDisposables);
                _cellsPresenter.OneWayBind(TemplatedDataGridCellsPresenter.ColumnsProperty, this, TemplatedDataGridRow.ColumnsProperty, CellsDisposables);
            }
        }

        private void InvalidateBottomGridLine()
        {
            BottomGridLineDisposables?.Dispose();
            BottomGridLineDisposables = null;

            if (_bottomGridLine is null)
            {
                return;
            }

            BottomGridLineDisposables = new CompositeDisposable();
            
            _bottomGridLine.OneWayBind(
                Visual.IsVisibleProperty, 
                this.GetObservable(TemplatedDataGridRow.GridLinesVisibilityProperty).Select(x => new BindingValue<bool>(x.HasFlag(TemplatedDataGridGridLinesVisibility.Horizontal))), 
                BottomGridLineDisposables);
        }

        private void UpdatePseudoClassesSelectedItem(object? selectedItem, object? dataContext)
        {
            var item = GetItem(this);
            var index = GetIndex(this);
            var isSelected = dataContext is { } && selectedItem == item;
#if DEBUG
            if (isSelected)
            {
                //Console.WriteLine($"[UpdatePseudoClassesSelectedItem] isSelected='{isSelected}', Tag='{Tag}', item='{item}', index='{index}' selectedItem='{selectedItem}', dataContext='{dataContext}'");
            }
#endif
            PseudoClasses.Set(":selected", isSelected);
        }
    }
}
