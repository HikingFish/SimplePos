using Microsoft.Extensions.Caching.Distributed;
using SimplePos.Application.Abstractions.Identity;
using SimplePos.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace SimplePos.Infrastructure.Identity
{
    public class PermissionCacheService : IPermissionCacheService
    {
        private readonly IDistributedCache _cache;
        private readonly IUserPermissionRepository _userPermissionRepository;

        private static readonly DistributedCacheEntryOptions CacheOptions = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
            SlidingExpiration = TimeSpan.FromMinutes(10)
        };

        public PermissionCacheService(IDistributedCache cache, IUserPermissionRepository userPermissionRepository)
        {
            _cache = cache;
            _userPermissionRepository = userPermissionRepository;
        }

        private static string CacheKey(Guid userId) => $"permissions:{userId}";

        public async Task<HashSet<string>> GetPermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var cached = await _cache.GetStringAsync(CacheKey(userId), cancellationToken);
            if(cached != null)
            {
                return JsonSerializer.Deserialize<HashSet<string>>(cached) ?? new HashSet<string>();
            }

            var permissions = await _userPermissionRepository.GetPermissionsByUserIdAsync(userId);
            var permissionNames = permissions.Select(p => p.Name).ToHashSet();

            await SetPermissionAsync(userId, permissionNames, cancellationToken);
            return permissionNames;
        }

        public async Task InvalidatePermissionsCacheAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            await _cache.RemoveAsync(CacheKey(userId), cancellationToken);
        }

        public async Task SetPermissionAsync(Guid userId, IEnumerable<string> permissions, CancellationToken cancellationToken = default)
        {
            var serializedPermissions = JsonSerializer.Serialize(permissions);
            await _cache.SetStringAsync(CacheKey(userId), serializedPermissions, CacheOptions, cancellationToken);
        }
    }
}
