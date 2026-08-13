using Microsoft.Extensions.Caching.Distributed;
using SimplePos.Application.Abstractions.Identity;
using SimplePos.Domain.Permissions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

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

        private static string CacheKey(Guid userId) => $"permissions:{userId}";

        public async Task<HashSet<string>> GetPermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var cached = await _cache.GetStringAsync(CacheKey(userId), cancellationToken);
            if(cached != null)
            {
                return JsonSerializer.Deserialize<HashSet<string>>(cached) ?? new HashSet<string>();
            }

            var permissions = await _permissionRepository.GetPermissionsByUserIdAsync(userId);
            var permissionNames = permissions.Select(p => p.Name).ToHashSet();

            await SetPermissionAsync(userId, permissionNames, cancellationToken);
            return permissionNames;
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
