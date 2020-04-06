using System;
using System.Collections.Generic;

namespace Snippy.Models
{
	public class Owner
	{
		public Owner()
		{
			URLs = new List<ShortURL>();
		}
		public string Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public IList<ShortURL> URLs { get; set; }
	}
}
