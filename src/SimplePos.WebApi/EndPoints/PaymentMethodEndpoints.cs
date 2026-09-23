using Microsoft.AspNetCore.Mvc;
using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.PaymentMethods.Commands.ActivatePaymentMethod;
using SimplePos.Application.PaymentMethods.Commands.CreatePaymentMethod;
using SimplePos.Application.PaymentMethods.Commands.DeactivatePaymentMethod;
using SimplePos.Application.PaymentMethods.Commands.DeletePaymentMethod;
using SimplePos.Application.PaymentMethods.Commands.UpdatePaymentMethod;
using SimplePos.Application.PaymentMethods.Common;
using SimplePos.Application.PaymentMethods.Queries.GetActivePaymentMethodsByCompany;
using SimplePos.Application.PaymentMethods.Queries.GetPaymentMethodById;
using SimplePos.Application.PaymentMethods.Queries.GetPaymentMethodsByCompany;
using SimplePos.Domain.Common.ResultPattern;
using System.Security.Claims;

namespace SimplePos.WebApi.EndPoints;

public static class PaymentMethodEndpoints
{
    public record CreatePaymentMethodRequest(string Name, Guid? CompanyId = null);
    public record UpdatePaymentMethodRequest(string Name);

    public static void MapPaymentMethodEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/payment-methods").WithTags("Payment Methods");

        // 1. GET /api/payment-methods - Get all payment methods for company
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

            var query = new GetPaymentMethodsByCompanyQuery(targetCompanyId);
            var result = await dispatcher.QueryAsync<Result<List<PaymentMethodResponse>>>(query, ct);

            if (result.IsSuccess)
                return Results.Ok(result.Data);

            return ToProblemResult(result.Error);
        })
        .Produces<List<PaymentMethodResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();

        // 2. GET /api/payment-methods/active - Get active payment methods for company
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

            var query = new GetActivePaymentMethodsByCompanyQuery(targetCompanyId);
            var result = await dispatcher.QueryAsync<Result<List<PaymentMethodResponse>>>(query, ct);

            if (result.IsSuccess)
                return Results.Ok(result.Data);

            return ToProblemResult(result.Error);
        })
        .Produces<List<PaymentMethodResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();

        // 3. GET /api/payment-methods/{id:guid} - Get payment method by ID
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

            var query = new GetPaymentMethodByIdQuery(id, targetCompanyId);
            var result = await dispatcher.QueryAsync<Result<PaymentMethodResponse>>(query, ct);

            if (result.IsSuccess)
                return Results.Ok(result.Data);

            return ToProblemResult(result.Error);
        })
        .Produces<PaymentMethodResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization();

        // 4. POST /api/payment-methods - Create payment method
        group.MapPost("/", async (
            CreatePaymentMethodRequest request,
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            if (!TryGetCompanyId(user, request.CompanyId, out var targetCompanyId))
            {
                return Results.Unauthorized();
            }

            var command = new CreatePaymentMethodCommand(targetCompanyId, request.Name);
            var result = await dispatcher.SendAsync<CreatePaymentMethodCommand, Result<Guid>>(command, ct);

            if (result.IsSuccess)
            {
                return Results.Created($"/api/payment-methods/{result.Data}", new { id = result.Data });
            }

            return ToProblemResult(result.Error);
        })
        .Produces(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();

        // 5. PUT /api/payment-methods/{id:guid} - Update payment method name
        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdatePaymentMethodRequest request,
            [FromQuery] Guid? companyId,
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            if (!TryGetCompanyId(user, companyId, out var targetCompanyId))
            {
                return Results.Unauthorized();
            }

            var command = new UpdatePaymentMethodCommand(id, targetCompanyId, request.Name);
            var result = await dispatcher.SendAsync<UpdatePaymentMethodCommand, Result>(command, ct);

            if (result.IsSuccess)
                return Results.NoContent();

            return ToProblemResult(result.Error);
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization();

        // 6. PATCH /api/payment-methods/{id:guid}/activate - Activate payment method
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

            var command = new ActivatePaymentMethodCommand(id, targetCompanyId);
            var result = await dispatcher.SendAsync<ActivatePaymentMethodCommand, Result>(command, ct);

            if (result.IsSuccess)
                return Results.NoContent();

            return ToProblemResult(result.Error);
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .RequireAuthorization();

        // 7. PATCH /api/payment-methods/{id:guid}/deactivate - Deactivate payment method
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

            var command = new DeactivatePaymentMethodCommand(id, targetCompanyId);
            var result = await dispatcher.SendAsync<DeactivatePaymentMethodCommand, Result>(command, ct);

            if (result.IsSuccess)
                return Results.NoContent();

            return ToProblemResult(result.Error);
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .RequireAuthorization();

        // 8. DELETE /api/payment-methods/{id:guid} - Soft delete payment method
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

            var command = new DeletePaymentMethodCommand(id, targetCompanyId);
            var result = await dispatcher.SendAsync<DeletePaymentMethodCommand, Result>(command, ct);

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
