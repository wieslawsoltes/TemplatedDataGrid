using System;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Data;

namespace TemplatedDataGrid
{
    internal static class BindingExtensions
    {
        public static void BindOneWay(this IAvaloniaObject target, AvaloniaProperty targetProperty, IAvaloniaObject source, AvaloniaProperty sourceProperty, CompositeDisposable compositeDisposable)
        {
            var targetDisposable = target.Bind(targetProperty, source.GetObservable(sourceProperty));
            compositeDisposable.Add(targetDisposable);
        }

        public static void BindTwoWay(this IAvaloniaObject target, AvaloniaProperty targetProperty, IAvaloniaObject source, AvaloniaProperty sourceProperty, CompositeDisposable compositeDisposable)
        {
            var targetDisposable = target.Bind(targetProperty, source.GetObservable(sourceProperty));
            var sourceDisposable = source.Bind(sourceProperty, target.GetObservable(targetProperty));
            compositeDisposable.Add(targetDisposable);
            compositeDisposable.Add(sourceDisposable);
        }
        
        public static void BindOneWay<T>(this IAvaloniaObject target, AvaloniaProperty<T> targetProperty, IObservable<BindingValue<T>> source, CompositeDisposable compositeDisposable)
        {
            var targetDisposable = target.Bind(targetProperty, source);
            compositeDisposable.Add(targetDisposable);
        }
    }
}
