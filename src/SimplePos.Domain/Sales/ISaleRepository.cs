namespace SimplePos.Domain.Sales;

public interface ISaleRepository
{
    void AddSale(Sale sale);
    Task<Sale?> GetSaleByIdAsync(Guid saleId);
    Task<List<Sale>> GetSalesByOutletIdAsync(Guid outletId);
    Task<List<Sale>> GetSalesByDateRangeAsync(Guid outletId, DateTime from, DateTime to);
    Task<List<Sale>> GetSalesByUserIdAsync(Guid userId);
    void UpdateSale(Sale sale);
    // void AddPayment(SalePayment payment);
    Task<List<SalePayment>> GetPaymentsBySaleIdAsync(Guid saleId);
}

