using System;
using System.Collections.Generic;
using System.Linq;

namespace uLearn
{
	public static class EnumerableExtensions
	{
		public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			var hash = new HashSet<TKey>();
			return source.Where(p => hash.Add(keySelector(p)));
		}

		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> items, Random random = null)
		{
			random = random ?? new Random();
			var copy = items.ToList();
			for (int i = 0; i < copy.Count; i++)
			{
				var nextIndex = random.Next(i, copy.Count);
				yield return copy[nextIndex];
				copy[nextIndex] = copy[i];
			}
		}
	}
}
