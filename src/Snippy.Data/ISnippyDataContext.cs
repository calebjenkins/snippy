using Microsoft.EntityFrameworkCore;
using Snippy.Data.Models;


namespace Snippy.Data
{
	public interface ISnippyDataContext
	{
		DbSet<Click> Clicks { get; set; }
		DbSet<Owner> Owners { get; set; }
		DbSet<OwnerUrls> OwnerUrls { get; set; }
		DbSet<ShortURL> URLs { get; set; }
	}
}
