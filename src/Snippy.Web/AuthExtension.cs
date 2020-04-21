using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security.OpenIdConnect;
using System;
using System.Linq;

namespace Snippy.Web
{
	public static class AuthExtension
	{
		public static void UseCustomAuthentication(this IServiceCollection services, IConfiguration Configuration)
		{
			/*
			 * Use the following CLI commands to make sure you have values in your dev environment
			 * dotnet user-secrets list
			 * dotnet user-secrets add "OIDC:ClientId" "[secret]"
			 * dotnet user-secrets add "OIDC:ClientSecret" "[secret]"
			 */
			var tenant = "common";
			var authority = $"https://login.microsoftonline.com/{tenant}/v2.0";
			var oAuthAuthorize = $"https://login.microsoftonline.com/{tenant}/oauth2/v2.0/authorize";
			var oAuthToken = $"https://login.microsoftonline.com/{tenant}/oauth2/v2.0/token";
			var clientID = Configuration["OIDC:ClientId"];
			var clientSecret = Configuration["OIDC:ClientSecret"];
			var redirectUrl = "https://localhost:44379";

			var msOptions = new MicrosoftAccountOptions();
			var owinOptions = CreateOptionsFromPolicy("", "", "", "", "");

			services.AddAuthentication(options =>
			{
				options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
			})
			.AddCookie()
			//.AddMicrosoftAccount(options =>
			//{
			//	options.AuthorizationEndpoint = authority;
			//	options.ClientId = clientID;
			//	options.ClientSecret = clientSecret;
			//	options.ReturnUrlParameter = redirectUrl;
			//	options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
			//	options.SaveTokens = true;
			//});

			// .AddOpenIdConnect(owinOptions);

			.AddOpenIdConnect(options =>
			{
				// options.ReturnUrlParameter = redirectUrl;
				options.Authority = authority;
				options.ClientId = clientID;
				options.ClientSecret = clientSecret;
				options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.RequireHttpsMetadata = true;
				options.ResponseType = OpenIdConnectResponseType.Token;
				// options.GetClaimsFromUserInfoEndpoint = true;
				// options.Scope.Add("openid");
				// options.Scope.Add("profile");
				options.SaveTokens = true;
				options.TokenValidationParameters.ValidateIssuer = false;
				options.AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet;
				//options.TokenValidationParameters = new TokenValidationParameters
				//{
				//	// NameClaimType = "name",
				//	// RoleClaimType = "groups",
				//	ValidateIssuer = true
				//	//Notifications = new OpenIdConnectAuthenticationNotifications
				//	//{
				//	//    AuthenticationFailed = OnAuthenticationFailed
				//	//}
				//};
			});

			services.AddAuthorization();
		}

		// <add key = "ida:Tenant" value="fabrikamb2c.onmicrosoft.com" />
		// <add key = "ida:ClientId" value="90c0fe63-bcf2-44d5-8fb7-b8bbc0b29dc6" />
		// <add key = "ida:AadInstance" value="https://login.microsoftonline.com/{0}/v2.0/.well-known/openid-configuration?p={1}" />
		// <add key = "ida:RedirectUri" value="https://localhost:44316/" />
		// <add key = "ida:SignUpPolicyId" value="b2c_1_sign_up" />
		// <add key = "ida:SignInPolicyId" value="b2c_1_sign_in" />
		// <add key = "ida:UserProfilePolicyId" value="b2c_1_edit_profile" />
		// <add key = "api:TaskServiceUrl" value="https://aadb2cplayground.azurewebsites.net" />

		private static OpenIdConnectAuthenticationOptions CreateOptionsFromPolicy
			(string policy, string WellKnownUrl, string clientId, string tenant, string redirectUri)
		{
			return new OpenIdConnectAuthenticationOptions
			{
				// For each policy, give OWIN the policy-specific metadata address, and
				// set the authentication type to the id of the policy
				MetadataAddress = String.Format(WellKnownUrl, tenant, policy),
				AuthenticationType = policy,

				// These are standard OpenID Connect parameters, with values pulled from web.config
				ClientId = clientId,
				RedirectUri = redirectUri,
				PostLogoutRedirectUri = redirectUri,
				//Notifications = new OpenIdConnectAuthenticationNotifications
				//{
				//	AuthenticationFailed = OnAuthenticationFailed,
				//},
				Scope = "openid",
				ResponseType = "id_token",

				TokenValidationParameters = new TokenValidationParameters
				{
					NameClaimType = "name",
					SaveSigninToken = true,
				},
			};
		}
	}
}
