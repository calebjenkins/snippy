using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Snippy.Data;
using Snippy.Data.Models;
using Snippy.Models.Productivity;
using Xunit;

namespace Snippy.Tests.Integration.Database
{
	public class DatabaseTests
	{
		private ISnippyDataContext getContext(ILogger<SnippyDataContext> logger = null)
		{
			var connString = @"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=SnippyDB;Integrated Security=SSPI";
			var optionsBuilder = new DbContextOptionsBuilder<SnippyDataContext>();
			var dbOption = optionsBuilder.UseSqlServer(connString).Options;
			return new SnippyDataContext(dbOption, null);
		}
		private Owner genericOwner()
		{
			var now = DateTime.Now;
			return new Owner()
			{
				CreatedBy = "first.last",
				CreatedOn = now,
				UpdatedBy = "first.last",
				UpdatedOn = now,
				Email = "first.last@email.com",
				FullName = "First Last",
				UserName = "first.last@email.com"
			};
		}
		private ShortURL genericURL(Owner o, string key = "test", string url = "http://google.com/?q=test")
		{
			var now = DateTime.Now;
			return new ShortURL()
			{
				CreatedBy = o.UserName,
				CreatedOn = now,
				UpdatedBy = o.UserName,
				UpdatedOn = now,
				Key = key,
				Url = url
			};
		}
		private OwnerUrls getOwnerUrl(Owner o, ShortURL url)
		{
			var now = DateTime.Now;
			return new OwnerUrls()
			{
				CreatedBy = o.UserName,
				CreatedOn = now,
				UpdatedBy = o.UserName,
				UpdatedOn = now,
				Owner = o,
				OwnerUserName = o.UserName,
				URL = url,
				ShortUrlKey = url.Key
			};
		}

		[Fact]
		public void CanConnectToDB()
		{
			using (var db = getContext())
			{
				var owners = db.Owners.FirstOrDefault();
				Assert.True(true);
			}
		}

		[Fact]
		void CanWriteToOwners()
		{
			// Write
			using (var db = getContext())
			{
				var o = genericOwner();
				db.Owners.Add(o);
				db.SaveChanges();
				Assert.True(true);
			}

			// Read
			using (var db = getContext())
			{
				var o = genericOwner();
				var savedOwner = db.Owners.Where(dbOwner => dbOwner.UserName == o.UserName).FirstOrDefault();
				Assert.NotNull(savedOwner);
				Assert.True(savedOwner.FullName == o.FullName);
			}

			// Delete / Cleanup
			using (var db = getContext())
			{
				var o = genericOwner();
				var savedOwner = db.Owners.Where(dbOwner => dbOwner.UserName == o.UserName).FirstOrDefault();
				if (savedOwner != null)
				{
					db.Owners.Remove(savedOwner);
					db.SaveChanges();
				}
			}

			// Confirm Removed
			using (var db = getContext())
			{
				var o = genericOwner();
				var savedOwner = db.Owners.Where(dbOwner => dbOwner.UserName == o.UserName).FirstOrDefault();
				Assert.Null(savedOwner);

			}
		}

		[Fact]
		void CanRegisterURL()
		{
			var o = genericOwner();
			var url = genericURL(o, "key", "http://google.com");
			var url2 = genericURL(o, "key2", "http://bing.com");

			var relationShip = getOwnerUrl(o, url);
			var relationShip2 = getOwnerUrl(o, url2);

			var now = DateTime.Now;

			// Write
			using (var db = getContext())
			{
				db.Owners.Add(o);
				db.URLs.Add(url);
				db.OwnerUrls.Add(relationShip);
				db.OwnerUrls.Add(relationShip2);
				db.SaveChanges();
			}

			// Read
			using (var db = getContext())
			{
				var dbOwner = db.Owners.Where(dbO => dbO.UserName == o.UserName)
					.Include(o => o.OwnerURLs)
					.ThenInclude(ou => ou.URL)
					.FirstOrDefault();

				var urls = dbOwner.OwnerURLs.Items<OwnerUrls, ShortURL>((itm) => itm.URL);

				Assert.Equal<int>(2, urls.Count());
				Assert.Equal<int>(2, dbOwner.OwnerURLs.Count);
			}

			// Clean Up
			using (var db = getContext())
			{
				var dbOwner = db.Owners.Where(dbO => dbO.UserName == o.UserName)
					.Include(o => o.OwnerURLs)
					.ThenInclude(ou => ou.URL)
					.FirstOrDefault();
				var urls = dbOwner.OwnerURLs.Items<OwnerUrls, ShortURL>((itm) => itm.URL);

				db.Owners.Remove(dbOwner);
				db.SaveChanges();

				foreach (var u in urls)
				{
					db.URLs.Remove(u);
				}
				db.SaveChanges();
			}
		}

		[Fact]
		void CanWriteClicksToUnknownURLs()
		{

			var Id = Guid.NewGuid();
			var now = DateTime.Now;
			var c = new Click()
			{
				CreatedBy = "",
				CreatedOn = now,
				Id = Id,
				IdentId = "",
				UpdatedBy = "",
				UpdatedOn = now,
				ShortUrlKey = "BlahBlah",
				SourceIp = "111.111.111.111"
			};

			// Write
			using (var db = getContext())
			{
				db.Clicks.Add(c);
				var result = db.SaveChanges();
				Assert.True(result > 0);
			}


			// clear
			using (var db = getContext())
			{
				var clicks = db.Clicks.Where(c => c.Id == Id).ToList();
				db.Clicks.RemoveRange(clicks);
				var result = db.SaveChanges();
				Assert.True(result > 0);
			}
		}
	}
}
