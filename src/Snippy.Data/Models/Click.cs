using System;

namespace Snippy.Data.Models
{
	public class Click : DataModelBase
	{
		public Guid Id { get; set; }
		public string SourceIp { get; set; }
		public string IdentId { get; set; }
		public string ShortUrlKey { get; set; }
		public ShortURL UrlClicked { get; set; }
	}
}
