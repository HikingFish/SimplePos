using SimplePos.Domain.Companies;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Infrastructure.Persistence.Companies;

public class CompanyRepository : ICompanyRepository
{
    public Task AddCompanyAsync(Company company)
    {
        throw new NotImplementedException();
    }

    public Task DeleteCompanyAsync(Guid companyId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Company>> GetAllCompaniesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Company?> GetCompanyByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task<Company?> GetCompanyByIdAsync(Guid companyId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateCompanyAsync(Company company)
    {
        throw new NotImplementedException();
    }
}
