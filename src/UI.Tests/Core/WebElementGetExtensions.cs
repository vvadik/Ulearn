using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OpenQA.Selenium;

namespace UI.Tests.Core
{
	public static class WebElementGetExtensions
	{
		private static readonly List<TypeFactory> factories = new List<TypeFactory>();

		static WebElementGetExtensions()
		{
			factories.Add(new TypeFactory(t => t == typeof(Browser), info => info.Browser));
			factories.Add(new TypeFactory(t => t == typeof(IWebElement), info => info.Elements.Single()));
			factories.Add(new TypeFactory(t => t == typeof(string), info => info.Elements.Single().Text));
			factories.Add(new TypeFactory(typeof(PageObject).IsAssignableFrom, GetPageObjectFieldValue));
			factories.Add(new TypeFactory(IsLazy, GetLazyFieldValue));
			factories.Add(new TypeFactory(IsPageObjectArray, GetPageObjectArrayFieldValue));
		}

		public static TPageObject Get<TPageObject>(this IWebElement context, Browser browser)
		{
			return (TPageObject)context.Get(typeof(TPageObject), browser);
		}

		public static TPageObject[] All<TPageObject>(this IWebElement context, Browser browser)
		{
			return context.All(typeof(TPageObject), browser).Cast<TPageObject>().ToArray();
		}

		public static object Get(this IWebElement context, Type type, Browser browser)
		{
			return Get(new []{context}, type, browser);
		}

		public static object Get(IWebElement[] context, Type type, Browser browser)
		{
			var items = All(context, type, browser).ToList();
			if (items.Count == 0)
				throw new WebDriverException(type.Name + " not found");
			return items.Single();
		}

		private static object ParseAs(Type type, Browser browser, IWebElement pageObjectContext)
		{
			var constructorInfo = type.GetConstructor(new Type[0]);
			if (constructorInfo == null)
				throw new Exception(type.Name + " should have parameterless constructor!");
			var pageObject = constructorInfo.Invoke(new object[0]);
			try
			{
				InjectProperties(pageObject, pageObjectContext, browser);
				return pageObject;
			}
			catch (WebDriverException e)
			{
				throw new WebDriverException("Can't create " + type.Name + ". " + e.Message, e);
			}
		}

		private static IEnumerable<object> All(this IWebElement context, Type type, Browser browser)
		{
			return context.FindElementsByAttribute(type).Select(e => ParseAs(type, browser, e));
		}
		
		private static IEnumerable<object> All(IWebElement[] context, Type type, Browser browser)
		{
			return context.SelectMany(c => c.All(type, browser));
		}

		public static object CreateFieldValue(FieldInfo fieldInfo, ValueConstructionInfo info)
		{
			var criteria = fieldInfo.FindAttr<FindByAttribute>().SafeGet(a => a.GetCriteria());
			var refinedInfo = info.RefineWith(criteria);
			return CreateValue(refinedInfo);
		}

		public static object CreateValue(ValueConstructionInfo info)
		{
			var factory = factories.FirstOrDefault(f => f.CanCreate(info.ValueType));
			if (factory == null)
				throw new WebDriverException(string.Format("Unsupported type {0}", info.ValueType));
			try
			{
				return factory.Create(info);
			}
			catch (WebDriverException e)
			{
				throw new WebDriverException("Can't create " + info.ValueType.Name + ". " + e.Message, e);
			}
		}

		private static void InjectProperties(object pageObject, IWebElement context, Browser browser)
		{
			var pageObjectType = pageObject.GetType();
			var fields = pageObjectType.GetFields(BindingFlags.Instance | BindingFlags.Public);
			foreach (var fieldInfo in fields)
			{
				if (fieldInfo.FindAttr<DontInjectAttribute>() != null)
					continue;
				var value = CreateFieldValue(fieldInfo, new ValueConstructionInfo(fieldInfo.FieldType, new[] { context }, browser));
				fieldInfo.SetValue(pageObject, value);
			}
		}

		private static object GetPageObjectFieldValue(ValueConstructionInfo info)
		{
			return Get(info.Elements, info.ValueType, info.Browser);
		}

		private static bool IsLazy(Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Lazy<>);
		}

		private static object GetLazyFieldValue(ValueConstructionInfo info)
		{
			return LazyUtils.MakeLazy(
				info.ValueType,
				() => CreateValue(
					new ValueConstructionInfo(
						info.ValueType.GetGenericArguments().Single(),
						info.Elements,
						info.Browser)));
		}

		private static bool IsPageObjectArray(Type type)
		{
			return type.IsArray && typeof(PageObject).IsAssignableFrom(type.GetElementType());
		}

		private static object GetPageObjectArrayFieldValue(ValueConstructionInfo info)
		{
			var elementType = info.ValueType.GetElementType();
			return CreateArray(new ValueConstructionInfo(elementType, info.Elements, info.Browser));
		}

		private static IEnumerable<IWebElement> FindElementsByAttribute(this IWebElement element, ICustomAttributeProvider attributeProvider)
		{
			var criteria = attributeProvider.FindAttr<FindByAttribute>().SafeGet(a => a.GetCriteria());
			if (criteria == null)
				return new[] { element };
			return element.FindElements(criteria);
		}

		private static object CreateArray(ValueConstructionInfo info)
		{
			return All(info.Elements, info.ValueType, info.Browser).ToArray(info.ValueType);
		}
	}
}