using System;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Data;

namespace TemplatedDataGrid.Internal
{
    internal static class AvaloniaObjectExtensions
    {
        public static void OneWayBind(
            this AvaloniaObject target, 
            AvaloniaProperty targetProperty, 
            AvaloniaObject source, 
            AvaloniaProperty sourceProperty, 
            CompositeDisposable compositeDisposable)
        {
            var targetDisposable = target.Bind(targetProperty, source.GetObservable(sourceProperty));
            compositeDisposable.Add(targetDisposable);
        }

        public static void TwoWayBind(
            this AvaloniaObject target,
            AvaloniaProperty targetProperty, 
            AvaloniaObject source, 
            AvaloniaProperty sourceProperty, 
            CompositeDisposable compositeDisposable)
        {
            var targetDisposable = target.Bind(targetProperty, source.GetObservable(sourceProperty));
            var sourceDisposable = source.Bind(sourceProperty, target.GetObservable(targetProperty));
            compositeDisposable.Add(targetDisposable);
            compositeDisposable.Add(sourceDisposable);
        }
        
        public static void OneWayBind<T>(
            this AvaloniaObject target, 
            AvaloniaProperty<T> targetProperty, 
            IObservable<BindingValue<T>> source, 
            CompositeDisposable compositeDisposable)
        {
            var targetDisposable = target.Bind(targetProperty, source);
            compositeDisposable.Add(targetDisposable);
        }
    }
}
