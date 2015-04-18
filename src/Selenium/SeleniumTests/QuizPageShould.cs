using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using Selenium.UlearnDriverComponents;
using Selenium.UlearnDriverComponents.Interfaces;
using Selenium.UlearnDriverComponents.Pages;

namespace Selenium.SeleniumTests
{
	[TestFixture]
	public class QuizPageShould
	{
		[Test]
		public void CheckAnswers()
		{
			using (var driver = new ChromeDriver())
			{
				driver.Navigate().GoToUrl(ULearnReferences.StartPage);
				UlearnDriver ulearnDriver = new UlearnDriver(driver);
				ulearnDriver.GoToRegistrationPage();
				var registrationPage = ulearnDriver.Get<RegistrationPage>();
				ulearnDriver.LoginAdminAndGoToCourse(Titles.BasicProgrammingTitle);
				//var first
				var page = ulearnDriver.Get<SlidePage>();
			}
		}
	}
}
