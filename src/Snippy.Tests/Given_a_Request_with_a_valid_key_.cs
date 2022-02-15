using FakeAuth.Profiles;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using Snippy.Data;
using Snippy.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xunit;

namespace Snippy.Tests;

public class Given_a_Request_with_a_valid_key_
{
    ILogger<HomeController> _logger;
    IData _dataLayer;
    IActionContextAccessor _http;


    [Fact]
    public async void Should_register_click()
    {
        _logger = new Mock<ILogger<HomeController>>().Object;
        _dataLayer = new Mock<IData>().Object;
        _http = new Mock<IActionContextAccessor>().Object;

        var controller = new HomeController(_logger, _dataLayer, _http);

        var results = controller.Index("goog");
        results.Should().NotBeNull();

        var shortResult = controller.Short("goog", "morestuff");
        shortResult.Should().NotBeNull();

    }

    [Fact]
    public void Should_forward_URL_to_full_URL()
    {

    }

    [Fact]
    public void List_FakeAuth_Claims()
    {
        List<string> keys = (List<string>)new AzureProfile().GetClaimKeys();

        Trace.WriteLine("Keys:");
        keys.ForEach((k) => Trace.WriteLine($" ---> {k}"));

        Debug.WriteLine("Keys:");
        keys.ForEach((k) => Debug.WriteLine($" ---> {k}"));


        Console.WriteLine("Keys:");
        keys.ForEach((k) => Console.WriteLine($" ---> {k}"));
    }
}

