using System.Collections.Generic;
using System.Linq;

namespace AntiPlagiarism.Web.Extensions
{
	// ReSharper disable once InconsistentNaming
	public static class IEnumerableExtensions
	{
		public static IEnumerable<ItemWithIndex<T>> Enumerate<T>(this IEnumerable<T> collection, int start=0)
		{
			var index = start;

			foreach (var element in collection)
				yield return new ItemWithIndex<T>(element, index++);
		}
		
		public static string Join(this IEnumerable<object> collection, string separator)
		{
			return string.Join(separator, collection.Select(o => o.ToString()));
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

		public void Deconstruct(out int index, out T item)
		{
			index = Index;
			item = Item;
		}
	}
}