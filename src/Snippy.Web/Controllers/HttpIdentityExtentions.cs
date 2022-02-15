using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Security.Claims;

namespace Snippy.Web.Controllers;

public static  class HttpIdentityExtentions
{
    public static ClaimsIdentity? GetIdentity(this IActionContextAccessor http)
    {
        return http?.ActionContext?.HttpContext.User.Identity as ClaimsIdentity;
    }

    public static string GetClaim(this IActionContextAccessor http, string key)
    {
        var identity = http.GetIdentity();
        string? value = identity?.Claims?.FirstOrDefault(c => c.Type == key)?.Value;
        return value ?? string.Empty;
    }
    public static string GetClaim(this ClaimsIdentity identity, string key)
    {
        string? value = identity?.Claims?.FirstOrDefault(c => c.Type == key)?.Value;
        return value ?? string.Empty;
    }
    public static IEnumerable<Claim> GetClaims (this IActionContextAccessor http)
    {
        var identity = http.GetIdentity();
        if(identity?.Claims != null)
        {
            return identity.Claims;
        }

        return new List<Claim>();
    }

    public static JsonResult ToJsonResult(this object value)
    {
        return new JsonResult(value);
    }
}
