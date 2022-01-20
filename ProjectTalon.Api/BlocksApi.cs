using Blockfrost.Api;
using Blockfrost.Api.Models;
using Blockfrost.Api.Services;
using Polly.Caching;

namespace ProjectTalon.Api;

public class BlocksApi
{
    public static void AddEndpoints(WebApplication app)
    {
        app.MapGet("/blockfrost/blocks/latest", GetLatest);
        app.MapGet("/blockfrost/blocks/latest/txs", GetLatestTransactions);
        app.MapGet("/blockfrost/blocks/{blockHash}", GetInfo);
        app.MapGet("/blockfrost/blocks/{blockHash}/next", GetNext);
        app.MapGet("/blockfrost/blocks/{blockHash}/previous", GetPrevious);
        app.MapGet("/blockfrost/blocks/{blockHash}/txs", GetTransactions);
        app.MapGet("/blockfrost/blocks/slot/{slotNumber}", GetBySlot);
        app.MapGet("/blockfrost/blocks/epoch/{epochNumber}/slot/{slotNumber}", GetByEpochAndSlot);
    }

    private static async Task<IResult> GetLatest(IBlocksService cardanoService)
    {
        try
        {
            BlockContentResponse response;
            try
            {
                response = await cardanoService.GetLatestAsync();
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

    private static async Task<IResult> GetLatestTransactions(IBlocksService cardanoService)
    {
        try
        {
            StringCollection response;
            try
            {
                response = await cardanoService.GetLatestTxsAsync();
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

    private static async Task<IResult> GetInfo(string blockHash, IBlocksService cardanoService)
    {
        try
        {
            BlockContentResponse response;
            try
            {
                response = await cardanoService.GetBlocksAsync(blockHash);
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

    private static async Task<IResult> GetNext(string blockHash, IBlocksService cardanoService, int count = 100, int page = 1)
    {
        try
        {
            BlockContentResponseCollection response;
            try
            {
                response = await cardanoService.GetNextAsync(blockHash, count, page);
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

    private static async Task<IResult> GetPrevious(string blockHash, IBlocksService cardanoService, int count = 100, int page = 1)
    {
        try
        {
            BlockContentResponseCollection response;
            try
            {
                response = await cardanoService.GetPreviousAsync(blockHash, count, page);
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

    private static async Task<IResult> GetTransactions(string blockHash, IBlocksService cardanoService, int count = 100, int page = 1, string order = "asc")
    {
        try
        {
            var orderBy = order switch
            {
                "asc" => ESortOrder.Asc,
                "desc" => ESortOrder.Desc,
                _ => ESortOrder.Asc
            };
            
            StringCollection response;
            try
            {
                response = await cardanoService.GetTxsAsync(blockHash, count, page, orderBy);
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

    private static async Task<IResult> GetBySlot(int slotNumber, IBlocksService cardanoService)
    {
        try
        {
            BlockContentResponse response;
            try
            {
                response = await cardanoService.GetSlotAsync(slotNumber);
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

    private static async Task<IResult> GetByEpochAndSlot(int epochNumber, int slotNumber, IBlocksService cardanoService)
    {
        try
        {
            BlockContentResponse response;
            try
            {
                response = await cardanoService.GetEpochSlotAsync(epochNumber, slotNumber);
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