using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Snippy.Data;
using Snippy.Models;
using Snippy.Web.Models;
using System.Security.Claims;

namespace Snippy.Web.Controllers;

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

    [HttpGet("api/shorts")]
    public IActionResult GetOwnedShorts()
    {
        var httpIdentity = _httpAccessor?.GetIdentity();
        string? preferred_username = httpIdentity?.GetClaim("preferred_username");
        string? user_name = httpIdentity?.GetClaim(ClaimTypes.NameIdentifier);
        string? user_email = httpIdentity?.GetClaim(ClaimTypes.Email);

        string? source_ip = _httpAccessor?.ActionContext?.HttpContext?.Connection?.RemoteIpAddress?.ToString();

        IEnumerable<ShortURL> shorts = new List<ShortURL>();

        var owner = _data.GetOwner(user_name);
        if (owner == null)
            return shorts.ToJsonResult();


        shorts = _data.GetURLs(owner.Id);
        return shorts.ToJsonResult();
    }
    [HttpGet("api/keys")]
    public JsonResult getKeys(string startswith, int limit = 5)
    {

        return null;
    }

    [HttpGet("api/whoami")]
    public JsonResult WhoAmI()
    {
        List<(string, string)> claimPairs = new List<(string, string)>();

        var claims = _httpAccessor.GetClaims();

        foreach(Claim c in claims)
        {
            claimPairs.Add(new (c.Type, c.Value));
        }

        return claimPairs.ToJsonResult();
    }
}
