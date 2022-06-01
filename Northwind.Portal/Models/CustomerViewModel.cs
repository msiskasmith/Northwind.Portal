using Northwind.DataModels;
using Northwind.DataModels.Location;
using Northwind.DataModels.Shipment;
using System.Collections.Generic;

namespace Northwind.Portal.Models
{
    public class CustomerViewModel
    {
        public CustomerDto Customer { get; set; }
        public IEnumerable<CustomerDto> Customers { get; set; }
        public IEnumerable<RegionDto> Regions { get; set; }          
    }
}
