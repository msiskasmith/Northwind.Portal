using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Northwind.DataModels;
using Northwind.Portal.DataAccess;
using Northwind.Portal.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Northwind.Portal.Controllers
{
    public class ShipperController : BaseController
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IShipperData _shipperData;

        public ShipperController(IHttpClientFactory httpClientFactory
            , IHttpContextAccessor httpContextAccessor
            , IShipperData shipperData)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _shipperData = shipperData;
        }
        public async Task<IActionResult> Index()
        {
            ShipperViewModel shipperViewModel = new()
            {
                Shippers = await _shipperData.GetShippersAsync(),
            };

            return View(shipperViewModel);
        }

        public async Task<IActionResult> Details([FromQuery] short shipperId)
        {
            var shipper = await _shipperData.GetShipperAsync(shipperId);

            return View(shipper);
        }

        [ActionName("Create")]
        public IActionResult Create()
        {
            var shipperViewModel = new ShipperViewModel()
            {
                Shipper = GetShipperObject()
            };

            return View(shipperViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ShipperViewModel shipperViewModel)
        {


            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient("northwindconnection");

                var response = await client.PostAsJsonAsync($"Shippers/AddShipper"
                    , shipperViewModel.Shipper);

                if (response.IsSuccessStatusCode)
                {
                    NotifyUser("The shipper was added successfully", "Shipper Added");

                    return Redirect($"/Shipper/Index");
                }

                NotifyUser(response.Content.ReadAsStringAsync().Result, 
                    "Shipper Not Added", NotificationType.error);
            }

            CreateObjectCookie("ShipperDtoCookie", shipperViewModel.Shipper);

            return Redirect("/Shipper/Create");
        }

        public async Task<IActionResult> Update([FromQuery] short shipperId)
        {
            var shipperViewModel = new ShipperViewModel()
            {
                Shipper = await _shipperData.GetShipperAsync(shipperId)
            };

            return View(shipperViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Update(ShipperViewModel shipperViewModel)
        {
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient("northwindconnection");

                var response = await client.PutAsJsonAsync(
                    $"Shippers/UpdateShipper", shipperViewModel.Shipper);

                if (response.IsSuccessStatusCode)
                {
                    NotifyUser("The shipper was updated successfully", "Shipper Updated");
                    return Redirect($"/Shipper/Index");
                }

                NotifyUser(response.Content.ReadAsStringAsync().Result, 
                    "Shipper Update Failed", NotificationType.error);
            };

            return Redirect($"/Shipper/Update?shipperId={shipperViewModel.Shipper.ShipperId}");
        }

        public async Task<IActionResult> Delete([FromQuery] short shipperId)
        {
            var shipper = await _shipperData.GetShipperAsync(shipperId);

            return View(shipper);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(short shipperId)
        {
            var client = _httpClientFactory.CreateClient("northwindconnection");
            var response = await client.DeleteAsync(
                $"Shippers/DeleteShipper?shipperId={shipperId}");

            if (response.IsSuccessStatusCode)
            {
                NotifyUser("The shipper was deleted successfully", "Shipper Deleted");

                return Redirect(
                "/Shipper/Index");
            }

            NotifyUser(response.Content.ReadAsStringAsync().Result, 
                "Shipper Not Deleted", NotificationType.error);

            return Redirect("/Shipper/Index");
        }

        public ShipperDto GetShipperObject()
        {
            var shipperDto = new ShipperDto();

            shipperDto = GetObjectCookie<ShipperDto>(_httpContextAccessor,
                "ShipperDtoCookie", shipperDto);

            return shipperDto;
        }
    }
}
