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

			// Used in Controllers
			For<IActionContextAccessor>().Use<ActionContextAccessor>().Singleton();

			// Data Layer
			For<IData>().Use<SnippyData>();
			// For<IDataConfiguration>().Use<DataConfiguration>().Singleton(); // (Moving this to StartUp.cs Service registration so I can utilize the IConfiguration)
			For<ISnippyDataContext>().Use<SnippyDataContext>();
		}
	}
}
