using Microsoft.Extensions.Logging;
using app = Snippy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Snippy.Data.Models;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore;

namespace Snippy.Data
{
	public class SnippyData : IData
	{
		ISnippyDataContext _db;
		ILogger<SnippyData> _logger;

		public SnippyData(ISnippyDataContext DataContext, ILogger<SnippyData> Logger)
		{
			_db = DataContext ?? throw new ArgumentNullException(nameof(DataContext));
			_logger = Logger ?? throw new ArgumentNullException(nameof(Logger));
		}

		public app.Owner GetOwner(string IdentId)
		{
			var owner = _db.Owners
			.Where(s => s.UserName == IdentId)
			.FirstOrDefault();

			return owner.Convert() ?? new app.Owner();
		}
		public IList<app.ShortURL> GetUnOwnedURLs()
		{
			var urls = _db.URLs
				.Where(u => u.OwnerURLs == null)
				.Include(u=>u.OwnerURLs)
				.ThenInclude(ou=>ou.Owner)
				.ToList();

			return urls.Convert();
		}

		public IList<app.ShortURL> GetURLs(string IdentId)
		{
			IList<ShortURL> urls = _db.URLs
					.Where(u => u.OwnerURLs.Any(ou => ou.OwnerUserName == IdentId)).ToList();

			return urls.Convert();

		}

		public bool IsIdAvail(string UrlKey)
		{
			var result = _db.URLs.Where(u => u.Key == UrlKey).FirstOrDefault();

			return result == null;
		}

		public app.ShortURL RegisterClick(app.ClickRequest request)
		{
			var click = request.Convert();
			_db.Clicks.Add(click);
			_db.SaveChanges();

			var url = _db.URLs.Where(u => u.Key == request.ShortUrlKey).FirstOrDefault();
			return url.Convert();

		}

		public bool RegisterUrl(app.ShortURL url, app.Owner owner)
		{
			var dbOwner = upsert(owner);
			var dbUrl = upsert(url, owner.Id);
			var relationship = upsert(dbOwner, dbUrl);

			return relationship != null;
		}

		private Owner upsert(app.Owner owner)
		{
			var now = DateTime.Now;
			var user = _db.Owners
				.Where(o => o.UserName == owner.Id)
				.FirstOrDefault();

			var newOwner = user ?? new Owner()
			{
				CreatedBy = owner.Id,
				CreatedOn = now,
				UpdatedBy = owner.Id,
				UpdatedOn = now,
				Email = owner.Email,
				FullName = owner.FullName,
				UserName = owner.Id
			};

			if (user == null)
			{
				_db.Owners.Add(newOwner);
				_db.SaveChanges();
			}

			return newOwner;
		}
		private ShortURL upsert(app.ShortURL url, string UserId)
		{
			var now = DateTime.Now;
			var dbUrl = _db.URLs.Where(u => u.Key == url.Key).FirstOrDefault();
			var newURL = dbUrl ?? new ShortURL()
			{
				Key = url.Key,
				Url = url.Url,
				CreatedBy = UserId,
				CreatedOn = now,
				UpdatedBy = UserId,
				UpdatedOn = now
			};

			if (dbUrl == null)
			{
				_db.URLs.Add(newURL);
				_db.SaveChanges();
			}

			return newURL;
		}
		private OwnerUrls upsert(Owner owner, ShortURL url)
		{
			var now = DateTime.Now;
			var relationship = _db.OwnerUrls.Where(ou => ou.OwnerUserName == owner.UserName && ou.ShortUrlKey == url.Key).FirstOrDefault();
			var newRalationship = relationship ?? new OwnerUrls()
			{
				CreatedBy = owner.UserName,
				CreatedOn = now,
				UpdatedBy = owner.UserName,
				UpdatedOn = now,
				OwnerUserName = owner.UserName,
				ShortUrlKey = url.Key,
				Owner = owner,
				URL = url
			};

			if (relationship == null)
			{
				_db.OwnerUrls.Add(newRalationship);
				_db.SaveChanges();
			}

			return newRalationship;
		}
	}
}
