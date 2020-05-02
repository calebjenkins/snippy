using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Snippy.Data;
using System.IO;
using System.Reflection;

namespace Snippy.Web
{
	public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<SnippyDataContext>
	{
		readonly string _cnString;
		public ApplicationDbContextFactory()
		{
			var baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
			baseDir = baseDir.Replace("file:\\", "");

			IConfiguration configuration = new ConfigurationBuilder()
					.SetBasePath(baseDir) // Directory where the json files are located
					.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
					.AddUserSecrets(Assembly.GetExecutingAssembly())

					.Build();

			_cnString = configuration.GetValue<string>("DBConfig:ConnectionString");
		}

		public SnippyDataContext CreateDbContext(string[] args)
		{
			var optionsBuilder = new DbContextOptionsBuilder<SnippyDataContext>();
			optionsBuilder.UseSqlServer(_cnString);
			return new SnippyDataContext(optionsBuilder.Options, null);
		}

	}
}
