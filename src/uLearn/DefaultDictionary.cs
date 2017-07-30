using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace uLearn
{
	public class DefaultDictionary<TKey, TValue> : Dictionary<TKey, TValue>
	{
		private Func<TValue> DefaultSelector { get; }

		public DefaultDictionary()
		{
		}

		public DefaultDictionary([NotNull] IDictionary<TKey, TValue> dict)
			: base(dict)
		{
		}

		public DefaultDictionary([NotNull] IDictionary<TKey, TValue> dict, Func<TValue> defaultSelector)
			: base(dict)
		{
			DefaultSelector = defaultSelector;
		}

		public DefaultDictionary(Func<TValue> defaultSelector)
		{
			DefaultSelector = defaultSelector;
		}

		public new TValue this[TKey key]
		{
			get
			{
				TValue value;
				if (TryGetValue(key, out value))
					return value;

				value = DefaultSelector != null ? DefaultSelector() : GetEmptyValue();
				Add(key, value);
				return value;
			}
			set { base[key] = value; }
		}

		private static TValue GetEmptyValue()
		{
			if (typeof(TValue) == typeof(string))
				return default(TValue);
			return typeof(TValue).IsValueType ? default(TValue) : Activator.CreateInstance<TValue>();
		}
	}

	public static class DictionaryExtension
	{
		public static DefaultDictionary<TKey, TValue> ToDefaultDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dict)
		{
			return new DefaultDictionary<TKey, TValue>(dict);
		}

		public static DefaultDictionary<TKey, TValue> ToDefaultDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dict, Func<TValue> defaultSelector)
		{
			return new DefaultDictionary<TKey, TValue>(dict, defaultSelector);
		}

		public static SortedDictionary<TKey, TValue> ToSortedDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dict)
		{
			return new SortedDictionary<TKey, TValue>(dict);
		}
	}
}