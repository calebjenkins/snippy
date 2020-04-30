using System;

namespace Snippy.Data.Models
{
	public abstract class DataModelBase
	{
		public string CreatedBy { get; set; }
		public DateTime CreatedOn { get; set; }
		public string UpdatedBy { get; set; }
		public DateTime UpdatedOn { get; set; }
	}
}
