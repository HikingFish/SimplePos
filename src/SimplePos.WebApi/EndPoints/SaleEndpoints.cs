using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.Sales.Commands.AddSaleItem;
using SimplePos.Application.Sales.Commands.AddSalePayment;
using SimplePos.Application.Sales.Commands.CloseSale;
using SimplePos.Application.Sales.Commands.CreateSale;
using SimplePos.Application.Sales.Commands.DeleteSale;
using SimplePos.Application.Sales.Commands.RemoveSaleItem;
using SimplePos.Application.Sales.Commands.RemoveSalePayment;
using SimplePos.Application.Sales.Commands.UnvoidSale;
using SimplePos.Application.Sales.Commands.UnvoidSaleItem;
using SimplePos.Application.Sales.Commands.UpdateSaleItem;
using SimplePos.Application.Sales.Commands.UpdateSaleItemQuantity;
using SimplePos.Application.Sales.Commands.VoidSale;
using SimplePos.Application.Sales.Commands.VoidSaleItem;
using SimplePos.Application.Sales.Common;
using SimplePos.Application.Sales.Queries.GetSaleById;
using SimplePos.Application.Sales.Queries.GetSalePayments;
using SimplePos.Application.Sales.Queries.GetSales;
using SimplePos.Domain.Common.ResultPattern;
using System.Security.Claims;

namespace SimplePos.WebApi.EndPoints;

public static class SaleEndpoints
{
    public record CreateSaleRequest(Guid? OutletId, DateTime financialDate, List<CreateSaleItemRequest> SaleItems, List<CreateSalePaymentRequest>? SalePayments);
    public record CreateSaleItemRequest(Guid productId, int quantity, string? remarks, decimal? discount, decimal? price, DateTime financialDate);
    public record CreateSalePaymentRequest(Guid paymentMethod, decimal amountPaid, DateTime paymentDate, string referenceNumber);

    public record AddSaleItemRequest(Guid ProductId, decimal Quantity, decimal? Price, decimal? Discount, string? Remarks, DateTime? FinancialDate);
    public record UpdateSaleItemRequest(decimal Quantity, decimal UnitPrice, decimal UnitDiscount, string? Remark, List<Guid>? TaxIds);
    public record UpdateSaleItemQuantityRequest(decimal Quantity);

    public record AddSalePaymentRequest(Guid PaymentMethodId, decimal AmountPaid, DateTime? PaymentDate, string? ReferenceNumber);

    public static void MapSaleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/sales").WithTags("Sales");

