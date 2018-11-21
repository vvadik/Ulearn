using System;
using System.IO;
using System.Linq;
using System.Xml;
using Ulearn.Common.Extensions;
using Ulearn.Core.Courses.Slides.Exercises;
using Ulearn.Core.Courses.Slides.Quizzes;
using Ulearn.Core.Courses.Units;

namespace Ulearn.Core.Courses.Slides
{
	public class XmlSlideLoader : ISlideLoader
	{
		/// <summary>
		/// Loads slide from XML file
		/// </summary>
		/// <returns>Slide, QuizSlide or ExerciseSlide object</returns>
		public Slide Load(FileInfo file, int slideIndex, Unit unit, string courseId, CourseSettings settings)
		{
			var fileContent = file.ReadAllContent();

			return Load(fileContent, slideIndex, unit, courseId, settings, file);
		}

		public Slide Load(byte[] fileContent, int slideIndex, Unit unit, string courseId, CourseSettings settings, FileInfo slideFile=null)
		{
			if (slideFile == null)
				slideFile = new FileInfo("<internal>");
			
			var slideType = DetectSlideType(fileContent, "<internal>");

			var slide = (Slide)fileContent.DeserializeXml(slideType);

			var context = new CourseLoadingContext(courseId, unit, settings, slideFile, slideIndex);
			slide.BuildUp(context);

			slide.Validate(context);

			return slide;
		}

		/// <summary>
		/// Automatically detects slide type by XML file.
		/// Slide file starts with &lt;slide type="quiz|exercise|lesson" ...%gt;, so we just extract attribute "type".
		/// </summary>
		/// <param name="content">XML file with slide</param>
		/// <param name="filename">filename, just for error message formatting</param>
		/// <returns>typeof(Slide), typeof(QuizSlide) or typeof(ExerciseSlide)</returns>
		private static Type DetectSlideType(byte[] content, string filename)
		{
			var xmlDocument = new XmlDocument();
			try
			{
				using (var stream = new MemoryStream(content))
					xmlDocument.Load(stream);
			}
			catch (Exception e)
			{
				throw new CourseLoadingException($"Не могу прочитать слайд из файла {filename}. Возможно, там некорректный XML?");
			}

			if (xmlDocument.DocumentElement == null)
				throw new CourseLoadingException($"Не могу определить, что за слайд лежит в {filename}. Возможно, там некорректный XML или он не начинается с тега <slide>...</slide>?");
			
			var typeAttribute = xmlDocument.DocumentElement.Attributes["type"];
			if (typeAttribute == null)
				return typeof(Slide);

			if (typeAttribute.Value.EqualsIgnoreCase(SlideType.Quiz.GetXmlEnumName()))
				return typeof(QuizSlide);
			
			if (typeAttribute.Value.EqualsIgnoreCase(SlideType.Exercise.GetXmlEnumName()))
				return typeof(ExerciseSlide);

			var allowedTypes = string.Join(", ", Enum.GetValues(typeof(SlideType)).Cast<SlideType>().Select(t => $"\"{t.GetXmlEnumName()}\""));
			throw new CourseLoadingException(
				$"Не могу определить, что за слайд лежит в {filename}. " +
				$"Атрибут type (в <slide type=\"...\">) может быть одним из следующих: {allowedTypes}."
			);
		}
	}
}