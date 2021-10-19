using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WIMP_Server.Dtos.Esi;

namespace WIMP_Server.DataServices.Http
{
    public class EsiDataClient : IEsiDataClient
    {
        private const int CHUNK_SIZE = 32;
        private const int SHIP_CATEGORY = 6;
        private readonly HttpClient _httpClient;
        private readonly ILogger<EsiDataClient> _logger;
        private readonly string _esiService;

        public EsiDataClient(HttpClient httpClient, IConfiguration configuration, ILogger<EsiDataClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            _esiService = configuration["EsiService"];
        }

        public async Task<IEnumerable<EsiNameIdPairDto>> GetAllShips()
        {
            var categoryResponse = await _httpClient
                .GetAsync($"{_esiService}/latest/universe/categories/{SHIP_CATEGORY}")
                .ConfigureAwait(true);
            if (!categoryResponse.IsSuccessStatusCode)
            {
                _logger.LogError($"couldn't fetch categories: {categoryResponse.StatusCode}");
                return null;
            }

            var category = JsonSerializer.Deserialize<EsiReadCategoryDto>(
                await categoryResponse.Content.ReadAsStringAsync()
                    .ConfigureAwait(true));

            var types = new List<int>();
            foreach (var groupId in category.Groups)
            {
                var groupResponse = await _httpClient
                    .GetAsync($"{_esiService}/latest/universe/groups/{groupId}")
                    .ConfigureAwait(true);
                if (!categoryResponse.IsSuccessStatusCode)
                {
                    _logger.LogError($"couldn't fetch group {groupId}: {groupResponse.StatusCode}");
                    continue;
                }

                var group = JsonSerializer.Deserialize<EsiReadGroupDto>(
                    await groupResponse.Content.ReadAsStringAsync()
                        .ConfigureAwait(true));

                types.AddRange(group.Types);
            }

            var requestJson = JsonSerializer.Serialize(types);
            var requestContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
            var namesResponse = await _httpClient
                .PostAsync($"{_esiService}/latest/universe/names/", requestContent)
                .ConfigureAwait(true);
            if (!namesResponse.IsSuccessStatusCode)
            {
                _logger.LogError($"couldn't fetch ship type names: {namesResponse.StatusCode}");
                return null;
            }

            var ships = JsonSerializer.Deserialize<IEnumerable<EsiNameIdPairDto>>(
                await namesResponse.Content.ReadAsStringAsync()
                    .ConfigureAwait(true));

            return ships;
        }

        public async Task<IEnumerable<EsiReadSystemDto>> GetAllSystems()
        {
            var systemsResponse = await _httpClient
                .GetAsync($"{_esiService}/latest/universe/systems/")
                .ConfigureAwait(true);
            if (!systemsResponse.IsSuccessStatusCode)
            {
                _logger.LogError($"couldn't fetch systems: {systemsResponse.StatusCode}");
                throw new Exception($"couldn't fetch systems: {systemsResponse.StatusCode}");
            }

            var allSystemIds = JsonSerializer.Deserialize<IEnumerable<int>>(
                await systemsResponse.Content.ReadAsStringAsync()
                    .ConfigureAwait(true));

            _logger.LogInformation($"fetching information for {allSystemIds.Count()} systems");

            var allSystems = new List<EsiReadSystemDto>();

            while (allSystemIds.Any())
            {
                var fetchSystemsWithIds = allSystemIds.Take(CHUNK_SIZE);
                var getSystemTasks = fetchSystemsWithIds
                    .Select(id => GetSystemWithId(id))
                    .ToArray();

                Task.WaitAll(getSystemTasks);

                var failedIds = fetchSystemsWithIds.Where((_, index) => getSystemTasks[index].Result == null);
                if (failedIds.Any())
                {
                    _logger.LogWarning($"failed to fetch {failedIds.Count()} systems, adding them to queue for retry");
                    allSystemIds = allSystemIds.Concat(failedIds);
                }

                allSystemIds = allSystemIds.Skip(CHUNK_SIZE);

                allSystems.AddRange(getSystemTasks.Where(t => t.Result != null).Select(t => t.Result));
            }

            return allSystems;
        }

        public async Task<EsiUniverseBulkSearchResponseDto> UniverseSearchNames(IEnumerable<string> names)
        {
            var requestJson = JsonSerializer.Serialize(names);
            var requestContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_esiService}/latest/universe/ids/", requestContent)
                .ConfigureAwait(true);
            _logger.LogDebug($"UniverseSearchNames request response: {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<EsiUniverseBulkSearchResponseDto>(
                    await response.Content.ReadAsStringAsync().ConfigureAwait(true));
            }
            else
            {
                _logger.LogError($"UniverseSearchNames request failed with status: {response.StatusCode}, content body: {requestJson}");
                return EsiUniverseBulkSearchResponseDto.Empty;
            }
        }

        public async Task<EsiReadSystemDto> GetSystemWithId(int id)
        {
            var systemResponse = await _httpClient
                .GetAsync($"{_esiService}/latest/universe/systems/{id}/")
                .ConfigureAwait(true);
            if (!systemResponse.IsSuccessStatusCode)
            {
                _logger.LogError($"couldn't fetch system: {systemResponse.StatusCode}");
                return null;
            }

            return JsonSerializer.Deserialize<EsiReadSystemDto>(
                await systemResponse.Content.ReadAsStringAsync()
                    .ConfigureAwait(true));
        }

        public async Task<EsiReadStargateDto> GetStargateWithId(int id)
        {
            var stargateResponse = await _httpClient
                .GetAsync($"{_esiService}/latest/universe/stargates/{id}")
                .ConfigureAwait(true);
            if (!stargateResponse.IsSuccessStatusCode)
            {
                _logger.LogError($"couldn't fetch stargate: {stargateResponse.StatusCode}");
                return null;
            }

            return JsonSerializer.Deserialize<EsiReadStargateDto>(
                await stargateResponse.Content.ReadAsStringAsync()
                    .ConfigureAwait(true));
        }

        public Task<IEnumerable<EsiReadStargateDto>> GetAllStargatesInSystems(IEnumerable<EsiReadSystemDto> systems)
        {
            var allStargateIds = new List<int>();
            foreach (var system in systems)
            {
                if (system.Stargates != null)
                {
                    allStargateIds.AddRange(system.Stargates);
                }
            }

            var fetchStargateIds = allStargateIds.AsEnumerable();

            var stargates = new List<EsiReadStargateDto>();

            _logger.LogInformation($"fetching information for {fetchStargateIds.Count()} stargates");

            while (fetchStargateIds.Any())
            {
                var fetchStargatesWithIds = fetchStargateIds.Take(CHUNK_SIZE);
                var getStargateTasks = fetchStargatesWithIds
                    .Select(id => GetStargateWithId(id))
                    .ToArray();

                Task.WaitAll(getStargateTasks);

                var failedIds = fetchStargatesWithIds.Where((_, index) => getStargateTasks[index].Result == null);
                if (failedIds.Any())
                {
                    _logger.LogWarning($"failed to fetch {failedIds.Count()} stargates, adding them to queue for retry");
                    fetchStargateIds = fetchStargateIds.Concat(failedIds);
                }

                fetchStargateIds = fetchStargateIds.Skip(CHUNK_SIZE);

                stargates.AddRange(getStargateTasks.Where(t => t.Result != null).Select(t => t.Result));
            }

            return Task.FromResult<IEnumerable<EsiReadStargateDto>>(stargates);
        }
    }
}