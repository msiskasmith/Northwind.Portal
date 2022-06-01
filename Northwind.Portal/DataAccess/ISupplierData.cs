using Northwind.DataModels;
using Northwind.DataModels.Products;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Northwind.Portal.DataAccess
{
    public interface ISupplierData
    {
        Task<SupplierDto> GetSupplierAsync(short supplierId);
        Task<IEnumerable<SupplierDto>> GetSuppliersAsync();
    }
}