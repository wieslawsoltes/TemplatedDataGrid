using System;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Data;

namespace TemplatedDataGrid
{
    internal static class BindingExtensions
    {
        public static void BindOneWay(this IAvaloniaObject target, AvaloniaProperty targetProperty, IAvaloniaObject source, AvaloniaProperty sourceProperty, CompositeDisposable? compositeDisposable = null)
        {
            var targetDisposable = target.Bind(targetProperty, source.GetObservable(sourceProperty));
            if (compositeDisposable is { })
            {
                compositeDisposable.Add(targetDisposable);
            }
        }

        public static void BindTwoWay(this IAvaloniaObject target, AvaloniaProperty targetProperty, IAvaloniaObject source, AvaloniaProperty sourceProperty, CompositeDisposable? compositeDisposable = null)
        {
            var targetDisposable = target.Bind(targetProperty, source.GetObservable(sourceProperty));
            var sourceDisposable = source.Bind(sourceProperty, target.GetObservable(targetProperty));
            if (compositeDisposable is { })
            {
                compositeDisposable.Add(targetDisposable);
                compositeDisposable.Add(sourceDisposable);
            }
        }
        
        public static void BindOneWay<T>(this IAvaloniaObject target, AvaloniaProperty<T> targetProperty, IObservable<BindingValue<T>> observable, CompositeDisposable? compositeDisposable = null)
        {
            var targetDisposable = target.Bind(targetProperty, observable);
            if (compositeDisposable is { })
            {
                compositeDisposable.Add(targetDisposable);
            }
        }
    }
}
