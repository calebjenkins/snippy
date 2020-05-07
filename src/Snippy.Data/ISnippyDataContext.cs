using Microsoft.EntityFrameworkCore;
using Snippy.Data.Models;
using System;

namespace Snippy.Data
{
	public interface ISnippyDataContext : IDisposable
	{
		int SaveChanges();
		DbSet<Click> Clicks { get; set; }
		DbSet<Owner> Owners { get; set; }
		DbSet<OwnerUrls> OwnerUrls { get; set; }
		DbSet<ShortURL> URLs { get; set; }
	}
}
