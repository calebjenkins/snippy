using Lamar;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Snippy.Data;
using Snippy.Web.Controllers;
using System;
using System.Linq;

namespace Snippy.Web
{
	public class DependencyRegistration : ServiceRegistry
	{
		public DependencyRegistration()
		{
			For(typeof(ILogger<>)).Use(typeof(Logger<>));
			For<IData>().Use<SampleData>();
			For<IActionContextAccessor>().Use<ActionContextAccessor>();
		}
	}
}
