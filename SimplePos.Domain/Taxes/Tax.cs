namespace SimplePos.Domain.Taxes
{
    public class Tax
    {
        public Guid TaxId { get; private set; }
        public Guid CompanyId { get; private set; }
        public string TaxName { get; private set; } = string.Empty;
        public decimal TaxRate { get; private set; }
    }
}