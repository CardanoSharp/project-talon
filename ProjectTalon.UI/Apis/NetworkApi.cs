using System;
using System.Threading.Tasks;
using CardanoSharp.Koios.Sdk;
using CardanoSharp.Koios.Sdk.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ProjectTalon.UI.Apis;

public class NetworkApi
{
    public static void AddEndpoints(WebApplication app)
    {
        app.MapGet("/network/tip", GetTip).Produces<BlockSummary[]>();
    }
    
    private static async Task<IResult> GetTip(INetworkClient cardanoClient)
    {
        try
        {
            return Results.Ok((await cardanoClient.GetChainTip()).Content);
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }
    }
}