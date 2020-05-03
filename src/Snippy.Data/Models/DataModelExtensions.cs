using System;
using System.Collections.Generic;
using app = Snippy.Models;

namespace Snippy.Data.Models
{
	public static class DataModelExtensions
	{
		public static app.Owner Convert(this Owner model)
		{
			if (model == null)
				return new app.Owner();

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

		public static IList<app.ShortURL> Convert(this IList<ShortURL> model)
		{
			IList<app.ShortURL> urls = new List<app.ShortURL>();
			foreach(var u in model)
			{
				urls.Add(u.Convert());
			}

			return urls;
		}

		public static app.ShortURL Convert(this ShortURL model)
		{
			return (model == null) ? new app.ShortURL() : new app.ShortURL()
			{
				Key = model.Key,
				Url = model.Url
			};
		}
		public static ShortURL Convert(this app.ShortURL model)
		{
			return model.NewOrPopulate<ShortURL>(
				(to) =>
				{
					to.Url = model.Url;
					to.Key = model.Key;
				});
		}
		public static Owner Convert (this app.Owner model)
		{
			return new Owner()
			{
				Email = model.Email,
				FullName = model.FullName,
				UserName = model.Id
			};
		}

		private static To NewOrPopulate<To>(this object model, Action<To> Setter) where To: class, new()
		{
			To returnValue = new To();
			if (model != null)
			{
				Setter(returnValue);
			}

			return returnValue;
		}
	}
}
