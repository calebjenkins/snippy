
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace Snippy.Web
{
	public class Startup
	{
		public Startup(IConfiguration configuration, IWebHostEnvironment env)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// services.UseCustomAuthentication(Configuration);
			// services.AddControllersWithViews();

			services.Configure<CookiePolicyOptions>(options =>
			{
				// This lambda determines whether user consent for non-essential cookies is needed for a given request.
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

			services.AddAuthentication(AzureADDefaults.AuthenticationScheme)
							.AddAzureAD(options => Configuration.Bind("AzureAd", options));

			services.Configure<OpenIdConnectOptions>(AzureADDefaults.OpenIdScheme, options =>
			{
				options.Authority = options.Authority + "/v2.0/";

				// Per the code below, this application signs in users in any Work and School
				// accounts and any Microsoft Personal Accounts.
				// If you want to direct Azure AD to restrict the users that can sign-in, change
				// the tenant value of the appsettings.json file in the following way:
				// - only Work and School accounts => 'organizations'
				// - only Microsoft Personal accounts => 'consumers'
				// - Work and School and Personal accounts => 'common'

				// If you want to restrict the users that can sign-in to only one tenant
				// set the tenant value in the appsettings.json file to the tenant ID of this
				// organization, and set ValidateIssuer below to true.

				// If you want to restrict the users that can sign-in to several organizations
				// Set the tenant value in the appsettings.json file to 'organizations', set
				// ValidateIssuer, above to 'true', and add the issuers you want to accept to the
				// options.TokenValidationParameters.ValidIssuers collection
				options.TokenValidationParameters.ValidateIssuer = false;
			});

			services.AddMvc(options =>
			{
				var policy = new AuthorizationPolicyBuilder()
						.RequireAuthenticatedUser()
						.Build();
				options.Filters.Add(new AuthorizeFilter(policy));
				options.EnableEndpointRouting = false;
			});

			// .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
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
			app.UseCookiePolicy();

			app.UseRouting();

			app.UseAuthentication();
			// app.UseAuthorization();

			/*
			 * It took me about a week of part time research to figure out how
			 * routing and endpoints work in .NET CORE 3+
			 * These two articles were my best help:
			 * https://docs.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-3.1
			 * https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-3.1
			 */

			app.UseMvc(routes =>
			{
				routes.MapRoute(
						name: "default",
						template: "{controller=Home}/{action=Index}/{id?}");
			});

			// app.UseEndpoints(endpoints =>
			// {
			//endpoints.MapHealthChecks("api/hc").RequireAuthorization();
			// at some point I want to explore adding standard health checks to all my apps.
			// https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks#Tutorials,-demos-and-walkthroughs-on-ASP.NET-Core-HealthChecks


			// endpoints.MapDefaultControllerRoute();

			//endpoints.MapControllerRoute(name: "short",
			//	pattern: "/{id:alpha}/{*ExtraPath}",
			//	defaults: new { controller = "Home", action = "Short" }
			//	);

			//endpoints.MapControllerRoute(
			//					name: "default",
			//					pattern: "/",
			//					defaults: new { controller = "Home", action = "index" }
			//					).RequireAuthorization() ;

			//endpoints.MapControllerRoute(
			//					name: "api",
			//					pattern: "api/{controller=Home}/{action=index}/{id?}"
			//					);

			//endpoints.MapControllerRoute(
			//	name: "php",
			//	pattern: "/home.php/{action=index}/{id?}",
			//	defaults: new { controller = "Home", action = "index" }
			//	);
			// });
		}
	}
}
