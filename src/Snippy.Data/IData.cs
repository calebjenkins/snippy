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
		IList<ShortURL> GetUnOwnedURLs();
		Owner GetOwner(string IdentId);
		bool RegisterUrl(ShortURL Url, Owner owner);
		bool IsIdAvail(string UrlKey);
	}
}
