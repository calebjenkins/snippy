using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Snippy.Web
{
	public static class AuthExtension
	{
		public static void UseCustomAuthentication(this IServiceCollection services, IConfiguration Configuration)
		{
			// This is really 3 lines of code and a TON of comments/notes
			// Use:
			//  dotnet user-secrets add "AzureAd:Instance" "https://login.microsoftonline.com/"
			//  dotnet user-secrets add "AzureAd:TenantId" "common"
			//  dotnet user-secrets add "AzureAd:ClientId" "CLIENT_IF_FROM_AZURE_AD"
			//  dotnet user-secrets add "AzureAd:CallbackPath" "/signin-oidc"
			// Update appSettings.json:
			//"AzureAd": {
			//  "Instance": "https://login.microsoftonline.com/",
			//  "TenantId": "common",
			//  "ClientId": "CLIENT_IF_FROM_AZURE_AD",
			//  "CallbackPath": "/signin-oidc"
			//},

			services.AddAuthentication(AzureADDefaults.AuthenticationScheme)
								.AddAzureAD(options => Configuration.Bind("AzureAd", options));

			services.Configure<OpenIdConnectOptions>(AzureADDefaults.OpenIdScheme, options =>
			{
				options.Authority = options.Authority + "/v2.0/";

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

}

		//public static void UseCustomAuthentication(this IServiceCollection services, IConfiguration Configuration)
		//{
		//	/*
		//	 * Use the following CLI commands to make sure you have values in your dev environment
		//	 * dotnet user-secrets list
		//	 * dotnet user-secrets add "OIDC:ClientId" "[secret]"
		//	 * dotnet user-secrets add "OIDC:ClientSecret" "[secret]"
		//	 */
		//	var tenant = "common";
		//	var authority = $"https://login.microsoftonline.com/{tenant}/v2.0";
		//	var oAuthAuthorize = $"https://login.microsoftonline.com/{tenant}/oauth2/v2.0/authorize";
		//	var oAuthToken = $"https://login.microsoftonline.com/{tenant}/oauth2/v2.0/token";
		//	var clientID = Configuration["OIDC:ClientId"];
		//	var clientSecret = Configuration["OIDC:ClientSecret"];
		//	var redirectUrl = "https://localhost:44379";

		//	var msOptions = new MicrosoftAccountOptions();
		//	var owinOptions = CreateOptionsFromPolicy("", "", "", "", "");

		//	services.AddAuthentication(options =>
		//	{
		//		options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
		//		options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
		//	})
		//	.AddCookie()
		//	//.AddMicrosoftAccount(options =>
		//	//{
		//	//	options.AuthorizationEndpoint = authority;
		//	//	options.ClientId = clientID;
		//	//	options.ClientSecret = clientSecret;
		//	//	options.ReturnUrlParameter = redirectUrl;
		//	//	options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
		//	//	options.SaveTokens = true;
		//	//});

		//	// .AddOpenIdConnect(owinOptions);

		//	.AddOpenIdConnect(options =>
		//	{
		//		// options.ReturnUrlParameter = redirectUrl;
		//		options.Authority = authority;
		//		options.ClientId = clientID;
		//		options.ClientSecret = clientSecret;
		//		options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
		//		options.RequireHttpsMetadata = true;
		//		options.ResponseType = OpenIdConnectResponseType.Token;
		//		// options.GetClaimsFromUserInfoEndpoint = true;
		//		// options.Scope.Add("openid");
		//		// options.Scope.Add("profile");
		//		options.SaveTokens = true;
		//		options.TokenValidationParameters.ValidateIssuer = false;
		//		options.AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet;
		//		//options.TokenValidationParameters = new TokenValidationParameters
		//		//{
		//		//	// NameClaimType = "name",
		//		//	// RoleClaimType = "groups",
		//		//	ValidateIssuer = true
		//		//	//Notifications = new OpenIdConnectAuthenticationNotifications
		//		//	//{
		//		//	//    AuthenticationFailed = OnAuthenticationFailed
		//		//	//}
		//		//};
		//	});

		//	services.AddAuthorization();
		//}

		// <add key = "ida:Tenant" value="fabrikamb2c.onmicrosoft.com" />
		// <add key = "ida:ClientId" value="" />
		// <add key = "ida:AadInstance" value="https://login.microsoftonline.com/{0}/v2.0/.well-known/openid-configuration?p={1}" />
		// <add key = "ida:RedirectUri" value="https://localhost:44316/" />
		// <add key = "ida:SignUpPolicyId" value="b2c_1_sign_up" />
		// <add key = "ida:SignInPolicyId" value="b2c_1_sign_in" />
		// <add key = "ida:UserProfilePolicyId" value="b2c_1_edit_profile" />
		// <add key = "api:TaskServiceUrl" value="https://aadb2cplayground.azurewebsites.net" />

		//private static OpenIdConnectAuthenticationOptions CreateOptionsFromPolicy
		//	(string policy, string WellKnownUrl, string clientId, string tenant, string redirectUri)
		//{
		//	return new OpenIdConnectAuthenticationOptions
		//	{
		//		// For each policy, give OWIN the policy-specific metadata address, and
		//		// set the authentication type to the id of the policy
		//		MetadataAddress = String.Format(WellKnownUrl, tenant, policy),
		//		AuthenticationType = policy,

		//		// These are standard OpenID Connect parameters, with values pulled from web.config
		//		ClientId = clientId,
		//		RedirectUri = redirectUri,
		//		PostLogoutRedirectUri = redirectUri,
		//		//Notifications = new OpenIdConnectAuthenticationNotifications
		//		//{
		//		//	AuthenticationFailed = OnAuthenticationFailed,
		//		//},
		//		Scope = "openid",
		//		ResponseType = "id_token",

		//		TokenValidationParameters = new TokenValidationParameters
		//		{
		//			NameClaimType = "name",
		//			SaveSigninToken = true,
		//		},
		//	};
		//}
	}
}
