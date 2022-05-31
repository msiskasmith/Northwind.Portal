using Northwind.DataModels;
using Northwind.DataModels.Products;
using System.Collections.Generic;

namespace Northwind.Portal.Models
{
    public class ProductViewModel
    {
        public ProductDto Product { get; set; }
        public IEnumerable<ProductDto> Products { get; set; }
        public IEnumerable<SupplierDto> Suppliers { get; set; }
        public IEnumerable<ProductCategoryDto> ProductCategories { get; set; }
    }
}
