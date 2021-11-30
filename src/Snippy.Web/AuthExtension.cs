using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using static Snippy.Web.CustomFakeAuthHandler;

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

		public static void UseFakeAuth(this IServiceCollection services)
		{
			// services.AddAuthentication(FakeAuthConst.SchemaName);
			// services.Configure<FakeAuthenticationSchemeOptions>((opt) => opt.ClaimsIssuer = FakeAuthConst.SchemaName);
			//services.AddAuthorization(options =>
			//{
			//	options.AddPolicy( DefaultAuthorizedPolicy, policy =>
			//	{
			//		policy.Requirements.Add(new TokenAuthRequirement());
			//	});
			//});
			//services.AddSingleton<IAuthorizationHandler, AuthTokenPolicy>();

			services.AddAuthentication(FakeAuthConst.SchemaName).AddScheme<CustomFakeAuthOptions, CustomFakeAuthHandler>(FakeAuthConst.SchemaName, null);

		}

		public static void UseDevAuth(this IApplicationBuilder app)
		{
			app.UseMiddleware<FakeAuthorizationMiddleware>();
			//app.Run(async context =>
			//{
			//	var fakeUser = new ClaimsIdentity("fakeAuth")
			//	{
			//		Label = "fakeAuth"
			//	};
			//	fakeUser.AddClaim(new Claim("preferred_username", "fake joe"));

			//	context.User.AddIdentity(fakeUser);

			// var identity = _httpAccessor.ActionContext.HttpContext.User.Identity as ClaimsIdentity; // Azure AD V2 endpoint specific
			// string preferred_username = identity.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;

			//var msg = (Id.IsNullOrEmpty()) ? $"Welcome {preferred_username}!" : "--> 404 Not Found: {Id}";
			// });
		}

		public static IApplicationBuilder UseFAKEAuthorization(this IApplicationBuilder app)
		{
			if (app == null)
			{
				throw new ArgumentNullException(nameof(app));
			}

			return app.UseMiddleware<FakeAUTHMiddleware>();
		}
	}

	public class CustomFakeAuthOptions : AuthenticationSchemeOptions
	{
		public string Realm = FakeAuthConst.SchemaName;
	}

	public class CustomFakeAuthHandler : AuthenticationHandler<CustomFakeAuthOptions>
	{

		private const string AuthorizationHeaderName = "Authorization";
		private const string BasicSchemeName = FakeAuthConst.SchemaName;


		public CustomFakeAuthHandler(IOptionsMonitor<CustomFakeAuthOptions> options,
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

			var xyz = Request.Headers[AuthorizationHeaderName];
			if (!AuthenticationHeaderValue.TryParse(Request.Headers[AuthorizationHeaderName], out AuthenticationHeaderValue headerValue))
			{
				//	//Invalid Authorization header
				//	return AuthenticateResult.NoResult();
				var test = false;

			}

			//if (!BasicSchemeName.Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase))
			//{
			//	//Not Basic authentication header
			//	return AuthenticateResult.NoResult();
			//}

			//byte[] headerValueBytes = Convert.FromBase64String(headerValue.Parameter);
			//string userAndPassword = Encoding.UTF8.GetString(headerValueBytes);
			//string[] parts = userAndPassword.Split(':');
			//if (parts.Length != 2)
			//{
			//	return AuthenticateResult.Fail("Invalid Basic authentication header");
			//}
			//string user = parts[0];
			//string password = parts[1];

			string user = "Fake User";
			string password = "Fake Password";

			//if (user != "michael" || password != "ThePassword")
			//{
			//	return AuthenticateResult.Fail("Invalid username or password");
			//}


			var claims = new[] { new Claim(ClaimTypes.Name, "TestUserNameClaim") };
			var identity = new ClaimsIdentity(claims, Scheme.Name);
			var principal = new ClaimsPrincipal(identity);
			var ticket = new AuthenticationTicket(principal, Scheme.Name);
			return AuthenticateResult.Success(ticket);
		}

		public static class FakeAuthConst
		{
			public const string SchemaName = "FakeAuth";
		}
		public class FakeAuthPolicyBuilder : AuthenticationBuilder
		{
			public FakeAuthPolicyBuilder(IServiceCollection services) : base(services)
			{
				base.AddPolicyScheme(FakeAuthConst.SchemaName, FakeAuthConst.SchemaName, (opt) => opt.ClaimsIssuer = FakeAuthConst.SchemaName);
			}
		}
		public class FakeAuthenticationSchemeOptions : AuthenticationSchemeOptions
		{

		}

		public class FakeAuthenticationHandler : AuthenticationHandler<FakeAuthenticationSchemeOptions>
		{
			public FakeAuthenticationHandler(
				 IOptionsMonitor<FakeAuthenticationSchemeOptions> options,
				 ILoggerFactory logger,
				 UrlEncoder encoder,
				 ISystemClock clock)
				 : base(options, logger, encoder, clock)
			{
			}

			protected override Task<AuthenticateResult> HandleAuthenticateAsync()
			{
				// validation comes in here

				return new Task<AuthenticateResult>(() =>
				{
					return new FakeAuthResults();
				});
			}
		}

		public class FakeAUTHMiddleware
		{
			private readonly RequestDelegate _next;
			private readonly IAuthorizationPolicyProvider _policyProvider;

			public FakeAUTHMiddleware(RequestDelegate next, IAuthorizationPolicyProvider policyProvider)
			{
				_next = next;
				_policyProvider = policyProvider;
			}

			public async Task InvokeAsync(HttpContext context)
			{
				var endpoint = context.GetEndpoint();
				var authorizeData = endpoint?.Metadata.GetOrderedMetadata<IAuthorizeData>() ?? Array.Empty<IAuthorizeData>();
				var policy = await AuthorizationPolicy.CombineAsync(_policyProvider, authorizeData);
				var policyEvaluator = context.RequestServices.GetRequiredService<IPolicyEvaluator>();
				var authenticateResult = await policyEvaluator.AuthenticateAsync(policy, context);
				var authFeatures = new AuthenticationFeatures(authenticateResult);
				context.Features.Set<IHttpAuthenticationFeature>(authFeatures);

				var fakeUser = new ClaimsIdentity(FakeAuthConst.SchemaName)
				{
					Label = "FakeAuth"
				};
				fakeUser.AddClaim(new Claim("preferred_username", "fake joe"));
				context.User.AddIdentity(fakeUser);


				// Call the next delegate/middleware in the pipeline
				await _next(context);
			}
		}

		public class FakeAuthorizationMiddleware
		{
			// AppContext switch used to control whether HttpContext or endpoint is passed as a resource to AuthZ
			private const string SuppressUseHttpContextAsAuthorizationResource = "Microsoft.AspNetCore.Authorization.SuppressUseHttpContextAsAuthorizationResource";

			// Property key is used by Endpoint routing to determine if Authorization has run
			private const string AuthorizationMiddlewareInvokedWithEndpointKey = "__AuthorizationMiddlewareWithEndpointInvoked";
			private static readonly object AuthorizationMiddlewareWithEndpointInvokedValue = new object();

			private readonly RequestDelegate _next;
			private readonly IAuthorizationPolicyProvider _policyProvider;

			/// <summary>
			/// Initializes a new instance of <see cref="AuthorizationMiddleware"/>.
			/// </summary>
			/// <param name="next">The next middleware in the application middleware pipeline.</param>
			/// <param name="policyProvider">The <see cref="IAuthorizationPolicyProvider"/>.</param>
			public FakeAuthorizationMiddleware(RequestDelegate next, IAuthorizationPolicyProvider policyProvider)
			{
				_next = next ?? throw new ArgumentNullException(nameof(next));
				_policyProvider = policyProvider ?? throw new ArgumentNullException(nameof(policyProvider));
			}

			/// <summary>
			/// Invokes the middleware performing authorization.
			/// </summary>
			/// <param name="context">The <see cref="HttpContext"/>.</param>
			public async Task Invoke(HttpContext context)
			{
				if (context == null)
				{
					throw new ArgumentNullException(nameof(context));
				}

				var endpoint = context.GetEndpoint();

				if (endpoint != null)
				{
					// EndpointRoutingMiddleware uses this flag to check if the Authorization middleware processed auth metadata on the endpoint.
					// The Authorization middleware can only make this claim if it observes an actual endpoint.
					context.Items[AuthorizationMiddlewareInvokedWithEndpointKey] = AuthorizationMiddlewareWithEndpointInvokedValue;
				}

				// IMPORTANT: Changes to authorization logic should be mirrored in MVC's AuthorizeFilter
				var authorizeData = endpoint?.Metadata.GetOrderedMetadata<IAuthorizeData>() ?? Array.Empty<IAuthorizeData>();
				var policy = await AuthorizationPolicy.CombineAsync(_policyProvider, authorizeData);
				if (policy == null)
				{
					await _next(context);
					return;
				}

				// Policy evaluator has transient lifetime so it's fetched from request services instead of injecting in constructor
				var policyEvaluator = context.RequestServices.GetRequiredService<IPolicyEvaluator>();

				var authenticateResult = await policyEvaluator.AuthenticateAsync(policy, context);

				if (authenticateResult?.Succeeded ?? false)
				{
					//if (context.Features.Get<Microsoft.AspNetCore.Authentication.IAuthenticateResultFeature>() is IAuthenticateResultFeature authenticateResultFeature)
					//{
					//	authenticateResultFeature.AuthenticateResult = authenticateResult;
					//}
					//else
					{
						var authFeatures = new AuthenticationFeatures(authenticateResult);
						context.Features.Set<IHttpAuthenticationFeature>(authFeatures);
						// context.Features.Set<IAuthenticateResultFeature>(authFeatures);
					}
				}

				// Allow Anonymous still wants to run authorization to populate the User but skips any failure/challenge handling
				if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null)
				{
					await _next(context);
					return;
				}

				object? resource;
				if (AppContext.TryGetSwitch(SuppressUseHttpContextAsAuthorizationResource, out var useEndpointAsResource) && useEndpointAsResource)
				{
					resource = endpoint;
				}
				else
				{
					resource = context;
				}

				var authorizeResult = await policyEvaluator.AuthorizeAsync(policy, authenticateResult!, context, resource);
				var authorizationMiddlewareResultHandler = context.RequestServices.GetRequiredService<IAuthorizationMiddlewareResultHandler>();
				await authorizationMiddlewareResultHandler.HandleAsync(_next, context, policy, authorizeResult);
			}
		}

		public class FakeAuthResults : AuthenticateResult
		{

		}
		public class FakeAuthPolicyProvider : IAuthorizationPolicyProvider
		{

			private IEnumerable<IAuthorizationRequirement> requirements;
			private IEnumerable<string> schemes;
			private AuthorizationPolicy authorizationPolicy;

			public FakeAuthPolicyProvider()
			{
				requirements = new List<IAuthorizationRequirement>();
				schemes = new List<string>() { "FakeAuth" };
				authorizationPolicy = new AuthorizationPolicy(requirements, schemes);
			}

			public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
			{
				return Task.FromResult(authorizationPolicy);
			}

			public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
			{
				return Task.FromResult(authorizationPolicy);
			}

			public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
			{
				return Task.FromResult(authorizationPolicy);
			}
		}

		public class AuthenticationFeatures : IHttpAuthenticationFeature // , IAuthenticateResultFeature
		{
			private ClaimsPrincipal? _user;
			private AuthenticateResult? _result;

			public AuthenticationFeatures(AuthenticateResult result)
			{
				AuthenticateResult = result;
			}

			public AuthenticateResult? AuthenticateResult
			{
				get => _result;
				set
				{
					_result = value;
					_user = _result?.Principal;
				}
			}

			public ClaimsPrincipal? User
			{
				get => _user;
				set
				{
					_user = value;
					_result = null;
				}
			}
		}
	}

}

