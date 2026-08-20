namespace SimplePos.Domain.Payments;

public interface IPaymentMethodRepository
{
    Task<PaymentMethod?> GetPaymentMethodByIdAsync(Guid paymentMethodId);
    Task<List<PaymentMethod>> GetPaymentMethodsByCompanyIdAsync(Guid companyId);
    Task<List<PaymentMethod>> GetActivePaymentMethodsByCompanyIdAsync(Guid companyId);
    void AddPaymentMethod(PaymentMethod paymentMethod);
    void UpdatePaymentMethod(PaymentMethod paymentMethod);
    void DeletePaymentMethod(PaymentMethod paymentMethod);
}

