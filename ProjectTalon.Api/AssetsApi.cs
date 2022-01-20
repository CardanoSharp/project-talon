using Blockfrost.Api;
using Blockfrost.Api.Models;
using Blockfrost.Api.Services;
using Blockfrost.Api.Services.Extensions;

namespace ProjectTalon.Api;

public class AssetsApi
{
    public static void AddEndpoints(WebApplication app)
    {
        app.MapGet("/blockfrost/assets", GetAssets);
        app.MapGet("/blockfrost/assets/{asset}", GetInfo);
        app.MapGet("/blockfrost/assets/{asset}/history", GetHistory);
        app.MapGet("/blockfrost/assets/{asset}/transaction", GetTransactions);
        app.MapGet("/blockfrost/assets/{asset}/addresses", GetAddresses);
        app.MapGet("/blockfrost/assets/policy/{policyId}", GetByPolicyId);
    }

    private static async Task<IResult> GetAssets(IAssetsService cardanoService, int count = 100, int page = 1, string order = "asc")
    {
        try
        {
            var orderBy = order switch
            {
                "asc" => ESortOrder.Asc,
                "desc" => ESortOrder.Desc,
                _ => ESortOrder.Asc
            };

            AssetsResponseCollection response;
            try
            {
                response = await cardanoService.GetAssetsAsync(count, page, orderBy);
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
    
    private static async Task<IResult> GetInfo(string asset, IAssetsService cardanoService)
    {
        try
        {

            AssetResponse response;
            try
            {
                response = await cardanoService.GetAssetsAsync(asset);
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
    
    private static async Task<IResult> GetHistory(string asset, IAssetsService cardanoService)
    {
        try
        {

            AssetHistoryResponseCollection response;
            try
            {
                response = await cardanoService.GetHistoryAsync(asset);
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
    
    private static async Task<IResult> GetTransactions(string asset, IAssetsService cardanoService)
    {
        try
        {

            AssetTransactionsResponseCollection response;
            try
            {
                response = await cardanoService.GetTransactionsAsync(asset);
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
    
    private static async Task<IResult> GetAddresses(string asset, IAssetsService cardanoService)
    {
        try
        {

            AssetAddressesResponseCollection response;
            try
            {
                response = await cardanoService.GetAddressesAsync(asset);
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
    
    private static async Task<IResult> GetByPolicyId(string policyId, IAssetsService cardanoService)
    {
        try
        {

            AssetPolicyResponseCollection response;
            try
            {
                response = await cardanoService.GetPolicyAsync(policyId);
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
}