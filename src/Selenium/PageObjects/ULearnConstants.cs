using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace Selenium.PageObjects
{

	public class ULearnReferences
	{
		public static string startPage { get { return "https://localhost:44300/"; } }
	}

	public class XPaths
	{
		public static string TOCXPath { get { return "/html/body/ul"; } }
	}

	public class Titles
	{
		public static string StartPageTitle { get { return "Главная | uLearn"; } }

		public static string SignInPageTitle { get { return "Вход | uLearn"; } }

		public static string BasicProgrammingTitle { get { return "Основы программирования | uLearn"; } }

		public static string LinqTitle { get { return "Основы Linq | uLearn"; } }

		
	}

	public class ElementsId
	{
		public static By UserNameField { get { return By.Id("UserName"); } }

		public static By UserPasswordField { get { return By.Id("Password"); } }

		public static By SignInButton { get { return By.Id("loginLink"); } }

		public static By NextNavArrow { get { return By.Id("next_slide_button"); } }

		public static By PrevNavArrow { get { return By.Id("prev_slide_button"); } }

		public static By NextSolutionsButton { get { return By.Id("next_solutions_button"); } }
	}

	public class Admin
	{
		public static string Password { get { return "fullcontrol"; } }

		public static string Login { get { return "admin"; } }
	}

	public enum Rate
	{
		[StringValue("unedrstand-btn")] Understand,
		[StringValue("not-unedrstand-btn")] NotUnderstand,
		[StringValue("trivial-btn")] Trivial
	}

	public class StringValueAttribute : Attribute
	{
		private readonly string value;

		public StringValueAttribute(string value)
		{
			this.value = value;
		}

		public string Value
		{
			get { return value; }
		}
	}

	public class StringValue
	{
		private static Dictionary<Enum, StringValueAttribute> _stringValues = new Dictionary<Enum, StringValueAttribute>();

		public static string GetStringValue(Enum value)
		{
			string output = null;
			var type = value.GetType();

			//Check first in our cached results...
			if (_stringValues.ContainsKey(value))
				output = (_stringValues[value] as StringValueAttribute).Value;
			else
			{
				//Look for our 'StringValueAttribute' 
				//in the field's custom attributes
				var fi = type.GetField(value.ToString());
				var attrs = fi.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];
				if (attrs.Length > 0)
				{
					_stringValues.Add(value, attrs[0]);
					output = attrs[0].Value;
				}
			}

			return output;
		}
	}
}
