using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Sales
{
    public static class SaleError
    {
        public static readonly Error OutletIdEmpty = new Error(
            "Sales.OutletIdEmpty", "Outlet ID cannot be empty.");
        public static readonly Error SaleItemNull = new Error(
            "Sales.SaleItemNull", "Sale item cannot be null.");
        public static readonly Error SalePaymentNull = new Error(
            "Sales.SalePaymentNull", "Sale payment cannot be null.");
        public static readonly Error SoftDeleted = new Error(
            "Sales.SoftDeleted", "Operation cannot be performed on a soft deleted sale.");
    }
}
