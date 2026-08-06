using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Sales;
public static class SaleError
{
    public static readonly Error OutletIdEmpty = new Error(
        "Sales.OutletIdEmpty", "Outlet ID cannot be empty.", ErrorType.Validation);
    public static readonly Error SaleItemNull = new Error(
        "Sales.SaleItemNull", "Sale item cannot be null.", ErrorType.Validation);
    public static readonly Error SalePaymentNull = new Error(
        "Sales.SalePaymentNull", "Sale payment cannot be null.", ErrorType.Validation);
    public static readonly Error SoftDeleted = new Error(
        "Sales.SoftDeleted", "Operation cannot be performed on a soft deleted sale.", ErrorType.Conflict);
    public static readonly Error SaleItemNotFound = new Error(
        "Sales.SaleItemNotFound", "Sale item not found.", ErrorType.NotFound);
    public static readonly Error SaleItemIsEmpty = new Error(
        "Sales.SaleItemIsEmpty", "Sale item is empty.", ErrorType.Validation);
    public static readonly Error SalePaymentNotFound = new Error(
        "Sales.SalePaymentNotFound", "Sale payment is not found.", ErrorType.NotFound);
    public static readonly Error SaleFullyPaid = new Error(
        "Sales.SaleFullyPaid", "Sale is already fully paid.", ErrorType.Conflict);
    public static readonly Error AmountNegativeOrZero = new Error(
        "Sales.AmountNegativeOrZero", "Amount cannot be negative or zero.", ErrorType.Validation);
    public static readonly Error Void = new Error(
        "Sales.Void", "Operation cannot be performed on a void sale.", ErrorType.Conflict);
    public static readonly Error NotVoid = new Error(
        "Sales.NotVoid", "Operation cannot be performed on a not void sale.", ErrorType.Conflict);
}

