using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using CardanoSharp.Koios.Sdk;
using CardanoSharp.Wallet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using ProjectTalon.Core.Common;
using ProjectTalon.Core.Data;
using ProjectTalon.Core.Data.Models;
using ProjectTalon.Core.Services;
using ProjectTalon.UI.Apis;
using ProjectTalon.UI.ViewModels;
using ReactiveUI;
using Splat;
using AddressService = ProjectTalon.Core.Services.AddressService;
using IAddressService = ProjectTalon.Core.Services.IAddressService;

namespace ProjectTalon.UI.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        private Thread apiThread;
        private WebApplication? api;
        
        private int paddingRight = 20;
        private int paddingBottom = 85;

        private Label addressLabel => this.FindControl<Label>("lblAddress");

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            
            this.WhenActivated(d => d(ViewModel!.ImportWalletDialog.RegisterHandler(ShowImportWalletDialogAsync)));
            this.WhenActivated(d => d(ViewModel!.CreateWalletDialog.RegisterHandler(ShowCreateWalletDialogAsync)));
            this.WhenActivated(d => d(ViewModel!.ViewConnectionsDialog.RegisterHandler(ShowConnectionsDialogAsync)));
            this.WhenActivated(d => d(ViewModel!.ViewSettingsDialog.RegisterHandler(ShowSettingsDialogAsync)));
            this.WhenActivated(d => d(ViewModel!.AuthorizeAppDialog.RegisterHandler(ShowAuthorizeAppDialogAsync)));
            this.WhenActivated(d => d(ViewModel!.AuthorizeTransactionDialog.RegisterHandler(ShowAuthorizeTransactionDialogAsync)));
            
            this.WhenActivated(d =>
            {
                Disposable
                    .Create(() =>
                    {
                        if (api != null)
                        {
                            Task.Run(async () => await api.StopAsync());
                        }
                    })
                    .DisposeWith(d);
            });
            
            SetupWindow();
            ExtendClientAreaChromeHints = Avalonia.Platform.ExtendClientAreaChromeHints.OSXThickTitleBar;

            Task.Run(async () => await InitializeSettings());
            Task.Run(async () => await DetermineApiStatus());
        }

        private async Task DetermineApiStatus()
        {
            var settingsDatabase = Locator.Current.GetService<ISettingsDatabase>();
            var apiSettings = await settingsDatabase.GetByKeyAsync(SettingKeys.API_ENABLED);
            
            if (api is null && (apiThread is null || !apiThread.IsAlive) && apiSettings.Value == true.ToString())
            {
                apiThread = new Thread(new ThreadStart(() => RunApi(new string[]{})));
                apiThread.Start();
            }

            if (api is not null && (apiThread is null || apiThread.IsAlive) && apiSettings.Value == false.ToString())
            {
                await api.StopAsync();
                api = null;
            }
        }

        private async Task InitializeSettings()
        {
            var settingsDatabase = Locator.Current.GetService<ISettingsDatabase>();

            //CHECK NETWORK SETTING
            var networkSettings = await settingsDatabase.GetByKeyAsync(SettingKeys.NETWORK);
            if (networkSettings is null)
                await settingsDatabase.SaveAsync(new Settings
                {
                    Key = SettingKeys.NETWORK,
                    Value = NetworkOptions.TESTNET
                });
            
            //CHECK API SETTING
            var apiSettings = await settingsDatabase.GetByKeyAsync(SettingKeys.API_ENABLED);
            if (apiSettings is null)
                await settingsDatabase.SaveAsync(new Settings
                {
                    Key = SettingKeys.API_ENABLED,
                    Value = false.ToString()
                });
        }

        private async Task ShowImportWalletDialogAsync(InteractionContext<ImportWalletViewModel, ImportWalletWizardViewModel?> interaction)
        {
            var dialog = new ImportWalletWindow
            {
                DataContext = interaction.Input
            };

            var result = await dialog.ShowDialog<ImportWalletWizardViewModel?>(this);
            interaction.SetOutput(result);
        }
        
        private async Task ShowCreateWalletDialogAsync(InteractionContext<CreateWalletViewModel, CreateWalletViewModel?> interaction)
        {
            var dialog = new CreateWalletWindow
            {
                DataContext = interaction.Input
            };

            var result = await dialog.ShowDialog<CreateWalletViewModel?>(this);
            interaction.SetOutput(result);
        }
        
        private async Task ShowConnectionsDialogAsync(InteractionContext<ConnectionsViewModel, ViewConnectionsViewModel?> interaction)
        {
            var dialog = new ConnectionsWindow();
            dialog.DataContext = interaction.Input;

            var result = await dialog.ShowDialog<ViewConnectionsViewModel?>(this);
            interaction.SetOutput(result);
        }
        
        private async Task ShowSettingsDialogAsync(InteractionContext<SettingsViewModel, ManageSettingsViewModel?> interaction)
        {
            var dialog = new SettingsWindow();
            dialog.Width = 400;
            dialog.Height = 200;
            dialog.DataContext = interaction.Input;

            var result = await dialog.ShowDialog<ManageSettingsViewModel?>(this);
            interaction.SetOutput(result);

            await DetermineApiStatus();
        }
        
        private async Task ShowAuthorizeAppDialogAsync(InteractionContext<AuthorizeAppViewModel, AuthorizeAppViewModel?> interaction)
        {
            var dialog = new AuthorizeAppWindow();
            dialog.Width = 350;
            dialog.Height = 600;
            dialog.DataContext = interaction.Input;

            var result = await dialog.ShowDialog<AuthorizeAppViewModel?>(this);
            interaction.SetOutput(result);

            ViewModel.AuthAppWindowIsOpen = false;
        }
        
        private async Task ShowAuthorizeTransactionDialogAsync(InteractionContext<AuthorizeTransactionViewModel, AuthorizeTransactionViewModel?> interaction)
        {
            var dialog = new AuthorizeTransactionWindow();
            dialog.Width = 350;
            dialog.Height = 600;
            dialog.DataContext = interaction.Input;

            var result = await dialog.ShowDialog<AuthorizeTransactionViewModel?>(this);
            interaction.SetOutput(result);

            ViewModel.AuthAppWindowIsOpen = false;
        }
        
        private void SetupWindow()
        {
            Width = 350;
            Height = 600;
            CanResize = false;

            var window = this.GetSelfAndLogicalAncestors().OfType<Window>().First();

            var screen = window.Screens.ScreenFromPoint(Position);

            var newX = screen.Bounds.Width - paddingRight - 350;
            var newY = screen.Bounds.Height - paddingBottom - 600;

            Position = new PixelPoint(newX, newY);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void RunApi(string[] args)
        {
            //Api
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(x =>
            {
                x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "JWT Authorization header using bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
                x.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {new OpenApiSecurityScheme() {Reference = new OpenApiReference()
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                                    }}, new List<string>()
                    }
                });
            });

            builder.Services.AddKoios("https://testnet.koios.rest/api/v0");

            builder.Services.AddTransient<IWalletDatabase, WalletDatabase>();
            builder.Services.AddTransient<IWalletKeyDatabase, WalletKeyDatabase>();
            builder.Services.AddTransient<IAppConnectDatabase, AppConnectDatabase>();
            builder.Services.AddTransient<ITransactionRequestDatabase, TransactionRequestDatabase>();
            builder.Services.AddTransient<ITransactionService, TransactionService>();
            builder.Services.AddTransient<IAddressService, AddressService>();

            builder.Services.AddTransient<IMnemonicService, MnemonicService>();
            
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
            // builder.Services.AddAuthorization(options =>
            // {
            //     // options.FallbackPolicy = new AuthorizationPolicyBuilder()
            //     //     .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            //     //     .RequireAuthenticatedUser()
            //     //     .Build();
            // });
            
            api = builder.Build();

            api.UseSwagger();
            api.UseSwaggerUI();

            api.UseHttpsRedirection();
            api.UseAuthentication();
            //api.UseAuthorization();

            ConnectorApi.AddEndpoints(api);
            AccountsApi.AddEndpoints(api);
            AddressesApi.AddEndpoints(api);
            AssetsApi.AddEndpoints(api);
            BlocksApi.AddEndpoints(api);
            EpochsApi.AddEndpoints(api);
            LedgerApi.AddEndpoints(api);
            MetadataApi.AddEndpoints(api);
            NetworkApi.AddEndpoints(api);
            MetadataApi.AddEndpoints(api);
            PoolsApi.AddEndpoints(api);
            ScriptsApi.AddEndpoints(api);
            TransactionsApi.AddEndpoints(api);

            api.Run();
        }
    }
}