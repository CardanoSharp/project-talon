using Blockfrost.Api.Extensions;
using Blockfrost.Api.Services;
using CardanoSharp.Wallet;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Keys;
using ProjectTalon.Core.Data;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddBlockfrost("testnet", "kL2vAF27FpfuzrnhSofc1JawdlL0BNkh");

builder.Services.AddTransient<IWalletDatabase, WalletDatabase>();
builder.Services.AddTransient<IWalletKeyDatabase, WalletKeyDatabase>();

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
.WithName("GenerateMnemonic");

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

    return new { Address = baseAdd.ToString(), TotalBalance = amount };
});

app.Run();


internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}