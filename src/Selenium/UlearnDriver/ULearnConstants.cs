using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace Selenium.UlearnDriver
{

	public class Constants
	{
		public static int SomeoneSolutionsCount { get { return 10; } }
	}

	public class ULearnReferences
	{
		public static string StartPage { get { return "https://localhost:44300/"; } }
		//public static string StartPage { get { return "https://ulearn.azurewebsites.net/"; } }
	}

	public class XPaths
	{
		public static string UserNameXPath { get { return "/html/body/div/div/div[1]/ul/form/ul/li/a"; } }

		public static string TocXPath { get { return "/html/body/ul"; } }

		public static string SlideXPath { get { return "/ul/li"; } }

		public static string SlideLabelXPath { get { return "/ul/i"; } }

		public static string SlideBodyXPath { get { return "/html/body/div[2]/div/div/div"; } }

		//public static string QuizTextXPath(int index)
		//{
		//	return SlideBodyXPath + string.Format("/p[{0}]", index);
		//}

		public static string QuizBlocksXPath { get { return "/html/body/div[2]/div/div/div/div[@class = \"quiz-block-mark\" or @class = \"quiz-block-input\"]"; } }

		public static string QuizChoiseBlocksXPath { get { return "/html/body/div[2]/div/div/div/div[@class = \"quiz-block-mark\"]"; } }

		public static string QuizFillInBlocksXPath { get { return "/html/body/div[2]/div/div/div/div[@class = \"quiz-block-input\"]"; } }

		public static string QuizFillInBlockField(int blockIndex)
		{
			return QuizBlocksXPath + String.Format("[{0}]/label/input", blockIndex);
		}

		public static string QuizBlockDescription(int blockIndex)
		{
			return String.Format("/html/body/div[2]/div/div/div/h4[{0}]", blockIndex + 1);
		}


		public static string QuizItemInfoXPath(int blockIndex)
		{
			return QuizBlocksXPath + String.Format(
				"[{0}]/div[@class = \"quiz\"]/label", blockIndex);
		}

		public static string QuizItemXPath(int blockIndex)
		{
			return QuizBlocksXPath + String.Format(
				"[{0}]/div[@class = \"quiz\"]/label/input", blockIndex);
		}

		public static string QuizSubmitAgainButtonXPath { get { return "div[@id = \"SubmitQuiz\"]/form/button"; } }

		public static string QuizSubmitAgainStatusXPath { get { return "div[@id = \"SubmitQuiz\"]/form/small"; } }

		public static string SlideHeaderXPath { get { return "/html/body/div[2]/div/div/div/h1"; } }

		public static string UnitInfoXPath(int unitIndex)
		{
			return String.Format("/html/body/ul/li[{0}]/ul", unitIndex + 1);
		}

		public static string QuizQuestionXPath(int blockIndex)
		{
			return String.Format(
				"/html/body/div[2]/div/div/div/h4[{0}]", blockIndex + 1);
		}

		public static string QuizQuestionStatusXPath(int blockIndex)
		{
			return QuizQuestionXPath(blockIndex) + "/i";
		}

		public static string SomeoneSolutionLikeXPath(int solutionIndex)
		{
			return String.Format(
				"/html/body/div[2]/div/div/div[{0}]/button", solutionIndex + 2);
		}

		public static string SomeoneSolutionXPath(int solutionIndex)
		{
			return String.Format(
				"/html/body/div[2]/div/div/div[{0}]/textarea", solutionIndex + 2);
		}
	}

	public class Titles
	{
		public static string StartPageTitle { get { return "Главная | uLearn"; } }

		public static string SignInPageTitle { get { return "Вход | uLearn"; } }

		public static string BasicProgrammingTitle { get { return "Основы программирования | uLearn"; } }

		public static string LinqTitle { get { return "Основы Linq | uLearn"; } }

		
	}

	public class ElementsClasses
	{

		public static By CodeExercise { get { return By.ClassName("code-exercise"); } }

		public static By CodeMirror { get { return By.ClassName("CodeMirror"); } }

		public static By RunSolutionButton { get { return By.ClassName("run-solution-button"); } }

		public static By ResetButton { get { return By.ClassName("reset-btn"); } }

		public static By GetHintsButton { get { return By.ClassName("hints-btn"); } }

		public static By QuizSubmitButton { get { return By.ClassName("quiz-submit-btn"); } }
	}

	public class ElementsId
	{
		public static By UserNameField { get { return By.Id("UserName"); } }

		public static By UserPasswordField { get { return By.Id("Password"); } }

		public static By SignInButton { get { return By.Id("loginLink"); } }

		public static By NextNavArrow { get { return By.Id("next_slide_button"); } }

		public static By PrevNavArrow { get { return By.Id("prev_slide_button"); } }

		public static By NextSolutionsButton { get { return By.Id("next_solutions_button"); } }

		public static By SecreteCode { get { return By.Id("secretCodeExercise"); } }
	}

	public class Admin
	{
		public static string Password { get { return "fullcontrol"; } }

		public static string Login { get { return "admin"; } }
	}

	public enum Rate
	{
		[StringValue("understand-btn")] Understand,
		[StringValue("not-understand-btn")] NotUnderstand,
		[StringValue("trivial-btn")] Trivial
	}

	public enum PageType
	{
		SolutionsPage,
		ExerciseSlidePage,
		SlidePage,
		SignInPage,
		StartPage,
		Quiz,
		IncomprehensibleType
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
