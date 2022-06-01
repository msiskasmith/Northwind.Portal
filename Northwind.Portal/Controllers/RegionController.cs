using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Northwind.DataModels;
using Northwind.DataModels.Location;
using Northwind.Portal.DataAccess;
using Northwind.Portal.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Northwind.Portal.Controllers
{
    public class RegionController : BaseController
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRegionData _regionData;

        public RegionController(IHttpClientFactory httpClientFactory
            , IHttpContextAccessor httpContextAccessor
            , IRegionData regionData)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _regionData = regionData;
        }
        public async Task<IActionResult> Index()
        {
            RegionViewModel regionViewModel = new()
            {
                Regions = await _regionData.GetRegionsAsync(),
            };

            return View(regionViewModel);
        }

        public async Task<IActionResult> Details([FromQuery] short regionId)
        {
            var region = await _regionData.GetRegionAsync(regionId);

            return View(region);
        }

        [ActionName("Create")]
        public IActionResult Create()
        {
            var regionViewModel = new RegionViewModel()
            {
                Region = GetRegionObject()
            };

            return View(regionViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RegionViewModel regionViewModel)
        {


            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient("northwindconnection");

                var response = await client.PostAsJsonAsync($"Regions/AddRegion"
                    , regionViewModel.Region);

                if (response.IsSuccessStatusCode)
                {
                    NotifyUser("The region was added successfully", "Region Added");

                    return Redirect($"/Region/Index");
                }

                NotifyUser(response.Content.ReadAsStringAsync().Result, 
                    "Region Not Added", NotificationType.error);

                CreateObjectCookie("RegionDtoCookie", regionViewModel.Region);

                return Redirect("/Region/Create");
            }

            return View(regionViewModel);
        }

        public async Task<IActionResult> Update([FromQuery] short regionId)
        {
            var regionViewModel = new RegionViewModel()
            {
                Region = await _regionData.GetRegionAsync(regionId)
            };

            return View(regionViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Update(RegionViewModel regionViewModel)
        {
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient("northwindconnection");

                var response = await client.PutAsJsonAsync(
                    $"Regions/UpdateRegion", regionViewModel.Region);

                if (response.IsSuccessStatusCode)
                {
                    NotifyUser("The region was updated successfully", 
                        "Region Updated");
                    return Redirect($"/Region/Index");
                }

                NotifyUser(response.Content.ReadAsStringAsync().Result,
                "Region Update Failed", NotificationType.error);

                return Redirect($"/Region/Update?regionId={regionViewModel.Region.RegionId}");
            }

            return View(regionViewModel);
        }

        public async Task<IActionResult> Delete([FromQuery] short regionId)
        {
            var region = await _regionData.GetRegionAsync(regionId);

            return View(region);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(short regionId)
        {
            var client = _httpClientFactory.CreateClient("northwindconnection");
            var response = await client.DeleteAsync(
                $"Regions/DeleteRegion?regionId={regionId}");

            if (response.IsSuccessStatusCode)
            {
                NotifyUser("The region was deleted successfully", "Region Deleted");

                return Redirect(
                "/Region/Index");
            }

            NotifyUser(response.Content.ReadAsStringAsync().Result, 
                "Region Not Deleted", NotificationType.error);

            return Redirect("/Region/Index");
        }
        public RegionDto GetRegionObject()
        {
            var regionDto = new RegionDto();

            regionDto = GetObjectCookie<RegionDto>(_httpContextAccessor, 
                "RegionDtoCookie", regionDto);

            return regionDto;
        }
    }
}
