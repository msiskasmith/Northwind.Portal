using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Northwind.DataModels;
using Northwind.DataModels.Products;
using Northwind.Portal.DataAccess;
using Northwind.Portal.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Northwind.Portal.Controllers
{
    public class ProductController : BaseController
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProductData _productData;
        private readonly IProductCategoryData _productCategoryData;
        private readonly ISupplierData _supplierData;

        public ProductController(ILogger<ProductController> logger, IHttpClientFactory httpClientFactory
            , IHttpContextAccessor httpContextAccessor
            , IProductData productData
            , IProductCategoryData productCategoryData
            , ISupplierData supplierData)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _productData = productData;
            _productCategoryData = productCategoryData;
            _supplierData = supplierData;
        }
        public async Task<IActionResult> Index()
        {
            ProductViewModel productViewModel = new()
            {
                Products = await _productData.GetProductsAsync(),
            };

            return View(productViewModel);
        }

        public async Task<IActionResult> Details([FromQuery] short productId)
        {
            var product = await _productData.GetProductAsync(productId);

            return View(product);
        }

        [ActionName("Create")]
        public async Task<IActionResult> Create()
        {
            var productViewModel = await GetProductViewModel();

            return View(productViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductViewModel productViewModel)
        {
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient("northwindconnection");

                var response = await client.PostAsJsonAsync($"Products/AddProduct"
                    , productViewModel.Product);

                if (response.IsSuccessStatusCode)
                {
                    NotifyUser("The product was added successfully", "Product Added");

                    return Redirect($"/Product/Index");
                }

                NotifyUser(response.Content.ReadAsStringAsync().Result, 
                    "Product Not Added", NotificationType.error);

                CreateObjectCookie("ProductDtoCookie", productViewModel.Product);

                return Redirect("/Product/Create");
            }

            productViewModel = await GetProductViewModel(productViewModel.Product);
            
            return View(productViewModel);  
        }

        public async Task<IActionResult> Update([FromQuery] short productId)
        {
            var productViewModel = new ProductViewModel()
            {
                Product = await _productData.GetProductAsync(productId),
                Suppliers = await _supplierData.GetSuppliersAsync(),
                ProductCategories = await _productCategoryData.GetProductCategoriesAsync()
            };

            return View(productViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Update(ProductViewModel productViewModel)
        {
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient("northwindconnection");

                var response = await client.PutAsJsonAsync(
                    $"Products/UpdateProduct", productViewModel.Product);

                if (response.IsSuccessStatusCode)
                {
                    NotifyUser("The product was updated successfully", "Product Updated");
                    return Redirect($"/Product/Index");
                }

                NotifyUser(response.Content.ReadAsStringAsync().Result,
                "Product Updated Failed", NotificationType.error);

                return Redirect($"/Product/Update?productId={productViewModel.Product.ProductId}");
            }

            productViewModel = await GetProductViewModel(productViewModel.Product);

            return View(productViewModel);
        }

        public async Task<IActionResult> Delete([FromQuery] short productId)
        {
            var product = await _productData.GetProductAsync(productId);

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(short productId)
        {
            var client = _httpClientFactory.CreateClient("northwindconnection");
            var response = await client.DeleteAsync(
                $"Products/DeleteProduct?productId={productId}");

            if (response.IsSuccessStatusCode)
            {
                NotifyUser("The product was deleted successfully", "Product Deleted");

                return Redirect(
                "/Product/Index");
            }


            NotifyUser(response.Content.ReadAsStringAsync().Result, 
                "Product Not Deleted", NotificationType.error);

            return Redirect("/Product/Index");
        }

        public ProductDto GetProductObject()
        {
            var productDto = new ProductDto();

            productDto = GetObjectCookie<ProductDto>(_httpContextAccessor,
                "ProductDtoCookie", productDto);

            return productDto;
        }

        public async Task<ProductViewModel> GetProductViewModel(ProductDto product = null)
        {
            if (product == null)
            {
                product = GetProductObject();
            }

            var productViewModel = new ProductViewModel()
            {
                Product = product,
                Suppliers = await _supplierData.GetSuppliersAsync(),
                ProductCategories = await _productCategoryData.GetProductCategoriesAsync()
            };

            return productViewModel;
        }
    }
}
