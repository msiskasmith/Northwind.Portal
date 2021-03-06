using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Northwind.Portal.Models;
using System.Collections.Generic;
using System.Diagnostics;

namespace Northwind.Portal.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        public void NotifyUser(string message, string notificationTitle, NotificationType notificationType = NotificationType.success)
        {
            var notificationMessageViewModel = new
            {
                NotificationMessage = message,
                NotificationTitle = notificationTitle,
                NotificationType = notificationType.ToString(),
                NotificationProvider = "sweetAlert"
            };

            CreateObjectCookie("NotificationMessageViewModel", notificationMessageViewModel);
         }

        public void CreateObjectCookie(string cookieKey, object entity)
        {
            HttpContext.Response.Cookies.Append(cookieKey, JsonConvert.SerializeObject(entity));
        }

        public T GetObjectCookie<T>(IHttpContextAccessor httpContextAccessor, string cookieKey, T entityDto)
        {
            
            string objectCookie = httpContextAccessor.HttpContext.Request.Cookies[cookieKey];

            if (objectCookie is not null)
            {
                entityDto = JsonConvert.DeserializeObject<T>(objectCookie);
                httpContextAccessor.HttpContext.Response.Cookies.Delete(cookieKey);
            }

            return entityDto;
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
