using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Blockfrost.Api.Extensions;
using CardanoSharp.Koios.Sdk;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using ProjectTalon.Core.Common;
using ProjectTalon.Core.Data;
using ProjectTalon.Core.Data.Models;
using ProjectTalon.UI.Apis;
using ProjectTalon.UI.ViewModels;
using ReactiveUI;
using Splat;
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
            builder.Services.AddSwaggerGen();
            
            builder.Services.AddBlockfrost("testnet", "kL2vAF27FpfuzrnhSofc1JawdlL0BNkh");

            builder.Services.AddKoios("https://testnet.koios.rest/api/v0");

            builder.Services.AddTransient<IWalletDatabase, WalletDatabase>();
            builder.Services.AddTransient<IWalletKeyDatabase, WalletKeyDatabase>();
            builder.Services.AddTransient<IAppConnectDatabase, AppConnectDatabase>();
            
            builder.Services
                .AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };
                }
            );
            
            builder.Services.AddAuthorization();
            
            api = builder.Build();

            if (api.Environment.IsDevelopment())
            {
                api.UseSwagger();
                api.UseSwaggerUI();
            }

            api.UseHttpsRedirection();
            api.UseAuthorization();
            api.UseAuthentication();

            api.MapGet("/hello", () =>
            {
                return Results.Ok("Hello, World!");
            });
            
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