using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

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


		public static void UseFakeAuth(this IServiceCollection services, FakeAuthProfile profile = FakeAuthProfile.DEFAULT)
		{
			Action<FakeAuthOptions> options = null;

			switch (profile)
			{
				case FakeAuthProfile.DEFAULT:
					{
						options = new Action<FakeAuthOptions>(x =>
						{
							x.Claims.Add(new Claim("preferred_username", "Fake User"));
						});
						break;
					}

				case FakeAuthProfile.AZURE_AD:
					{
						options = new Action<FakeAuthOptions>(x =>
						{
							x.Claims.Add(azureClaim("preferred_username", "fake@fakeUser.com"));
							x.Claims.Add(azureClaim("name", "Fake User"));
							x.Claims.Add(azureClaim(ClaimTypes.NameIdentifier, "FAKE"));
							x.Claims.Add(azureClaim("aio", "fAkezxy"));
							x.Claims.Add(azureClaim("http://schemas.microsoft.com/identity/claims/objectidentifier", "FAKE"));
							x.Claims.Add(azureClaim("http://schemas.microsoft.com/identity/claims/tenantid", "FAKE"));
						});
						break;
					}
			}

			UseFakeAuth(services, options);
		}

		public static void UseFakeAuth(this IServiceCollection services, Action<FakeAuthOptions> options)
		{
			services.AddAuthentication(FakeAuthConst.SchemaName)
			.AddScheme<FakeAuthOptions, FakeAuthHandler>(FakeAuthConst.SchemaName, null);

			services.Configure<FakeAuthOptions>(FakeAuthConst.SchemaName, options);
		}

		private static Claim azureClaim(string key, string value)
		{
			var issuer = "https://login.microsoftonline.com/FAKE-GUID/v2.0";
			var xmlValueType = "http://www.w3.org/2001/XMLSchema#string";

			return new Claim(key, value, xmlValueType, issuer, issuer);
		}
	}

	public enum FakeAuthProfile
	{
		DEFAULT = 0,   // "default",
		AZURE_AD = 1,  // "azure ad",
		OAUTH = 2,     // "oauth",
		OKTA = 3       // "okta"
	}

	public class FakeAuthOptions : AuthenticationSchemeOptions
	{
		public FakeAuthOptions()
		{
			Claims = new List<Claim>();
		}

		public string Realm = FakeAuthConst.SchemaName;
		public List<Claim> Claims { get; set; }
	}

	public class FakeAuthHandler : AuthenticationHandler<FakeAuthOptions>
	{

		private const string AuthorizationHeaderName = "Authorization";
		private const string BasicSchemeName = FakeAuthConst.SchemaName;


		public FakeAuthHandler(IOptionsMonitor<FakeAuthOptions> options,
		ILoggerFactory logger,
		UrlEncoder encoder,
		ISystemClock clock
		)
		: base(options, logger, encoder, clock)
		{
			Console.WriteLine("Created the authentication handler.");
			//todo launch up code. 
		}

		protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			//if (!Request.Headers.ContainsKey(AuthorizationHeaderName))
			//{
			//	//Authorization header not in request
			//	return AuthenticateResult.NoResult();
			//}

			var identity = new ClaimsIdentity(Options.Claims, Scheme.Name);
			var principal = new ClaimsPrincipal(identity);
			var ticket = new AuthenticationTicket(principal, Scheme.Name);
			return AuthenticateResult.Success(ticket);
		}
	}

	public static class FakeAuthConst
	{
		public const string SchemaName = "FakeAuth";
	}
}