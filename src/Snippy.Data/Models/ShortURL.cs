using System;
using System.Collections.Generic;
using System.Text;

namespace Snippy.Data.Models
{
	public class ShortURL : DataModelBase
	{
		public string Key { get; set; }
		public string Url { get; set; }
		public IList<OwnerUrls> OwnerURLs { get; set; }
		public IList<Click> Clicks { get; set; }
	}
}
