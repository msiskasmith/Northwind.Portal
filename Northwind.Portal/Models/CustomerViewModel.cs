using Northwind.DataModels;
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
