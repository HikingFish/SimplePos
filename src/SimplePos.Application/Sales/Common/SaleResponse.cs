using SimplePos.Domain.Sales;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimplePos.Application.Sales.Common;

public record SaleItemTaxResponse(
    Guid SaleItemTaxId,
    Guid SaleItemId,
    Guid TaxId,
    string TaxName,
    decimal TaxRate,
    decimal TaxAmount
);

public record SaleItemResponse(
    Guid SaleItemId,
    Guid ProductId,
    Guid SaleId,
    Guid AddedByUserId,
    DateTime DateTimeAdded,
    bool Void,
    Guid? VoidedByUserId,
    DateTime? DateTimeVoided,
    decimal Quantity,
    decimal UnitPrice,
    decimal GrossAmount,
    decimal UnitDiscount,
    decimal DiscountedUnitPrice,
    decimal NetAmount,
    decimal TotalDiscount,
    decimal TotalLineAmount,
    string? Remark,
    decimal TaxRate,
    decimal TaxAmount,
    List<SaleItemTaxResponse> SaleItemTaxes
);

public record SalePaymentResponse(
    Guid SalePaymentId,
    Guid SaleId,
    Guid PaymentMethodId,
    Guid ProcessedByUserId,
    decimal AmountPaid,
    DateTime PaymentDate,
    string? ReferenceNumber
);

public record SaleResponse(
    Guid SaleId,
    Guid OutletId,
    Guid CreatedByUserId,
    string InvoiceNumber,
    decimal SubTotal,
    decimal TaxAmount,
    decimal NetAmount,
    decimal TotalAmount,
    DateTime DateTimeCreated,
    bool Void,
    Guid? VoidedByUserId,
    DateTime? DateTimeVoided,
    bool Closed,
    Guid? ClosedByUserId,
    DateTime? DateTimeClosed,
    decimal TotalPaid,
    decimal TotalChange,
    decimal TotalOutstanding,
    decimal TotalDiscount,
    List<SaleItemResponse> SaleItems,
    List<SalePaymentResponse> SalePayments
);

public static class SaleMappings
{
    public static SaleItemTaxResponse ToResponse(this SaleItemTax tax)
    {
        return new SaleItemTaxResponse(
            tax.SaleItemTaxId,
            tax.SaleItemId,
            tax.TaxId,
            tax.TaxName,
            tax.TaxRate,
            tax.TaxAmount
        );
    }

    public static SaleItemResponse ToResponse(this SaleItem item)
    {
        return new SaleItemResponse(
            item.SaleItemId,
            item.ProductId,
            item.SaleId,
            item.AddedByUserId,
            item.DateTimeAdded,
            item.Void,
            item.VoidedByUserId,
            item.DateTimeVoided,
            item.Quantity,
            item.UnitPrice,
            item.GrossAmount,
            item.UnitDiscount,
            item.DiscountedUnitPrice,
            item.NetAmount,
            item.TotalDiscount,
            item.TotalLineAmount,
            item.Remark,
            item.TaxRate,
            item.TaxAmount,
            item.SaleItemTaxes.Select(t => t.ToResponse()).ToList()
        );
    }

    public static SalePaymentResponse ToResponse(this SalePayment payment)
    {
        return new SalePaymentResponse(
            payment.SalePaymentId,
            payment.SaleId,
            payment.PaymentMethodId,
            payment.ProcessedByUserId,
            payment.AmountPaid,
            payment.PaymentDate,
            payment.ReferenceNumber
        );
    }

    public static SaleResponse ToResponse(this Sale sale)
    {
        return new SaleResponse(
            sale.SaleId,
            sale.OutletId,
            sale.CreatedByUserId,
            sale.InvoiceNumber,
            sale.SubTotal,
            sale.TaxAmount,
            sale.NetAmount,
            sale.TotalAmount,
            sale.DateTimeCreated,
            sale.Void,
            sale.VoidedByUserId,
            sale.DateTimeVoided,
            sale.Closed,
            sale.ClosedByUserId,
            sale.DateTimeClosed,
            sale.TotalPaid,
            sale.TotalChange,
            sale.TotalOutstanding,
            sale.TotalDiscount,
            sale.SaleItems.Select(si => si.ToResponse()).ToList(),
            sale.SalePayments.Select(sp => sp.ToResponse()).ToList()
        );
    }
}
