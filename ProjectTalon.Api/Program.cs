using Blockfrost.Api.Extensions;
using CardanoSharp.Koios.Sdk;
using CardanoSharp.Wallet.Models.Addresses;
using ProjectTalon.Api;
using ProjectTalon.Core.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddBlockfrost("testnet", "kL2vAF27FpfuzrnhSofc1JawdlL0BNkh");

builder.Services.AddTransient<IWalletDatabase, WalletDatabase>();
builder.Services.AddTransient<IWalletKeyDatabase, WalletKeyDatabase>();
builder.Services.AddTransient<IAppConnectDatabase, AppConnectDatabase>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

ConnectorApi.AddEndpoints(app);
AccountsApi.AddEndpoints(app);
AddressesApi.AddEndpoints(app);
AssetsApi.AddEndpoints(app);
BlocksApi.AddEndpoints(app);
EpochsApi.AddEndpoints(app);
LedgerApi.AddEndpoints(app);
MetadataApi.AddEndpoints(app);
NetworkApi.AddEndpoints(app);
MetadataApi.AddEndpoints(app);
PoolsApi.AddEndpoints(app);
ScriptsApi.AddEndpoints(app);
TransactionsApi.AddEndpoints(app);

app.Run();

internal record ConnectRequest(string Name);