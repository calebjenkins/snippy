using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Snippy.Web
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
			services.AddControllersWithViews();
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

			//app.UseAuthentication();
			//app.UseAuthorization();

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
					pattern: "/{id:alpha}/{*ExtraPath}",
					defaults: new { controller = "Home", action = "Short" }
					);

				endpoints.MapControllerRoute(
									name: "default",
									pattern: "/",
									defaults: new { controller = "Home", action = "index" }
									);

				endpoints.MapControllerRoute(
									name: "api",
									pattern: "api/{controller=Home}/{action=index}/{id?}"
									);

				endpoints.MapControllerRoute(
					name: "php",
					pattern: "/home.php/{action=index}/{id?}",
					defaults: new { controller = "Home", action = "index" }
					);
			});
		}
	}
}
