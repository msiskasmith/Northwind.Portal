using Northwind.DataModels;
using Northwind.DataModels.Location;
using System.Collections.Generic;

namespace Northwind.Portal.Models
{
    public class RegionViewModel
    {
        public RegionDto Region { get; set; }
        public IEnumerable<RegionDto> Regions { get; set; }
    }
}
