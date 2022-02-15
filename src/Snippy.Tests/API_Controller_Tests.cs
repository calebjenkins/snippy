using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using Snippy.Data;
using Snippy.Web.Controllers;
using System.Collections.Generic;
using System.Security.Claims;
using Xunit;

namespace Snippy.Tests;

public class API_Controller_Tests
{
    private Mock<IActionContextAccessor> getHttpMockWith(IEnumerable<Claim> claims)
    {

        var Ident = new ClaimsIdentity(claims);
        var User = new ClaimsPrincipal(Ident);
        var HttpCx = new TestHttpContext() { User = User };
        var ActionCx = new ActionContext() { HttpContext = HttpCx };
        var MockAction = new Mock<IActionContextAccessor>();
        MockAction.Setup(x => x.ActionContext).Returns(ActionCx);

        return MockAction;
    }

    [Fact]
    public void API_WHoIAm_Should_return_all_claims_asJsonResults()
    {
        var mockLogger = new Mock<ILogger<ApiController>>();
        var mockData = new Mock<IData>();

        IEnumerable<Claim> claims = new List<Claim>()
        {
            new Claim ("name", "Joe"),
            new Claim("role", "Test")
        };

        var httpMock = getHttpMockWith(claims);
        var api = new ApiController(mockLogger.Object, mockData.Object, httpMock.Object);

        var results = api.WhoAmI();

        results.Should().NotBeNull();
        var keyValue = results.Value as IEnumerable<(string, string)>;
        keyValue.Should().NotBeNull();

        Dictionary<string, string> map = new Dictionary<string, string>();
        foreach (var kv in keyValue)
        {
            map.Add(kv.Item1, kv.Item2);
        }
        map.Count.Should().Be(2);

        // var list = RockLib.Serialization.Serializer.FromJson<IEnumerable<(string, string)>>();
    }

}

