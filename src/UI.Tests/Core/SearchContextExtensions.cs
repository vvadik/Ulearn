using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using OpenQA.Selenium;

namespace UI.Tests.Core
{
	public static class SearchContextExtensions
	{
		public static TPageObject Get<TPageObject>(this ISearchContext context, Browser browser)
		{
			return (TPageObject)context.Get(typeof(TPageObject), browser);
		}

		public static TPageObject[] All<TPageObject>(this ISearchContext context, Browser browser)
		{
			return (TPageObject[])context.All(typeof(TPageObject), browser);
		}

		public static object Get(this ISearchContext context, Type type, Browser browser)
		{
			var constructorInfo = type.GetConstructor(new Type[0]);
			if (constructorInfo == null)
				throw new Exception("PageObject " + type.Name + " should have parameterless constructor!");
			var pageObject = constructorInfo.Invoke(new object[0]);
			InjectProperties(pageObject, context, browser);
			return pageObject;
		}

		private static Array All(this ISearchContext context, Type pageObjectType, Browser browser)
		{
			By objectElementCriteria = pageObjectType.FindAttr<FindByAttribute>().SafeGet(a => a.GetCriteria());
			return (Array)CreateArray(pageObjectType, context.FindElements(objectElementCriteria), browser);
		}

		private static void InjectProperties(object pageObject, ISearchContext context, Browser browser)
		{
			var pageObjectType = pageObject.GetType();
			var fields = pageObjectType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			foreach (var fieldInfo in fields)
			{
				if (fieldInfo.FieldType == typeof(Browser))
					fieldInfo.SetValue(pageObject, browser);
				else if (fieldInfo.FieldType == typeof(IWebElement))
					fieldInfo.SetValue(pageObject, CreateElement(pageObjectType, context));
				else
				{
					var criteria = fieldInfo.FindAttr<FindByAttribute>().SafeGet(a => a.GetCriteria());
					if (criteria != null)
					{
						var elements = context.FindElements(criteria);
						var value = CreateValue(fieldInfo.FieldType, elements, browser);
						fieldInfo.SetValue(pageObject, value);
					}
				}
			}
		}

		private static object CreateValue(Type fieldType, ReadOnlyCollection<IWebElement> elements, Browser browser)
		{
			if (typeof(PageObject).IsAssignableFrom(fieldType))
				return elements.Single().Get(fieldType, browser); //TODO Friendly error!
			if (fieldType == typeof(string))
				return elements.Single().Text;
			if (fieldType.IsArray)
				return CreateArray(fieldType.GetElementType(), elements, browser);
			if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(Lazy<>))
				return LazyUtils.MakeLazy(fieldType, () => CreateValue(fieldType.GetGenericArguments().Single(), elements, browser));
			throw new NotSupportedException(fieldType.Name);
		}

		private static object CreateArray(Type elementType, ReadOnlyCollection<IWebElement> elements, Browser browser)
		{
			var res = Array.CreateInstance(elementType, elements.Count);
			for (int i = 0; i < elements.Count; i++)
			{
				var value = CreateValue(elementType, new ReadOnlyCollection<IWebElement>(new[] { elements[i] }), browser);
				res.SetValue(value, i);
			}
			return res;
		}

		private static IWebElement CreateElement(Type pageObjectType, ISearchContext context)
		{
			var criteria = pageObjectType.FindAttr<FindByAttribute>().SafeGet(a => a.GetCriteria());
			if (criteria != null) return context.FindElement(criteria);
			return context as IWebElement;
		}
	}
}