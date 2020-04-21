using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Snippy.Data;
using Snippy.Web.Models;

namespace Snippy.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly IData _data;

		public HomeController(ILogger<HomeController> logger, IData SnippyData)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_data = SnippyData ?? throw new ArgumentNullException(nameof(SnippyData));
		}

		public IActionResult Index(string Id)
		{
			var model = new IndexViewModel()
			{
				Title = "Snippy Web | Main",
				Platform = Environment.OSVersion.ToString(),
				AuthenticatedUser = _data.GetOwner("hello"),
				Message = "-->" + Id + "<--"
			};

			_logger.LogInformation($"Log Info from Index controller { DateTime.Now.ToString() }");

			return View(model);
		}

		public IActionResult Short(string Id, string ExtraPath)
		{
			var model = new IndexViewModel()
			{
				Title = "Snippy Web | Short",
				Platform = Environment.OSVersion.ToString(),
				AuthenticatedUser = _data.GetOwner("hello"),
				Message = $"-->{Id}<-- -->{ExtraPath}<--"
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
