using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Northwind.DataModels;
using Northwind.DataModels.Shipment;
using Northwind.Portal.DataAccess;
using Northwind.Portal.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Northwind.Portal.Controllers
{
    public class OrderController : BaseController
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderData _orderData;
        private readonly IShipperData _shipperData;
        private readonly IRegionData _regionData;
        private readonly IEmployeeData _employeeData;
        private readonly ICustomerData _customerData;

        public OrderController(IHttpClientFactory httpClientFactory
            , IHttpContextAccessor httpContextAccessor
            , IOrderData orderData
            , IShipperData shipperData
            , IRegionData regionData
            , IEmployeeData employeeData
            , ICustomerData customerData)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _orderData = orderData;
            _shipperData = shipperData;
            _regionData = regionData;
            _employeeData = employeeData;
            _customerData = customerData;
        }
        public async Task<IActionResult> Index()
        {
            OrderViewModel orderViewModel = new()
            {
                Orders = await _orderData.GetOrdersAsync(),
            };

            return View(orderViewModel);
        }

        public async Task<IActionResult> Details([FromQuery] short orderId)
        {
            var order = await _orderData.GetOrderAsync(orderId);

            return View(order);
        }

        [ActionName("Create")]
        public async Task<IActionResult> Create()
        {
            var orderViewModel = await GetOrderViewModel();

            return View(orderViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderViewModel orderViewModel)
        {
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient("northwindconnection");

                var response = await client.PostAsJsonAsync($"Orders/AddOrder"
                    , orderViewModel.Order);

                if (response.IsSuccessStatusCode)
                {
                    NotifyUser("The order was added successfully", "Order Added");

                    return Redirect($"/Order/Index");
                }

                NotifyUser(response.Content.ReadAsStringAsync().Result, 
                    "Order Not Added", NotificationType.error);

                CreateObjectCookie("OrderDtoCookie", orderViewModel.Order);

                return Redirect("/Order/Create");
            }

            orderViewModel = await GetOrderViewModel(orderViewModel.Order);

            return View(orderViewModel);
        }

        public async Task<IActionResult> Update([FromQuery] short orderId)
        {
            var orderViewModel = await GetOrderViewModel(await _orderData.GetOrderAsync(orderId));
            
            return View(orderViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Update(OrderViewModel orderViewModel)
        {
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient("northwindconnection");

                var response = await client.PutAsJsonAsync(
                    $"Orders/UpdateOrder", orderViewModel.Order);

                if (response.IsSuccessStatusCode)
                {
                    NotifyUser("The order was updated successfully", "Order Updated");
                    return Redirect($"/Order/Index");
                }

                NotifyUser(response.Content.ReadAsStringAsync().Result, 
                    "Order Update Failed", NotificationType.error);

                return Redirect($"/Order/Update?orderId={orderViewModel.Order.OrderId}");
            }

            orderViewModel = await GetOrderViewModel(orderViewModel.Order);

            return View(orderViewModel);
            
        }

        public async Task<IActionResult> Delete([FromQuery] short orderId)
        {
            var order = await _orderData.GetOrderAsync(orderId);

            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(short orderId)
        {
            var client = _httpClientFactory.CreateClient("northwindconnection");
            var response = await client.DeleteAsync(
                $"Orders/DeleteOrder?orderId={orderId}");

            if (response.IsSuccessStatusCode)
            {
                NotifyUser("The order was deleted successfully", "Order Deleted");

                return Redirect(
                "/Order/Index");
            }

            NotifyUser(response.Content.ReadAsStringAsync().Result, 
                "Order Not Deleted", NotificationType.error);

            return Redirect("/Order/Index");
        }

        public OrderDto GetOrderObject()
        {
            var orderDto = new OrderDto();

            orderDto = GetObjectCookie<OrderDto>(_httpContextAccessor,
                "OrderDtoCookie", orderDto);

            return orderDto;
        }

        public async Task<OrderViewModel> GetOrderViewModel(OrderDto order = null)
        {
            if (order == null)
            {
                order = GetOrderObject();
            }

            var orderViewModel = new OrderViewModel()
            {
                Order = order,
                ShipRegions = await _regionData.GetRegionsAsync(),
                Employees = await _employeeData.GetEmployeesAsync(),
                Customers = await _customerData.GetCustomersAsync(),
                Shippers = await _shipperData.GetShippersAsync()
            };

            return orderViewModel;
        }
    }
}
