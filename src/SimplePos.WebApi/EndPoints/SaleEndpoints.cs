using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.Sales.Commands.CreateSale;
using SimplePos.Domain.Common.ResultPattern;
using System.Security.Claims;

namespace SimplePos.WebApi.EndPoints
{
    public static class SaleEndpoints
    {
        public record CreateSaleRequest(Guid? OutletId, DateTime financialDate, List<CreateSaleItemRequest> SaleItems, List<CreateSalePaymentRequest>? SalePayments);
        public record CreateSaleItemRequest(Guid productId, int quantity, string? remarks, decimal? discount, decimal? price, DateTime financialDate);
        public record CreateSalePaymentRequest(Guid paymentMethod, decimal amountPaid, DateTime paymentDate, string referenceNumber);
        public static void MapSaleEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/sales").WithTags("Sales");

            group.MapPost("/", async (
                CreateSaleRequest request,
                ClaimsPrincipal user,
                ICqrsDispatcher dispatcher,
                CancellationToken ct) =>
            {
                string? userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
                Guid.TryParse(userIdClaim, out var userGuid);

                Guid outletId;

                if (request.OutletId.HasValue)
                {
                    outletId = request.OutletId.Value;
                }
                else
                {
                    string? outletIdClaim = user.FindFirstValue("OutletId");
                    outletId = request.OutletId ?? (Guid.TryParse(user.FindFirstValue("OutletId"), out var claimOutlet) ? claimOutlet : Guid.Empty);
                }

                string? companyIdClaim = user.FindFirstValue("CompanyId");
                Guid companyId = Guid.TryParse(companyIdClaim, out var companyGuid) ? companyGuid : Guid.Empty;

                List<CreateSalePayment>? createPaymentRequest = request.SalePayments?
                    .Select(sp => new CreateSalePayment(sp.paymentMethod, sp.amountPaid, sp.paymentDate, sp.referenceNumber))
                    .ToList();

                List<CreateSaleItem> createSaleItemRequest = request.SaleItems
                    .Select(si => new CreateSaleItem(si.productId, si.quantity, si.remarks, si.discount, si.price, si.financialDate))
                    .ToList();

                CreateSaleCommand command = new CreateSaleCommand(userGuid, outletId, companyId, request.financialDate, createSaleItemRequest, createPaymentRequest);

                var result = await dispatcher.SendAsync<CreateSaleCommand, Result<Guid>> (command, ct);

                if (result.IsSuccess)
                    return Results.Ok(result.Data);

                if (result.Error.Type == ErrorType.NotFound)
                    return Results.NotFound(new { detail = result.Error.Description });

                return Results.Problem(detail: result.Error.Description, statusCode: 500);
            })
                .Produces<Guid>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .RequireAuthorization();
        }
    }
}
