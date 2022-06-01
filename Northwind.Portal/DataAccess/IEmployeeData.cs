using Northwind.DataModels;
using Northwind.DataModels.Employees;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Northwind.Portal.DataAccess
{
    public interface IEmployeeData
    {
        Task<EmployeeDto> GetEmployeeAsync(short employeeId);
        Task<IEnumerable<EmployeeDto>> GetEmployeesAsync();
    }
}