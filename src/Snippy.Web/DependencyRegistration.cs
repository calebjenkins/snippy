using Lamar;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Snippy.Data;

namespace Snippy.Web
{
	public class DependencyRegistration : ServiceRegistry
	{
		public DependencyRegistration()
		{
			For(typeof(ILogger<>)).Use(typeof(Logger<>));
			For<IData>().Use<SampleData>().Singleton();
			For<IActionContextAccessor>().Use<ActionContextAccessor>().Singleton();
			For<IDataConfiguration>().Use<DataConfiguration>().Singleton();
			For<ISnippyDataContext>().Use<SnippyDataContext>();
		}
	}
}
