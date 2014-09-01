using System.Collections.Generic;

namespace uLearn
{
	public static class CollectionExtensions
	{
		public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
		{
			TValue v;
			if (dictionary.TryGetValue(key, out v)) return v;
			throw new KeyNotFoundException("Key " + key + " not found in dictionary");
		}

		public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
		{
			TValue v;
			if (dictionary.TryGetValue(key, out v)) return v;
			return defaultValue;
		}
	}
}