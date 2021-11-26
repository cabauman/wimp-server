using System.Collections.Generic;
using System.Threading.Tasks;
using WIMP_Server.Dtos.Esi;

namespace WIMP_Server.DataServices.Http;

public interface IEsiDataClient
{
    Task<EsiUniverseBulkSearchResponseDto> UniverseSearchNames(IEnumerable<string> names);
    Task<IEnumerable<EsiNameIdPairDto>> GetAllShips();
    Task<IEnumerable<EsiReadSystemDto>> GetAllSystems();
    Task<EsiReadSystemDto> GetSystemWithId(int id);
    Task<EsiReadStargateDto> GetStargateWithId(int id);
    Task<IEnumerable<EsiReadStargateDto>> GetAllStargatesInSystems(IEnumerable<EsiReadSystemDto> systems);
}
