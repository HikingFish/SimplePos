namespace SimplePos.Domain.Outlets;
public interface IOutletRepository
{
    void AddOutlet(Outlet outlet);
    Task<Outlet?> GetOutletByIdAsync(Guid outletId);
    Task<IEnumerable<Outlet>> GetOutletsByCompanyIdAsync(Guid companyId);
    void UpdateOutlet(Outlet outlet);
    Task DeleteOutletAsync(Guid outletId);
}