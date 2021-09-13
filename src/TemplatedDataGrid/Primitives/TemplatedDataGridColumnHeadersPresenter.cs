using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;

namespace TemplatedDataGrid.Primitives
{
    public class TemplatedDataGridColumnHeadersPresenter : TemplatedControl
    {
        internal static readonly DirectProperty<TemplatedDataGridColumnHeadersPresenter, IScrollable?> ScrollProperty =
            AvaloniaProperty.RegisterDirect<TemplatedDataGridColumnHeadersPresenter, IScrollable?>(
                nameof(Scroll), 
                o => o.Scroll, 
                (o, v) => o.Scroll = v);
        
        internal static readonly DirectProperty<TemplatedDataGridColumnHeadersPresenter, AvaloniaList<TemplatedDataGridColumn>?> ColumnsProperty =
            AvaloniaProperty.RegisterDirect<TemplatedDataGridColumnHeadersPresenter, AvaloniaList<TemplatedDataGridColumn>?>(
                nameof(Columns), 
                o => o.Columns, 
                (o, v) => o.Columns = v);

        internal static readonly DirectProperty<TemplatedDataGridColumnHeadersPresenter, AvaloniaList<TemplatedDataGridColumnHeader>> ColumnHeadersProperty =
            AvaloniaProperty.RegisterDirect<TemplatedDataGridColumnHeadersPresenter, AvaloniaList<TemplatedDataGridColumnHeader>>(
                nameof(ColumnHeaders), 
                o => o.ColumnHeaders, 
                (o, v) => o.ColumnHeaders = v);

        internal static readonly StyledProperty<bool> CanUserSortColumnsProperty = 
            AvaloniaProperty.Register<TemplatedDataGridColumnHeadersPresenter, bool>(nameof(CanUserSortColumns));

        internal static readonly StyledProperty<bool> CanUserResizeColumnsProperty = 
            AvaloniaProperty.Register<TemplatedDataGridColumnHeadersPresenter, bool>(nameof(CanUserResizeColumns));

        private IScrollable? _scroll;
        private AvaloniaList<TemplatedDataGridColumn>? _columns;
        private AvaloniaList<TemplatedDataGridColumnHeader> _columnHeaders = new AvaloniaList<TemplatedDataGridColumnHeader>();
        private Grid? _root;
        private readonly List<Control> _rootChildren = new List<Control>();

        internal IScrollable? Scroll
        {
            get => _scroll;
            set => SetAndRaise(ScrollProperty, ref _scroll, value);
        }

        internal AvaloniaList<TemplatedDataGridColumn>? Columns
        {
            get => _columns;
            set => SetAndRaise(ColumnsProperty, ref _columns, value);
        }
 
        internal AvaloniaList<TemplatedDataGridColumnHeader> ColumnHeaders
        {
            get => _columnHeaders;
            set => SetAndRaise(ColumnHeadersProperty, ref _columnHeaders, value);
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

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _root = e.NameScope.Find<Grid>("PART_Root");

            InvalidateRoot();
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == ColumnsProperty)
            {
                InvalidateRoot();
            }

            if (change.Property == ScrollProperty)
            {
                var oldScroll = change.OldValue.GetValueOrDefault<IScrollable?>();
                if (oldScroll is { })
                {
                    if (oldScroll is ScrollViewer scrollViewer)
                    {
                        scrollViewer.ScrollChanged -= ScrollViewer_OnScrollChanged;
                    }
                }

                var newScroll = change.NewValue.GetValueOrDefault<IScrollable?>();
                if (newScroll is { })
                {
                    if (newScroll is ScrollViewer scrollViewer)
                    {
                        scrollViewer.ScrollChanged += ScrollViewer_OnScrollChanged;
                    }
                }
            }
        }

        private void ScrollViewer_OnScrollChanged(object? sender, ScrollChangedEventArgs e)
        {
            if (Scroll is { })
            {
                Margin = new Thickness(-Scroll.Offset.X, 0, 0, 0);
            }
        }
        
        private CompositeDisposable? _rootDisposables;

