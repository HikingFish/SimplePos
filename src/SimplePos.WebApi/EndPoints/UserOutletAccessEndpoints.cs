using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.Users.Commands.GrantOutletAccess;
using SimplePos.Application.Users.Commands.RevokeOutletAccess;
using SimplePos.Application.Users.Commands.SetOutletAccess;
using SimplePos.Application.Users.Queries.GetOutletAccess;
using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.WebApi.EndPoints;

public record GrantUserOutletAccessRequest(Guid OutletId);
public record SetUserOutletAccessesRequest(List<Guid> OutletIds);

public static class UserOutletAccessEndpoints
{
    public static void MapUserOutletAccessEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users/{userId:guid}/outlet-access")
                       .WithTags("User Outlet Access");

        // 1. GET /api/users/{userId}/outlet-access
        group.MapGet("/", async (Guid userId, ICqrsDispatcher dispatcher, CancellationToken ct) =>
        {
            var result = await dispatcher.QueryAsync<Result<List<UserOutletAccessResponse>>>(
                new GetUserOutletAccessesQuery(userId), ct);

            if (result.IsSuccess)
                return Results.Ok(result.Data);

            if (result.Error.Type == ErrorType.NotFound)
                return Results.NotFound(new { detail = result.Error.Description });

            return Results.Problem(detail: result.Error.Description, statusCode: 500);
        })
        .RequireAuthorization("Admin")
        .Produces<List<UserOutletAccessResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);

        // 2. POST /api/users/{userId}/outlet-access
        group.MapPost("/", async (
            Guid userId, 
            GrantUserOutletAccessRequest request, 
            ICqrsDispatcher dispatcher, 
            CancellationToken ct) =>
        {
            var result = await dispatcher.SendAsync<GrantUserOutletAccessCommand, Result>(
                new GrantUserOutletAccessCommand(userId, request.OutletId), ct);

            if (result.IsSuccess)
                return Results.Created($"/api/users/{userId}/outlet-access", null);

            if (result.Error.Type == ErrorType.Validation)
                return Results.BadRequest(new { detail = result.Error.Description });
            if (result.Error.Type == ErrorType.NotFound)
                return Results.NotFound(new { detail = result.Error.Description });
            if (result.Error.Type == ErrorType.Conflict)
                return Results.Conflict(new { detail = result.Error.Description });

            return Results.Problem(detail: result.Error.Description, statusCode: 500);
        })
        .RequireAuthorization("Admin")
        .Produces(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);

        // 3. PUT /api/users/{userId}/outlet-access
        group.MapPut("/", async (
            Guid userId, 
            SetUserOutletAccessesRequest request, 
            ICqrsDispatcher dispatcher, 
            CancellationToken ct) =>
        {
            var result = await dispatcher.SendAsync<SetUserOutletAccessesCommand, Result>(
                new SetUserOutletAccessesCommand(userId, request.OutletIds), ct);

            if (result.IsSuccess)
                return Results.NoContent();

            if (result.Error.Type == ErrorType.Validation)
                return Results.BadRequest(new { detail = result.Error.Description });
            if (result.Error.Type == ErrorType.NotFound)
                return Results.NotFound(new { detail = result.Error.Description });

            return Results.Problem(detail: result.Error.Description, statusCode: 500);
        })
        .RequireAuthorization("Admin")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);

        // 4. DELETE /api/users/{userId}/outlet-access/{outletId}
        group.MapDelete("/{outletId:guid}", async (
            Guid userId, 
            Guid outletId, 
            ICqrsDispatcher dispatcher, 
            CancellationToken ct) =>
        {
            var result = await dispatcher.SendAsync<RevokeUserOutletAccessCommand, Result>(
                new RevokeUserOutletAccessCommand(userId, outletId), ct);

            if (result.IsSuccess)
                return Results.NoContent();

            if (result.Error.Type == ErrorType.NotFound)
                return Results.NotFound(new { detail = result.Error.Description });

            return Results.Problem(detail: result.Error.Description, statusCode: 500);
        })
        .RequireAuthorization("Admin")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
