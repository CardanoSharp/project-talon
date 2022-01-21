using CardanoSharp.Koios.Sdk;

namespace ProjectTalon.Api;

public class NetworkApi
{
    public static void AddEndpoints(WebApplication app)
    {
        app.MapGet("/network/tip", GetTip);
    }
    
    private static async Task<IResult> GetTip(INetworkClient cardanoClient)
    {
        try
        {
            return Results.Ok(await cardanoClient.GetChainTip());
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }
    }
}