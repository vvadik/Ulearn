using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace Selenium.UlearnDriverComponents.PageObjects
{
	public abstract class PageObject
	{
		protected IWebDriver Driver;

		protected PageObject(IWebDriver driver)
		{
			Driver = driver;
		}
	}
}
