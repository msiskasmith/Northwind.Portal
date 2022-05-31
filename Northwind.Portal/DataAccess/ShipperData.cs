using Northwind.DataModels;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Northwind.Portal.DataAccess
{
    public class ShipperData : IShipperData
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ShipperData(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ShipperDto> GetShipperAsync(short shipperId)
        {
            var client = _httpClientFactory.CreateClient("northwindconnection");

            var shipper = await client.GetFromJsonAsync<ShipperDto>(
                $"Shippers/GetShipper?shipperId={shipperId}");

            return shipper;
        }

        public async Task<IEnumerable<ShipperDto>> GetShippersAsync()
        {
            var client = _httpClientFactory.CreateClient("northwindconnection");

            var shippers = await client.GetFromJsonAsync<IEnumerable<ShipperDto>>(
                "Shippers/GetShippers?pageNumber=1");

            return shippers;
        }
    }
}
