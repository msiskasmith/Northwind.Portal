using Northwind.DataModels;
using Northwind.DataModels.Products;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Northwind.Portal.DataAccess
{
    public class SupplierData : ISupplierData
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SupplierData(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<SupplierDto> GetSupplierAsync(short supplierId)
        {
            var client = _httpClientFactory.CreateClient("northwindconnection");

            var supplier = await client.GetFromJsonAsync<SupplierDto>(
                $"Suppliers/GetSupplier?supplierId={supplierId}");

            return supplier;
        }


        public async Task<IEnumerable<SupplierDto>> GetSuppliersAsync()
        {
            var client = _httpClientFactory.CreateClient("northwindconnection");

            var suppliers = await client.GetFromJsonAsync<IEnumerable<SupplierDto>>(
                "Suppliers/GetSuppliers?pageNumber=1");

            return suppliers;
        }
    }
}
