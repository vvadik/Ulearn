using System;
using System.Collections.Generic;

namespace AntiPlagiarism.Web.Extensions
{
	public static class IEnumerableExtensions
	{
		public static IEnumerable<ItemWithIndex<T>> Enumerate<T>(this IEnumerable<T> collection, int start=0)
		{
			var index = start;

			foreach (var element in collection)
				yield return new ItemWithIndex<T>(element, index++);
		}
	}

	public class ItemWithIndex<T>
	{
		public T Item { get; private set; }
		public int Index { get; private set; }

		public ItemWithIndex(T item, int index)
		{
			Item = item;
			Index = index;
		}
	}
}