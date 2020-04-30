using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Snippy.Data.Models;
using System;
using System.Linq;

namespace Snippy.Data
{
	public class SnippyDataContext : DbContext, ISnippyDataContext
	{
		IDataConfiguration _config;
		public SnippyDataContext(IDataConfiguration Config)
		{
			_config = Config;
		}

		public DbSet<Click> Clicks { get; set; }
		public DbSet<Owner> Owners { get; set; }
		public DbSet<ShortURL> URLs { get; set; }
		public DbSet<OwnerUrls> OwnerUrls { get; set; }


		protected override void OnConfiguring(DbContextOptionsBuilder options)
				 => options.UseSqlServer(_config.ConnectionString);

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Owner>()
				.HasKey(o => o.UserName);

			modelBuilder.Entity<ShortURL>()
				.HasKey(url => url.Key);

			modelBuilder.Entity<Click>()
				.HasKey(c => c.Id);

			modelBuilder.Entity<Click>()
				.HasOne<ShortURL>(c => c.UrlClicked)
				.WithMany(url => url.Clicks)
				.HasForeignKey(c => c.ShortUrlKey)
				.IsRequired();

			modelBuilder.Entity<OwnerUrls>()
				.HasKey(x => new { x.OwnerUserName, x.ShortUrlKey, });

			modelBuilder.Entity<OwnerUrls>()
				.HasOne(ou => ou.Owner)
				.WithMany(o => o.OwnerURLs)
				.HasForeignKey(ou => ou.OwnerUserName);

			modelBuilder.Entity<OwnerUrls>()
				.HasOne(ou => ou.URL)
				.WithMany(url => url.OwnerURLs)
				.HasForeignKey(ou => ou.ShortUrlKey);
		}
	}
}
