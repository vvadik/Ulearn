using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using Selenium.UlearnDriver.PageObjects;

namespace Selenium.UlearnDriver.Pages
{
	public class UlearnContentPage : UlearnPage
	{

		private readonly Lazy<NavArrows> navArrows;

		public UlearnContentPage(IWebDriver driver)
			: base(driver)
		{
			navArrows = new Lazy<NavArrows>(() => new NavArrows(driver));
		}
		public UlearnDriver ClickNextButton()
		{
			return navArrows.Value.ClickNextButton();
		}

		public UlearnDriver ClickPrevButton()
		{
			return navArrows.Value.ClickPrevButton();
		}

		public bool IsActiveNextButton()
		{
			return navArrows.Value.IsActiveNextButton();
		}
	}
}
