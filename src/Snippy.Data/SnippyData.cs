using Microsoft.Extensions.Logging;
using Snippy.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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

		public Owner GetOwner(string IdentId)
		{
			var owner = _db.Owners
			.Where(s => s.UserName == IdentId)
			.FirstOrDefault();

			//return owner;
			throw new NotImplementedException();
		}

		public IList<ShortURL> GetURLs(string IdentId)
		{
			throw new NotImplementedException();
		}

		public bool IsIdAvail(string UrlKey)
		{
			throw new NotImplementedException();
		}

		public ShortURL RegisterClick(ClickRequest request)
		{
			throw new NotImplementedException();
		}

		public bool RegisterUrl(ShortURL Url, Owner owner)
		{
			throw new NotImplementedException();
		}
	}
}
