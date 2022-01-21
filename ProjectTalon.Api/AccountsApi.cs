using System.Text.Json;
using Blockfrost.Api.Services;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Keys;
using ProjectTalon.Core.Data;

namespace ProjectTalon.Api;

public static class AccountsApi
{
    public static void AddEndpoints(WebApplication app)
    {
        app.MapGet("/blockfrost/account/rewards", GetRewards);
        app.MapGet("/blockfrost/account/history", GetHistory);
        app.MapGet("/blockfrost/account/delegations", GetDelegations);
        app.MapGet("/blockfrost/account/registrations", GetRegistrations);
        app.MapGet("/blockfrost/account/withdrawals", GetWithdrawls);
    }

    private static async Task<IResult> GetRewards(IAccountsService accountsService, IWalletKeyDatabase keyDatabase,
        string? stakeaddress = null)
    {
        try
        {
            if (stakeaddress is not null) return Results.Ok(await accountsService.GetRewardsAsync(stakeaddress));

            var wallet = await keyDatabase.GetWalletKeysAsync(1);
            var publicKey = JsonSerializer.Deserialize<PublicKey>(wallet.First().Vkey);
            var address = GetStakeAddress(publicKey);
            return Results.Ok(await accountsService.GetRewardsAsync(address));
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }
    }

    private static async Task<IResult> GetHistory(IAccountsService accountsService, IWalletKeyDatabase keyDatabase,
        string? stakeaddress = null)
    {
        try
        {
            if (stakeaddress is not null) return Results.Ok(await accountsService.GetHistoryAsync(stakeaddress));

            var wallet = await keyDatabase.GetWalletKeysAsync(1);
            var publicKey = JsonSerializer.Deserialize<PublicKey>(wallet.First().Vkey);
            var address = GetStakeAddress(publicKey);
            return Results.Ok(await accountsService.GetHistoryAsync(address));
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }
    }

    private static async Task<IResult> GetDelegations(IAccountsService accountsService, IWalletKeyDatabase keyDatabase,
        string? stakeaddress = null)
    {
        try
        {
            if (stakeaddress is not null) return Results.Ok(await accountsService.GetDelegationsAsync(stakeaddress));

            var wallet = await keyDatabase.GetWalletKeysAsync(1);
            var publicKey = JsonSerializer.Deserialize<PublicKey>(wallet.First().Vkey);
            var address = GetStakeAddress(publicKey);
            return Results.Ok(await accountsService.GetDelegationsAsync(address));
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }
    }

    private static async Task<IResult> GetRegistrations(IAccountsService accountsService,
        IWalletKeyDatabase keyDatabase,
        string? stakeaddress = null)
    {
        try
        {
            if (stakeaddress is not null) return Results.Ok(await accountsService.GetRegistrationsAsync(stakeaddress));

            var wallet = await keyDatabase.GetWalletKeysAsync(1);
            var publicKey = JsonSerializer.Deserialize<PublicKey>(wallet.First().Vkey);
            var address = GetStakeAddress(publicKey);
            return Results.Ok(await accountsService.GetRegistrationsAsync(address));
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }
    }
    
    private static async Task<IResult> GetWithdrawls(IAccountsService accountsService,
        IWalletKeyDatabase keyDatabase,
        string? stakeaddress = null)
    {
        try
        {
            if (stakeaddress is not null) return Results.Ok(await accountsService.GetWithdrawalsAsync(stakeaddress));

            var wallet = await keyDatabase.GetWalletKeysAsync(1);
            var publicKey = JsonSerializer.Deserialize<PublicKey>(wallet.First().Vkey);
            var address = GetStakeAddress(publicKey);
            return Results.Ok(await accountsService.GetWithdrawalsAsync(address));
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }
    }

    private static string? GetStakeAddress(PublicKey publicKey)
    {
        var payment = publicKey
            .Derive(RoleType.ExternalChain)
            .Derive(0);

        return publicKey
            .Derive(RoleType.Staking)
            .Derive(0).ToString();
    }
}