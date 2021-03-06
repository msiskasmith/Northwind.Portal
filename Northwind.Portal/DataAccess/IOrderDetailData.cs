using Northwind.DataModels.Shipment;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Northwind.Portal.DataAccess
{
    public interface IOrderDetailData
    {
        Task<OrderDetailDto> GetOrderDetailAsync(short orderDetailId);
        Task<IEnumerable<OrderDetailDto>> GetOrderDetailsByOrderAsync(short orderId);
    }
}