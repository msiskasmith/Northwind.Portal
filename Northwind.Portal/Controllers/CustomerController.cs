using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Northwind.DataModels;
using Northwind.DataModels.Shipment;
using Northwind.Portal.DataAccess;
using Northwind.Portal.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Northwind.Portal.Controllers
{
    public class CustomerController : BaseController
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICustomerData _customerData;
        private readonly IRegionData _regionData;

        public CustomerController(IHttpClientFactory httpClientFactory
            , IHttpContextAccessor httpContextAccessor
            , ICustomerData customerData
            , IRegionData regionData)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _customerData = customerData;
            _regionData = regionData;
        }
        public async Task<IActionResult> Index()
        {
            CustomerViewModel customerViewModel = new()
            {
                Customers = await _customerData.GetCustomersAsync(),
            };

            return View(customerViewModel);
        }

        public async Task<IActionResult> Details([FromQuery] string customerId)
        {
            var customer = await _customerData.GetCustomerAsync(customerId);

            return View(customer);
        }

        [ActionName("Create")]
        public async Task<IActionResult> Create()
        {
            var customerViewModel = await GetCustomerViewModel();

            return View(customerViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CustomerViewModel customerViewModel)
        {
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient("northwindconnection");

                var response = await client.PostAsJsonAsync($"Customers/AddCustomer"
                    , customerViewModel.Customer);

                if (response.IsSuccessStatusCode)
                {
                    NotifyUser("The customer was added successfully", "Customer Added");

                    return Redirect($"/Customer/Index");
                }

                NotifyUser(response.Content.ReadAsStringAsync().Result, 
                    "Customer Not Added", NotificationType.error);

                CreateObjectCookie("CustomerDtoCookie", customerViewModel.Customer);

                return Redirect("/Customer/Create");
            }

            customerViewModel = await GetCustomerViewModel(customerViewModel.Customer);

            return View(customerViewModel);
        }

        public async Task<IActionResult> Update([FromQuery] string customerId)
        {
            var customerViewModel = new CustomerViewModel()
            {
                Customer = await _customerData.GetCustomerAsync(customerId),
                Regions = await _regionData.GetRegionsAsync()
            };

            return View(customerViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Update(CustomerViewModel customerViewModel)
        {
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient("northwindconnection");

                var response = await client.PutAsJsonAsync(
                    $"Customers/UpdateCustomer", customerViewModel.Customer);

                if (response.IsSuccessStatusCode)
                {
                    NotifyUser("The customer was updated successfully", "Customer Updated");
                    return Redirect($"/Customer/Index");
                }

                NotifyUser(response.Content.ReadAsStringAsync().Result, 
                    "Customer Update Failed", NotificationType.error);

                return Redirect($"/Customer/Update?customerId={customerViewModel.Customer.CustomerId}");
            }

            customerViewModel = await GetCustomerViewModel(customerViewModel.Customer);

            return View(customerViewModel);
        }

        public async Task<IActionResult> Delete([FromQuery] string customerId)
        {
            var customer = await _customerData.GetCustomerAsync(customerId);

            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string customerId)
        {
            var client = _httpClientFactory.CreateClient("northwindconnection");
            var response = await client.DeleteAsync(
                $"Customers/DeleteCustomer?customerId={customerId}");

            if (response.IsSuccessStatusCode)
            {
                NotifyUser("The customer was deleted successfully", "Customer Deleted");

                return Redirect(
                "/Customer/Index");
            }

            NotifyUser(response.Content.ReadAsStringAsync().Result, 
                "Customer Not Deleted", NotificationType.error);

            return Redirect("/Customer/Index");
        }

        public CustomerDto GetCustomerObject()
        {
            var customerDto = new CustomerDto();

            customerDto = GetObjectCookie<CustomerDto>(_httpContextAccessor,
                "CustomerDtoCookie", customerDto);

            return customerDto;
        }

        public async Task<CustomerViewModel> GetCustomerViewModel(CustomerDto customer = null)
        {
            if(customer == null)
            {
                customer = GetCustomerObject();
            }

            var customerViewModel = new CustomerViewModel()
            {
                Customer = customer,
                Regions = await _regionData.GetRegionsAsync(),
                Customers = await _customerData.GetCustomersAsync(),
            };

            return customerViewModel;
        }
    }
}
