using Blockfrost.Api.Models;
using Blockfrost.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ProjectTalon.Api;

public class TransactionsApi
{
    public static void AddEndpoints(WebApplication app)
    {
        app.MapGet("/blockfrost/transaction/{addresshash}/all", GetAllTransactions);
    }

    private static async Task<IResult> GetAllTransactions(IAddressesService addressesService,
        string address)
    {
        try
        {
            return Results.Ok((await addressesService.GetTotalAsync(address)));
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }
    }
}