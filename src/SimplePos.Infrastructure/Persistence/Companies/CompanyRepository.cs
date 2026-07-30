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

    public async Task AddCompanyAsync(Company company)
    {
        await _appDbContext.Companies.AddAsync(company);
        await _appDbContext.SaveChangesAsync();
    }

    public async Task DeleteCompanyAsync(Guid companyId)
    {
        var company = await GetCompanyByIdAsync(companyId);
        if(company != null)
        {
            _appDbContext.Remove(company);
        }
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
        return await _appDbContext.Companies.FirstOrDefaultAsync(c => c.CompanyId.Equals(companyId));
    }

    public Task UpdateCompanyAsync(Company company)
    {
        _appDbContext.Companies.Update(company);
        return Task.CompletedTask;
    }
}
