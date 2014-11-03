using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace Selenium.PageObjects
{
	public class ULearnReferences
	{
		public static string startPage = "https://localhost:44300/";
	}

	public class Titles
	{
		public static string startPageTitle = "Главная | uLearn";

		public static string signInPageTitle = "Вход | uLearn";

		public static string basicProgrammingTitle = "Основы программирования | uLearn";

		public static string linqTitle = "Основы Linq | uLearn";
	}

	public class ElementsId
	{
		public static By userNameField = By.Id("UserName");

		public static By userPasswordField = By.Id("Password");

		public static By signInButton = By.Id("loginLink");
	}

	public class Admin
	{
		public static string password = "fullcontrol";

		public static string login = "admin";
	}
}
