using Northwind.DataModels;
using Northwind.DataModels.Employees;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Northwind.Portal.DataAccess
{
    public class EmployeeData : IEmployeeData
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EmployeeData(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<EmployeeDto> GetEmployeeAsync(short employeeId)
        {
            var client = _httpClientFactory.CreateClient("northwindconnection");

            var employee = await client.GetFromJsonAsync<EmployeeDto>(
                $"Employees/GetEmployee?employeeId={employeeId}");

            return employee;
        }

        public async Task<IEnumerable<EmployeeDto>> GetEmployeesAsync()
        {
            var client = _httpClientFactory.CreateClient("northwindconnection");

            var employees = await client.GetFromJsonAsync<IEnumerable<EmployeeDto>>(
                "Employees/GetEmployees?pageNumber=1");

            return employees;
        }
    }
}
