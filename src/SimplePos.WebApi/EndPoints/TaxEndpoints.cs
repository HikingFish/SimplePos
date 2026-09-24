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
    public record CreateTaxRequest(string TaxName, decimal TaxRate);
    public record UpdateTaxRequest(string TaxName, decimal TaxRate);

    public static void MapTaxEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/taxes").WithTags("Taxes");

        // 1. GET /api/taxes - Get all taxes for company from token
        group.MapGet("/", async (
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            string? companyIdClaim = user.FindFirst("CompanyId")?.Value;
            if (!Guid.TryParse(companyIdClaim, out var companyId))
            {
                return Results.Unauthorized();
            }

            var query = new GetTaxesByCompanyQuery(companyId);
            var result = await dispatcher.QueryAsync<Result<List<TaxResponse>>>(query, ct);

            if (result.IsSuccess)
                return Results.Ok(result.Data);

            if (result.Error.Type == ErrorType.NotFound)
                return Results.NotFound(new { detail = result.Error.Description });
            if (result.Error.Type == ErrorType.Validation)
                return Results.BadRequest(new { detail = result.Error.Description });

            return Results.Problem(detail: result.Error.Description, statusCode: 500);
        })
        .Produces<List<TaxResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();

        // 2. GET /api/taxes/active - Get active taxes for company from token
        group.MapGet("/active", async (
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            string? companyIdClaim = user.FindFirst("CompanyId")?.Value;
            if (!Guid.TryParse(companyIdClaim, out var companyId))
            {
                return Results.Unauthorized();
            }

            var query = new GetActiveTaxesByCompanyQuery(companyId);
            var result = await dispatcher.QueryAsync<Result<List<TaxResponse>>>(query, ct);

            if (result.IsSuccess)
                return Results.Ok(result.Data);

            if (result.Error.Type == ErrorType.NotFound)
                return Results.NotFound(new { detail = result.Error.Description });
            if (result.Error.Type == ErrorType.Validation)
                return Results.BadRequest(new { detail = result.Error.Description });

            return Results.Problem(detail: result.Error.Description, statusCode: 500);
        })
        .Produces<List<TaxResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();

        // 3. GET /api/taxes/{id:guid} - Get tax by ID (scoped to company in token)
        group.MapGet("/{id:guid}", async (
            Guid id,
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            string? companyIdClaim = user.FindFirst("CompanyId")?.Value;
            if (!Guid.TryParse(companyIdClaim, out var companyId))
            {
                return Results.Unauthorized();
            }

            var query = new GetTaxByIdQuery(id, companyId);
            var result = await dispatcher.QueryAsync<Result<TaxResponse>>(query, ct);

            if (result.IsSuccess)
                return Results.Ok(result.Data);

            if (result.Error.Type == ErrorType.NotFound)
                return Results.NotFound(new { detail = result.Error.Description });

            return Results.Problem(detail: result.Error.Description, statusCode: 500);
        })
        .Produces<TaxResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization();

        // 4. POST /api/taxes - Create tax (scoped to company in token)
        group.MapPost("/", async (
            CreateTaxRequest request,
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            string? companyIdClaim = user.FindFirst("CompanyId")?.Value;
            if (!Guid.TryParse(companyIdClaim, out var companyId))
            {
                return Results.Unauthorized();
            }

            var command = new CreateTaxCommand(companyId, request.TaxName, request.TaxRate);
            var result = await dispatcher.SendAsync<CreateTaxCommand, Result<Guid>>(command, ct);

            if (result.IsSuccess)
            {
                return Results.Created($"/api/taxes/{result.Data}", new { id = result.Data });
            }

            if (result.Error.Type == ErrorType.Validation)
                return Results.BadRequest(new { detail = result.Error.Description });
            if (result.Error.Type == ErrorType.Conflict)
                return Results.Conflict(new { detail = result.Error.Description });

            return Results.Problem(detail: result.Error.Description, statusCode: 500);
        })
        .Produces(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();

        // 5. PUT /api/taxes/{id:guid} - Update tax info (name, rate)
        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateTaxRequest request,
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            string? companyIdClaim = user.FindFirst("CompanyId")?.Value;
            if (!Guid.TryParse(companyIdClaim, out var companyId))
            {
                return Results.Unauthorized();
            }

            var command = new UpdateTaxCommand(id, companyId, request.TaxName, request.TaxRate);
            var result = await dispatcher.SendAsync<UpdateTaxCommand, Result>(command, ct);

            if (result.IsSuccess)
                return Results.NoContent();

            if (result.Error.Type == ErrorType.NotFound)
                return Results.NotFound(new { detail = result.Error.Description });
            if (result.Error.Type == ErrorType.Validation)
                return Results.BadRequest(new { detail = result.Error.Description });
            if (result.Error.Type == ErrorType.Conflict)
                return Results.Conflict(new { detail = result.Error.Description });

            return Results.Problem(detail: result.Error.Description, statusCode: 500);
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization();

        // 6. PATCH /api/taxes/{id:guid}/activate - Activate tax
        group.MapPatch("/{id:guid}/activate", async (
            Guid id,
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            string? companyIdClaim = user.FindFirst("CompanyId")?.Value;
            if (!Guid.TryParse(companyIdClaim, out var companyId))
            {
                return Results.Unauthorized();
            }

            var command = new ActivateTaxCommand(id, companyId);
            var result = await dispatcher.SendAsync<ActivateTaxCommand, Result>(command, ct);

            if (result.IsSuccess)
                return Results.NoContent();

            if (result.Error.Type == ErrorType.NotFound)
                return Results.NotFound(new { detail = result.Error.Description });
            if (result.Error.Type == ErrorType.Conflict)
                return Results.Conflict(new { detail = result.Error.Description });

            return Results.Problem(detail: result.Error.Description, statusCode: 500);
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .RequireAuthorization();

        // 7. PATCH /api/taxes/{id:guid}/deactivate - Deactivate tax
        group.MapPatch("/{id:guid}/deactivate", async (
            Guid id,
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            string? companyIdClaim = user.FindFirst("CompanyId")?.Value;
            if (!Guid.TryParse(companyIdClaim, out var companyId))
            {
                return Results.Unauthorized();
            }

            var command = new DeactivateTaxCommand(id, companyId);
            var result = await dispatcher.SendAsync<DeactivateTaxCommand, Result>(command, ct);

            if (result.IsSuccess)
                return Results.NoContent();

            if (result.Error.Type == ErrorType.NotFound)
                return Results.NotFound(new { detail = result.Error.Description });
            if (result.Error.Type == ErrorType.Conflict)
                return Results.Conflict(new { detail = result.Error.Description });

            return Results.Problem(detail: result.Error.Description, statusCode: 500);
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .RequireAuthorization();

        // 8. DELETE /api/taxes/{id:guid} - Soft delete tax
        group.MapDelete("/{id:guid}", async (
            Guid id,
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            string? companyIdClaim = user.FindFirst("CompanyId")?.Value;
            if (!Guid.TryParse(companyIdClaim, out var companyId))
            {
                return Results.Unauthorized();
            }

            var command = new DeleteTaxCommand(id, companyId);
            var result = await dispatcher.SendAsync<DeleteTaxCommand, Result>(command, ct);

            if (result.IsSuccess)
                return Results.NoContent();

            if (result.Error.Type == ErrorType.NotFound)
                return Results.NotFound(new { detail = result.Error.Description });
            if (result.Error.Type == ErrorType.Conflict)
                return Results.Conflict(new { detail = result.Error.Description });

            return Results.Problem(detail: result.Error.Description, statusCode: 500);
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .RequireAuthorization();
    }
}
