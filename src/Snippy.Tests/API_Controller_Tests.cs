using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using Snippy.Data;
using Snippy.Models;
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
        MockAction.Setup(x => x.ActionContext).Returns(ActionCx).Verifiable();

        return MockAction;
    }

    [Fact]
    public void API_WhoAmI_Should_return_all_claims_asJsonResults()
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

        httpMock.VerifyAll();
    }

    [Fact]
    public void API_DeleteShort_shouldVerifyOwner_and_call_delete()
    {
        var owner = new Owner() { Email = "email@email.com", Id = "123", UserName = "Fake Joe" };
        IList<ShortURL> urls = new List<ShortURL>()
        {
            new ShortURL() { Key = "goog", Url = "http://google.com "},
            new ShortURL() { Key = "456", Url = "https://developingux.com"}
        };

        var mockLogger = new Mock<ILogger<ApiController>>();
        var mockData = new Mock<IData>();
        mockData.Setup(x => x.GetOwner(It.Is<string>(s => s == owner.Id)))
            .Returns(owner)
            .Verifiable();
        mockData.Setup(x => x.GetURLs(It.Is<string>(s => s == owner.Id)))
            .Returns(urls)
            .Verifiable();

        mockData.Setup(x => x.DeleteShort(It.Is<string>(s => s == "456")))
            .Returns(true)
            .Verifiable();

        IEnumerable<Claim> claims = getClaimsFor(owner);

        var httpMock = getHttpMockWith(claims);
        var api = new ApiController(mockLogger.Object, mockData.Object, httpMock.Object);

        var results = api.DeleteShort("456");
        results.Value.Should().Be(true);

        mockData.VerifyAll();
        httpMock.VerifyAll();
    }

    [Fact]
    public void API_DeleteShort_should_return_false_when_no_urls_found()
    {
        var owner = new Owner() { Email = "email@email.com", Id = "123", UserName = "Fake Joe" };

        var mockLogger = new Mock<ILogger<ApiController>>();
        var mockData = new Mock<IData>();
        mockData.Setup(x => x.GetOwner(It.Is<string>(s => s == owner.Id)))
            .Returns(owner)
            .Verifiable();

        IEnumerable<Claim> claims = getClaimsFor(owner);

        var httpMock = getHttpMockWith(claims);
        var api = new ApiController(mockLogger.Object, mockData.Object, httpMock.Object);

        var results = api.DeleteShort("456");
        results.Value.Should().Be(false);

        mockData.VerifyAll();
        httpMock.VerifyAll();
    }

    private static IEnumerable<Claim> getClaimsFor(Owner owner)
    {
        return new List<Claim>()
         {
             new Claim (ClaimTypes.NameIdentifier, owner.Id),
             new Claim(ClaimTypes.Email, owner.Email),
             new Claim("preferred_username", owner.UserName)
         };
    }
}
