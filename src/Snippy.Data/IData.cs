using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Snippy.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Snippy.Data
{
	public interface IData
	{
		ShortURL RegisterClick(ClickRequest request);

		IList<ShortURL> GetURLs(string IdentId);
		Owner GetOwner(string IdentId);
		bool RegisterUrl(ShortURL Url, Owner owner);
		bool IsIdAvail(string UrlKey);
	}
	public class SampleData : IData
	{

		private Dictionary<string, ShortURL> _urls = new Dictionary<string, ShortURL>();
		private readonly DbContextOptions<SnippyDataContext> _options;
		private readonly ILogger<SnippyDataContext> _dbLogger;
		public SampleData(DbContextOptions<SnippyDataContext> Options, ILogger<SnippyDataContext> dbLogger)
		{
			_options = Options;
			_dbLogger = dbLogger;

			List<ShortURL> urls = new List<ShortURL>()
			{
				new ShortURL() { Key = "goog", Url = "https://google.com" },
				new ShortURL() { Key = "me", Url = "https://developingux.com" },
				new ShortURL() { Key = "index", Url = "https://linkedin.com" },
				new ShortURL() { Key = "li", Url = "https://linkedin.com/in/calebjenkins/" },
			};

			foreach (var url in urls)
			{
				_urls.Add(url.Key, url);
			}

			using (var db = new SnippyDataContext(_options, dbLogger))
			{
				var o = db.Owners
					.Where(own => own.FullName == "Caleb Jenkins")
					.FirstOrDefault();
			}
		}

		public Owner GetOwner(string IdentId)
		{
			return new Owner()
			{
				Email = "caleb.jenkins@solera.com",
				Id = IdentId,
				URLs = _urls.Values.ToList()
			};
		}

		public IList<ShortURL> GetURLs(string IdentId)
		{
			return _urls.Values.ToList();
		}

		public bool IsIdAvail(string UrlKey)
		{
			return !_urls.ContainsKey(UrlKey);
		}

		public ShortURL RegisterClick(ClickRequest request)
		{
			return (IsIdAvail(request.ShortUrlId)) ? null : _urls[request.ShortUrlId];
		}

		public bool RegisterUrl(ShortURL Url, Owner owner)
		{
			if (!IsIdAvail(Url.Key))
				return false;
			else
			{
				_urls.Add(Url.Key, Url);
				return true;
			}
		}
	}
}
