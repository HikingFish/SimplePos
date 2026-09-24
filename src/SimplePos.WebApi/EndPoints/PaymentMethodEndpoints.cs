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
    public record CreatePaymentMethodRequest(string Name);
    public record UpdatePaymentMethodRequest(string Name);

    public static void MapPaymentMethodEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/payment-methods").WithTags("Payment Methods");

        // 1. GET /api/payment-methods - Get all payment methods for company from token
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

            var query = new GetPaymentMethodsByCompanyQuery(companyId);
            var result = await dispatcher.QueryAsync<Result<List<PaymentMethodResponse>>>(query, ct);

            if (result.IsSuccess)
                return Results.Ok(result.Data);

            if (result.Error.Type == ErrorType.NotFound)
                return Results.NotFound(new { detail = result.Error.Description });
            if (result.Error.Type == ErrorType.Validation)
                return Results.BadRequest(new { detail = result.Error.Description });

            return Results.Problem(detail: result.Error.Description, statusCode: 500);
        })
        .Produces<List<PaymentMethodResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();

        // 2. GET /api/payment-methods/active - Get active payment methods for company from token
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

            var query = new GetActivePaymentMethodsByCompanyQuery(companyId);
            var result = await dispatcher.QueryAsync<Result<List<PaymentMethodResponse>>>(query, ct);

            if (result.IsSuccess)
                return Results.Ok(result.Data);

            if (result.Error.Type == ErrorType.NotFound)
                return Results.NotFound(new { detail = result.Error.Description });
            if (result.Error.Type == ErrorType.Validation)
                return Results.BadRequest(new { detail = result.Error.Description });

            return Results.Problem(detail: result.Error.Description, statusCode: 500);
        })
        .Produces<List<PaymentMethodResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();

        // 3. GET /api/payment-methods/{id:guid} - Get payment method by ID (scoped to company in token)
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

            var query = new GetPaymentMethodByIdQuery(id, companyId);
            var result = await dispatcher.QueryAsync<Result<PaymentMethodResponse>>(query, ct);

            if (result.IsSuccess)
                return Results.Ok(result.Data);

            if (result.Error.Type == ErrorType.NotFound)
                return Results.NotFound(new { detail = result.Error.Description });

            return Results.Problem(detail: result.Error.Description, statusCode: 500);
        })
        .Produces<PaymentMethodResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization();

        // 4. POST /api/payment-methods - Create payment method (scoped to company in token)
        group.MapPost("/", async (
            CreatePaymentMethodRequest request,
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            string? companyIdClaim = user.FindFirst("CompanyId")?.Value;
            if (!Guid.TryParse(companyIdClaim, out var companyId))
            {
                return Results.Unauthorized();
            }

            var command = new CreatePaymentMethodCommand(companyId, request.Name);
            var result = await dispatcher.SendAsync<CreatePaymentMethodCommand, Result<Guid>>(command, ct);

            if (result.IsSuccess)
            {
                return Results.Created($"/api/payment-methods/{result.Data}", new { id = result.Data });
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

        // 5. PUT /api/payment-methods/{id:guid} - Update payment method name (scoped to company in token)
        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdatePaymentMethodRequest request,
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            string? companyIdClaim = user.FindFirst("CompanyId")?.Value;
            if (!Guid.TryParse(companyIdClaim, out var companyId))
            {
                return Results.Unauthorized();
            }

            var command = new UpdatePaymentMethodCommand(id, companyId, request.Name);
            var result = await dispatcher.SendAsync<UpdatePaymentMethodCommand, Result>(command, ct);

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

        // 6. PATCH /api/payment-methods/{id:guid}/activate - Activate payment method (scoped to company in token)
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

            var command = new ActivatePaymentMethodCommand(id, companyId);
            var result = await dispatcher.SendAsync<ActivatePaymentMethodCommand, Result>(command, ct);

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

        // 7. PATCH /api/payment-methods/{id:guid}/deactivate - Deactivate payment method (scoped to company in token)
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

            var command = new DeactivatePaymentMethodCommand(id, companyId);
            var result = await dispatcher.SendAsync<DeactivatePaymentMethodCommand, Result>(command, ct);

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

        // 8. DELETE /api/payment-methods/{id:guid} - Soft delete payment method (scoped to company in token)
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

            var command = new DeletePaymentMethodCommand(id, companyId);
            var result = await dispatcher.SendAsync<DeletePaymentMethodCommand, Result>(command, ct);

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
