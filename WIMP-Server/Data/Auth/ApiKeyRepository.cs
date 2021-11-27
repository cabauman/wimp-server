using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WIMP_Server.Models.Auth;

namespace WIMP_Server.Data.Auth;

public class ApiKeyRepository : IApiKeyRepository
{
    private readonly WimpDbContext _wimpDbContext;

    public ApiKeyRepository(WimpDbContext wimpDbContext)
    {
        _wimpDbContext = wimpDbContext;
    }

    public void Add(ApiKey key)
    {
        _wimpDbContext.ApiKeys.Add(key);
    }

    public void Delete(ApiKey key)
    {
        _wimpDbContext.ApiKeys.Remove(key);
    }

    public ApiKey Get(string apiKey)
    {
        return _wimpDbContext.ApiKeys
            .Include(apiKey => apiKey.Roles)
            .FirstOrDefault(key => key.Key == apiKey);
    }

    public IEnumerable<ApiKey> GetByOwner(string owner)
    {
        return _wimpDbContext.ApiKeys
            .Where(apiKey => apiKey.Owner == owner)
            .Include(apiKey => apiKey.Roles)
            .AsEnumerable();
    }

    public bool Save()
    {
        return _wimpDbContext.SaveChanges() > 0;
    }
}
