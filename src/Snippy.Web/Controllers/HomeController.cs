using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Snippy.Data;
using Snippy.Models;
using Snippy.Web.Models;

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
			var identity = User.Identity as ClaimsIdentity; // Azure AD V2 endpoint specific
			string preferred_username = identity.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;


			var model = new IndexViewModel()
			{
				Title = "Snippy Web | Main",
				Platform = Environment.OSVersion.ToString(),
				AuthenticatedUser = _data.GetOwner("hello"),
				Message = $"-->{Id}<-- Welcome: {preferred_username}"
			};

			_logger.LogInformation($"Log Info from Index controller { DateTime.Now.ToString() }");

			return View(model);
		}

		[AllowAnonymous]
		public IActionResult Short(string Id, string ExtraPath)
		{
			var httpIdentity = _httpAccessor.ActionContext.HttpContext.User.Identity as ClaimsIdentity;
			string preferred_username = httpIdentity.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
			string user_name = httpIdentity.Claims.FirstOrDefault(c => c.Type == "name")?.Value;

			var request = new ClickRequest()
			{
				ShortUrlId = Id,
				IdentId = preferred_username,
				SourceIp = _httpAccessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString()
			};

			var data = _data.RegisterClick(request);

			var extraPathWithQuery = ExtraPath + HttpContext.Request.QueryString.Value;
			var urlDivider = (data.Url.EndsWith('/')) ? string.Empty : "/";
			var completeURL = (extraPathWithQuery.Length > 0) ? data.Url + urlDivider + extraPathWithQuery : data.Url;

			var model = new IndexViewModel()
			{
				Title = "Snippy Web | Short",
				Platform = Environment.OSVersion.ToString(),
				AuthenticatedUser = _data.GetOwner(preferred_username),
				Message = $"-->{Id} | {completeURL}<-- for: {user_name} ({preferred_username}) from IP {request.SourceIp}"
			};

			_logger.LogInformation($"Log Info from Shorty controller { DateTime.Now.ToString() }");

			return View("Index", model);
		}


		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