        // 1. POST /api/sales/ - Create a new sale
        group.MapPost("/", async (
            CreateSaleRequest request,
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            Guid userGuid = GetUserId(user);
            Guid outletId;

            if (request.OutletId.HasValue)
            {
                outletId = request.OutletId.Value;
            }
            else
            {
                string? outletIdClaim = user.FindFirstValue("OutletId");
                outletId = Guid.TryParse(outletIdClaim, out var claimOutlet) ? claimOutlet : Guid.Empty;
            }

            Guid companyId = GetCompanyId(user);

            List<CreateSalePayment>? createPaymentRequest = request.SalePayments?
                .Select(sp => new CreateSalePayment(sp.paymentMethod, sp.amountPaid, sp.paymentDate, sp.referenceNumber))
                .ToList();

            List<CreateSaleItem> createSaleItemRequest = request.SaleItems
                .Select(si => new CreateSaleItem(si.productId, si.quantity, si.remarks, si.discount, si.price, si.financialDate))
                .ToList();

            CreateSaleCommand command = new CreateSaleCommand(userGuid, outletId, companyId, request.financialDate, createSaleItemRequest, createPaymentRequest);

            var result = await dispatcher.SendAsync<CreateSaleCommand, Result<Guid>>(command, ct);

            if (result.IsSuccess)
                return Results.Created($"/api/sales/{result.Data}", new { id = result.Data });

            if (result.Error.Type == ErrorType.NotFound)
                return Results.NotFound(new { detail = result.Error.Description });
            if (result.Error.Type == ErrorType.Validation)
                return Results.BadRequest(new { detail = result.Error.Description });
            if (result.Error.Type == ErrorType.Conflict)
                return Results.Conflict(new { detail = result.Error.Description });

            return Results.Problem(detail: result.Error.Description, statusCode: 500);
        })
        .Produces(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization();

        // 2. GET /api/sales/{id:guid} - Get sale by ID
        group.MapGet("/{id:guid}", async (
            Guid id,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            var query = new GetSaleByIdQuery(id);
            var result = await dispatcher.QueryAsync<Result<SaleResponse>>(query, ct);

            if (result.IsSuccess)
                return Results.Ok(result.Data);

            if (result.Error.Type == ErrorType.NotFound)
                return Results.NotFound(new { detail = result.Error.Description });

            return Results.Problem(detail: result.Error.Description, statusCode: 500);
        })
        .Produces<SaleResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization();

        // 3. GET /api/sales - Get sales (filtered by outletId, from/to date range, or userId)
        group.MapGet("/", async (
            string? outletId,
            string? from,
            string? to,
            string? userId,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            Guid? parsedOutletId = null;
            if (!string.IsNullOrWhiteSpace(outletId))
            {
                if (!Guid.TryParse(outletId, out var oId))
                    return Results.BadRequest(new { detail = "Invalid outletId format." });
                parsedOutletId = oId;
            }

            DateTime? parsedFrom = null;
            if (!string.IsNullOrWhiteSpace(from))
            {
                if (!DateTime.TryParse(from, out var fDate))
                    return Results.BadRequest(new { detail = "Invalid from date format." });
                parsedFrom = fDate;
            }

            DateTime? parsedTo = null;
            if (!string.IsNullOrWhiteSpace(to))
            {
                if (!DateTime.TryParse(to, out var tDate))
                    return Results.BadRequest(new { detail = "Invalid to date format." });
                parsedTo = tDate;
            }

            Guid? parsedUserId = null;
            if (!string.IsNullOrWhiteSpace(userId))
            {
                if (!Guid.TryParse(userId, out var uId))
                    return Results.BadRequest(new { detail = "Invalid userId format." });
                parsedUserId = uId;
            }

            var query = new GetSalesQuery(parsedOutletId, parsedFrom, parsedTo, parsedUserId);
            var result = await dispatcher.QueryAsync<Result<List<SaleResponse>>>(query, ct);

            if (result.IsSuccess)
                return Results.Ok(result.Data);

            if (result.Error.Type == ErrorType.NotFound)
                return Results.NotFound(new { detail = result.Error.Description });
            if (result.Error.Type == ErrorType.Validation)
                return Results.BadRequest(new { detail = result.Error.Description });

            return Results.Problem(detail: result.Error.Description, statusCode: 500);
        })
        .Produces<List<SaleResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();

        // 4. PATCH /api/sales/{id:guid}/void - Void a sale
        group.MapPatch("/{id:guid}/void", async (
            Guid id,
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            Guid userGuid = GetUserId(user);
            var command = new VoidSaleCommand(id, userGuid);
            var result = await dispatcher.SendAsync<VoidSaleCommand, Result>(command, ct);

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
        .ProducesProblem(StatusCodes.Status409Conflict)
        .RequireAuthorization();

        // 5. PATCH /api/sales/{id:guid}/unvoid - Unvoid a sale
        group.MapPatch("/{id:guid}/unvoid", async (
            Guid id,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            var command = new UnvoidSaleCommand(id);
            var result = await dispatcher.SendAsync<UnvoidSaleCommand, Result>(command, ct);

            if (result.IsSuccess)
                return Results.NoContent();

            if (result.Error.Type == ErrorType.NotFound)
                return Results.NotFound(new { detail = result.Error.Description });
            if (result.Error.Type == ErrorType.Conflict)
                return Results.Conflict(new { detail = result.Error.Description });

            return Results.Problem(detail: result.Error.Description, statusCode: 500);
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .RequireAuthorization();

        // 6. PATCH /api/sales/{id:guid}/close - Close a sale
        group.MapPatch("/{id:guid}/close", async (
            Guid id,
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            Guid userGuid = GetUserId(user);
            var command = new CloseSaleCommand(id, userGuid);
            var result = await dispatcher.SendAsync<CloseSaleCommand, Result>(command, ct);

            if (result.IsSuccess)
                return Results.NoContent();

            if (result.Error.Type == ErrorType.NotFound)
                return Results.NotFound(new { detail = result.Error.Description });
            if (result.Error.Type == ErrorType.Conflict)
                return Results.Conflict(new { detail = result.Error.Description });
            if (result.Error.Type == ErrorType.Validation)
                return Results.BadRequest(new { detail = result.Error.Description });

            return Results.Problem(detail: result.Error.Description, statusCode: 500);
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .RequireAuthorization();

        // 7. DELETE /api/sales/{id:guid} - Soft-delete sale
        group.MapDelete("/{id:guid}", async (
            Guid id,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            var command = new DeleteSaleCommand(id);
            var result = await dispatcher.SendAsync<DeleteSaleCommand, Result>(command, ct);

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

        // --- Sale Items Sub-Resource ---

        // 8. POST /api/sales/{id:guid}/items - Add item to sale
        group.MapPost("/{id:guid}/items", async (
            Guid id,
            AddSaleItemRequest request,
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            Guid userGuid = GetUserId(user);
            Guid companyId = GetCompanyId(user);

            var command = new AddSaleItemCommand(
                id,
                userGuid,
                companyId,
                request.ProductId,
                request.Quantity,
                request.Price,
                request.Discount,
                request.Remarks,
                request.FinancialDate);

            var result = await dispatcher.SendAsync<AddSaleItemCommand, Result<Guid>>(command, ct);

            if (result.IsSuccess)
                return Results.Created($"/api/sales/{id}/items/{result.Data}", new { id = result.Data });

            if (result.Error.Type == ErrorType.NotFound)
                return Results.NotFound(new { detail = result.Error.Description });
            if (result.Error.Type == ErrorType.Validation)
                return Results.BadRequest(new { detail = result.Error.Description });
            if (result.Error.Type == ErrorType.Conflict)
                return Results.Conflict(new { detail = result.Error.Description });

            return Results.Problem(detail: result.Error.Description, statusCode: 500);
        })
        .Produces(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .RequireAuthorization();

        // 9. PUT /api/sales/{saleId:guid}/items/{itemId:guid} - Update sale item
        group.MapPut("/{saleId:guid}/items/{itemId:guid}", async (
            Guid saleId,
            Guid itemId,
            UpdateSaleItemRequest request,
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            Guid companyId = GetCompanyId(user);

            var command = new UpdateSaleItemCommand(
                saleId,
                itemId,
                request.Quantity,
                request.UnitPrice,
                request.UnitDiscount,
                request.Remark,
                request.TaxIds,
                companyId);

            var result = await dispatcher.SendAsync<UpdateSaleItemCommand, Result>(command, ct);

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
        .ProducesProblem(StatusCodes.Status409Conflict)
        .RequireAuthorization();

        // 10. PATCH /api/sales/{saleId:guid}/items/{itemId:guid}/quantity - Update sale item quantity
        group.MapPatch("/{saleId:guid}/items/{itemId:guid}/quantity", async (
            Guid saleId,
            Guid itemId,
            UpdateSaleItemQuantityRequest request,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            var command = new UpdateSaleItemQuantityCommand(saleId, itemId, request.Quantity);
            var result = await dispatcher.SendAsync<UpdateSaleItemQuantityCommand, Result>(command, ct);

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
        .ProducesProblem(StatusCodes.Status409Conflict)
        .RequireAuthorization();

        // 11. PATCH /api/sales/{saleId:guid}/items/{itemId:guid}/void - Void a sale item
        group.MapPatch("/{saleId:guid}/items/{itemId:guid}/void", async (
            Guid saleId,
            Guid itemId,
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            Guid userGuid = GetUserId(user);
            var command = new VoidSaleItemCommand(saleId, itemId, userGuid);
            var result = await dispatcher.SendAsync<VoidSaleItemCommand, Result>(command, ct);

            if (result.IsSuccess)
                return Results.NoContent();

            if (result.Error.Type == ErrorType.NotFound)
                return Results.NotFound(new { detail = result.Error.Description });
            if (result.Error.Type == ErrorType.Conflict)
                return Results.Conflict(new { detail = result.Error.Description });

            return Results.Problem(detail: result.Error.Description, statusCode: 500);
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .RequireAuthorization();

        // 12. PATCH /api/sales/{saleId:guid}/items/{itemId:guid}/unvoid - Unvoid a sale item
        group.MapPatch("/{saleId:guid}/items/{itemId:guid}/unvoid", async (
            Guid saleId,
            Guid itemId,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            var command = new UnvoidSaleItemCommand(saleId, itemId);
            var result = await dispatcher.SendAsync<UnvoidSaleItemCommand, Result>(command, ct);

            if (result.IsSuccess)
                return Results.NoContent();

            if (result.Error.Type == ErrorType.NotFound)
                return Results.NotFound(new { detail = result.Error.Description });
            if (result.Error.Type == ErrorType.Conflict)
                return Results.Conflict(new { detail = result.Error.Description });

            return Results.Problem(detail: result.Error.Description, statusCode: 500);
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .RequireAuthorization();

        // 13. DELETE /api/sales/{saleId:guid}/items/{itemId:guid} - Remove item from sale
        group.MapDelete("/{saleId:guid}/items/{itemId:guid}", async (
            Guid saleId,
            Guid itemId,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            var command = new RemoveSaleItemCommand(saleId, itemId);
            var result = await dispatcher.SendAsync<RemoveSaleItemCommand, Result>(command, ct);

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

        // --- Sale Payments Sub-Resource ---

        // 14. POST /api/sales/{id:guid}/payments - Add payment to sale
        group.MapPost("/{id:guid}/payments", async (
            Guid id,
            AddSalePaymentRequest request,
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            Guid userGuid = GetUserId(user);

            var command = new AddSalePaymentCommand(
                id,
                request.PaymentMethodId,
                userGuid,
                request.AmountPaid,
                request.PaymentDate,
                request.ReferenceNumber);

            var result = await dispatcher.SendAsync<AddSalePaymentCommand, Result<Guid>>(command, ct);

            if (result.IsSuccess)
                return Results.Created($"/api/sales/{id}/payments/{result.Data}", new { id = result.Data });

            if (result.Error.Type == ErrorType.NotFound)
                return Results.NotFound(new { detail = result.Error.Description });
            if (result.Error.Type == ErrorType.Validation)
                return Results.BadRequest(new { detail = result.Error.Description });
            if (result.Error.Type == ErrorType.Conflict)
                return Results.Conflict(new { detail = result.Error.Description });

            return Results.Problem(detail: result.Error.Description, statusCode: 500);
        })
        .Produces(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .RequireAuthorization();

        // 15. GET /api/sales/{id:guid}/payments - Get payments for a sale
        group.MapGet("/{id:guid}/payments", async (
            Guid id,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            var query = new GetSalePaymentsQuery(id);
            var result = await dispatcher.QueryAsync<Result<List<SalePaymentResponse>>>(query, ct);

            if (result.IsSuccess)
                return Results.Ok(result.Data);

            if (result.Error.Type == ErrorType.NotFound)
                return Results.NotFound(new { detail = result.Error.Description });

            return Results.Problem(detail: result.Error.Description, statusCode: 500);
        })
        .Produces<List<SalePaymentResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization();

        // 16. DELETE /api/sales/{saleId:guid}/payments/{paymentId:guid} - Remove payment from sale
        group.MapDelete("/{saleId:guid}/payments/{paymentId:guid}", async (
            Guid saleId,
            Guid paymentId,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            var command = new RemoveSalePaymentCommand(saleId, paymentId);
            var result = await dispatcher.SendAsync<RemoveSalePaymentCommand, Result>(command, ct);

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

    private static Guid GetUserId(ClaimsPrincipal user)
    {
        string? userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdClaim, out var userGuid) ? userGuid : Guid.Empty;
    }

    private static Guid GetCompanyId(ClaimsPrincipal user)
    {
        string? companyIdClaim = user.FindFirst("CompanyId")?.Value;
        return Guid.TryParse(companyIdClaim, out var companyGuid) ? companyGuid : Guid.Empty;
    }
}
