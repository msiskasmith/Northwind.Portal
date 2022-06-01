using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
    
    public class OrderDetailController : BaseController
    {
        private readonly ILogger<OrderDetailController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderDetailData _orderDetailsData;
        private readonly IProductData _productData;

        public OrderDetailController(ILogger<OrderDetailController> logger, IHttpClientFactory httpClientFactory
            , IHttpContextAccessor httpContextAccessor
            , IOrderDetailData orderDetailsData
            , IProductData productData)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _orderDetailsData = orderDetailsData;
            _productData = productData;
        }

        public async Task<IActionResult> Index([FromQuery] short orderId)
        {
            OrderDetailsViewModel orderDetailViewModel = new()
            {
                OrderDetails = await _orderDetailsData.GetOrderDetailsByOrderAsync(orderId),
                OrderId = orderId
            };

            return View(orderDetailViewModel);
        }

        public async Task<IActionResult> Details([FromQuery] short orderDetailId)
        {
            var orderDetail = await _orderDetailsData.GetOrderDetailAsync(orderDetailId);

            return View(orderDetail);
        }

        [ActionName("Create")]
        public async Task<IActionResult> Create([FromQuery] short orderId)
        {
            var orderDetailViewModel = await GetOrderDetailViewModel(orderId: orderId);

            return View(orderDetailViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderDetailsViewModel orderDetailViewModel)
        {
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient("northwindconnection");

                var response = await client.PostAsJsonAsync($"OrderDetails/AddOrderDetail"
                    , orderDetailViewModel.OrderDetail);

                if (response.IsSuccessStatusCode)
                {
                    NotifyUser("The order detail was added successfully", 
                        "Order Detail Added");

                    return Redirect($"/OrderDetail/Index?orderId={orderDetailViewModel.OrderDetail.OrderId}");
                }

                NotifyUser(response.Content.ReadAsStringAsync().Result, 
                    "Order Detail Not Added", NotificationType.error);

                CreateObjectCookie("OrderDetailDtoCookie", orderDetailViewModel.OrderDetail);

                return Redirect($"/OrderDetail/Create?orderId={orderDetailViewModel.OrderDetail.OrderId}");
            }

            orderDetailViewModel = await GetOrderDetailViewModel(orderDetailViewModel.OrderDetail);

            return View(orderDetailViewModel);
        }

        public async Task<IActionResult> Update([FromQuery] short orderDetailId)
        {
            var orderDetailViewModel = new OrderDetailsViewModel()
            {
                OrderDetail = await _orderDetailsData.GetOrderDetailAsync(orderDetailId),
                Products = await _productData.GetProductsAsync()
            };

            return View(orderDetailViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Update(OrderDetailsViewModel orderDetailViewModel)
        {
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient("northwindconnection");

                var response = await client.PutAsJsonAsync(
                    $"OrderDetails/UpdateOrderDetail", orderDetailViewModel.OrderDetail);

                if (response.IsSuccessStatusCode)
                {
                    NotifyUser("The order detail was updated successfully", "Order Detail Updated");
                    return Redirect($"/OrderDetail/Index");
                }

                NotifyUser(response.Content.ReadAsStringAsync().Result,
                "Order Detail Update Failed", NotificationType.error);

                return Redirect($"/OrderDetail/Update?orderDetailId={orderDetailViewModel.OrderDetail.OrderDetailId}");
            }

            orderDetailViewModel = await GetOrderDetailViewModel(orderDetailViewModel.OrderDetail);
            return View(orderDetailViewModel);
        }

        public async Task<IActionResult> Delete([FromQuery] short orderDetailId)
        {
            var orderDetail = await _orderDetailsData.GetOrderDetailAsync(orderDetailId);

            return View(orderDetail);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(short orderDetailId)
        {
            var client = _httpClientFactory.CreateClient("northwindconnection");
            var response = await client.DeleteAsync(
                $"OrderDetails/DeleteOrderDetail?orderDetailId={orderDetailId}");

            if (response.IsSuccessStatusCode)
            {
                NotifyUser("The order detail was deleted successfully", "Order Detail Deleted");

                return Redirect(
                "/OrderDetail/Index");
            }

            NotifyUser(response.Content.ReadAsStringAsync().Result, 
                "Order Detail Not Deleted", NotificationType.error);

            return Redirect("/OrderDetail/Index");
        }

        public OrderDetailDto GetOrderDetailObject(short orderId = 0)
        {
            var orderDetailDto = new OrderDetailDto();

            orderDetailDto = GetObjectCookie<OrderDetailDto>(_httpContextAccessor,
                "OrderDetailDtoCookie", orderDetailDto);

            if(orderId != 0)
            {
                orderDetailDto.OrderId = orderId;
            }

            return orderDetailDto;
        }

        public async Task<OrderDetailsViewModel> GetOrderDetailViewModel(OrderDetailDto orderDetail = null, short orderId = 0)
        {
            if (orderDetail == null)
            {
                orderDetail = GetOrderDetailObject(orderId);
            }

            var orderDetailViewModel = new OrderDetailsViewModel()
            {
                OrderDetail = orderDetail,
                Products = await _productData.GetProductsAsync()
            };

            return orderDetailViewModel;
        }
    }
}
