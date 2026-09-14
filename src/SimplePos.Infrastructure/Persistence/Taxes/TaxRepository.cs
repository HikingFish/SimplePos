using SimplePos.Domain.Taxes;
using Microsoft.EntityFrameworkCore;

namespace SimplePos.Infrastructure.Persistence.Taxes;

public class TaxRepository : ITaxRepository
{
    private readonly AppDbContext _appDbContext;

    public TaxRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public void AddTax(Tax tax)
    {
        _appDbContext.Taxes.Add(tax);
    }

    public void DeleteTax(Tax tax)
    {
        _appDbContext.Taxes.Remove(tax);
    }

    public async Task<List<Tax>> GetActiveTaxesByCompanyIdAsync(Guid companyId)
    {
        return await _appDbContext.Taxes.Where(t => t.CompanyId == companyId && t.IsActive).ToListAsync();
    }

    public async Task<Tax?> GetTaxByIdAsync(Guid taxId)
    {
        return await _appDbContext.Taxes.FirstOrDefaultAsync(t => t.TaxId == taxId);
    }

    public async Task<List<Tax>> GetTaxesByIdsAsync(IEnumerable<Guid> taxIds)
    {
        return await _appDbContext.Taxes
            .Where(t => taxIds.Contains(t.TaxId))
            .ToListAsync();
    }

    public async Task<List<Tax>> GetTaxesByCompanyIdAsync(Guid companyId)
    {
        return await _appDbContext.Taxes.Where(t => t.CompanyId == companyId).ToListAsync();
    }

    public void UpdateTax(Tax tax)
    {
        _appDbContext.Taxes.Update(tax);
    }
}