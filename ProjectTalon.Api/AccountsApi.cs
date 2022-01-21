using System.Text.Json;
using Blockfrost.Api.Services;
using CardanoSharp.Koios.Sdk;
using CardanoSharp.Koios.Sdk.Contracts;
using CardanoSharp.Wallet;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Keys;
using ProjectTalon.Core.Data;

namespace ProjectTalon.Api;

public static class AccountsApi
{
    public static void AddEndpoints(WebApplication app)
    {
        app.MapGet("/account/info", GetInfo);
        app.MapGet("/account/rewards", GetRewards);
        app.MapGet("/account/history", GetHistory);
        app.MapGet("/account/updates", GetUpdates);
        app.MapGet("/account/address", GetAddresses);
        app.MapGet("/account/assets", GetAssets);
    }

    private static async Task<IResult> GetInfo(
        IAccountClient cardanoClient, 
        IWalletKeyDatabase keyDatabase,
        string? address = null,
        int limit = 25, 
        int offset = 0)
    {
        try
        {
            address ??= await GetStakeAddress(keyDatabase);
            return Results.Ok(await cardanoClient.GetStakeInformation(address, limit, offset));
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }
    }
    
    private static async Task<IResult> GetRewards(
        IAccountClient cardanoClient, 
        IWalletKeyDatabase keyDatabase,
        string? address = null,
        string? epochNo = null,
        int limit = 25, 
        int offset = 0)
    {
        try
        {
            address ??= await GetStakeAddress(keyDatabase);
            
            var response = Array.Empty<StakeReward>();
            try
            {
                if (address != null) response = await cardanoClient.GetStakeRewards(address, epochNo, limit, offset);
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

    private static async Task<IResult> GetHistory(
        IAccountClient cardanoClient, 
        IWalletKeyDatabase keyDatabase,
        string? address = null,
        int limit = 25, 
        int offset = 0)
    {
        try
        {
            address ??= await GetStakeAddress(keyDatabase);
            
            var response = Array.Empty<StakeHistory>();
            try
            {
                if (address != null) response = await cardanoClient.GetStakeHistory(address, limit, offset);
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

    private static async Task<IResult> GetUpdates(
        IAccountClient cardanoClient, 
        IWalletKeyDatabase keyDatabase,
        string? address = null,
        int limit = 25, 
        int offset = 0)
    {
        try
        {
            address ??= await GetStakeAddress(keyDatabase);
            
            var response = Array.Empty<StakeUpdate>();
            try
            {
                if (address != null) response = await cardanoClient.GetStakeUpdates(address, limit, offset);
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

    private static async Task<IResult> GetAddresses(
        IAccountClient cardanoClient, 
        IWalletKeyDatabase keyDatabase,
        string? address = null,
        int limit = 25, 
        int offset = 0)
    {
        try
        {
            address ??= await GetStakeAddress(keyDatabase);
            
            var response = Array.Empty<StakeAddress>();
            try
            {
                if (address != null) response = await cardanoClient.GetStakeAddresses(address, limit, offset);
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

    private static async Task<IResult> GetAssets(
        IAccountClient cardanoClient, 
        IWalletKeyDatabase keyDatabase,
        string? address = null,
        int limit = 25, 
        int offset = 0)
    {
        try
        {
            address ??= await GetStakeAddress(keyDatabase);
            
            var response = Array.Empty<StakeAsset>();
            try
            {
                if (address != null) response = await cardanoClient.GetStakeAssets(address, limit, offset);
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

    private static async Task<string?> GetStakeAddress(IWalletKeyDatabase keyDatabase)
    {
        var wallet = await keyDatabase.GetWalletKeysAsync(1);
        var publicKey = JsonSerializer.Deserialize<PublicKey>(wallet.First().Vkey);
        if (publicKey is null)
            throw new Exception("Wallet not found");

        var stake = publicKey
            .Derive(RoleType.Staking)
            .Derive(0);

        var address = new AddressService()
            .GetRewardAddress(stake.PublicKey, NetworkType.Testnet);

        return address.ToString();
    }
}