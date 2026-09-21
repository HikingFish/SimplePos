using SimplePos.Domain.Sales;
using Microsoft.EntityFrameworkCore;

namespace SimplePos.Infrastructure.Persistence.Sales;

public class SaleRepository : ISaleRepository
{
    private readonly AppDbContext _appDbContext;

    public SaleRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public void AddSale(Sale sale)
    {
        _appDbContext.Sales.Add(sale);
    }

    public void UpdateSale(Sale sale)
    {
        _appDbContext.Sales.Update(sale);
    }

    public void AddSaleItem(SaleItem saleItem)
    {
        _appDbContext.SaleItems.Add(saleItem);
    }

    public void RemoveSaleItem(SaleItem saleItem)
    {
        _appDbContext.SaleItems.Remove(saleItem);
    }

    public void AddSaleItemTaxes(IEnumerable<SaleItemTax> taxes)
    {
        _appDbContext.SaleItemTaxes.AddRange(taxes);
    }

    public void AddPayment(SalePayment payment)
    {
        _appDbContext.SalePayments.Add(payment);
    }

    public void RemovePayment(SalePayment payment)
    {
        _appDbContext.SalePayments.Remove(payment);
    }

    public async Task<Sale?> GetSaleByIdAsync(Guid saleId)
    {
        return await _appDbContext.Sales
            .Include(s => s.SaleItems)
                .ThenInclude(si => si.SaleItemTaxes)
            .Include(s => s.SalePayments)
            .FirstOrDefaultAsync(s => s.SaleId == saleId);
    }

    public async Task<List<Sale>> GetSalesByOutletIdAsync(Guid outletId)
    {
        return await _appDbContext.Sales
            .Include(s => s.SaleItems)
                .ThenInclude(si => si.SaleItemTaxes)
            .Include(s => s.SalePayments)
            .Where(s => s.OutletId == outletId)
            .ToListAsync();
    }

    public async Task<List<Sale>> GetSalesByDateRangeAsync(Guid outletId, DateTime from, DateTime to)
    {
        return await _appDbContext.Sales
            .Include(s => s.SaleItems)
                .ThenInclude(si => si.SaleItemTaxes)
            .Include(s => s.SalePayments)
            .Where(s => s.OutletId == outletId && s.DateTimeCreated >= from && s.DateTimeCreated <= to)
            .ToListAsync();
    }

    public async Task<List<Sale>> GetSalesByUserIdAsync(Guid userId)
    {
        return await _appDbContext.Sales
            .Include(s => s.SaleItems)
                .ThenInclude(si => si.SaleItemTaxes)
            .Include(s => s.SalePayments)
            .Where(s => s.CreatedByUserId == userId)
            .ToListAsync();
    }

    public async Task<List<SalePayment>> GetPaymentsBySaleIdAsync(Guid saleId)
    {
        var sale = await _appDbContext.Sales
            .Include(s => s.SalePayments)
            .FirstOrDefaultAsync(s => s.SaleId == saleId);

        return sale?.SalePayments.ToList() ?? new List<SalePayment>();
    }
}