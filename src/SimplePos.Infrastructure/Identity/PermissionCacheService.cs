using Microsoft.Extensions.Caching.Distributed;
using SimplePos.Application.Abstractions.Identity;
using SimplePos.Domain.Permissions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Infrastructure.Identity
{
    public class PermissionCacheService : IPermissionCacheService
    {
        private readonly IDistributedCache _cache;
        private readonly IPermissionRepository _permissionRepository;

        private static readonly DistributedCacheEntryOptions CacheOptions = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
            SlidingExpiration = TimeSpan.FromMinutes(10)
        };

        public PermissionCacheService(IDistributedCache cache, IPermissionRepository permissionRepository)
        {
            _cache = cache;
            _permissionRepository = permissionRepository;
        }

        public Task<HashSet<string>> GetPermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task InvalidatePermissionsCacheAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task SetPermissionAsync(Guid userId, IEnumerable<string> permissions, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
