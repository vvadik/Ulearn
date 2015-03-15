using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using Selenium.UlearnDriverComponents;
using Selenium.UlearnDriverComponents.Interfaces;

namespace Selenium.SeleniumTests
{
	[TestFixture]
	public class ExercisePageShould
	{
		[Explicit]
		[Test]
		public void RunSolution()
		{
			using (var driver = new ChromeDriver())
			{
				driver.Navigate().GoToUrl(ULearnReferences.StartPage);
				IUlearnDriver ulearnDriver = new UlearnDriverComponents.UlearnDriver(driver);
				ulearnDriver.LoginAdminAndGoToCourse(Titles.BasicProgrammingTitle);
				driver.Navigate().GoToUrl("https://localhost:44300/Course/BasicProgramming/Slide/21");
			}
			
		}
	}
}
