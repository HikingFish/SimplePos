using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Sales;
public static class SalePaymentError
{
    public static readonly Error SaleIdEmpty = new Error(
        "SalePayment.SaleIdEmpty", "Sale ID cannot be empty.");

    public static readonly Error PaymentMethodIdEmpty = new Error(
        "SalePayment.PaymentMethodIdEmpty", "Payment method ID cannot be empty.");

    public static readonly Error AmountNegativeOrZero = new Error(
        "SalePayment.AmountNegativeOrZero", "Payment amount must be greater than zero.");
}

