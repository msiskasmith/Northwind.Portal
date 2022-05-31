using Northwind.DataModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Northwind.Portal.DataAccess
{
    public interface IProductData
    {
        Task<ProductDto> GetProductAsync(short productId);
        Task<IEnumerable<ProductDto>> GetProductsAsync();
    }
}