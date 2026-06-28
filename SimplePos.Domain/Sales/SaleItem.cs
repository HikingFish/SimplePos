using System.Dynamic;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Taxes;

namespace SimplePos.Domain.Sales
{
    public class SaleItem
    {
        public Guid SaleItemId { get; private set; }
        public Guid ProductId { get; private set; }
        public Guid SaleId { get; private set; }
        public decimal Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        //Gross amount total without discount and tax
        public decimal GrossAmount { get; private set; }
        public decimal UnitDiscount { get; private set; }
        public decimal DiscountedUnitPrice { get; private set; }
        //Net Amount is total without tax but with discount
        public decimal NetAmount { get; private set; }
        public decimal TotalLineAmount { get; private set; }
        public string? Remark { get; private set; } 
        public decimal TaxRate { get; private set; }
        public decimal TaxAmount { get; private set; }

        private SaleItem(){}

        private SaleItem(Guid productId, 
            Guid saleId, 
            decimal quantity,
            decimal unitPrice,
            decimal unitDiscount,
            string? remark,
            decimal taxRate)
        {
            SaleItemId = Guid.CreateVersion7();
            ProductId = productId;
            SaleId = saleId;
            Quantity = quantity;
            UnitPrice = unitPrice;
            UnitDiscount = unitDiscount;
            Remark = remark;
            TaxRate = taxRate;

            CalculateLineTotal();
        }

        public static Result<SaleItem> Create(Guid productId, 
            Guid saleId, 
            decimal quantity,
            decimal unitPrice,
            decimal unitDiscount,
            string? remark,
            decimal taxRate)
        {
            if (productId == Guid.Empty)
            {
                return Result<SaleItem>.Failure(SaleItemError.ProductIdEmpty);
            }

            if (saleId == Guid.Empty)
            {
                return Result<SaleItem>.Failure(SaleItemError.SaleIdEmpty);
            }

            if (taxRate < 0)
            {
                return Result<SaleItem>.Failure(SaleItemError.TaxRateNegative);
            }

            if (quantity <= 0)
            {
                return Result<SaleItem>.Failure(SaleItemError.QuantityZero);
            }

            return Result<SaleItem>.Success(new SaleItem(productId, saleId, quantity, unitPrice, unitDiscount, remark, taxRate));
        }

        private void CalculateLineTotal()
        {
            DiscountedUnitPrice = decimal.Round(UnitPrice * UnitDiscount, 2);
            GrossAmount = decimal.Round(Quantity * UnitPrice, 2);
            NetAmount = decimal.Round(Quantity * DiscountedUnitPrice, 2);
            TaxAmount = decimal.Round(NetAmount * TaxRate, 2);
            TotalLineAmount = decimal.Round(NetAmount + TaxAmount, 2);
        }
    }
}