namespace SimplePos.Domain.Payments;
public interface IPaymentMethodRepository
{
    Task<PaymentMethod?> GetPaymentMethodByIdAsync(Guid paymentMethodId);
    Task<List<PaymentMethod>> GetPaymentMethodsByCompanyIdAsync(Guid companyId);
    Task AddPaymentMethodAsync(PaymentMethod paymentMethod);
    Task UpdatePaymentMethodAsync(PaymentMethod paymentMethod);
    Task DeletePaymentMethodAsync(Guid paymentMethodId);
}

