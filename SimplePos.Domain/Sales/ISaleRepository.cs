namespace SimplePos.Domain.Sales;
public interface ISaleRepository
{
    Task AddSaleAsync(Sale sale);
    Task<Sale?> GetSaleByIdAsync(Guid saleId);
    Task<List<Sale>> GetSalesByOutletIdAsync(Guid outletId);
    Task<List<Sale>> GetSalesByDateRangeAsync(Guid outletId, DateTime from, DateTime to);
    Task<List<Sale>> GetSalesByUserIdAsync(Guid userId);
    Task UpdateSaleAsync(Sale sale);
    // Task AddPaymentAsync(SalePayment payment);
    Task<List<SalePayment>> GetPaymentsBySaleIdAsync(Guid saleId);
}

