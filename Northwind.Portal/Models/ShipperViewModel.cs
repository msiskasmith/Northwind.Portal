using Northwind.DataModels;
using System.Collections.Generic;

namespace Northwind.Portal.Models
{
    public class ShipperViewModel
    {
        public ShipperDto Shipper { get; set; }
        public IEnumerable<ShipperDto> Shippers { get; set; }
    }
}
