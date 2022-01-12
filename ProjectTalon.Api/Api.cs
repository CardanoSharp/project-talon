using System.Text.Json;
using Blockfrost.Api.Services;
using CardanoSharp.Wallet;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Keys;
using ProjectTalon.Core.Data;

namespace ProjectTalon.Api;

public static class Api
{
    public static void ConfigureApi(this WebApplication app)
    {
        app.MapGet("/mnemonic/{size}", GenerateMnemonic);

        app.MapGet("/wallet/{id}/balance", GetWalletBalance);
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