        private void InvalidateRoot()
        {
            _rootDisposables?.Dispose();

            if (_root is null)
            {
                return;
            }

            _rootDisposables = new CompositeDisposable();
            
            foreach (var child in _rootChildren)
            {
                _root.Children.Remove(child);
            }

            _columnHeaders.Clear();

            var columns = Columns;
            if (columns is null)
            {
                return;
            }

            //  Generate RowDefinitions

            var rowDefinitions = new List<RowDefinition>();

            rowDefinitions.Add(new RowDefinition(GridLength.Auto));

            // Generate ColumnDefinitions

            var columnDefinitions = new List<ColumnDefinition>();
            var splitterColumnIndexes = new List<int>();

            for (var i = 0; i < columns.Count; i++)
            {
                var column = columns[i];
                var isStarWidth = column.Width.IsStar;
                var isAutoWidth = column.Width.IsAuto;
                var isPixelWidth = column.Width.IsAbsolute;

                var columnDefinition = new ColumnDefinition();

                columnDefinition.TwoWayBind(
                    ColumnDefinition.MinWidthProperty, 
                    column, 
                    TemplatedDataGridColumn.MinWidthProperty, 
                    _rootDisposables);
                columnDefinition.TwoWayBind(
                    ColumnDefinition.MaxWidthProperty, 
                    column, 
                    TemplatedDataGridColumn.MaxWidthProperty,
                    _rootDisposables);

                if (isStarWidth)
                {
                    columnDefinition.TwoWayBind(
                        ColumnDefinition.WidthProperty, 
                        column, 
                        TemplatedDataGridColumn.WidthProperty, 
                        _rootDisposables);
                }

                if (isAutoWidth)
                {
                    columnDefinition.TwoWayBind(
                        ColumnDefinition.WidthProperty, 
                        column, 
                        TemplatedDataGridColumn.WidthProperty,
                        _rootDisposables);

                    columnDefinition.SetValue(DefinitionBase.SharedSizeGroupProperty, $"Column{i}");
                }

                if (isPixelWidth)
                {
                    columnDefinition.TwoWayBind(
                        ColumnDefinition.WidthProperty, 
                        column, 
                        TemplatedDataGridColumn.WidthProperty, 
                        _rootDisposables);
                }

                columnDefinitions.Add(columnDefinition);

                // Generate DataGridColumnHeader's
                var columnHeader = new TemplatedDataGridColumnHeader
                {
                    [Grid.RowProperty] = 0,
                    [Grid.ColumnProperty] = columnDefinitions.Count - 1,
                    [TemplatedDataGridColumnHeader.ColumnProperty] = column
                };
                
                columnHeader.OneWayBind(
                    TemplatedDataGridColumnHeader.HeaderProperty, 
                    column, 
                    TemplatedDataGridColumn.HeaderProperty, 
                    _rootDisposables);
                columnHeader.OneWayBind(
                    TemplatedDataGridColumnHeader.CanUserSortColumnsProperty, 
                    this, 
                    TemplatedDataGridColumnHeadersPresenter.CanUserSortColumnsProperty, 
                    _rootDisposables);
                columnHeader.OneWayBind(
                    TemplatedDataGridColumnHeader.CanUserResizeColumnsProperty, 
                    this, 
                    TemplatedDataGridColumnHeadersPresenter.CanUserResizeColumnsProperty, 
                    _rootDisposables);
                columnHeader.OneWayBind(
                    TemplatedDataGridColumnHeader.ColumnHeadersProperty, 
                    this, 
                    TemplatedDataGridColumnHeadersPresenter.ColumnHeadersProperty, 
                    _rootDisposables);
                
                _columnHeaders.Add(columnHeader);
                _rootChildren.Add(columnHeader);

                column.OneWayBind(
                    TemplatedDataGridColumn.ActualWidthProperty,
                    columnHeader.GetObservable(Visual.BoundsProperty).Select(_ => columnDefinition.ActualWidth));

                if (i < columns.Count)
                {
                    columnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Pixel)));
                    splitterColumnIndexes.Add(columnDefinitions.Count - 1);
                }
            }

            columnDefinitions.Add(new ColumnDefinition(GridLength.Auto));

            // Generate Vertical Grid Lines
            // Generate GridSplitter's

            foreach (var columnIndex in splitterColumnIndexes)
            {
                var verticalColumnGridLine = new Rectangle
                {
                    [Grid.RowProperty] = 0,
                    [Grid.RowSpanProperty] = 1,
                    [Grid.ColumnProperty] = columnIndex,
                    [Rectangle.IsHitTestVisibleProperty] = false
                };
                ((IPseudoClasses)verticalColumnGridLine.Classes).Add(":vertical");
                _rootChildren.Add(verticalColumnGridLine);

                var verticalGridSplitter = new GridSplitter
                {
                    [Grid.RowProperty] = 0,
                    [Grid.RowSpanProperty] = 1,
                    [Grid.ColumnProperty] = columnIndex
                };

                verticalGridSplitter.OneWayBind(
                    GridSplitter.IsEnabledProperty, 
                    this, 
                    TemplatedDataGridColumnHeadersPresenter.CanUserResizeColumnsProperty, 
                    _rootDisposables);

                _rootChildren.Add(verticalGridSplitter);
            }

            _root.RowDefinitions.Clear();
            _root.RowDefinitions.AddRange(rowDefinitions);

            _root.ColumnDefinitions.Clear();
            _root.ColumnDefinitions.AddRange(columnDefinitions);

            foreach (var child in _rootChildren)
            {
                _root.Children.Add(child);
            }
        }
    }
}
