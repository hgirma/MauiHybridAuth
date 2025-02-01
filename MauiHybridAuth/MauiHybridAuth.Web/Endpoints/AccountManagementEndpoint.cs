using MauiHybridAuth.Shared.Models;
using MauiHybridAuth.Web.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Girmanators.Server.Endpoints;

public static class AccountManagementEndpoint
{
    public static void MapAccountManagementEndpoint(this WebApplication app)
    {
        app
            .MapPost("/register2", RegisterAsync)
            .AllowAnonymous();
    }

    private static async Task<Results<Ok<RegisterResponse>, BadRequest<RegisterResponse>>> RegisterAsync(
        RegisterRequest request,
        [FromServices] UserManager<ApplicationUser> userManager)
    {
        // simplified example
        var appUser = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(appUser);

        if (result.Succeeded)
        {
            await userManager.AddPasswordAsync(appUser, request.Password);

            return TypedResults.Ok(new RegisterResponse() { IsSuccessFull = true });
        }
        else
        {
            return TypedResults.BadRequest(new RegisterResponse
            {
                ErrorMessage = string.Join(", ", result.Errors.Select(e => e.Description))
            });
        }
    }
}
