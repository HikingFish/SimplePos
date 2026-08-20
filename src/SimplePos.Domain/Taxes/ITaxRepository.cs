namespace SimplePos.Domain.Taxes;

public interface ITaxRepository
{
    Task<Tax?> GetTaxByIdAsync(Guid taxId);
    Task<List<Tax>> GetTaxesByCompanyIdAsync(Guid companyId);
    Task<List<Tax>> GetActiveTaxesByCompanyIdAsync(Guid companyId);
    void AddTax(Tax tax);
    void UpdateTax(Tax tax);
    void DeleteTax(Tax tax);
}
