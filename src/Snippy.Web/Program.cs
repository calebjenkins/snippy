using Lamar.Microsoft.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Snippy.Web
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
				Host.CreateDefaultBuilder(args)
						.UseLamar(new DependencyRegistration())

						.ConfigureWebHostDefaults(webBuilder =>
						{
							webBuilder.UseStartup<Startup>();
						});
	}
}
