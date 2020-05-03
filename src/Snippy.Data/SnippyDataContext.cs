using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using Snippy.Data.Models;
using System;
using System.Linq;

namespace Snippy.Data
{
	public class SnippyDataContext : DbContext, ISnippyDataContext
	{
		/*  ***************************************************************************
		 *  Use dotnet cli with dotnet-ef tool to make migrations work properly.
		 *  > dotnet tool add dotnet-ef --global
		 *  (need to make sure the dotnet tools director is in the Path - for Windows
		 *    %userfolder%\.dotnet\tools    ex: C:\Users\First.Last\.dotnet\tools)
		 *  then cd ./Snippy.Data/
		 *  dotnet ef migrations add InitialCreate
		 *  dotnet ef migrations list
		 *  dotnet ef database update
		 *  dotnet ef database drop
		 *  dotnet ef migrations remove
		 *  dotnet ef migrations add InitialCreate --startup-project ..\Snippy.Web\
		 */

		DbContextOptions _options;
		ILogger<SnippyDataContext> _logger;

		public SnippyDataContext(DbContextOptions<SnippyDataContext> options, ILogger<SnippyDataContext> logger) : base(options)
		{
			_options = options ?? throw new ArgumentNullException(nameof(options));
			_logger = logger; // can't check or null... since EF Migrations Design Factory needs to pass null.
		}

		public DbSet<Click> Clicks { get; set; }
		public DbSet<Owner> Owners { get; set; }
		public DbSet<ShortURL> URLs { get; set; }
		public DbSet<OwnerUrls> OwnerUrls { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Owner
			configureBaseProperties(modelBuilder.Entity<Owner>());
			modelBuilder.Entity<Owner>()
				.HasKey(o => o.UserName);

			modelBuilder.Entity<Owner>()
				.Property<string>(o => o.UserName)
				.HasMaxLength(100);

			modelBuilder.Entity<Owner>()
				.Property<string>(o => o.FullName)
				.HasMaxLength(200);

			modelBuilder.Entity<Owner>()
				.Property<string>(o => o.Email)
				.HasMaxLength(100);


			// Short URL
			configureBaseProperties(modelBuilder.Entity<ShortURL>());
			modelBuilder.Entity<ShortURL>()
				.HasKey(url => url.Key);

			modelBuilder.Entity<ShortURL>()
				.Property(url => url.Key)
				.HasMaxLength(60);


			// Click
			configureBaseProperties(modelBuilder.Entity<Click>(), false);
			modelBuilder.Entity<Click>()
				.HasKey(c => c.Id);

			modelBuilder.Entity<Click>()
				.HasOne<ShortURL>(c => c.UrlClicked)
				.WithMany(url => url.Clicks)
				.HasForeignKey(c => c.ShortUrlKey)
				.IsRequired();

			modelBuilder.Entity<Click>()
				.Property<string>(c => c.SourceIp)
				.HasMaxLength(20);

			modelBuilder.Entity<Click>()
				.Property<string>(c => c.IdentId)
				.HasMaxLength(100);

			// Owner URL Many-to-Many
			configureBaseProperties(modelBuilder.Entity<OwnerUrls>());
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

		private void configureBaseProperties<T>(EntityTypeBuilder<T> entity, bool IsAuth = true) where T : DataModelBase
		{
			entity
				.Property<string>(e => e.CreatedBy)
				.HasMaxLength(100);

			entity
				.Property<string>(e => e.UpdatedBy)
				.HasMaxLength(100);

			entity
				.Property<DateTime>(e => e.CreatedOn)
				.IsRequired();

			entity
				.Property<DateTime>(e => e.UpdatedOn)
				.IsRequired();

			if (IsAuth) // These models are only updated when a user is authenticated
			{
				entity
					.Property<string>(e => e.CreatedBy)
					.IsRequired();

				entity
					.Property<string>(e => e.UpdatedBy)
					.IsRequired();
			}
		}
	}
}
