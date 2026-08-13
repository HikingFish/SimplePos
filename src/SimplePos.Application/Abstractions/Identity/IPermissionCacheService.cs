using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Application.Abstractions.Identity
{
    public interface IPermissionCacheService
    {
        Task<HashSet<string>> GetPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);
        Task InvalidatePermissionsCacheAsync(Guid userId, CancellationToken cancellationToken = default);
        Task SetPermissionAsync(Guid userId, IEnumerable<string> permissions, CancellationToken cancellationToken = default);
    }
}
