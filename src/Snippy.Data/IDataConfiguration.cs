using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace Snippy.Data
{
	public interface IDataConfiguration
	{
		public string ConnectionString { get; }
	}
}
