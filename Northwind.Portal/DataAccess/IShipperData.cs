using Northwind.DataModels;
using Northwind.DataModels.Shipment;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Northwind.Portal.DataAccess
{
    public interface IShipperData
    {
        Task<ShipperDto> GetShipperAsync(short shipperId);
        Task<IEnumerable<ShipperDto>> GetShippersAsync();
    }
}