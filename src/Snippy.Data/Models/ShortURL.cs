using System;
using System.Collections.Generic;

namespace Snippy.Data.Models
{
	public class ShortURL : DataModelBase
	{
		public string Key { get; set; }
		public string Url { get; set; }
		public ICollection<OwnerUrls> OwnerURLs { get; } = new List<OwnerUrls>();
		//public ICollection<Click> Clicks { get; } = new List<Click>();
	}
}
