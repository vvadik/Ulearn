using System.Linq;
using OpenQA.Selenium;

namespace UI.Tests.PageObjects.PageObjects
{
	public class TeX
	{
		private readonly IWebElement element;

		public TeX(IWebElement element)
		{
			this.element = element;
			IsRendered = element.FindElements(By.TagName("span")).Any();
		}

		public bool IsRendered { get; private set; }

		public string GetContent()
		{
			return element.Text;
		}
	}
}
