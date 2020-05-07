using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Snippy.Data;
using Snippy.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Snippy.Web.Controllers
{
	[ApiController]
	[Authorize]
	public class ApiController : Controller
	{
		private readonly ILogger<ApiController> _logger;
		private readonly IData _data;
		private readonly IActionContextAccessor _httpAccessor;

		public ApiController(ILogger<ApiController> logger, IData SnippyData, IActionContextAccessor HttpAccessor)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_data = SnippyData ?? throw new ArgumentNullException(nameof(SnippyData));
			_httpAccessor = HttpAccessor ?? throw new ArgumentNullException(nameof(HttpAccessor));
		}

		// [HttpGet]
		public IActionResult hc(string Id)
		{
			return null;
		}

		// [HttpGet]
		public ActionResult<bool> IsKeyAvail(string Key)
		{
			return _data.IsIdAvail(Key);
		}

		// [HttpPost]
		public ActionResult<bool> RegisterUrl(ShortURL url)
		{

			return true;
		}
	}
}
