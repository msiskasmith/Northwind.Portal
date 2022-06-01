using Northwind.DataModels;
using Northwind.DataModels.Shipment;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Northwind.Portal.DataAccess
{
    public class CustomerData : ICustomerData
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CustomerData(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<CustomerDto> GetCustomerAsync(string customerId)
        {
            var client = _httpClientFactory.CreateClient("northwindconnection");

            var customer = await client.GetFromJsonAsync<CustomerDto>(
                $"Customers/GetCustomer?customerId={customerId}");

            return customer;
        }

        public async Task<IEnumerable<CustomerDto>> GetCustomersAsync()
        {
            var client = _httpClientFactory.CreateClient("northwindconnection");

            var customers = await client.GetFromJsonAsync<IEnumerable<CustomerDto>>(
                "Customers/GetCustomers?pageNumber=1");

            return customers;
        }
    }
}
