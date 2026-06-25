using SimplePos.Domain.Common;

namespace SimplePos.Domain.Sales
{
    public class Sale
    {
        public Guid SaleId { get; private set; }
        public Guid OutletId { get; private set; }
        public string InvoiceNumber { get; private set; } 
        public decimal SubTotal { get; private set; }
        public decimal TaxAmount { get; private set; }
        public string PaymentMethod { get; private set; }
        public decimal TotalAmount { get; private set; }
        public DateTime DateTimeCreated { get; private set; }
        public bool SoftDeleted { get; private set; }
        private readonly List<SaleItem> _saleItems  = new List<SaleItem>();
        public IReadOnlyCollection<SaleItem> SaleItems => _saleItems.AsReadOnly();

        private Sale() { }

        private Sale(Guid outletId)
        {
            SaleId = Guid.CreateVersion7();
            OutletId = outletId;
            DateTimeCreated = DateTime.UtcNow;
            SoftDeleted = false;
        }

        public static Sale Create(Guid outletId)
        {
            if (outletId == Guid.Empty)
            {
                throw new DomainException("Outlet ID cannot be empty.");
            }

            return new Sale(outletId);
        }

        public void AddSaleItem(SaleItem saleItem)
        {
            EnsureNotSoftDeleted();

            if (saleItem == null)
            {
                throw new DomainException("Sale item cannot be null.");
            }

            _saleItems.Add(saleItem);
        }

        public void CalculateTotals()
        {
            
        }

        private void EnsureNotSoftDeleted()
        {
            if (SoftDeleted)
            {
                throw new DomainException("Operation cannot be performed on a soft deleted sale.");
            }
        }
    }
}