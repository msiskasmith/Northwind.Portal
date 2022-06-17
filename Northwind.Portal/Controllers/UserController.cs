using Microsoft.AspNetCore.Mvc;
using Northwind.DataModels.Authentication;
using Northwind.Portal.DataAccess;
using System.Threading.Tasks;

namespace Northwind.Portal.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserData _userData;

        public UserController(IUserData userData)
        {
            _userData = userData;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userData.GetUsersAsync();

            return View(users);
        }

        public async Task<IActionResult> Details([FromQuery] string userId)
        {
            var user = await _userData.GetUserAsync(userId);

            return View(user);
        }

        public IActionResult Create()
        {
            RegisterUserDto registerUserDto = new();

            return View(registerUserDto);
        }

        public async Task<IActionResult> Delete([FromQuery] string userId)
        {
            var user = await _userData.GetUserAsync(userId);

            return View(user);
        }
    }
}
