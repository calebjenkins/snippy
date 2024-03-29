﻿using Snippy.Models;
using System.Collections.Generic;
using System.Linq;

namespace Snippy.Data
{
    public class SampleData : IData
	{

		private Dictionary<string, ShortURL> _urls = new Dictionary<string, ShortURL>();
		IDataConfiguration _config;
		public SampleData(IDataConfiguration config)
		{
			_config = config;

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
