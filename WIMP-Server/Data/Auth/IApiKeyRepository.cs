using System.Collections.Generic;
using System.Threading.Tasks;
using WIMP_Server.Models.Auth;

namespace WIMP_Server.Data.Auth;

public interface IApiKeyRepository
{
    void Add(ApiKey key);

    ApiKey Get(string apiKey);

    IEnumerable<ApiKey> GetByOwner(string owner);

    void Delete(ApiKey key);

    bool Save();
}
