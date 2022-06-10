using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Portal.Data;

[assembly: HostingStartup(typeof(Northwind.Portal.Areas.Identity.IdentityHostingStartup))]
namespace Northwind.Portal.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<NorthwindPortalContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("NorthwindPortalContextConnection")));

                services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<NorthwindPortalContext>();
            });
        }
    }
}