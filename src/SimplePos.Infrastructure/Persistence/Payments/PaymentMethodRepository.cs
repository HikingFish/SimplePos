using SimplePos.Domain.Payments;
using Microsoft.EntityFrameworkCore;

namespace SimplePos.Infrastructure.Persistence.Payments;

public class PaymentMethodRepository : IPaymentMethodRepository
{
    private readonly AppDbContext _appDbContext;

    public PaymentMethodRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public void AddPaymentMethod(PaymentMethod paymentMethod)
    {
        _appDbContext.PaymentMethods.Add(paymentMethod);
    }

    public void DeletePaymentMethod(PaymentMethod paymentMethod)
    {
        _appDbContext.PaymentMethods.Remove(paymentMethod);
    }

    public async Task<PaymentMethod?> GetPaymentMethodByIdAsync(Guid paymentMethodId)
    {
        return await _appDbContext.PaymentMethods.FindAsync(paymentMethodId);
    }

    public async Task<List<PaymentMethod>> GetPaymentMethodsByCompanyIdAsync(Guid companyId)
    {
        return await _appDbContext.PaymentMethods.Where(pm => pm.CompanyId.Equals(companyId)).ToListAsync();
    }

    public async Task<List<PaymentMethod>> GetActivePaymentMethodsByCompanyIdAsync(Guid companyId)
    {
        return await _appDbContext.PaymentMethods.Where(pm => pm.CompanyId.Equals(companyId) && pm.IsActive).ToListAsync();
    }

    public void UpdatePaymentMethod(PaymentMethod paymentMethod)
    {
        _appDbContext.PaymentMethods.Update(paymentMethod);
    }
}