using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Northwind.DataModels;
using Northwind.DataModels.Products;
using Northwind.Portal.DataAccess;
using Northwind.Portal.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Northwind.Portal.Controllers
{
    public class ProductCategoryController : BaseController
    {
        private readonly ILogger<ProductCategoryController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProductCategoryData _productCategoryData;

        public ProductCategoryController(ILogger<ProductCategoryController> logger, IHttpClientFactory httpClientFactory
            , IHttpContextAccessor httpContextAccessor
            , IProductCategoryData productCategoryData)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _productCategoryData = productCategoryData;
        }

        public async Task<IActionResult> Index()
        {
            ProductCategoryViewModel productCategoryViewModel = new()
            {
                ProductCategories = await _productCategoryData.GetProductCategoriesAsync(),
            };

            return View(productCategoryViewModel);
        }

        public async Task<IActionResult> Details([FromQuery] short productCategoryId)
        {
            var productCategory = await _productCategoryData.GetProductCategoryAsync(productCategoryId);

            return View(productCategory);
        }

        [ActionName("Create")]
        public IActionResult Create()
        {
            var productCategoryViewModel = new ProductCategoryViewModel()
            {
                ProductCategory = GetProductCategoryObject()
            };

            return View(productCategoryViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCategoryViewModel productCategoryViewModel)
        {
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient("northwindconnection");

                var response = await client.PostAsJsonAsync($"ProductCategories/AddProductCategory"
                    , productCategoryViewModel.ProductCategory);

                if (response.IsSuccessStatusCode)
                {
                    NotifyUser("The product category was added successfully",
                        "Product Category Added");

                    return Redirect($"/ProductCategory/Index");
                }

                NotifyUser(response.Content.ReadAsStringAsync().Result, 
                    "Product Category Not Added", NotificationType.error);

                return View(productCategoryViewModel);
            }

            CreateObjectCookie("ProductCategoryDtoCookie", productCategoryViewModel.ProductCategory);

            return Redirect("/ProductCategory/Create");
        }

        public async Task<IActionResult> Update([FromQuery] short productCategoryId)
        {
            var productCategoryViewModel = new ProductCategoryViewModel()
            {
                ProductCategory = await _productCategoryData.GetProductCategoryAsync(productCategoryId)
            };

            return View(productCategoryViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Update(ProductCategoryViewModel productCategoryViewModel)
        {
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient("northwindconnection");

                var response = await client.PutAsJsonAsync(
                    $"ProductCategories/UpdateProductCategory", productCategoryViewModel.ProductCategory);
                
                if (response.IsSuccessStatusCode)
                {
                    NotifyUser("The product category was updated successfully", "Product Category Updated");
                    return Redirect($"/ProductCategory/Index");
                }

                NotifyUser(response.Content.ReadAsStringAsync().Result, 
                    "Product Category Update Failed", NotificationType.error);
            }

            return Redirect(
                $"/ProductCategory/Update?productCategoryId={productCategoryViewModel.ProductCategory.ProductCategoryId}");
        }

        public async Task<IActionResult> Delete([FromQuery] short productCategoryId)
        {
            var productCategory = await _productCategoryData.GetProductCategoryAsync(productCategoryId);

            return View(productCategory);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(short productCategoryId)
        {
            var client = _httpClientFactory.CreateClient("northwindconnection");
            var response = await client.DeleteAsync(
                $"ProductCategories/DeleteProductCategory?productCategoryId={productCategoryId}");

            if (response.IsSuccessStatusCode)
            {
                NotifyUser("The product category was deleted successfully", "Product Category Deleted");

                return Redirect(
                "/ProductCategory/Index");
            }                

            NotifyUser(response.Content.ReadAsStringAsync().Result, 
                "Product Category Not Deleted", NotificationType.error);

            return Redirect("/ProductCategory/Index");
        }

        public ProductCategoryDto GetProductCategoryObject()
        {
            var productCategoryDto = new ProductCategoryDto();

            productCategoryDto = GetObjectCookie<ProductCategoryDto>(_httpContextAccessor,
                "ProductCategoryDtoCookie", productCategoryDto);

            return productCategoryDto;
        }
    }
}
