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

			app.UseEndpoints(endpoints =>
			{
				//endpoints.MapHealthChecks("/healthz").RequireAuthorization();

				endpoints.MapGet("/hello/{name:alpha}", async context =>
				{
					var name = context.Request.RouteValues["name"];
					await context.Response.WriteAsync($"Hello there {name}");
				});

				endpoints.MapGet("/", async context =>
				{
					await context.Response.WriteAsync($"Hola Amigo! {DateTime.Now.ToString()}");
				});

				endpoints.MapDefaultControllerRoute();

				endpoints.MapControllerRoute(name: "blog",
					pattern: "blog/{id}/{*ExtraPath}",
					defaults: new { controller ="Home", action = "Short" }
					);

				//endpoints.MapControllerRoute(
				//					name: "default",
				//					pattern: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
