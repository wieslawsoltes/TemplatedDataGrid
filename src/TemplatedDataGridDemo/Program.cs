using System;
using Avalonia;
using Avalonia.ReactiveUI;
using TemplatedDataGrid;

namespace TemplatedDataGridDemo
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            GC.KeepAlive(typeof(DataGrid).Assembly);

            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        public static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .With(new Win32PlatformOptions()
                {
                    UseDeferredRendering = true
                })
                .With(new X11PlatformOptions()
                {
                    UseDeferredRendering = true
                })
                .With(new AvaloniaNativePlatformOptions()
                {
                    UseDeferredRendering = true
                })
                .LogToTrace()
                .UseReactiveUI();
        }
    }
}
