using System;
using Avalonia;
using Avalonia.ReactiveUI;

namespace TemplatedDataGridDemo
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        public static AppBuilder BuildAvaloniaApp()
        {
            GC.KeepAlive(typeof(TemplatedDataGrid.TemplatedDataGrid).Assembly);

            return AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
        }
    }
}
