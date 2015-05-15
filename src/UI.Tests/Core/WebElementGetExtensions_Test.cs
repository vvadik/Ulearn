using System;
using System.Collections.ObjectModel;
using FakeItEasy;
using NUnit.Framework;
using OpenQA.Selenium;

namespace UI.Tests.Core
{
	[TestFixture]
	public class WebElementGetExtensions_Test
	{
		private IWebElement rootElement;
		private IWebElement pageObjectElement;
		private Browser browser = new Browser(null, null, "");
		private IWebElement controlElement;

		[FindBy(Css = ".control")]
		public class Simple : PageObject
		{
		}

		[FindBy(Css = ".pageObject")]
		public class SimpleHolder : PageObject
		{
			public Simple Simple;
		}

		[FindBy(Css = ".pageObject")]
		public class Sample : PageObject
		{
			[FindBy(Css = ".control"), UsedImplicitly]
			public PageObject Control;
		}

		[FindBy(Css = ".pageObject"), UsedImplicitly]
		public class ArrayHolder
		{
			[FindBy(Css = ".control"), UsedImplicitly]
			public PageObject[] Controls;
		}

		[FindBy(Css = ".pageObject"), UsedImplicitly]
		public class LazyHolder
		{
			[FindBy(Css = ".control"), UsedImplicitly]
			public Lazy<PageObject> Control;
		}

		[FindBy(Css = ".pageObject"), UsedImplicitly]
		public class LazyArrayHolder
		{
			[FindBy(Css = ".control"), UsedImplicitly]
			public Lazy<PageObject[]> Controls;
		}

		[SetUp]
		public void SetUp()
		{
			rootElement = A.Fake<IWebElement>();
			pageObjectElement = A.Fake<IWebElement>();
			controlElement = A.Fake<IWebElement>();

			A.CallTo(() => rootElement.FindElement(null)).WithAnyArguments().Returns(pageObjectElement);
			A.CallTo(() => rootElement.FindElements(null)).WithAnyArguments().Returns(AsReadOnlyCollection(pageObjectElement));

			A.CallTo(() => pageObjectElement.FindElement(null)).WithAnyArguments().Returns(controlElement);
			A.CallTo(() => pageObjectElement.FindElements(null)).WithAnyArguments().Returns(AsReadOnlyCollection(controlElement));

			A.CallTo(() => rootElement.ToString()).Returns("root");
			A.CallTo(() => pageObjectElement.ToString()).Returns("pageObject");
			A.CallTo(() => controlElement.ToString()).Returns("control");
		}

		private ReadOnlyCollection<T> AsReadOnlyCollection<T>(params T[] items)
		{
			return new ReadOnlyCollection<T>(items);
		}

		[Test]
		public void InjectWebElement()
		{
			var fieldInfo = typeof(Sample).GetField("Element");
			var info = new ValueConstructionInfo(fieldInfo.FieldType, new[] { pageObjectElement }, browser);
			var value = WebElementGetExtensions.CreateFieldValue(fieldInfo, info);
			Assert.AreEqual(pageObjectElement, value);
		}

		[Test]
		public void InjectBrowser()
		{
			var fieldInfo = typeof(Sample).GetField("Browser");
			var info = new ValueConstructionInfo(fieldInfo.FieldType, new[] { pageObjectElement }, browser);
			var value = WebElementGetExtensions.CreateFieldValue(fieldInfo, info);
			Assert.AreEqual(browser, value);
		}

		[Test]
		public void InjectPageObject()
		{
			var fieldInfo = typeof(Sample).GetField("Control");
			var info = new ValueConstructionInfo(fieldInfo.FieldType, new[] { pageObjectElement }, browser);
			var value = (PageObject)WebElementGetExtensions.CreateFieldValue(fieldInfo, info);
			Assert.AreEqual(browser, value.Browser);
			Assert.AreEqual(controlElement, value.Element);
		}

		[Test]
		public void GetFails_IfNoElement()
		{
			var ex = Assert.Throws<WebDriverException>(() => controlElement.Get<Sample>(browser));
			Console.WriteLine(ex);
			StringAssert.Contains("Sample", ex.Message);
		}


		[Test]
		public void Get()
		{
			var sample = rootElement.Get<Sample>(browser);
			Assert.AreEqual(browser, sample.Browser);
			Assert.AreEqual(pageObjectElement, sample.Element);
			Assert.AreEqual(controlElement, sample.Control.Element);
			Assert.AreEqual(browser, sample.Control.Browser);
		}

		[Test]
		public void Get_WithoutFieldAttribute()
		{
			var simpleHolder = rootElement.Get<SimpleHolder>(browser);
			Assert.AreEqual(controlElement, simpleHolder.Simple.Element);
		}

		[Test]
		public void GetArrayHolder()
		{
			A.CallTo(() => pageObjectElement.FindElements(null)).WithAnyArguments().Returns(new ReadOnlyCollection<IWebElement>(new[] { controlElement, controlElement }));
			var arrayHolder = rootElement.Get<ArrayHolder>(browser);
			Assert.AreEqual(2, arrayHolder.Controls.Length);
			Assert.AreEqual(controlElement, arrayHolder.Controls[0].Element);
		}

		[Test]
		public void GetLazyArrayHolder()
		{
			A.CallTo(() => pageObjectElement.FindElements(null)).WithAnyArguments().Returns(new ReadOnlyCollection<IWebElement>(new[] { controlElement, controlElement }));
			var arrayHolder = rootElement.Get<LazyArrayHolder>(browser);
			Assert.AreEqual(2, arrayHolder.Controls.Value.Length);
			Assert.AreEqual(controlElement, arrayHolder.Controls.Value[0].Element);
		}

		[Test]
		public void InjectedLazyField_FailsOnGetValue_IfNoElementsFound()
		{
			A.CallTo(() => pageObjectElement.FindElements(null)).WithAnyArguments().Returns(AsReadOnlyCollection<IWebElement>());
			var lazyHolder = rootElement.Get<LazyHolder>(browser);
			Assert.IsFalse(lazyHolder.Control.IsValueCreated);
			var ex = Assert.Throws<WebDriverException>(() => Console.Write(lazyHolder.Control.Value));
			Console.WriteLine(ex);
			StringAssert.Contains("PageObject not found", ex.Message);
		}

		[Test]
		public void InjectedLazyField_GetValue()
		{
			A.CallTo(() => pageObjectElement.FindElements(null)).WithAnyArguments().Returns(AsReadOnlyCollection(controlElement));
			var lazyHolder = rootElement.Get<LazyHolder>(browser);
			Assert.IsFalse(lazyHolder.Control.IsValueCreated);
			Assert.AreEqual(controlElement, lazyHolder.Control.Value.Element);
		}
	}
}