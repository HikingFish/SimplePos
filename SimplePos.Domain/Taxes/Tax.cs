using SimplePos.Domain.Common;

namespace SimplePos.Domain.Taxes
{
    public class Tax
    {
        public Guid TaxId { get; private set; }
        public Guid CompanyId { get; private set; }
        public string TaxName { get; private set; }
        public decimal TaxRate { get; private set; }
        public bool IsActive { get; private set; }

        private Tax() { }

        private Tax(Guid companyId, string taxName, decimal taxRate)
        {
            TaxId = Guid.CreateVersion7();
            CompanyId = companyId;
            TaxName = taxName;
            TaxRate = taxRate;
            IsActive = true;
        }

        public static Tax Create(Guid companyId, string taxName, decimal taxRate)
        {
            if (string.IsNullOrWhiteSpace(taxName))
            {
                throw new DomainException("Tax name cannot be empty.");
            }

            if (taxRate < 0)
            {
                throw new DomainException("Tax rate cannot be negative.");
            }

            return new Tax(companyId, taxName, taxRate);
        }

        public void UpdateTaxInfo(string newTaxName, decimal newTaxRate)
        {
            if (string.IsNullOrWhiteSpace(newTaxName))
            {
                throw new DomainException("Tax name cannot be empty.");
            }

            if (newTaxRate < 0)
            {
                throw new DomainException("Tax rate cannot be negative.");
            }

            TaxName = newTaxName;
            TaxRate = newTaxRate;
        }
        public void Deactivate()
        {
            if (!IsActive)
            {
                throw new DomainException("Tax is already deactivated.");
            }

            IsActive = false;
        }

        public void Activate()
        {
            if (IsActive)
            {
                throw new DomainException("Tax is already activated.");
            }

            IsActive = true;
        }
    }
}