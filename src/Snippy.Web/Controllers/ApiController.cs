using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Snippy.Data;
using Snippy.Models;
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
        return _data.GetKeys(startswith, limit).ToJsonResult();
    }

    [HttpGet("api/isavailable")]
    public JsonResult isAvail(string shortKey)
    {
        return _data.IsIdAvail(shortKey).ToJsonResult();
    }

    [HttpPost("api/short")]
    [ValidateAntiForgeryToken]
    public JsonResult CreateShort(ShortURL shortUrl)
    {
        string? userId = _httpAccessor.GetClaim(ClaimTypes.NameIdentifier);
        var owner = getOwnerFromDbOrIdent(_httpAccessor.GetIdentity());

        return _data.RegisterUrl(shortUrl, owner).ToJsonResult();
    }

    [HttpDelete("api/short")]
    [ValidateAntiForgeryToken]
    public JsonResult DeleteShort(string shortKey)
    {
        string? userId = _httpAccessor.GetClaim(ClaimTypes.NameIdentifier);
        var owner = getOwnerFromDbOrIdent(_httpAccessor.GetIdentity());

        var urls = _data.GetURLs(owner.Id);
        var url = urls?.Where(u => u.Key == shortKey).FirstOrDefault();
        if(url == null)
        {
            return false.ToJsonResult();
        }

        return _data.DeleteShort(shortKey).ToJsonResult();
    }

    private Owner getOwnerFromDbOrIdent(ClaimsIdentity? identity)
    {
        string? userId = identity?.GetClaim(ClaimTypes.NameIdentifier);
        var owner = _data.GetOwner(userId);
        if (owner != null)
        {
            return owner;
        }

        string? preferred_username = identity?.GetClaim("preferred_username");
        string? user_email = identity?.GetClaim(ClaimTypes.Email);

        owner = new Owner()
        {
            Email = user_email,
            UserName = preferred_username,
            Id = userId
        };

        return owner;
    }

    [HttpGet("api/whoami")]
    public JsonResult WhoAmI()
    {
        List<(string, string)> claimPairs = new List<(string, string)>();

        var claims = _httpAccessor.GetClaims();

        foreach (Claim c in claims)
        {
            claimPairs.Add(new(c.Type, c.Value));
        }

        return claimPairs.ToJsonResult();
    }
}
