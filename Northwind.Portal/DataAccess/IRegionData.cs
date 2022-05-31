using Northwind.DataModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Northwind.Portal.DataAccess
{
    public interface IRegionData
    {
        Task<RegionDto> GetRegionAsync(short regionId);
        Task<IEnumerable<RegionDto>> GetRegionsAsync();
    }
}