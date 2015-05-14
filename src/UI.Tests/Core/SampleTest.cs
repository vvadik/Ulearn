using System;
using NUnit.Framework;
using OpenQA.Selenium.Chrome;

namespace UI.Tests.Core
{
	[TestFixture]
	public class SampleTest
	{
		[Test]
		[Explicit("Демонстрация нового API")]
		public void Test()
		{
			using (var b = new Browser(new ChromeDriver()))
			{
				StartPage startPage = b.Open<StartPage>();
				var window = startPage.LoginButton.ClickAndOpen<PageObject>();
				Console.WriteLine(window.Get<TopMenu>());
				Console.WriteLine(b.WindowTitle);
			}
		}
	}

	[FindByCss(".top-navigation")]
	public class TopMenu : PageObject
	{
		[FindByText("Admin courses")]
		public Lazy<PageObject> AdminCoursesButton;

		[FindByText("Пользователи")]
		public Lazy<PageObject> UsersButton;

		[FindByText("Статистика")]
		public Lazy<PageObject> StatisticsButton;
	}

	[PageUrl("/")]
	public class StartPage : PageObject
	{
		[FindByCss("#loginLink")]
		public PageObject LoginButton;

		public Lazy<CourseTile[]> Courses;
	}

	[FindByCss(".course-tile")]
	public class CourseTile : PageObject
	{
		[FindByCss("h2")]
		public string Title;

		[FindByCss("a")]
		public PageObject StartButton;
	}
}