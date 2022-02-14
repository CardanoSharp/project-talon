using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using ProjectTalon.Core.Data;
using ProjectTalon.UI.ViewModels;
using Splat;

namespace ProjectTalon.UI
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            //Desktop App
            Bootstrapper.Register(Locator.CurrentMutable, Locator.Current);
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        
        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
    }
    public static class Bootstrapper
    {
        public static void Register(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.Register<IWalletDatabase>(() => new WalletDatabase());
            services.Register<IWalletKeyDatabase>(() => new WalletKeyDatabase());
            services.Register<IAppConnectDatabase>(() => new AppConnectDatabase());
            services.Register<ITransactionRequestDatabase>(() => new TransactionRequestDatabase());
            services.Register<ISettingsDatabase>(() => new SettingsDatabase());
            // services.RegisterLazySingleton<IMainWindowViewModel>(() => new MainWindowViewModel(
            //     resolver.GetService<IWalletDatabase>()
            // ));
        }
    }
}