namespace FakeAuth
{
	public sealed class PermissionsAttribute : AuthorizeAttribute
	{
		public const string PermissionsGroup = "Permissions";
		public const string RolesGroup = "Roles";
		public const string ScopesGroup = "Scopes";

		private string[] _permissions;
		private string[] _scopes;
		private string[] _roles;

		private bool _isDefault = true;

		public PermissionsAttribute()
		{
			_permissions = Array.Empty<string>();
			_roles = Array.Empty<string>();
			_scopes = Array.Empty<string>();
		}

		public string[] Permissions
		{
			get => _permissions;
			set
			{
				BuildPolicy(ref _permissions, value, PermissionsGroup);
			}
		}

		public string[] Scopes
		{
			get => _scopes;
			set
			{
				BuildPolicy(ref _scopes, value, ScopesGroup);
			}
		}

		new public string[] Roles
		{
			get => _roles;
			set
			{
				BuildPolicy(ref _roles, value, RolesGroup);
			}
		}

		private void BuildPolicy(ref string[] target, string[] value, string group)
		{
			target = value ?? Array.Empty<string>();

			if (_isDefault)
			{
				Policy = string.Empty;
				_isDefault = false;
			}

			Policy += $"{group}${string.Join("|", target)};";
		}
	}

	public interface IIdentifiable
	{
		Guid Identifier { get; }
	}

	public class PermissionsRequirement : IAuthorizationRequirement, IIdentifiable
	{
		public PermissionsRequirement(string permissions, Guid identifier)
		{
			Permissions = permissions;
			Identifier = identifier;
		}

		public string Permissions { get; }

		public Guid Identifier { get; set; }
	}

	public class RolesRequirement : IAuthorizationRequirement, IIdentifiable
	{
		public RolesRequirement(string roles, Guid identifier)
		{
			Roles = roles;
			Identifier = identifier;
		}

		public string Roles { get; }

		public Guid Identifier { get; set; }
	}

	public class ScopesRequirement : IAuthorizationRequirement, IIdentifiable
	{
		public ScopesRequirement(string scopes, Guid identifier)
		{
			Scopes = scopes;
			Identifier = identifier;
		}

		public string Scopes { get; }

		public Guid Identifier { get; set; }
	}
}
