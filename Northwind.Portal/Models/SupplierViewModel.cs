using Northwind.DataModels;
using Northwind.DataModels.Location;
using Northwind.DataModels.Products;
using System.Collections.Generic;

namespace Northwind.Portal.Models
{
    public class SupplierViewModel
    {
        public SupplierDto Supplier { get; set; }
        public IEnumerable<SupplierDto> Suppliers { get; set; }
        public IEnumerable<RegionDto> Regions { get; set; }
    }
}
