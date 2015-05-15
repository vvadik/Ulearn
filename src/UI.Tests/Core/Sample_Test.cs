using System;
using System.IO;
using NUnit.Framework;
using OpenQA.Selenium.Chrome;

namespace UI.Tests.Core
{
	[TestFixture]
	public class Sample_Test
	{
		[Test]
		[Explicit("Демонстрация нового API")]
		public void Test()
		{
			using (var b = UlearnBrowser.CreateDefault())
			{
				StartPage startPage = b.Open<StartPage>();
				foreach (var course in startPage.Courses.Value)
				{
					Console.WriteLine(course.Title);
				}
				var window = startPage.LoginButton.ClickAndOpen<PageObject>();
				var topMenu = window.Get<TopMenu>();
				Console.WriteLine(topMenu);
				Console.WriteLine(b.WindowTitle);
			}
		}
	}

	//Все, что ниже — должно переехать в PageObjects
	public static class UlearnBrowser
	{
		public static Browser CreateDefault()
		{
			return new Browser(new ChromeDriver(), new Screenshoter(new DirectoryInfo("../../screenshots")), "https://localhost:44300/");
		}
	}


	[FindBy(Css = ".top-navigation")]
	public class TopMenu : PageObject
	{
		[FindBy(LinkText = "Admin courses")]
		public Lazy<PageObject> AdminCoursesButton;

		[FindBy(LinkText = "Пользователи")]
		public Lazy<PageObject> UsersButton;

		[FindBy(LinkText = "Статистика")]
		public Lazy<PageObject> StatisticsButton;
	}

	[PageUrl("")]
	public class StartPage : PageObject
	{
		[FindBy(Css = "#loginLink")]
		public PageObject LoginButton;

		public Lazy<CourseTile[]> Courses;
	}

	//TODO: надо чтобы с этим указанием тоже работало
	[FindBy(Css = ".course-tile")]
	public class CourseTile : PageObject
	{
		[FindBy(Css = "h2")]
		public string Title;

		[FindBy(Css = "a")]
		public PageObject StartButton;
	}
}