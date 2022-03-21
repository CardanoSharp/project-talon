using System;
using System.Threading.Tasks;
using CardanoSharp.Koios.Sdk;
using CardanoSharp.Koios.Sdk.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ProjectTalon.UI.Apis;

public class EpochsApi
{
    public static void AddEndpoints(WebApplication app)
    {
        app.MapGet("/epoch/info", GetInfo).Produces<EpochInformation[]>();
        app.MapGet("/protocol-parameters", GetProtocolParameters).Produces<ProtocolParameters[]>();
    }

    private static async Task<IResult> GetInfo(
        IEpochClient cardanoClient,
        string? epochNo = null,
        int limit = 25,
        int offset = 0)
    {
        try
        {
            return Results.Ok(await cardanoClient.GetEpochInformation(epochNo, limit, offset));
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }
    }

    private static async Task<IResult> GetProtocolParameters(
        IEpochClient cardanoClient,
        string? epochNo = null,
        int limit = 25,
        int offset = 0)
    {
        try
        {
            return Results.Ok(await cardanoClient.GetProtocolParameters(epochNo, limit, offset));
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }
    }
}