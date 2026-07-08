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
}