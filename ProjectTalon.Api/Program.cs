using Blockfrost.Api.Extensions;
using Blockfrost.Api.Services;
using CardanoSharp.Wallet;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Keys;
using ProjectTalon.Core.Data.Models;
using Microsoft.AspNetCore.Mvc;
using ProjectTalon.Core.Data;
using System.Text.Json;
using ProjectTalon.Core.Common;

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

app.MapGet("/mnemonic/{size}", (int size) =>
{
    return new MnemonicService().Generate(size);
})
    .WithName("Generate Mnemonic");

app.MapGet("/wallet/{id}/balance", async (int id, IWalletDatabase walletdatabase, IWalletKeyDatabase keyDatabase, ICardanoService cardanoService) =>
{
    var wallets = await walletdatabase.GetWalletsAsync();
    var wallet = await keyDatabase.GetWalletKeysAsync(id);
    var publicKey = JsonSerializer.Deserialize<PublicKey>(wallet.First().Vkey);

    var payment = publicKey
        .Derive(RoleType.ExternalChain)
        .Derive(0);

    var stake = publicKey
        .Derive(RoleType.Staking)
        .Derive(0);

    var baseAdd = new AddressService()
        .GetAddress(payment.PublicKey, stake.PublicKey, NetworkType.Testnet, AddressType.Base);
    long amount = 0;
    try
    {
        var response = await cardanoService.Addresses.GetUtxosAsync(baseAdd.ToString());
        amount = response.SelectMany(m => m.Amount).Where(m => m.Unit == "lovelace").Sum(m => long.Parse(m.Quantity));
    }
    catch
    {
        amount = -1;
    }

    return Results.Ok(new { Address = baseAdd.ToString(), TotalBalance = amount });
})
    .WithName("Get Wallet Balance");

app.MapPost("/connect", async ([FromBody] ConnectRequest request, IAppConnectDatabase appConnectDatabase) =>
{
    var appId = Guid.NewGuid().ToString();

    await appConnectDatabase.SaveAppConnectionAsync(new AppConnect()
    {
        AppId = appId,
        Name = request.Name,
        ConnectionStatus = (int)ConnectionStatus.Pending
    });

    return Results.Ok(new { AppId = appId });

})
    .WithName("Connect");

app.MapGet("/connect/{appId}/status", async (string appId, IAppConnectDatabase appConnectDatabase) =>
{
    var appConnect = await appConnectDatabase.GetAppConnectionByAppIdAsync(appId);

    if (appConnect == null) 
        return Results.NotFound();
    else
        return Results.Ok(new { 
            Status = ((ConnectionStatus)appConnect.ConnectionStatus).ToString(),
            StatusCode = appConnect.ConnectionStatus
        });
})
    .WithName("Check Connection Status");

app.Run();


internal record ConnectRequest(string Name);