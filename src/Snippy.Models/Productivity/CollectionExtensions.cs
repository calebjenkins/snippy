using System;
using System.Collections.Generic;
using System.Linq;

namespace Snippy.Models.Productivity
{
	public static class CollectionExtensions
	{
		public static ICollection<SubT> Items<T, SubT>(this ICollection<T> DbSet, Func<T, SubT> Sorter)
		{
			IList<SubT> items = new List<SubT>();
			if (DbSet == null)
				return items;

			foreach (var itm in DbSet)
			{
				items.Add(Sorter(itm));
			}
			return items;
		}
	}
	public static class StringExtensions
	{
		public static bool IsNullOrEmpty(this string value)
		{
			return String.IsNullOrEmpty(value);
		}
	}
}
