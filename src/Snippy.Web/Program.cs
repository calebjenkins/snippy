using FakeAuth;
using FakeAuth.Profiles;
using Microsoft.Identity.Web.UI;
using Lamar.Microsoft.DependencyInjection;
using Snippy.Web;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Host.UseLamar<DependencyRegistration>();

// Authentication Schema
BuildAuth(builder);

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy.
    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

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
						pattern: "api/{controller=Home}/{action=index}/{id?}"
						);
});

app.Run();

public partial class Program
{
	// Added so we can override this in integration tests
	// Also added to csproj file: <InternalsVisibleTo Include="IntegrationTests" />
	public static Action<WebApplicationBuilder> BuildAuth = (builder) =>
	{
		builder.Services.UseFakeAuth<AzureProfile>();
		//builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
		// .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

	};
}
