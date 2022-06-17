using Northwind.DataModels.Authentication;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Northwind.Portal.DataAccess
{
    public interface IUserData
    {
        Task<UserDto> GetUserAsync(string userId);
        Task<IEnumerable<UserDto>> GetUsersAsync();
    }
}