using System.Collections.Generic;
using app = Snippy.Models;

namespace Snippy.Data.Models
{
	public static class DataModelExtensions
	{
		public static app.Owner Convert(this Owner model)
		{
			IList<app.ShortURL> urls = new List<app.ShortURL>();
			foreach (var u in model.OwnerURLs)
			{
				urls.Add(u.URL.Convert());
			}

			return new app.Owner()
			{
				Email = model.Email,
				Id = model.UserName,
				URLs = urls,
				FullName = model.FullName
			};
		}

		public static app.ShortURL Convert(this ShortURL model)
		{
			return new app.ShortURL()
			{
				Key = model.Key,
				Url = model.Url
			};
		}
	}
}
