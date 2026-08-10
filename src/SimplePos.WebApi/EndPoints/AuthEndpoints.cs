using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.Auths.Commands.Login;
using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.WebApi.EndPoints
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/auth").WithTags("Authorisation");

            group.MapPost("/login", async (LoginCommand command, ICqrsDispatcher dispatcher, CancellationToken ct) =>
            {
                var result = await dispatcher.SendAsync<LoginCommand, Result<string>>(command, ct);
                if (result.IsSuccess)
                    return Results.Ok(new { token = result.Data });
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
        }
    }
}
