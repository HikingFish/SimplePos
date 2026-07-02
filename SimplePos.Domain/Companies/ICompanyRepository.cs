namespace SimplePos.Domain.Companies;
public interface ICompanyRepository
{
    Task AddCompanyAsync(Company company);
    Task<Company?> GetCompanyByIdAsync(Guid companyId);
    Task<Company?> GetCompanyByEmailAsync(string email);
    Task<bool> ExistsByEmailAsync(string email);
    Task<IEnumerable<Company>> GetAllCompaniesAsync();
    Task UpdateCompanyAsync(Company company);
    Task DeleteCompanyAsync(Guid companyId);
    Task SoftDeleteCompanyAsync(Guid companyId);
    Task UpdateCompanyLastOnlineAsync(Guid companyId);
    Task UpdateCompanyActiveStatusAsync(Guid companyId, bool isActive);
}