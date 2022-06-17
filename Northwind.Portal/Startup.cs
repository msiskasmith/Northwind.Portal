using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Northwind.Portal.CustomFilters;
using Northwind.Portal.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Northwind.Portal
{
    public class Startup
    {


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ICustomerData, CustomerData>();
            services.AddScoped<IEmployeeData, EmployeeData>();
            services.AddScoped<IOrderData, OrderData>();
            services.AddScoped<IOrderDetailData, OrderDetailData>();
            services.AddScoped<IProductCategoryData, ProductCategoryData>();
            services.AddScoped<IProductData, ProductData>();
            services.AddScoped<IRegionData, RegionData>();
            services.AddScoped<IShipperData, ShipperData>();
            services.AddScoped<ISupplierData, SupplierData>();
            services.AddScoped<IUserData, UserData>();

            services.AddSingleton<ILogger>(provider =>
                provider.GetRequiredService<ILogger<Startup>>());

            //services.AddLogging();

            services.AddControllersWithViews(options =>
                {
                    options.Filters.Add(typeof(LogExceptionFilter));
                })
                .AddRazorRuntimeCompilation()
                .AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
                .AddXmlSerializerFormatters();

            services.AddHttpClient();

            services.AddHttpClient("northwindconnection", (sp, c) =>
            {
                c.BaseAddress = new Uri(Configuration.GetValue<string>("apiLocation"));
            });

            services.AddHttpClient("identityconnection", (sp, c) =>
            {
                c.BaseAddress = new Uri(Configuration.GetValue<string>("identityLocation"));
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential 
                // cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                // requires using Microsoft.AspNetCore.Http;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Add authentication services
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "oauth";
            })
            .AddCookie("Cookies", options =>
            {
                options.Cookie.Name = "NorthwindSecurity.ControlPanel.Cookie";
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.IsEssential = true;
            })
            .AddOAuth("oauth", config =>
            {
                config.SignInScheme = "Cookies";
                config.ClientId = Configuration["AuthSettings:ClientId"];
                config.ClientSecret = Configuration["AuthSettings:Key"];
                config.AccessDeniedPath = new PathString("/Home/AccessDenied");
                config.CallbackPath = new PathString("/oauth/callback");
                config.AuthorizationEndpoint = $"{Configuration["AuthSettings:ValidIssuer"]}Account/Login";
                config.TokenEndpoint = $"{Configuration["AuthSettings:ValidIssuer"]}Account/Token";
                config.UserInformationEndpoint = $"{Configuration["AuthSettings:ValidIssuer"]}Account/userclaims";
                config.SaveTokens = true;

                config.CorrelationCookie.HttpOnly = true;
                config.CorrelationCookie.IsEssential = true;
                config.CorrelationCookie.SameSite = SameSiteMode.None;
                config.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
                config.CorrelationCookie.Name = "NorthwindSecurity.Oauth.Correlation.Cookie";

                config.Events = new OAuthEvents()
                {
                    OnCreatingTicket = async context =>
                    {
                        var request = new HttpRequestMessage(HttpMethod.Get, $"{context.Options.UserInformationEndpoint}?username={context.Request.Query["username"]}");
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                        var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                        response.EnsureSuccessStatusCode();
                        var data = await response.Content.ReadAsStringAsync();
                        var user = JsonDocument.Parse(await response.Content.ReadAsStringAsync());


                        context.RunClaimActions(user.RootElement);

                    },
                    OnRemoteFailure = context =>
                    {
                        //log error for debugging
                        //Logger.LogInformation(context.Failure.StackTrace, context.Failure.InnerException);

                        context.Response.Redirect($"/Errors/Error?statusCode={context.Response.StatusCode}&message={context.Failure.Message}");
                        context.HandleResponse();
                        return Task.CompletedTask;
                    }
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapRazorPages();
            });

            app.UseCors(c => c.AllowAnyOrigin().AllowAnyMethod());

            app.UseCookiePolicy();
        }
    }
}
