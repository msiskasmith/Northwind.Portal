
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Northwind.DataModels.Products;
using Northwind.Portal.DataAccess;
using Northwind.Portal.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Northwind.Portal.Controllers
{
    public class SupplierController : BaseController
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISupplierData _supplierData;
        private readonly IRegionData _regionData;

        public SupplierController(IHttpClientFactory httpClientFactory
            , IHttpContextAccessor httpContextAccessor
            , ISupplierData supplierData
            , IRegionData regionData)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _supplierData = supplierData;
            _regionData = regionData;
        }
        public async Task<IActionResult> Index()
        {
            SupplierViewModel supplierViewModel = new()
            {
                Suppliers = await _supplierData.GetSuppliersAsync(),
            };

            return View(supplierViewModel);
        }
        public async Task<IActionResult> Details([FromQuery] short supplierId)
        {
            var supplier = await _supplierData.GetSupplierAsync(supplierId);

            return View(supplier);
        }

        [ActionName("Create")]
        public async Task<IActionResult> Create()
        {
            var supplierViewModel = new SupplierViewModel()
            {
                Supplier = GetSupplierObject(),
                Regions = await _regionData.GetRegionsAsync()
            };

            return View(supplierViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SupplierViewModel supplierViewModel)
        {
            

            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient("northwindconnection");

                var response = await client.PostAsJsonAsync($"Suppliers/AddSupplier"
                    , supplierViewModel.Supplier);

                if (response.IsSuccessStatusCode)
                {
                    NotifyUser("The supplier was added successfully", 
                        "Supplier Added");

                    return Redirect($"/Supplier/Index");
                }

                NotifyUser(response.Content.ReadAsStringAsync().Result, 
                    "Supplier Not Added", NotificationType.error);

                CreateObjectCookie("SupplierDtoCookie", supplierViewModel.Supplier);

                return Redirect($"/Suppler/Create?supplierId={supplierViewModel.Supplier.SupplierId}");
            }

            supplierViewModel = await GetSupplierViewModel(supplierViewModel.Supplier);

            return View(supplierViewModel);
        }

        public async Task<IActionResult> Update([FromQuery] short supplierId)
        {
            var supplierViewModel = new SupplierViewModel()
            {
                Supplier = await _supplierData.GetSupplierAsync(supplierId),
                Regions = await _regionData.GetRegionsAsync()
            };

            return View(supplierViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Update(SupplierViewModel supplierViewModel)
        {
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient("northwindconnection");

                var response = await client.PutAsJsonAsync(
                    $"Suppliers/UpdateSupplier", supplierViewModel.Supplier);

                if (response.IsSuccessStatusCode)
                {
                    NotifyUser("The supplier was updated successfully", 
                        "Supplier Updated");

                    return Redirect($"/Supplier/Index");
                }

                NotifyUser(response.Content.ReadAsStringAsync().Result,
                "Supplier Update Failed", NotificationType.error);

                return Redirect($"/Supplier/Update?supplierId={supplierViewModel.Supplier.SupplierId}");
            }

            supplierViewModel = await GetSupplierViewModel(supplierViewModel.Supplier);
            return View(supplierViewModel);
        }

        public async Task<IActionResult> Delete([FromQuery] short supplierId)
        {
            var supplier = await _supplierData.GetSupplierAsync(supplierId);

            return View(supplier);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(short supplierId)
        {
            var client = _httpClientFactory.CreateClient("northwindconnection");
            var response = await client.DeleteAsync(
                $"Suppliers/DeleteSupplier?supplierId={supplierId}");

            if (response.IsSuccessStatusCode)
            {
                NotifyUser("The supplier was deleted successfully", "Supplier Deleted");

                return Redirect("/Supplier/Index");
            }


            NotifyUser(response.Content.ReadAsStringAsync().Result, 
                "Supplier Not Deleted", NotificationType.error);

            return Redirect("/Supplier/Index");
        }
        public SupplierDto GetSupplierObject()
        {
            var supplierDto = new SupplierDto();

            supplierDto = GetObjectCookie<SupplierDto>(_httpContextAccessor,
                "SupplierDtoCookie", supplierDto);

            return supplierDto;
        }

        public async Task<SupplierViewModel> GetSupplierViewModel(SupplierDto supplier = null)
        {
            if (supplier == null)
            {
                supplier = GetSupplierObject();
            }

            var supplierViewModel = new SupplierViewModel()
            {
                Supplier = supplier,
                Regions = await _regionData.GetRegionsAsync(),
            };

            return supplierViewModel;
        }
    }
}
