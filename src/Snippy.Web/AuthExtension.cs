using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Snippy.Web
{
	public static class AuthExtension
	{
		public static void UseCustomAzureAuthentication(this IServiceCollection services, IConfiguration Configuration)
		{
			services.AddAuthentication(AzureADDefaults.AuthenticationScheme)
								.AddAzureAD(options => Configuration.Bind("AzureAd", options));

			services.Configure<OpenIdConnectOptions>(AzureADDefaults.OpenIdScheme, options =>
			{
				options.Authority = options.Authority + "/v2.0/";
				options.TokenValidationParameters.ValidateIssuer = false;
			});
		}
	}
}