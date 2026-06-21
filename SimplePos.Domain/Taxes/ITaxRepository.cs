namespace SimplePos.Domain.Taxes
{
    public interface ITaxRepository
    {
        Task<Tax?> GetTaxByIdAsync(Guid taxId);
        Task<List<Tax>> GetTaxesByCompanyIdAsync(Guid companyId);
        Task AddTaxAsync(Tax tax);
        Task UpdateTaxAsync(Tax tax);
        Task DeleteTaxAsync(Guid taxId);
    }
}