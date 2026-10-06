using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.Auths.Commands.Login;
using SimplePos.Application.Auths.Commands.RefreshToken;
using SimplePos.Application.Auths.Commands.RevokeToken;
using SimplePos.Application.Auths.Common;
using SimplePos.Application.Auths.Queries;
using SimplePos.Domain.Common.ResultPattern;
using System.Security.Claims;

namespace SimplePos.WebApi.EndPoints
{
    public record RefreshTokenRequest(string RefreshToken);
    public record RevokeTokenRequest(string RefreshToken);

    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/auth").WithTags("Authorisation");

            group.MapPost("/login", async (LoginCommand command, ICqrsDispatcher dispatcher, CancellationToken ct) =>
            {
                var result = await dispatcher.SendAsync<LoginCommand, Result<AuthResponse>>(command, ct);
                if (result.IsSuccess)
                    return Results.Ok(new
                    {
                        token = result.Data!.AccessToken,
                        accessToken = result.Data.AccessToken,
                        refreshToken = result.Data.RefreshToken,
                        refreshTokenExpiresAtUtc = result.Data.RefreshTokenExpiresAtUtc
                    });
                if (result.Error.Type == ErrorType.Validation)
                    return Results.Problem(type: "blank", title: "Bad Request", detail: result.Error.Description, statusCode: 400);
                if (result.Error.Type == ErrorType.Unauthorized)
                    return Results.Problem(type: "blank", title: "Unauthorized", detail: result.Error.Description, statusCode: 401);
                if (result.Error.Type == ErrorType.Forbidden)
                    return Results.Problem(type: "blank", title: "Forbidden", detail: result.Error.Description, statusCode: 403);
                if (result.Error.Type == ErrorType.NotFound)
                    return Results.Problem(type: "blank", title: "Not Found", detail: result.Error.Description, statusCode: 404);
                return Results.Problem(type: "blank", title: "Error", detail: result.Error.Description, statusCode: 500);
            });

            group.MapPost("/refresh", async (RefreshTokenRequest request, ICqrsDispatcher dispatcher, CancellationToken ct) =>
            {
                var result = await dispatcher.SendAsync<RefreshTokenCommand, Result<AuthResponse>>(
                    new RefreshTokenCommand(request.RefreshToken), ct);

                if (result.IsSuccess)
                    return Results.Ok(result.Data);

                if (result.Error.Type == ErrorType.Validation)
                    return Results.Problem(type: "blank", title: "Bad Request", detail: result.Error.Description, statusCode: 400);
                if (result.Error.Type == ErrorType.Unauthorized)
                    return Results.Problem(type: "blank", title: "Unauthorized", detail: result.Error.Description, statusCode: 401);
                if (result.Error.Type == ErrorType.Forbidden)
                    return Results.Problem(type: "blank", title: "Forbidden", detail: result.Error.Description, statusCode: 403);
                if (result.Error.Type == ErrorType.NotFound)
                    return Results.Problem(type: "blank", title: "Not Found", detail: result.Error.Description, statusCode: 404);

                return Results.Problem(type: "blank", title: "Error", detail: result.Error.Description, statusCode: 500);
            });

            group.MapPost("/revoke", async (RevokeTokenRequest request, ICqrsDispatcher dispatcher, CancellationToken ct) =>
            {
                var result = await dispatcher.SendAsync<RevokeTokenCommand, Result>(
                    new RevokeTokenCommand(request.RefreshToken), ct);

                if (result.IsSuccess)
                    return Results.NoContent();

                if (result.Error.Type == ErrorType.Validation)
                    return Results.Problem(type: "blank", title: "Bad Request", detail: result.Error.Description, statusCode: 400);
                if (result.Error.Type == ErrorType.NotFound)
                    return Results.Problem(type: "blank", title: "Not Found", detail: result.Error.Description, statusCode: 404);

                return Results.Problem(type: "blank", title: "Error", detail: result.Error.Description, statusCode: 500);
            });

            group.MapPost("/me", async (ClaimsPrincipal user, ICqrsDispatcher dispatcher, CancellationToken ct) =>
            {
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Results.Problem(type: "blank", title: "Unauthorized", detail: "User ID not found", statusCode: 401);

                var result = await dispatcher.QueryAsync(new GetCurrentUserQuery(Guid.Parse(userIdClaim.Value)), ct);

                if (result.IsSuccess)
                    return Results.Ok(result.Data);

                if (result.Error.Type == ErrorType.Validation)
                    return Results.Problem(type: "blank", title: "Bad Request", detail: result.Error.Description, statusCode: 400);
                if (result.Error.Type == ErrorType.NotFound)
                    return Results.Problem(type: "blank", title: "Not Found", detail: result.Error.Description, statusCode: 404);
                if (result.Error.Type == ErrorType.Unexpected)
                    return Results.Problem(type: "blank", title: "Unexpected Error", detail: result.Error.Description, statusCode: 500);

                return Results.Problem(type: "blank", title: "Error", detail: result.Error.Description, statusCode: 500);
            })
            .RequireAuthorization();
        }
    }
}
