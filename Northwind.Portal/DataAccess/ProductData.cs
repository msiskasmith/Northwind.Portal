using Northwind.DataModels;
using Northwind.DataModels.Products;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Northwind.Portal.DataAccess
{
    public class ProductData : IProductData
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductData(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<ProductDto> GetProductAsync(short productId)
        {
            var client = _httpClientFactory.CreateClient("northwindconnection");

            var product = await client.GetFromJsonAsync<ProductDto>(
                $"Products/GetProduct?productId={productId}");

            return product;
        }


        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            var client = _httpClientFactory.CreateClient("northwindconnection");

            var products = await client.GetFromJsonAsync<IEnumerable<ProductDto>>(
                "Products/GetProducts?pageNumber=1");

            return products;
        }
    }
}
