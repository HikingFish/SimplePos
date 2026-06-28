using SimplePos.Domain.Common;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Taxes;

namespace SimplePos.Domain.Sales
{
    public class Sale
    {
        public Guid SaleId { get; private set; }
        public Guid OutletId { get; private set; }
        public string InvoiceNumber { get; private set; } 
        public decimal SubTotal { get; private set; }
        public decimal TaxAmount { get; private set; }
        public decimal TotalAmount { get; private set; }
        public DateTime DateTimeCreated { get; private set; }
        public bool SoftDeleted { get; private set; }
        private readonly List<SaleItem> _saleItems  = new List<SaleItem>();
        public IReadOnlyCollection<SaleItem> SaleItems => _saleItems.AsReadOnly();
        private readonly List<SalePayment> _salePayments = new List<SalePayment>();
        public IReadOnlyCollection<SalePayment> SalePayments => _salePayments.AsReadOnly();
        public decimal TotalPaid => _salePayments.Sum(p => p.Amount);

        private Sale() { }

        private Sale(Guid outletId, string invoiceNumber)
        {
            SaleId = Guid.CreateVersion7();
            OutletId = outletId;
            DateTimeCreated = DateTime.UtcNow;
            SoftDeleted = false;
            InvoiceNumber = invoiceNumber;
        }

        public static Result<Sale> Create(Guid outletId, string invoiceNumber)
        {
            if (outletId == Guid.Empty)
            {
                return Result<Sale>.Failure(SaleError.OutletIdEmpty);
            }

            return Result<Sale>.Success(new Sale(outletId, invoiceNumber));
        }

        public Result AddSaleItem(SaleItem saleItem)
        {
            var statusResult = EnsureNotSoftDeleted();
            if (!statusResult.IsSuccess)
            {
                return statusResult;
            }

            if (saleItem == null)
            {
                return Result.Failure(SaleError.SaleItemNull);
            }

            _saleItems.Add(saleItem);
            CalculateTotals();
            return Result.Success();
        }

        public Result AddPayment(SalePayment payment)
        {
            var statusResult = EnsureNotSoftDeleted();
            if (!statusResult.IsSuccess)
            {
                return statusResult;
            }

            if (payment == null)
            {
                return Result.Failure(SaleError.SalePaymentNull);
            }

            _salePayments.Add(payment);
            return Result.Success();
        }

        public Result CalculateTotals()
        {
            var statusResult = EnsureNotSoftDeleted();
            if (!statusResult.IsSuccess)
            {
                return statusResult;
            }

            decimal subTotal = 0;
            decimal taxAmount = 0;

            foreach (var saleItem in _saleItems)
            {
                subTotal += saleItem.NetAmount;
                taxAmount += saleItem.TaxAmount;
            }

            SubTotal = subTotal;
            TaxAmount = taxAmount;
            TotalAmount = SubTotal + TaxAmount;
            
            return Result.Success();
        }

        private Result EnsureNotSoftDeleted()
        {
            if (SoftDeleted)
            {
                return Result.Failure(SaleError.SoftDeleted);
            }
            return Result.Success();
        }
    }
}