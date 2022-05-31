using Northwind.DataModels.Shipment;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Northwind.Portal.DataAccess
{
    public interface IOrderData
    {
        Task<OrderDto> GetOrderAsync(short orderId);
        Task<IEnumerable<OrderDto>> GetOrdersAsync();
    }
}