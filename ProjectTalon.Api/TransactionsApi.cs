using CardanoSharp.Koios.Sdk;
using CardanoSharp.Koios.Sdk.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace ProjectTalon.Api;

public class TransactionsApi
{
    public static void AddEndpoints(WebApplication app)
    {
        app.MapPost("/transactions/info", GetInfo);
        app.MapPost("/transactions/utxo", GetUtxo);
        app.MapPost("/transactions/metadata", GetMetadata);
    }

    private static async Task<IResult> GetInfo(
        ITransactionClient cardanoClient, 
        [FromBody] GetTransactionRequest request,
        int limit = 25, 
        int offset = 0)
    {
        try
        {
            var response = Array.Empty<Transaction>();
            try
            {
                response = await cardanoClient.GetTransactionInformation(request, limit, offset);
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

    private static async Task<IResult> GetUtxo(
        ITransactionClient cardanoClient, 
        [FromBody] GetTransactionRequest request,
        int limit = 25, 
        int offset = 0)
    {
        try
        {
            var response = Array.Empty<Transaction>();
            try
            {
                response = await cardanoClient.GetTransactionUtxos(request, limit, offset);
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

    private static async Task<IResult> GetMetadata(
        ITransactionClient cardanoClient, 
        [FromBody] GetTransactionRequest request,
        int limit = 25, 
        int offset = 0)
    {
        try
        {
            var response = Array.Empty<TransactionMetadata>();
            try
            {
                response = await cardanoClient.GetTransactionMetadata(request, limit, offset);
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