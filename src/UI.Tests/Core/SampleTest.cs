using System;
using NUnit.Framework;

namespace UI.Tests.Core
{
	[TestFixture]
	public class SampleTest
	{
		[Test]
		[Explicit("Демонстрация нового API")]
		public void Test()
		{
			using (var b = Browser.CreateDefault())
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

	[PageUrl("/")]
	public class StartPage : PageObject
	{
		[FindBy(Css = "#loginLink")]
		public PageObject LoginButton;

		[FindBy(Css = ".course-tile")]
		public Lazy<CourseTile[]> Courses;
	}

	//TODO: надо чтобы с этим указанием тоже работало
	//[FindBy(Css = ".course-tile")]
	public class CourseTile : PageObject
	{
		[FindBy(Css = "h2")]
		public string Title;

		[FindBy(Css = "a")]
		public PageObject StartButton;
	}
}