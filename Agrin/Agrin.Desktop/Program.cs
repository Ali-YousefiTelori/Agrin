using System;
using Avalonia;
using Avalonia.Logging.Serilog;
using Agrin.Desktop.ViewModels;
using Agrin.Desktop.Views;

namespace Agrin.Desktop
{
    class Program
    {
        static void Main(string[] args)
        {
            BuildAvaloniaApp().Start<MainWindow>(() => new MainWindowViewModel());
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI()
                .LogToDebug();
    }
}
