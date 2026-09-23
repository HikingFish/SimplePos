using Microsoft.AspNetCore.Mvc;
using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.Taxes.Commands.ActivateTax;
using SimplePos.Application.Taxes.Commands.CreateTax;
using SimplePos.Application.Taxes.Commands.DeactivateTax;
using SimplePos.Application.Taxes.Commands.DeleteTax;
using SimplePos.Application.Taxes.Commands.UpdateTax;
using SimplePos.Application.Taxes.Common;
using SimplePos.Application.Taxes.Queries.GetActiveTaxesByCompany;
using SimplePos.Application.Taxes.Queries.GetTaxById;
using SimplePos.Application.Taxes.Queries.GetTaxesByCompany;
using SimplePos.Domain.Common.ResultPattern;
using System.Security.Claims;

namespace SimplePos.WebApi.EndPoints;

public static class TaxEndpoints
{
    public record CreateTaxRequest(string TaxName, decimal TaxRate, Guid? CompanyId = null);
    public record UpdateTaxRequest(string TaxName, decimal TaxRate);

    public static void MapTaxEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/taxes").WithTags("Taxes");

        // 1. GET /api/taxes - Get all taxes for company
        group.MapGet("/", async (
            [FromQuery] Guid? companyId,
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            if (!TryGetCompanyId(user, companyId, out var targetCompanyId))
            {
                return Results.Unauthorized();
            }

            var query = new GetTaxesByCompanyQuery(targetCompanyId);
            var result = await dispatcher.QueryAsync<Result<List<TaxResponse>>>(query, ct);

            if (result.IsSuccess)
                return Results.Ok(result.Data);

            return ToProblemResult(result.Error);
        })
        .Produces<List<TaxResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();

        // 2. GET /api/taxes/active - Get active taxes for company
        group.MapGet("/active", async (
            [FromQuery] Guid? companyId,
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            if (!TryGetCompanyId(user, companyId, out var targetCompanyId))
            {
                return Results.Unauthorized();
            }

            var query = new GetActiveTaxesByCompanyQuery(targetCompanyId);
            var result = await dispatcher.QueryAsync<Result<List<TaxResponse>>>(query, ct);

            if (result.IsSuccess)
                return Results.Ok(result.Data);

            return ToProblemResult(result.Error);
        })
        .Produces<List<TaxResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();

        // 3. GET /api/taxes/{id:guid} - Get tax by ID
        group.MapGet("/{id:guid}", async (
            Guid id,
            [FromQuery] Guid? companyId,
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            if (!TryGetCompanyId(user, companyId, out var targetCompanyId))
            {
                return Results.Unauthorized();
            }

            var query = new GetTaxByIdQuery(id, targetCompanyId);
            var result = await dispatcher.QueryAsync<Result<TaxResponse>>(query, ct);

            if (result.IsSuccess)
                return Results.Ok(result.Data);

            return ToProblemResult(result.Error);
        })
        .Produces<TaxResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization();

        // 4. POST /api/taxes - Create tax
        group.MapPost("/", async (
            CreateTaxRequest request,
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            if (!TryGetCompanyId(user, request.CompanyId, out var targetCompanyId))
            {
                return Results.Unauthorized();
            }

            var command = new CreateTaxCommand(targetCompanyId, request.TaxName, request.TaxRate);
            var result = await dispatcher.SendAsync<CreateTaxCommand, Result<Guid>>(command, ct);

            if (result.IsSuccess)
            {
                return Results.Created($"/api/taxes/{result.Data}", new { id = result.Data });
            }

            return ToProblemResult(result.Error);
        })
        .Produces(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();

        // 5. PUT /api/taxes/{id:guid} - Update tax info (name, rate)
        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateTaxRequest request,
            [FromQuery] Guid? companyId,
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            if (!TryGetCompanyId(user, companyId, out var targetCompanyId))
            {
                return Results.Unauthorized();
            }

            var command = new UpdateTaxCommand(id, targetCompanyId, request.TaxName, request.TaxRate);
            var result = await dispatcher.SendAsync<UpdateTaxCommand, Result>(command, ct);

            if (result.IsSuccess)
                return Results.NoContent();

            return ToProblemResult(result.Error);
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization();

        // 6. PATCH /api/taxes/{id:guid}/activate - Activate tax
        group.MapPatch("/{id:guid}/activate", async (
            Guid id,
            [FromQuery] Guid? companyId,
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            if (!TryGetCompanyId(user, companyId, out var targetCompanyId))
            {
                return Results.Unauthorized();
            }

            var command = new ActivateTaxCommand(id, targetCompanyId);
            var result = await dispatcher.SendAsync<ActivateTaxCommand, Result>(command, ct);

            if (result.IsSuccess)
                return Results.NoContent();

            return ToProblemResult(result.Error);
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .RequireAuthorization();

        // 7. PATCH /api/taxes/{id:guid}/deactivate - Deactivate tax
        group.MapPatch("/{id:guid}/deactivate", async (
            Guid id,
            [FromQuery] Guid? companyId,
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            if (!TryGetCompanyId(user, companyId, out var targetCompanyId))
            {
                return Results.Unauthorized();
            }

            var command = new DeactivateTaxCommand(id, targetCompanyId);
            var result = await dispatcher.SendAsync<DeactivateTaxCommand, Result>(command, ct);

            if (result.IsSuccess)
                return Results.NoContent();

            return ToProblemResult(result.Error);
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .RequireAuthorization();

        // 8. DELETE /api/taxes/{id:guid} - Soft delete tax
        group.MapDelete("/{id:guid}", async (
            Guid id,
            [FromQuery] Guid? companyId,
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            if (!TryGetCompanyId(user, companyId, out var targetCompanyId))
            {
                return Results.Unauthorized();
            }

            var command = new DeleteTaxCommand(id, targetCompanyId);
            var result = await dispatcher.SendAsync<DeleteTaxCommand, Result>(command, ct);

            if (result.IsSuccess)
                return Results.NoContent();

            return ToProblemResult(result.Error);
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .RequireAuthorization();
    }

    private static bool TryGetCompanyId(ClaimsPrincipal user, Guid? queryCompanyId, out Guid companyId)
    {
        if (queryCompanyId.HasValue && queryCompanyId.Value != Guid.Empty)
        {
            companyId = queryCompanyId.Value;
            return true;
        }

        string? claim = user.FindFirst("CompanyId")?.Value;
        return Guid.TryParse(claim, out companyId);
    }

    private static IResult ToProblemResult(Error error)
    {
        return error.Type switch
        {
            ErrorType.NotFound => Results.NotFound(new { detail = error.Description }),
            ErrorType.Validation => Results.BadRequest(new { detail = error.Description }),
            ErrorType.Conflict => Results.Conflict(new { detail = error.Description }),
            _ => Results.Problem(detail: error.Description, statusCode: 500)
        };
    }
}
