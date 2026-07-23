using SimplePos.Domain.Outlets;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Infrastructure.Persistence.Outlets;

public class OutletRepository : IOutletRepository
{
    public Task AddOutletAsync(Outlet outlet)
    {
        throw new NotImplementedException();
    }

    public Task DeleteOutletAsync(Guid outletId)
    {
        throw new NotImplementedException();
    }

    public Task<Outlet?> GetOutletByIdAsync(Guid outletId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Outlet>> GetOutletsByCompanyIdAsync(Guid companyId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateOutletAsync(Outlet outlet)
    {
        throw new NotImplementedException();
    }
}
