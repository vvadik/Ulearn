using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using uLearn;
using uLearn.Model.Blocks;
using uLearn.Model.EdxComponents;
using uLearn.Quizes;
using uLearnToEdx.Edx;
using Component = uLearn.Model.EdxComponents.Component;
using Course = uLearnToEdx.Edx.Course;

namespace uLearnToEdx
{
	internal class Program
	{
		private static void SaveOrdinarySlide(uLearn.Course course, string folderName, Slide slide)
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
						var component = block.ToEdxComponent(folderName, course.Id, "", slide, componentIndex);
						var comps = component as Component[] ?? component.ToArray();
						innerComponents.AddRange(comps);
						foreach (var comp in comps)
							comp.SaveAdditional();
						componentIndex++;
					}

					var slideComponent = new HtmlComponent
					{
						DisplayName = componentIndex <= blocks.Count ? slide.Title : "",
						FolderName = folderName,
						UrlName = slide.Guid + componentIndex,
						Filename = slide.Guid + componentIndex,
						Source = string.Join("", innerComponents.Select(x => x.AsHtmlString()))
					};
					components.Add(slideComponent);
					slideComponent.Save();
				}
				if (componentIndex >= slide.Blocks.Length)
					break;
				var otherComponent = slide.Blocks[componentIndex].ToEdxComponent(folderName, course.Id, componentIndex == 0 ? slide.Title : "", slide, componentIndex);
				var otherComps = otherComponent as Component[] ?? otherComponent.ToArray();
				components.AddRange(otherComps);
				foreach (var comp in otherComps)
					comp.Save();
				componentIndex++;
			}
			var solutionComponents = new List<Component>();
			foreach (var result in slide.Blocks.OfType<ExerciseBlock>())
			{
				var comp = result.GetSolutionsComponent(folderName, course.Id, "Решения", slide, componentIndex);
				comp.Save();
				solutionComponents.Add(comp);
				componentIndex++;
			}
			if (solutionComponents.Count > 0)
			{
				File.WriteAllText(folderName + "/vertical/" + slide.Guid + "0.xml",
					new Vertical
					{
						DisplayName = "Решения",
						Components = solutionComponents.Select(x => x.GetReference()).ToArray()
					}.XmlSerialize());
			}
			File.WriteAllText(folderName + "/vertical/" + slide.Guid + ".xml",
				new Vertical
				{
					DisplayName = slide.Title,
					Components = components.Select(x => x.GetReference()).ToArray()
				}.XmlSerialize());
		}

		private static void SaveQuizSlide(uLearn.Course course, string folderName, Slide slide)
		{
			var componentIndex = 0;
			var components = new List<Component>();
			foreach (var block in slide.Blocks)
			{
				var component = block.ToEdxComponent(folderName, course.Id, "", slide, componentIndex);
				components.AddRange(component);
				componentIndex++;
			}
			var slideComponent = new SlideProblemComponent
			{
				DisplayName = slide.Title,
				FolderName = folderName,
				UrlName = slide.Guid,
				Components = components.Select(x => x.AsXmlElement()).ToArray()
			};

			foreach (var component in components)
			{
				component.SaveAdditional();
			}

			slideComponent.Save();
			File.WriteAllText(folderName + "/vertical/" + slide.Guid + ".xml",
				new Vertical
				{
					DisplayName = slide.Title,
					Components = new[] { slideComponent.GetReference() }
				}.XmlSerialize());
		}

		private static void SaveSlide(uLearn.Course course, string folderName, Slide slide)
		{
			if (slide.Blocks.Any(x => x is AbstractQuestionBlock) && !slide.Blocks.Any(x => x is ExerciseBlock))
			{
				SaveQuizSlide(course, folderName, slide);
			}
			else
			{
				SaveOrdinarySlide(course, folderName, slide);
			}
		}

		private static void SaveUnit(uLearn.Course course, string folderName, string unit, int unitIndex)
		{
			var verticals = new List<VerticalUrl>();
			foreach (var slide in course.Slides.Where(x => x.Info.UnitName == unit))
			{
				SaveSlide(course, folderName, slide);
				verticals.Add(new VerticalUrl { UrlName = slide.Guid });
				if (slide.Blocks.Any(x => x is ExerciseBlock))
					verticals.Add(new VerticalUrl { UrlName = slide.Guid + "0" });
			}
			File.WriteAllText(string.Format("{0}/sequential/{1}-1-{2}.xml", folderName, course.Id, unitIndex), new Sequential
			{
				DisplayName = unit,
				Verticals = verticals.ToArray()
			}.XmlSerialize());
		}

		private static void Main(string[] args)
		{
			var cm = new CourseManager(new DirectoryInfo(@"..\..\..\uLearn.Web"));
			cm.ReloadCourse("OOP.zip");

			var course = cm.GetCourses().First();
			var folderName = course.Id;
			var c = new Course
			{
				CourseName = course.Id,
				Organization = "Kontur",
				UrlName = course.Id
			};
			var cc = new CourseWithChapters
			{
				AdvancedModules = "[\"lti\"]",
				DisplayName = course.Title,
				LtiPassports = "[\"myname:rfe:qwerty\"]",
				UseLatexCompiler = true,
				Chapters = new[] { new ChapterUrl { UrlName = course.Id + "-1" } }
			};
			var ch = new Chapter
			{
				DisplayName = course.Title,
				Sequentials = Enumerable.Range(0, course.GetUnits().Count()).Select(x => new SequentialUrl { UrlName = course.Id + "-1-" + x }).ToArray()
			};
			if (Directory.Exists(folderName))
				Directory.Delete(folderName, true);
			Directory.CreateDirectory(folderName);
			Directory.CreateDirectory(folderName + "/course");
			Directory.CreateDirectory(folderName + "/chapter");
			Directory.CreateDirectory(folderName + "/sequential");
			Directory.CreateDirectory(folderName + "/vertical");
			Directory.CreateDirectory(folderName + "/video");
			Directory.CreateDirectory(folderName + "/html");
			Directory.CreateDirectory(folderName + "/lti");
			Directory.CreateDirectory(folderName + "/static");
			Directory.CreateDirectory(folderName + "/problem");
			foreach (var file in Directory.GetFiles("static"))
				File.Copy(file, string.Format("{0}/static/{1}", folderName, Path.GetFileName(file)));
			File.WriteAllText(string.Format("{0}/course.xml", folderName), c.XmlSerialize());
			File.WriteAllText(string.Format("{0}/course/{1}.xml", folderName, course.Id), cc.XmlSerialize());
			File.WriteAllText(string.Format("{0}/chapter/{1}-1.xml", folderName, course.Id), ch.XmlSerialize());
			var unitIndex = 0;
			foreach (var unit in course.GetUnits())
			{
				SaveUnit(course, folderName, unit, unitIndex);
				unitIndex++;
			}
			ArchiveManager.CreateTar(folderName + ".tar.gz", folderName);
		}
	}
}
