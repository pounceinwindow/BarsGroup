using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

public class DistributedSessionStore : ITicketStore
{
    private readonly IDistributedCache _cache;
    private readonly IDataProtector _protector;

    public DistributedSessionStore(IDistributedCache cache, IDataProtectionProvider dataProtection)
    {
        _cache = cache;
        _protector = dataProtection.CreateProtector("AuthTicket");
    }

    public async Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        var key = Guid.NewGuid().ToString();
        await RenewAsync(key, ticket);
        return key;
    }

    public async Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        var data = TicketSerializer.Default.Serialize(ticket);
        var protectedData = _protector.Protect(data);

        await _cache.SetAsync(key, protectedData, new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromHours(8)
        });
    }

    public async Task<AuthenticationTicket?> RetrieveAsync(string key)
    {
        var protectedData = await _cache.GetAsync(key);
        if (protectedData == null)
            return null;

        var data = _protector.Unprotect(protectedData);
        return TicketSerializer.Default.Deserialize(data);
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }
}