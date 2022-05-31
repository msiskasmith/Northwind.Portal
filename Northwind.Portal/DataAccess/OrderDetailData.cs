using Northwind.DataModels.Shipment;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Northwind.Portal.DataAccess
{
    public class OrderDetailData : IOrderDetailData
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public OrderDetailData(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<OrderDetailDto> GetOrderDetailAsync(short orderDetailId)
        {
            var client = _httpClientFactory.CreateClient("northwindconnection");

            var orderDetail = await client.GetFromJsonAsync<OrderDetailDto>(
                $"OrderDetails/GetOrderDetail?orderDetailId={orderDetailId}");

            return orderDetail;
        }

        public async Task<IEnumerable<OrderDetailDto>> GetOrderDetailsByOrderAsync(short orderId)
        {
            var client = _httpClientFactory.CreateClient("northwindconnection");

            var orderDetails = await client.GetFromJsonAsync<IEnumerable<OrderDetailDto>>(
                $"OrderDetails/GetOrderDetailsByOrder?orderId={orderId}&page=1");

            return orderDetails;
        }
    }
}
