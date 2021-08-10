using System;
using Avalonia;
using Avalonia.ReactiveUI;
using ItemsRepeaterDataGrid;

namespace ItemsRepeaterDataGridDemo
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
                .LogToTrace()
                .UseReactiveUI();
        }
    }
}
