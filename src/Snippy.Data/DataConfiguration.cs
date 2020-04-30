using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace Snippy.Data
{
	public class DataConfiguration : IDataConfiguration
	{
		public DataConfiguration(IConfiguration Configuration)
		{
			Configuration.Bind("DBConfig", this);
		}

		public string ConnectionString { get; set; }
	}
}
