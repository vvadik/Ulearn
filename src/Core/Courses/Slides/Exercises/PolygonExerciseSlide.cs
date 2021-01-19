using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Ulearn.Core.Courses.Slides.Blocks;
using Ulearn.Core.Courses.Slides.Exercises.Blocks;

namespace Ulearn.Core.Courses.Slides.Exercises
{
	[XmlRoot("slide.polygon", IsNullable = false, Namespace = "https://ulearn.me/schema/v2")]
	public class PolygonExerciseSlide : ExerciseSlide
	{
		[XmlElement("polygonPath")]
		public string PolygonPath { get; set; }
		
		public override void Validate(SlideLoadingContext context)
		{
			if (string.IsNullOrEmpty(PolygonPath))
				throw new CourseLoadingException("В slide.polygon должен находиться атрибут polygonPath");
			base.Validate(context);
		}

		public override void BuildUp(SlideLoadingContext context)
		{
			var statementsPath = Path.Combine(context.Unit.Directory.FullName, PolygonPath, "statements");
			var statementsFilePath = Path.Combine(statementsPath, "russian", "problem-properties.json");
			var json = File.ReadAllText(statementsFilePath);
			var statements = JsonConvert.DeserializeObject<Statements>(json);
			
			Blocks = GetBlocksProblem(statementsPath, context.CourseId, Id)
				.Concat(Blocks.Where(block => !(block is MarkdownBlock)))
				.ToArray();
			Title = statements.Name;
			
			var polygonExercise = Blocks.Single(block => block is PolygonExerciseBlock) as PolygonExerciseBlock;
			polygonExercise!.ExerciseDirPath = Path.Combine(PolygonPath);
			
			var pathTests = Path.Combine(context.Unit.Directory.FullName, PolygonPath, "tests");
			var countTests = Directory.GetFiles(pathTests)
				.Count(filename => Regex.IsMatch(filename, @"\d+[^a]$"));
			polygonExercise.MsPerTest = statements.TimeLimit;
			polygonExercise.TimeLimit = statements.TimeLimit * countTests; // TimeLimit тут в мс

				
			base.BuildUp(context);
		}

		private IEnumerable<SlideBlock> GetBlocksProblem(string statementsPath, string courseId, Guid slideId)
		{
			var markdownBlock = Blocks.FirstOrDefault(block => block is MarkdownBlock);
			if (markdownBlock != null)
			{
				yield return markdownBlock;
			}
			else if (Directory.Exists(Path.Combine(statementsPath, ".html")))
			{
				var htmlDirectoryPath = Path.Combine(statementsPath, ".html", "russian");
				var htmlData = File.ReadAllText(Path.Combine(htmlDirectoryPath, "problem.html"));
				var cssData = File.ReadAllText(Path.Combine(htmlDirectoryPath, "problem-statement.css"));
				const string liMarker = "li:before { content: '' !important; }";  
				yield return new HtmlBlock($"<style>{cssData}\n{liMarker}</style>");
				yield return RenderFromHtml(htmlData, cssData);
			}

			yield return GetLinkOnPdf(courseId, slideId.ToString());
		}
		private static SlideBlock RenderFromHtml(string html, string css)
		{
			// опробовать навесить tex на всё решение, чтобы обрабатывались даже в LI
			var match = new Regex("(<DIV class=[\"']problem-statement['\"]>.*)</BODY>", RegexOptions.Singleline | RegexOptions.IgnoreCase)
				.Match(html);
			if (!match.Success)
				throw new Exception();
			var body = match.Groups[1].Value;
			var processedBody = body
				.Replace("$$$$$$", "$$")
				.Replace("$$$", "$");
			return new HtmlBlock($"<div class=\"tex\">{processedBody}</div>");
		}

		private static MarkdownBlock GetLinkOnPdf(string courseId, string slideId)
		{
			var link = $"/Exercise/GetPdf?courseId={courseId}&slideId={slideId}";
			return new MarkdownBlock($"[Скачать условия задачи в формате PDF]({link})");

		}
	}

	public class Statements
	{
		public string Name { get; set; }
		public int TimeLimit { get; set; }
	}
}