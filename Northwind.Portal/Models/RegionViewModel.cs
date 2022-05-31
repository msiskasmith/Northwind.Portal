using Northwind.DataModels;
using System.Collections.Generic;

namespace Northwind.Portal.Models
{
    public class RegionViewModel
    {
        public RegionDto Region { get; set; }
        public IEnumerable<RegionDto> Regions { get; set; }
    }
}
