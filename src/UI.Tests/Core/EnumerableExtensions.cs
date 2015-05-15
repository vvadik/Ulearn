using System;
using System.Collections.Generic;
using System.Linq;

namespace UI.Tests.Core
{
	public static class EnumerableExtensions
	{
		public static Array ToArray<T>(this IEnumerable<T> items, Type elementType)
		{
			var list = items.ToList();
			var array = Array.CreateInstance(elementType, list.Count);
			for (int i = 0; i < list.Count; i++)
				array.SetValue(list[i], i);
			return array;
		}
	}
}