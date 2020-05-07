
using Lamar;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Snippy.Data;

namespace Snippy.Web
{
	public class Startup
	{
		public Startup(IConfiguration configuration, IWebHostEnvironment env)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// public void ConfigureServices(ServiceRegistry services) // Lamar
		public void ConfigureServices(IServiceCollection services)

		{
			// Set Up Data Access
			var connString = Configuration.GetValue<string>("DBConfig:ConnectionString");
			var optionsBuilder = new DbContextOptionsBuilder<SnippyDataContext>();
			var dbOption = optionsBuilder.UseSqlServer(connString).Options;
			services.AddSingleton<DbContextOptions<SnippyDataContext>>(dbOption);

			services.Configure<CookiePolicyOptions>(options =>
			{
				// This lambda determines whether user consent for non-essential cookies is needed for a given request.
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

			services.UseCustomAuthentication(Configuration);

			services.AddMvc(options =>
			{
				var policy = new AuthorizationPolicyBuilder()
						.RequireAuthenticatedUser()
						.Build();
				options.Filters.Add(new AuthorizeFilter(policy));
				options.EnableEndpointRouting = false;
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
				app.UseHsts();
			}
			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseCookiePolicy();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			/*
			 * It took me about a week of part time research to figure out how
			 * routing and endpoints work in .NET CORE 3+
			 * These two articles were my best help:
			 * https://docs.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-3.1
			 * https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-3.1
			 */

			app.UseEndpoints(endpoints =>
			{
				//endpoints.MapHealthChecks("api/hc").RequireAuthorization();
				// at some point I want to explore adding standard health checks to all my apps.
				// https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks#Tutorials,-demos-and-walkthroughs-on-ASP.NET-Core-HealthChecks


				endpoints.MapDefaultControllerRoute();

				endpoints.MapControllerRoute(name: "short",
					pattern: "/{id}/{*ExtraPath}",
					defaults: new { controller = "Home", action = "Short" }
					);

				endpoints.MapControllerRoute(
									name: "default",
									pattern: "/",
									defaults: new { controller = "Home", action = "index" }
									);

				// Reserving now for future work...
				endpoints.MapControllerRoute(
									name: "api",
									pattern: "api/{controller}/{action}/{id?}",
									defaults: new { controller = "Api", action = "hc" }
									);
			});
		}
	}
}
