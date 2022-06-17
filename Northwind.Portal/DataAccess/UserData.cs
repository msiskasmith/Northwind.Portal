using Northwind.DataModels.Authentication;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Northwind.Portal.DataAccess
{
    public class UserData : IUserData
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public UserData(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<UserDto> GetUserAsync(string userId)
        {
            var client = _httpClientFactory.CreateClient("identityconnection");

            var user = await client.GetFromJsonAsync<UserDto>(
                $"Account/GetUser?userId={userId}");

            return user;
        }

        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            var client = _httpClientFactory.CreateClient("identityconnection");

            var users = await client.GetFromJsonAsync<IEnumerable<UserDto>>(
                "Account/GetUsers");

            return users;
        }
    }
}