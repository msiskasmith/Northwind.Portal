using Northwind.DataModels;
using Northwind.DataModels.Products;
using System.Collections.Generic;

namespace Northwind.Portal.Models
{
    public class ProductCategoryViewModel
    {
        public ProductCategoryDto ProductCategory { get; set; }
        public IEnumerable<ProductCategoryDto> ProductCategories { get; set; }
    }
}
