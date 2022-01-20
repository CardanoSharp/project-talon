using Blockfrost.Api.Services;
using CardanoSharp.Wallet;
using CardanoSharp.Wallet.Enums;
using ProjectTalon.Core.Data;
using System.Text.Json;
using Blockfrost.Api.Models;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Keys;

namespace ProjectTalon.Api;

public static class AddressesApi
{
    public static void AddEndpoints(WebApplication app)
    {
        app.MapGet("/blockfrost/address/utxos", GetUtxos);
        app.MapGet("/blockfrost/address/info", GetInformation);
    }

    private static async Task<IResult> GetUtxos(
        IWalletDatabase walletdatabase,
        IWalletKeyDatabase keyDatabase,
        IAddressesService cardanoService)
    {
        try
        {
            var wallet = await keyDatabase.GetWalletKeysAsync(1);
            var publicKey = JsonSerializer.Deserialize<PublicKey>(wallet.First().Vkey);
            if (publicKey is null)
                throw new Exception("Wallet not found");

            var address = await GetWalletAddress(publicKey);
            
            AddressUtxoContentResponseCollection response;
            try
            {
                response = await cardanoService.GetUtxosAsync(address);
            }
            catch
            {
                response = null;
            }

            return Results.Ok(response);
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }
    }

    private static async Task<IResult> GetInformation(
        IWalletDatabase walletdatabase,
        IWalletKeyDatabase keyDatabase,
        IAddressesService cardanoService)
    {
        try
        {
            var wallet = await keyDatabase.GetWalletKeysAsync(1);
            var publicKey = JsonSerializer.Deserialize<PublicKey>(wallet.First().Vkey);
            if (publicKey is null)
                throw new Exception("Wallet not found");

            var address = await GetWalletAddress(publicKey);
            
            AddressContentResponse response;
            try
            {
                response = await cardanoService.GetAddressesAsync(address);
            }
            catch
            {
                response = null;
            }

            return Results.Ok(response);
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }
    }

    private static async Task<string> GetWalletAddress(PublicKey publicKey)
    {
        var payment = publicKey
            .Derive(RoleType.ExternalChain)
            .Derive(0);

        var stake = publicKey
            .Derive(RoleType.Staking)
            .Derive(0);

        var baseAddr = new AddressService()
            .GetAddress(payment.PublicKey, stake.PublicKey, NetworkType.Testnet, AddressType.Base);

        return baseAddr.ToString();
    }
}