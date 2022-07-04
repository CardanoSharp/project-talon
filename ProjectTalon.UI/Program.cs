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
using CardanoSharp.Koios.Sdk;
using CardanoSharp.Wallet;
using ProjectTalon.Core.Data;
using ProjectTalon.Core.Services;
using Refit;
using Splat;
using WalletService = ProjectTalon.Core.Services.WalletService;

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
            
            services.Register(
                () => RestService.For<IEpochClient>("https://testnet.koios.rest/api/v0"));
            services.Register(
                () => RestService.For<INetworkClient>("https://testnet.koios.rest/api/v0"));
            services.Register(
                () => RestService.For<ITransactionClient>("https://testnet.koios.rest/api/v0"));
            services.Register(
                () => RestService.For<IAddressClient>("https://testnet.koios.rest/api/v0"));

            services.Register<Core.Services.IWalletService>(() => new WalletService(
                new MnemonicService(),
                resolver.GetService<IWalletKeyDatabase>(),
                resolver.GetService<IWalletDatabase>()
            ));
            
            services.Register<Core.Services.IAddressService>(() => new Core.Services.AddressService(
                resolver.GetService<IAddressClient>(),
                new MnemonicService(),
                resolver.GetService<IWalletDatabase>(),
                resolver.GetService<IWalletKeyDatabase>()
            ));

            services.Register<ITransactionService>(() => new TransactionService(
                resolver.GetService<ITransactionClient>(),
                resolver.GetService<IEpochClient>(),
                resolver.GetService<IAddressClient>(),
                resolver.GetService<Core.Services.IAddressService>(),
                resolver.GetService<INetworkClient>(),
                resolver.GetService<IWalletKeyDatabase>()
            ));
        }
    }
}
