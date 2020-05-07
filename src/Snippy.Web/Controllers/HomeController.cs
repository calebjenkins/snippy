using System;
using System.Linq;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Snippy.Data;
using Snippy.Models;
using Snippy.Web.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Snippy.Models.Productivity;

namespace Snippy.Web.Controllers
{
	[Authorize]
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly IData _data;
		private readonly IActionContextAccessor _httpAccessor;

		public HomeController(ILogger<HomeController> logger, IData SnippyData, IActionContextAccessor HttpAccessor)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_data = SnippyData ?? throw new ArgumentNullException(nameof(SnippyData));
			_httpAccessor = HttpAccessor ?? throw new ArgumentNullException(nameof(HttpAccessor));
		}

		public IActionResult Index(string Id)
		{
			var identity = _httpAccessor.ActionContext.HttpContext.User.Identity as ClaimsIdentity; // Azure AD V2 endpoint specific
			string preferred_username = identity.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;

			var msg = (Id.IsNullOrEmpty()) ? $"Welcome {preferred_username}!" : "--> 404 Not Found: {Id}";
			var model = new IndexViewModel()
			{
				Title = "Snippy Web | Main",
				Platform = Environment.OSVersion.ToString(),
				AuthenticatedUser = _data.GetOwner("hello"),
				Message = msg
			};

			_logger.LogInformation($"Log Info from Index controller { DateTime.Now.ToString() }");

			return View("Index", model);
		}

		[AllowAnonymous]
		public IActionResult Short(string Id, string ExtraPath)
		{
			var httpIdentity = _httpAccessor.ActionContext.HttpContext.User.Identity as ClaimsIdentity;
			string preferred_username = httpIdentity.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
			string user_name = httpIdentity.Claims.FirstOrDefault(c => c.Type == "name")?.Value;

			var request = new ClickRequest()
			{
				ShortUrlKey = Id,
				IdentId = preferred_username,
				SourceIp = _httpAccessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString()
			};

			var data = _data.RegisterClick(request);
			if (data == null)
			{
				return Index(Id);
			}

			var extraPathWithQuery = ExtraPath + HttpContext.Request.QueryString.Value;
			var urlDivider = (data.Url.EndsWith('/')) ? string.Empty : "/";
			var completeURL = (extraPathWithQuery.Length > 0) ? data.Url + urlDivider + extraPathWithQuery : data.Url;

			// Send to URL
			_logger.LogInformation($"Forwarding to {completeURL}");
			_httpAccessor.ActionContext.HttpContext.Response.Redirect(completeURL);
			return null;
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
