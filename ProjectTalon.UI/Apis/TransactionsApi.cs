using System;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CardanoSharp.Koios.Sdk;
using CardanoSharp.Koios.Sdk.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjectTalon.Core.Common;
using ProjectTalon.Core.Data;
using ProjectTalon.Core.Data.Models;
using ProjectTalon.Core.Requests;
using ProjectTalon.Core.Services;
using Swashbuckle.AspNetCore.Swagger;

namespace ProjectTalon.UI.Apis;

public class TransactionsApi
{
    public static void AddEndpoints(WebApplication app)
    {
        app.MapPost("/transactions/info", GetInfo).Produces<Transaction[]>();
        app.MapPost("/transactions/utxo", GetUtxo).Produces<Transaction[]>();
        app.MapPost("/transactions/metadata", GetMetadata).Produces<TransactionMetadata[]>();
        app.MapPost("/transaction/submit/{appId}", SubmitTransaction).Produces<UnknownSwaggerDocument>();
        app.MapPost("/transaction/status/{appId}/{referenceId}", StatusTransaction).Produces<UnknownSwaggerDocument>();
    }

    private static async Task<IResult> StatusTransaction(
        ITransactionRequestDatabase transactionDatabase,
        string appId,
        string referenceId)
    {
        try
        {
            var transactionRequest = await transactionDatabase.GetAsync(referenceId, appId);

            return Results.Ok(new
            {
                ReferenceId = referenceId, Status = ((TransactionRequestStatus) transactionRequest.StatusId).ToString()
            });
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }
    }

    private static async Task<IResult> SubmitTransaction(
        [FromBody] TransactionSubmitRequest request,
        ITransactionRequestDatabase transactionDatabase,
        ITransactionService transactionService,
        IAddressService addressService,
        string appId)
    {
        try
        {
            var referenceId = Guid.NewGuid().ToString();
            var transactionRequest = new TransactionRequest()
            {
                AppId = appId,
                StatusId = (int) TransactionRequestStatus.Pending,
                CreatedUtc = DateTime.UtcNow,
                Parameters = JsonConvert.SerializeObject(new
                {
                    request.Outputs,
                    request.Metadata
                }),
                ReferenceId = referenceId
            };

            await transactionDatabase.SaveAsync(transactionRequest);

            return Results.Ok(new {ReferenceId = referenceId});
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }
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