using Northwind.DataModels;
using Northwind.DataModels.Location;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Northwind.Portal.DataAccess
{
    public class RegionData : IRegionData
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RegionData(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<RegionDto> GetRegionAsync(short regionId)
        {
            var client = _httpClientFactory.CreateClient("northwindconnection");

            var region = await client.GetFromJsonAsync<RegionDto>(
                $"Regions/GetRegion?regionId={regionId}");

            return region;
        }

        public async Task<IEnumerable<RegionDto>> GetRegionsAsync()
        {
            var client = _httpClientFactory.CreateClient("northwindconnection");

            var regions = await client.GetFromJsonAsync<IEnumerable<RegionDto>>(
                "Regions/GetRegions?pageNumber=1");

            return regions;
        }
    }
}
