using Microsoft.EntityFrameworkCore;
using SimplePos.Domain.Companies;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Infrastructure.Persistence.Companies;

public class CompanyRepository : ICompanyRepository
{
    private readonly AppDbContext _appDbContext;

    public CompanyRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public void AddCompany(Company company)
    {
        _appDbContext.Companies.Add(company);
    }

    public void DeleteCompany(Company company)
    {
        _appDbContext.Companies.Remove(company);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _appDbContext.Companies.AnyAsync(c => c.Email.Value == email);
    }

    public async Task<IEnumerable<Company>> GetAllCompaniesAsync()
    {
        return await _appDbContext.Companies.ToListAsync();
    }

    public async Task<Company?> GetCompanyByEmailAsync(string email)
    {
        return await _appDbContext.Companies.FirstOrDefaultAsync(c => c.Email.Value.Equals(email));
    }

    public async Task<Company?> GetCompanyByIdAsync(Guid companyId)
    {
        return await _appDbContext.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.CompanyId.Equals(companyId));
    }

    public void UpdateCompany(Company company)
    {
        _appDbContext.Companies.Update(company);
    }
}
