namespace SimplePos.Domain.Outlets;
public interface IOutletRepository
{
    Task AddOutletAsync(Outlet outlet);
    Task<Outlet?> GetOutletByIdAsync(Guid outletId);
    Task<IEnumerable<Outlet>> GetOutletsByCompanyIdAsync(Guid companyId);
    Task UpdateOutletAsync(Outlet outlet);
    Task DeleteOutletAsync(Guid outletId);
}