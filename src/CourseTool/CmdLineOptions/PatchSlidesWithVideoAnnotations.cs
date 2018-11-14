using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using CommandLine;
using uLearn.Courses;
using uLearn.Courses.Slides;

namespace uLearn.CourseTool.CmdLineOptions
{
	[Verb("patch-annotations", HelpText = "Patch slides video-annotations, replacing them with data from doogle-doc")]
	class PatchSlidesWithVideoAnnotations : AbstractOptions
	{
		private static readonly XNamespace ns = "https://ulearn.azurewebsites.net/lesson";


		static string[] GetAnnotations(string fileId)
		{
			using (var client = new HttpClient())
			{
				// Key is intentionally hardcoded. Permissions restricted.
				var url = $@"https://www.googleapis.com/drive/v3/files/{fileId}/export?mimeType=text/plain&key=AIzaSyD2OrsI15dHkISxexd-bMQCkxRCJV8mu_c";
				return client
					.GetStringAsync(url)
					.Result
					.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			}
		}

		static IEnumerable<(Guid slideId, XElement annotation)> ParseAnnotations(string[] lines)
		{
			var i = 0;
			while (!lines[i].StartsWith("#")) i++;
			while (i < lines.Length)
			{
				var title = lines[i];
				i++;
				var annotationLines = new List<string>();
				while (i < lines.Length && !lines[i].StartsWith("#"))
				{
					annotationLines.Add(lines[i]);
					i++;
				}

				if (annotationLines.Count == 0)
				{
					Console.WriteLine($"Title: {title}. Empty annotation?!");
					continue;
				}
				if (annotationLines.Count(line => line.Trim().StartsWith("*")) == 0)
				{
					Console.WriteLine($"Title: {title}. No timecodes?!");
					continue;
				}
				if (annotationLines.Count(line => !line.Trim().StartsWith("*")) == 0)
				{
					Console.WriteLine($"Title: {title}. No abstract?!");
					continue;
				}
				if (annotationLines.TakeWhile(line => !line.Trim().StartsWith("*")).Count() != 2)
				{
					Console.WriteLine($"Title: {title}. Multiline abstract?!");
					continue;
				}
				if (annotationLines.Count(line => line.Trim().StartsWith("*")) == annotationLines.Count - 1)
				{
					Console.WriteLine($"Title: {title}. Bad annotation :(");
					continue;
				}

				var slideIdString = annotationLines[0].Split('/').Last().Split('_').Last();
				if (!Guid.TryParse(slideIdString, out var slideId))
				{
					Console.WriteLine($"Title: {title}. Bad slide id line [{annotationLines[0]}]. Cant parse guid from [{slideIdString}]");
					continue;
				}

				var text = annotationLines[1];
				var fragments = annotationLines.Skip(2).Select(line =>
				{
					var xElement = ParseFragment(line);
					if (xElement == null)
						Console.WriteLine($"Title: {title}. Bad timecode line [{line}]");
					return xElement;
				}).ToList();
				if (fragments.Any(f => f == null)) continue;
				yield return (
					slideId,
					new XElement(
						ns + "annotation",
						new XElement(ns + "text", new XText(text)),
						new XElement(ns + "fragments", fragments.Cast<object>().ToArray())
					));
			}

		}

		private static XElement ParseFragment(string line)
		{
			var fragmentPattern = @"^\*\s+(\d?\d\:\d\d)\s+([\-\—\–]\s+)?(.+)";
			var m = Regex.Match(line, fragmentPattern);
			if (!m.Success) return null;
			return
				new XElement(ns + "fragment",
					new XElement(ns + "start", new XText(m.Groups[1].Value)),
					new XElement(ns + "text", new XText(m.Groups[3].Value)));
		}

		public override void DoExecute()
		{
			var course = new CourseLoader().LoadCourse(new DirectoryInfo(Path.Combine(Dir, Config.ULearnCourseId)));
			Console.WriteLine($"{course.Slides.Count} slides loaded from {Config.ULearnCourseId}");
			var googleDocFileId = course.Settings.VideoAnnotationsGoogleDoc ?? throw new Exception("no video-annotations-google-doc element in course.xml");
			var annotations = ParseAnnotations(GetAnnotations(googleDocFileId)).ToList();
			Console.WriteLine($"{annotations.Count} annotations loaded from google doc {googleDocFileId}");
			var changedFiles = 0;
			var unchangedFiles = 0;
			foreach (var annotation in annotations)
			{
				var slide = course.GetSlideById(annotation.slideId);
				var xSlide = XDocument.Load(slide.Info.SlideFile.FullName);
				PatchSlide(xSlide, slide, annotation.annotation);
				var oldText = File.ReadAllText(slide.Info.SlideFile.FullName);
				xSlide.Save(slide.Info.SlideFile.FullName);
				var newText = File.ReadAllText(slide.Info.SlideFile.FullName);
				if (oldText != newText)
				{
					Console.WriteLine("patched " + slide.Info.SlideFile.Name);
					changedFiles++;
				}
				else unchangedFiles++;
			}
			Console.WriteLine($"Changed {changedFiles} files of {unchangedFiles} processed");
		}

		private static void PatchSlide(XDocument xSlide, Slide slide, XElement annotation)
		{
			var slideRoot = xSlide.Root ?? throw new Exception("no root?!");
			foreach (var annotationElement in slideRoot.Elements(ns + "annotation").ToList())
				annotationElement.Remove();
			var videoElement = slideRoot.Element(ns + "youtube") ?? throw new Exception($"no youtube block on slide {slide.Info.SlideFile}");
			videoElement.AddAfterSelf(annotation);
		}
	}
}