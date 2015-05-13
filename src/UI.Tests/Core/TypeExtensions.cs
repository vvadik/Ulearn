using System;
using System.Linq;
using System.Reflection;

namespace UI.Tests.Core
{
	public static class TypeExtensions
	{
		public static TAttr FindAttr<TAttr>(this ICustomAttributeProvider t)
		{
			return (TAttr)t.GetCustomAttributes(typeof(TAttr), true).FirstOrDefault();
		}

		public static TValue SafeGet<TSource, TValue>(this TSource t, Func<TSource, TValue> getValue, TValue defaultValue = default(TValue))
		{
			return t == null ? defaultValue : getValue(t);
		}
	}
}