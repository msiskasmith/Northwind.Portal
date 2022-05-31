using Northwind.DataModels.Shipment;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Northwind.Portal.DataAccess
{
    public class OrderData : IOrderData
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public OrderData(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<OrderDto> GetOrderAsync(short orderId)
        {
            var client = _httpClientFactory.CreateClient("northwindconnection");

            var order = await client.GetFromJsonAsync<OrderDto>(
                $"Orders/GetOrder?orderId={orderId}");

            return order;
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersAsync()
        {
            var client = _httpClientFactory.CreateClient("northwindconnection");

            var orders = await client.GetFromJsonAsync<IEnumerable<OrderDto>>(
                "Orders/GetOrders?pageNumber=1");

            return orders;
        }
    }
}
