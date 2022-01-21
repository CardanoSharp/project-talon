using Blockfrost.Api;
using Blockfrost.Api.Models;
using Blockfrost.Api.Services;
using Blockfrost.Api.Services.Extensions;
using CardanoSharp.Koios.Sdk;
using CardanoSharp.Koios.Sdk.Contracts;

namespace ProjectTalon.Api;

public class AssetsApi
{
    public static void AddEndpoints(WebApplication app)
    {
        app.MapGet("/assets/addresses/{policyId}/{assetName}", GetAddresses);
        app.MapGet("/assets/info/{policyId}/{assetName}", GetInfo);
        app.MapGet("/assets/transaction/{policyId}/{assetName}", GetTransactions);
    }

    private static async Task<IResult> GetAddresses(
        IAssetClient cardanoClient,
        string policyId,
        string assetName,
        int limit = 25, 
        int offset = 0)
    {
        try
        {
            var response = Array.Empty<AssetAddress>();
            try
            {
                response = await cardanoClient.GetAddresses(policyId, assetName, limit, offset);
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

    private static async Task<IResult> GetInfo(
        IAssetClient cardanoClient,
        string policyId,
        string assetName,
        int limit = 25, 
        int offset = 0)
    {
        try
        {
            var response = Array.Empty<AssetInformation>();
            try
            {
                response = await cardanoClient.GetInfo(policyId, assetName, limit, offset);
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

    private static async Task<IResult> GetTransactions(
        IAssetClient cardanoClient,
        string policyId,
        string assetName,
        int limit = 25, 
        int offset = 0)
    {
        try
        {
            var response = Array.Empty<AssetTransaction>();
            try
            {
                response = await cardanoClient.GetTransactions(policyId, assetName, limit, offset);
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
}