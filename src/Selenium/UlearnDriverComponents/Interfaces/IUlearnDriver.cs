using Selenium.UlearnDriverComponents.Pages;

namespace Selenium.UlearnDriverComponents.Interfaces
{
	public interface IUlearnDriver
	{
		Rate GetRateFromDb();
		string GetCurrentSlideId();
		string GetCurrentUserName();
		IUlearnDriver ClickRegistration();
		string GetCurrentSlideName();
		UlearnPage GetPage();
		IToc GetToc();
		IUlearnDriver GoToStartPage();
		IUlearnDriver LoginAdminAndGoToCourse(string courseTitle);
		IUlearnDriver LoginAndGoToCourse(string courseTitle, string login, string password);
		IUlearnDriver LoginVkAndGoToCourse(string courseTitle);

		bool IsLogin { get; }
	}
}
