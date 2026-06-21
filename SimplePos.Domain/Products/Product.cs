namespace SimplePos.Domain.Products
{
    public class Product
    {
        public Guid ProductId { get; private set; }
        public Guid CompanyId { get; private set; }
        public string SKU { get; private set; } = string.Empty;
        public string ProductName { get; private set; } = string.Empty;
        public decimal CostPrice { get; private set; }
        public decimal BasePrice { get; private set; }
        public bool IsActive { get; private set; }
    }
}