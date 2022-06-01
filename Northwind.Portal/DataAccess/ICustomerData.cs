using Northwind.DataModels;
using Northwind.DataModels.Shipment;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Northwind.Portal.DataAccess
{
    public interface ICustomerData
    {
        Task<CustomerDto> GetCustomerAsync(string customerId);
        Task<IEnumerable<CustomerDto>> GetCustomersAsync();
    }
}