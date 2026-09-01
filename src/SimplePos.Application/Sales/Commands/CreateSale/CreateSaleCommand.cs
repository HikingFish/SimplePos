using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Application.Sales.Commands.CreateSale;

public record CreateSaleCommand(Guid UserId, Guid? OutletId, DateTime financialDate, List<CreateSaleItem> SaleItems, List<CreateSalePayment>? SalePayments) : ICommand<Result<Guid>>;

public record CreateSaleItem(Guid productId, int quantity, string? remarks, decimal? discount, decimal price);

public record CreateSalePayment(Guid paymentMethod, decimal amountPaid, DateTime paymentDate, string refereceNumber);