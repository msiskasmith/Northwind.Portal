using Northwind.DataModels.Products;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Northwind.Portal.DataAccess
{
    public class ProductCategoryData : IProductCategoryData
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductCategoryData(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<ProductCategoryDto> GetProductCategoryAsync(short productCategoryId)
        {
            var client = _httpClientFactory.CreateClient("northwindconnection");

            var productCategory = await client.GetFromJsonAsync<ProductCategoryDto>(
                $"ProductCategories/GetProductCategory?productCategoryId={productCategoryId}");

            return productCategory;
        }

        public async Task<IEnumerable<ProductCategoryDto>> GetProductCategoriesAsync()
        {
            var client = _httpClientFactory.CreateClient("northwindconnection");

            var productCategories = await client.GetFromJsonAsync<IEnumerable<ProductCategoryDto>>(
                "ProductCategories/GetProductCategories?pageNumber=1");

            return productCategories;
        }
    }
}
