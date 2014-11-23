using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace Selenium.PageObjects
{
	class ExerciseSlidePage : SlidePage
	{
		private const string SecretCode = "secretCodeExercise";
		private const string CodeExercise = "code-exercise";
		private const string CodeMirror = "CodeMirror";

		public ExerciseSlidePage(IWebDriver driver, string courseTitle)
			: base(driver, courseTitle)
		{
			CheckExerciseSlide(driver);
		}

		private static void CheckExerciseSlide(IWebDriver driver)
		{
			var secretCode = driver.FindElement(By.Id(SecretCode));
			if (secretCode == null)
			{
				throw new NotFoundException("не найдена секретная область кода");
			}
			var codeExercise = driver.FindElement(By.ClassName(CodeExercise));
			if (codeExercise == null)
			{
				throw new NotFoundException("не найдена область для кода");
			}
			var codeMirror = driver.FindElement(By.ClassName(CodeMirror));
			if (codeMirror == null)
			{
				throw new NotFoundException("не найден codemirror");
			}
		}
	}
}
