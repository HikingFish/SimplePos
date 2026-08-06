namespace SimplePos.Domain.Companies;
public interface ICompanyRepository
{
    void AddCompany(Company company);
    Task<Company?> GetCompanyByIdAsync(Guid companyId);
    Task<Company?> GetCompanyByEmailAsync(string email);
    Task<bool> ExistsByEmailAsync(string email);
    Task<IEnumerable<Company>> GetAllCompaniesAsync();
    void UpdateCompany(Company company);
    Task DeleteCompanyAsync(Guid companyId);
}