using Northwind.DataModels;
using Northwind.DataModels.Shipment;
using System.Collections.Generic;

namespace Northwind.Portal.Models
{
    public class OrderDetailsViewModel
    {
        public short OrderId { get; set; }  
        public OrderDetailDto OrderDetail { get; set; }
        public IEnumerable<OrderDetailDto> OrderDetails { get; set; }
        public IEnumerable<ProductDto> Products { get; set; }
    }
}
