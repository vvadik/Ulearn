using System;
using System.Collections.Generic;
using System.Linq;
using uLearn.Model.Blocks;
using uLearn.Model.Edx;
using uLearn.Model.Edx.EdxComponents;
using uLearn.Quizes;

namespace uLearn
{
	public class Slide
	{
		public readonly string Title;
		public readonly SlideBlock[] Blocks;
		public readonly SlideInfo Info;
		public int Index { get { return Info.Index; }}
		public readonly string Id;
		public string Guid { get { return Utils.GetNormalizedGuid(Id); } }
		public virtual bool ShouldBeSolved { get { return false; } }
		public int MaxScore { get; protected set; }


		public Slide(IEnumerable<SlideBlock> blocks, SlideInfo info, string title, string id)
		{
			try
			{
				Info = info;
				Title = title;
				Id = id;
				MaxScore = 0;
				Blocks = blocks.ToArray();
				foreach (var block in Blocks)
					block.Validate();
			}
			catch (Exception e)
			{
				throw new FormatException(string.Format("Error in slide {0} (id: {1}). {2}", title, id, e.Message), e);
			}
		}

		public override string ToString()
		{
			return string.Format("Title: {0}, Id: {1}, MaxScore: {2}", Title, Id, MaxScore);
		}

		private static IEnumerable<Vertical> OrdinarySlideToVerticals(string courseId, Slide slide, string exerciseUrl, string solutionsUrl, Dictionary<string, string> videoGuids, string ltiId)
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
					? exerciseBlock.GetExerciseComponent(componentIndex == 0 ? slide.Title : "", slide, componentIndex, string.Format(exerciseUrl, courseId, slide.Index), ltiId)
					: ((YoutubeBlock)slide.Blocks[componentIndex]).GetVideoComponent(componentIndex == 0 ? slide.Title : "", slide, componentIndex, videoGuids);

				components.Add(otherComponent);
				componentIndex++;
			}

			var solutionComponents = new List<Component>();
			foreach (var result in slide.Blocks.OfType<ExerciseBlock>())
			{
				var comp = result.GetSolutionsComponent("Решения", slide, componentIndex, string.Format(solutionsUrl, courseId, slide.Index), ltiId);
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

		public IEnumerable<Vertical> ToVerticals(string courseId, string exerciseUrl, string solutionsUrl, Dictionary<string, string> videoGuids, string ltiId)
		{
			var quizSlide = this as QuizSlide;
			if (quizSlide != null)
				return QuizToVerticals(quizSlide);
			return OrdinarySlideToVerticals(courseId, this, exerciseUrl, solutionsUrl, videoGuids, ltiId);
		}
	}
}