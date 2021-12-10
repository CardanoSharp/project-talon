using Blockfrost.Api.Extensions;
using Blockfrost.Api.Services;
using CardanoSharp.Wallet;
using ProjectTalon.Core.Data;

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

app.MapGet("/wallet/{id}/balance", async (int id, ICardanoService cardanoService) =>
{
    var block = await cardanoService.Blocks.GetLatestAsync();
    return block.Slot;
});

app.Run();


internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}