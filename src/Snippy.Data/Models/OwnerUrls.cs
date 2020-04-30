using System;
using System.Collections.Generic;

namespace Snippy.Data.Models
{
	public class OwnerUrls : DataModelBase
	{
		public string OwnerUserName { get; set; }
		public Owner Owner { get; set; }
		public string ShortUrlKey { get; set; }
		public ShortURL URL { get; set; }
	}
}
