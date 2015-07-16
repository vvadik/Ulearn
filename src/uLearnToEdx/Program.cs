using System.Collections.Generic;
using System.IO;
using System.Linq;
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
		private static void Main(string[] args)
		{
			var cm = new CourseManager(new DirectoryInfo(@"..\..\..\uLearn.Web"));
			cm.ReloadCourse("BasicProgramming.zip");

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
			File.WriteAllText(folderName + "/course.xml", c.XmlSerialize());
			File.WriteAllText(folderName + "/course/" + course.Id + ".xml", cc.XmlSerialize());
			File.WriteAllText(folderName + "/chapter/" + course.Id + "-1.xml", ch.XmlSerialize());
			int i = 0;
			foreach (var unit in course.GetUnits())
			{
				var unit1 = unit;
				var verticals = new List<VerticalUrl>();
				foreach (var slide in course.Slides.Where(x => x.Info.UnitName == unit1))
				{
					verticals.Add(new VerticalUrl { UrlName = slide.Guid} );
					if (slide.Blocks.Any(x => x is ExerciseBlock))
						verticals.Add(new VerticalUrl { UrlName = slide.Guid + "0" });
				}
				File.WriteAllText(folderName + "/sequential/" + course.Id + "-1-" + i + ".xml", new Sequential
				{
					DisplayName = unit,
					Verticals = verticals.ToArray()
				}.XmlSerialize());
				i++;

				foreach (var slide in course.Slides.Where(x => x.Info.UnitName == unit1))
				{
					int j = 0;
					var components = new List<Component>();
					if (slide.Blocks.Any(x => x is AbstractQuestionBlock) && !slide.Blocks.Any(x => x is YoutubeBlock || x is ExerciseBlock))
					{
						foreach (var block in slide.Blocks)
						{
							var component = block.ToEdxComponent(folderName, course.Id, "", slide, j);
							components.AddRange(component);
							j++;
						}
						var slideComponent = new SlideProblemComponent
						{
							DisplayName = slide.Title, 
							FolderName = folderName, 
							UrlName = slide.Guid,
							Components = components.Select(x => x.AsXmlElement()).ToArray()
						};

						foreach (var codeComponent in components.OfType<CodeComponent>())
						{
							codeComponent.SaveAdditional();
						}
						slideComponent.Save();
						File.WriteAllText(folderName + "/vertical/" + slide.Guid + ".xml",
							new Vertical
							{
								DisplayName = slide.Title,
								Components = new [] {slideComponent.GetReference() }
							}.XmlSerialize());
					}
					else
					{
						while (j < slide.Blocks.Length)
						{
							var blocks = slide.Blocks.Skip(j).TakeWhile(x => !(x is YoutubeBlock) && !(x is ExerciseBlock)).ToList();
							var innerComponents = new List<Component>();
							foreach (var block in blocks)
							{
								
								var component = block.ToEdxComponent(folderName, course.Id, "", slide, j);
								var comps = component as Component[] ?? component.ToArray();
								innerComponents.AddRange(comps);
								
								foreach (var comp in comps)
									comp.Save();
								j++;
							}
							if (blocks.Count != 0)
							{
								var slideComponent = new HtmlComponent
								{
									DisplayName = j <= blocks.Count ? slide.Title : "",
									FolderName = folderName,
									UrlName = slide.Guid + j,
									Filename = slide.Guid + j,
									Source = string.Join("", innerComponents.Select(x => x.AsHtmlString()))
								};
								components.Add(slideComponent);
								slideComponent.Save();
							}
							if (j >= slide.Blocks.Length)
								break;
							var otherComponent = slide.Blocks[j].ToEdxComponent(folderName, course.Id, j == 0 ? slide.Title : "", slide, j);
							var otherComps = otherComponent as Component[] ?? otherComponent.ToArray();
							components.AddRange(otherComps);
							foreach (var comp in otherComps)
								comp.Save();
							j++;
						}
						var solutionComponents = new List<Component>();
						foreach (var result in slide.Blocks.OfType<ExerciseBlock>())
						{
							var comp = result.GetSolutionsComponent(folderName, course.Id, "Решения", slide, j);
							comp.Save();
							solutionComponents.Add(comp);
							j++;
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
				}
			}
			ArchiveManager.CreateTar(folderName + ".tar.gz", folderName);
		}
	}
}
