using System.Text.Json;
using Blockfrost.Api.Services;
using CardanoSharp.Wallet;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Keys;
using Microsoft.AspNetCore.Mvc;
using ProjectTalon.Core.Common;
using ProjectTalon.Core.Data;
using ProjectTalon.Core.Data.Models;

namespace ProjectTalon.Api;

public static class ConnectorApi
{
    public static void AddEndpoints(WebApplication app)
    {
        app.MapPost("/connect", Connect);
        app.MapGet("/connect/{appId}/status", CheckConnectionStatus);
    }

    private static async Task<IResult> Connect(
        [FromBody] ConnectRequest request,
        IAppConnectDatabase appConnectDatabase)
    {
        try
        {
            var appId = Guid.NewGuid().ToString();

            await appConnectDatabase.SaveAsync(new AppConnect
            {
                AppId = appId,
                Name = request.Name,
                ConnectionStatus = (int) ConnectionStatus.Pending
            });

            return Results.Ok(new {AppId = appId});
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }
    }

    private static async Task<IResult> CheckConnectionStatus(string appId, IAppConnectDatabase appConnectDatabase)
    {
        var appConnect = await appConnectDatabase.GetByAppIdAsync(appId);

        if (appConnect is null)
            return Results.NotFound();
        return Results.Ok(new
        {
            Status = ((ConnectionStatus) appConnect.ConnectionStatus).ToString(),
            StatusCode = appConnect.ConnectionStatus
        });
    }

    private static IResult GenerateMnemonic(int size)
    {
        try
        {
            return Results.Ok(new MnemonicService().Generate(size));
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }
    }

    private static async Task<IResult> GetWalletBalance(int id, IWalletDatabase walletdatabase,
        IWalletKeyDatabase keyDatabase, ICardanoService cardanoService)
    {
        try
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
                amount = response.SelectMany(m => m.Amount).Where(m => m.Unit == "lovelace")
                    .Sum(m => long.Parse(m.Quantity));
            }
            catch
            {
                amount = -1;
            }

            return Results.Ok(new {Address = baseAdd.ToString(), TotalBalance = amount});
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }
    }
}