using Selenium.UlearnDriverComponents.Pages;

namespace Selenium.UlearnDriverComponents.Interfaces
{
	public interface IUlearnDriver
	{
		Rate GetRateFromDb();
		string GetCurrentSlideId();
		string GetCurrentUserName();
		void GoToRegistrationPage();
		string GetCurrentSlideName();
		UlearnPage GetPage();
		IToc GetToc();
		void GoToStartPage();
		void LoginAdminAndGoToCourse(string courseTitle);
		void LoginAndGoToCourse(string courseTitle, string login, string password);
		void LoginVkAndGoToCourse(string courseTitle);

		bool IsLogin { get; }
	}
}
