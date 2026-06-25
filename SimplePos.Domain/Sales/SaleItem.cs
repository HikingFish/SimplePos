namespace SimplePos.Domain.Sales
{
    public class SaleItem
    {
        public Guid SaleItemId { get; private set; }
        public Guid ProductId { get; private set; }
        public Guid SaleId { get; private set; }
        public decimal Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal DiscountAmount { get; private set; }
        public decimal TotalLineAmount { get; private set; }
        public string? Remark { get; private set; } 
        public decimal TaxRatePercentage { get; private set; }
        public decimal TaxAmount { get; private set; }
    }
}