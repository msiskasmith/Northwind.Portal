using Northwind.DataModels;
using Northwind.DataModels.Employees;
using Northwind.DataModels.Location;
using Northwind.DataModels.Shipment;
using System.Collections.Generic;

namespace Northwind.Portal.Models
{
    public class OrderViewModel
    {
        public OrderDto Order { get; set; }
        public IEnumerable<OrderDto> Orders { get; set; }
        public IEnumerable<ShipperDto> Shippers { get; set; }
        public IEnumerable<RegionDto> ShipRegions { get; set; }
        public IEnumerable<CustomerDto> Customers { get; set; }
        public IEnumerable<EmployeeDto> Employees { get; set; }
        public bool IsErrorResponse { get; set; } = false;
        public bool IsSuccessResponse { get; set; } = false;
    }
}
