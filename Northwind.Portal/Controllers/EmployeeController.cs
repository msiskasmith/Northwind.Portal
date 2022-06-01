using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Northwind.DataModels.Employees;
using Northwind.Portal.DataAccess;
using Northwind.Portal.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Northwind.Portal.Controllers
{
    public class EmployeeController : BaseController
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmployeeData _employeeData;
        private readonly IRegionData _regionData;

        public EmployeeController(IHttpClientFactory httpClientFactory
            , IHttpContextAccessor httpContextAccessor
            , IEmployeeData employeeData
            , IRegionData regionData)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _employeeData = employeeData;
            _regionData = regionData;
        }
        public async Task<IActionResult> Index()
        {
            EmployeeViewModel employeeViewModel = new()
            {
                Employees = await _employeeData.GetEmployeesAsync(),
            };

            return View(employeeViewModel);
        }

        public async Task<IActionResult> Details([FromQuery] short employeeId)
        {
            var employee = await _employeeData.GetEmployeeAsync(employeeId);

            return View(employee);
        }

        [ActionName("Create")]
        public async Task<IActionResult> Create()
        {
            var employeeViewModel = await GetEmployeeViewModel();

            return View(employeeViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(EmployeeViewModel employeeViewModel)
        {
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient("northwindconnection");

                var response = await client.PostAsJsonAsync($"Employees/AddEmployee"
                    , employeeViewModel.Employee);

                if (response.IsSuccessStatusCode)
                {
                    NotifyUser("The employee was added successfully", "Employee Added");

                    return Redirect($"/Employee/Index");
                }

                NotifyUser(response.Content.ReadAsStringAsync().Result, 
                    "Employee Not Added", NotificationType.error);

                CreateObjectCookie("EmployeeDtoCookie", employeeViewModel.Employee);

                return Redirect("/Employee/Create");
            }

            var viewModel = await GetEmployeeViewModel(employeeViewModel.Employee);

            return View(viewModel);
        }

        public async Task<IActionResult> Update([FromQuery] short employeeId)
        {
            var employeeViewModel = await GetEmployeeViewModel(await _employeeData.GetEmployeeAsync(employeeId));

            return View(employeeViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Update(EmployeeViewModel employeeViewModel)
        {
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient("northwindconnection");

                var response = await client.PutAsJsonAsync(
                    $"Employees/UpdateEmployee", employeeViewModel.Employee);

                if (response.IsSuccessStatusCode)
                {
                    NotifyUser("The employee was updated successfully", "Employee Updated");
                    return Redirect($"/Employee/Index");
                }

                NotifyUser(response.Content.ReadAsStringAsync().Result,
                "Employee Update Failed", NotificationType.error);

                return Redirect($"/Employee/Update?employeeId={employeeViewModel.Employee.EmployeeId}");
            }

            employeeViewModel = await GetEmployeeViewModel(employeeViewModel.Employee);

            return View(employeeViewModel);

        }

        public async Task<IActionResult> Delete([FromQuery] short employeeId)
        {
            var employee = await _employeeData.GetEmployeeAsync(employeeId);

            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed([FromQuery] short employeeId)
        {
            var client = _httpClientFactory.CreateClient("northwindconnection");
            var response = await client.DeleteAsync(
                $"Employees/DeleteEmployee?employeeId={employeeId}");

            if (response.IsSuccessStatusCode)
            {
                NotifyUser("The employee was deleted successfully", "Employee Deleted");

                return Redirect("/Employee/Index");
            }

            NotifyUser(response.Content.ReadAsStringAsync().Result, 
                "Employee Not Deleted", NotificationType.error);

            return Redirect("/Employee/Index");
        }

        public EmployeeDto GetEmployeeObject()
        {
            var employeeDto = new EmployeeDto();

            employeeDto = GetObjectCookie<EmployeeDto>(_httpContextAccessor,
                "EmployeeDtoCookie", employeeDto);

            return employeeDto;
        }

        public async Task<EmployeeViewModel> GetEmployeeViewModel(EmployeeDto employee = null)
        {
            if(employee is null)
            {
                employee = GetEmployeeObject();
            }

            var employeeViewModel = new EmployeeViewModel()
            {
                Employee = employee,
                Regions = await _regionData.GetRegionsAsync(),
                Employees = await _employeeData.GetEmployeesAsync()
            };
            
            return employeeViewModel;
        }
    }
}
