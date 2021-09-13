using System;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Data;

namespace TemplatedDataGrid
{
    internal static class BindingExtensions
    {
        public static IDisposable DisposeWith(this IDisposable item, CompositeDisposable compositeDisposable)
        {
            if (compositeDisposable is null)
            {
                throw new ArgumentNullException(nameof(compositeDisposable));
            }

            compositeDisposable.Add(item);
            return item;
        }

        public static IDisposable BindOneWay(this IAvaloniaObject target, AvaloniaProperty targetProperty, IAvaloniaObject source, AvaloniaProperty sourceProperty, CompositeDisposable? compositeDisposable = null)
        {
            var disposable = target.Bind(targetProperty, new Binding(sourceProperty.Name, BindingMode.OneWay) { Source = source });
            if (compositeDisposable is { })
            {
                compositeDisposable.Add(compositeDisposable);
            }
            return disposable;
        }

        public static IDisposable BindTwoWay(this IAvaloniaObject target, AvaloniaProperty targetProperty, IAvaloniaObject source, AvaloniaProperty sourceProperty, CompositeDisposable? compositeDisposable = null)
        {
            var disposable = target.Bind(targetProperty, new Binding(sourceProperty.Name, BindingMode.TwoWay) { Source = source });
            if (compositeDisposable is { })
            {
                compositeDisposable.Add(compositeDisposable);
            }
            return disposable;
        }
        
        public static IDisposable BindOneWay<T>(this IAvaloniaObject target, AvaloniaProperty targetProperty, IObservable<T> observable, CompositeDisposable? compositeDisposable = null)
        {
            var disposable = target.Bind(targetProperty, observable.ToBinding());

            if (compositeDisposable is { })
            {
                compositeDisposable.Add(compositeDisposable);
            }
            return disposable;
        }
    }
}
