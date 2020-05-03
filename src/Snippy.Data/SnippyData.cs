using Microsoft.Extensions.Logging;
using app = Snippy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Snippy.Data.Models;

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
			throw new NotImplementedException();
		}

		public bool RegisterUrl(app.ShortURL Url, app.Owner owner)
		{
			throw new NotImplementedException();
		}

		private void upsert(app.Owner owner)
		{

		}
	}
}
