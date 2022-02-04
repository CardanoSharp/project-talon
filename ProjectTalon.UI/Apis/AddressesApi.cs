using System;
using System.Linq;
using CardanoSharp.Wallet;
using CardanoSharp.Wallet.Enums;
using ProjectTalon.Core.Data;
using System.Text.Json;
using System.Threading.Tasks;
using CardanoSharp.Koios.Sdk;
using CardanoSharp.Koios.Sdk.Contracts;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Keys;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ProjectTalon.UI.Apis;

public static class AddressesApi
{
    public static void AddEndpoints(WebApplication app)
    {
        app.MapGet("/address", GetAddress);
        app.MapGet("/address/info", GetInformation);
    }

    private static async Task<IResult> GetAddress(
        IWalletKeyDatabase keyDatabase, 
        string? addressIndex = null)
    {
        try
        {
            int? index = null;
            if (int.TryParse(addressIndex, out _))
            {
                index = int.Parse(addressIndex);
            }
            
            return Results.Ok(await GetWalletAddress(keyDatabase, index));
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }
    }

    private static async Task<IResult> GetInformation(
        IWalletKeyDatabase keyDatabase,
        IAddressClient cardanoClient,
        string? address = null)
    {
        try
        {
            address ??= await GetWalletAddress(keyDatabase);

            var response = Array.Empty<AddressInformation>();
            try
            {
                if (address != null) response = await cardanoClient.GetAddressInformation(address);
            }
            catch
            {
                // ignored
            }

            return Results.Ok(response);
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }
    }

    private static async Task<string?> GetWalletAddress(IWalletKeyDatabase keyDatabase, int? addressIndex = null)
    {
        addressIndex ??= 0;
            
        var wallet = await keyDatabase.GetWalletKeysAsync(1);
        var publicKey = JsonSerializer.Deserialize<PublicKey>(wallet.First().Vkey);
        if (publicKey is null)
            throw new Exception("Wallet not found");
        var payment = publicKey
            .Derive(RoleType.ExternalChain)
            .Derive((int)addressIndex);

        var stake = publicKey
            .Derive(RoleType.Staking)
            .Derive(0);

        var address = new AddressService()
            .GetBaseAddress(payment.PublicKey, stake.PublicKey, NetworkType.Testnet);

        return address.ToString();
    }
}
