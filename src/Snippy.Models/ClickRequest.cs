using System;

namespace Snippy.Models
{
	public class ClickRequest
	{
		public Guid RequestId { get; set; }
		public string ShortUrlKey { get; set; }
		public string SourceIp { get; set; }
		public string IdentId { get; set; }
	}
}
