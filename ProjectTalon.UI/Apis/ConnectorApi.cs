using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CardanoSharp.Koios.Sdk;
using CardanoSharp.Wallet;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Keys;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProjectTalon.Core.Common;
using ProjectTalon.Core.Data;
using ProjectTalon.Core.Data.Models;
using ProjectTalon.Core.Requests;

namespace ProjectTalon.UI.Apis;

public static class ConnectorApi
{
    public static void AddEndpoints(WebApplication app)
    {
        app.MapPost("/connect", Connect).AllowAnonymous();

        app.MapGet("/connect/{appId}/status", CheckConnectionStatus).AllowAnonymous();
    }

    private static async Task<IResult> Connect(
        [FromBody] ConnectRequest request,
        IAppConnectDatabase appConnectDatabase)
    {
        try
        {
            var appId = Guid.NewGuid().ToString();

            await appConnectDatabase.SaveAsync(new AppConnect
            {
                AppId = appId,
                Name = request.Name,
                ConnectionStatus = (int) ConnectionStatus.Pending
            }); ;
            return Results.Ok(new
            {
                AppId = appId,
            });
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }
    }

    private static async Task<IResult> CheckConnectionStatus(string appId, IAppConnectDatabase appConnectDatabase,
        IConfiguration configuration)
    {
        var appConnect = await appConnectDatabase.GetByAppIdAsync(appId);

        if (appConnect is null)
            return Results.NotFound();
        
        if((ConnectionStatus) appConnect.ConnectionStatus == ConnectionStatus.Approved)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, appConnect.Name),
                new Claim(ClaimTypes.SerialNumber, appConnect.AppId)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(5),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])), 
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Results.Ok(new
            {
                Status = ((ConnectionStatus) appConnect.ConnectionStatus).ToString(),
                StatusCode = appConnect.ConnectionStatus,
                JwtToken = tokenHandler.WriteToken(token),
            });
        }
        
        return Results.Ok(new
        {
            Status = ((ConnectionStatus) appConnect.ConnectionStatus).ToString(),
            StatusCode = appConnect.ConnectionStatus,
        });
    }
}