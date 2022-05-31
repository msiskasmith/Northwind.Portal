using Northwind.DataModels;
using System.Collections.Generic;

namespace Northwind.Portal.Models
{
    public class EmployeeViewModel
    {
        public EmployeeDto Employee { get; set; }
        public IEnumerable<EmployeeDto> Employees { get; set; }
        public IEnumerable<RegionDto> Regions { get; set; }
    }
}
