using Snippy.Models;
using System;
using System.Collections.Generic;

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
}
