using Snippy.Models;
using System;

namespace Snippy.Web.Models
{
	public class IndexViewModel
	{
		public Owner AuthenticatedUser { get; set; }
		public string Title => "Snippy Web";
		public string Platform { get; set; }
	}
}
