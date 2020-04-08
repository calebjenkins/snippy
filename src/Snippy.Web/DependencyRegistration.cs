using Lamar;
using Microsoft.Extensions.Logging;
using Snippy.Data;
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
		}
	}
}
