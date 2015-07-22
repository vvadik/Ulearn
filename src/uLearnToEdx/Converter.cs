using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using ApprovalTests;
using ApprovalTests.Reporters;
using ApprovalUtilities.Utilities;
using NUnit.Framework;
using ObjectPrinter;
using uLearn;
using uLearn.Model.Blocks;
using uLearn.Model.Edx;
using uLearn.Model.Edx.EdxComponents;
using uLearn.Quizes;

namespace uLearnToEdx
{
	public static class Converter
	{
		private static string exerciesUrl = "https://192.168.33.1:44300/Course/{0}/LtiSlide/";
		private static string solutionsUrl = "https://192.168.33.1:44300/Course/{0}/AcceptedAlert/";

		private static IEnumerable<Vertical> OrdinarySlideToVerticals(string courseId, Slide slide)
		{
			var componentIndex = 0;
			var components = new List<Component>();
			while (componentIndex < slide.Blocks.Length)
			{
				var blocks = slide.Blocks.Skip(componentIndex).TakeWhile(x => !(x is YoutubeBlock) && !(x is ExerciseBlock)).ToList();
				if (blocks.Count != 0)
				{
					var innerComponents = new List<Component>();
					foreach (var block in blocks)
					{
						var component = block.ToEdxComponents("", slide, componentIndex);
						innerComponents.Add(component);
						componentIndex++;
					}

					var slideComponent = new HtmlComponent
					{
						DisplayName = componentIndex <= blocks.Count ? slide.Title : "",
						UrlName = slide.Guid + componentIndex,
						Filename = slide.Guid + componentIndex,
						Source = string.Join("", innerComponents.Select(x => x.AsHtmlString())),
						Subcomponents = innerComponents.ToArray()
					};
					components.Add(slideComponent);
				}
				if (componentIndex >= slide.Blocks.Length)
					break;
				
				var exerciseBlock = slide.Blocks[componentIndex] as ExerciseBlock;
				var otherComponent = exerciseBlock != null
					? exerciseBlock.GetExerciseComponent(componentIndex == 0 ? slide.Title : "", slide, componentIndex, string.Format(exerciesUrl, courseId))
					: slide.Blocks[componentIndex].ToEdxComponents(componentIndex == 0 ? slide.Title : "", slide, componentIndex);

				components.Add(otherComponent);
				componentIndex++;
			}

			var solutionComponents = new List<Component>();
			foreach (var result in slide.Blocks.OfType<ExerciseBlock>())
			{
				var comp = result.GetSolutionsComponent("Решения", slide, componentIndex, string.Format(solutionsUrl, courseId));
				solutionComponents.Add(comp);
				componentIndex++;
			}

			yield return new Vertical(slide.Guid, slide.Title, components.ToArray());
			if (solutionComponents.Count != 0)
				yield return new Vertical(slide.Guid + "0", "Решения", solutionComponents.ToArray());
		}

		private static IEnumerable<Vertical> QuizToVerticals(QuizSlide slide)
		{
			var components = slide.Blocks
				.Select((b, i) => b.ToEdxComponents("", slide, i))
				.ToArray();

			var slideComponent = new SlideProblemComponent
			{
				DisplayName = slide.Title,
				UrlName = slide.Guid,
				Subcomponents = components,
				XmlSubcomponents = components.Select(x => x.AsXmlElement()).ToArray()
			};
			yield return new Vertical(slide.Guid, slide.Title, new Component[] { slideComponent });
		}

		private static IEnumerable<Vertical> SlideToVerticals(string courseId, Slide slide)
		{
			var quizSlide = slide as QuizSlide;
			if (quizSlide != null)
				return QuizToVerticals(quizSlide);
			return OrdinarySlideToVerticals(courseId, slide);
		}

		private static Sequential[] CourseToSequentials(Course course)
		{
			var units = course.GetUnits().ToList();
			return Enumerable
				.Range(0, units.Count)
				.Select(
					x => new Sequential(course.Id + "-1-" + x, units[x], 
						course.Slides
							.Where(y => y.Info.UnitName == units[x])
							.SelectMany(y => SlideToVerticals(course.Id, y))
							.ToArray()
					)
				).ToArray();
		}

		public static EdxCourse ToEdxCourse(Course course, string organization, string[] advancedModules, string[] ltiPassports, string ltiHostname)
		{
			return new EdxCourse(
				course.Id, organization, course.Title, advancedModules, ltiPassports, 
				new [] { new Chapter(course.Id + "-1", course.Title, CourseToSequentials(course)) }
			);
		}
	}
}
