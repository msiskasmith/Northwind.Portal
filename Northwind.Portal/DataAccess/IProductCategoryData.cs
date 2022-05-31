using Northwind.DataModels.Products;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Northwind.Portal.DataAccess
{
    public interface IProductCategoryData
    {
        Task<IEnumerable<ProductCategoryDto>> GetProductCategoriesAsync();
        Task<ProductCategoryDto> GetProductCategoryAsync(short productCategoryId);
    }
}