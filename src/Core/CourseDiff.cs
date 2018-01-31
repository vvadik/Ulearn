using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using uLearn.Extensions;
using uLearn.Model.Blocks;
using uLearn.Quizes;
using Ulearn.Common.Extensions;

namespace uLearn
{
	public class CourseDiff
	{
		public Course OriginalCourse { get; private set; }
		public Course ChangedCourse { get; private set; }

		public List<Slide> RemovedSlides { get; set; }
		public List<Slide> InsertedSlides { get; set; }
		public List<SlideDiff> SlideDiffs { get; set; }

		public CourseDiff(Course original, Course changed)
		{
			OriginalCourse = original;
			ChangedCourse = changed;

			RemovedSlides = new List<Slide>();
			InsertedSlides = new List<Slide>();
			SlideDiffs = new List<SlideDiff>();
			FindDifferences();
		}

		private void FindDifferences()
		{
			var originalSlidesIds = OriginalCourse.Slides.Select(s => s.Id).ToImmutableHashSet();
			var changedSlidesIds = ChangedCourse.Slides.Select(s => s.Id).ToImmutableHashSet();
			foreach (var slide in OriginalCourse.Slides)
			{
				if (!changedSlidesIds.Contains(slide.Id))
					RemovedSlides.Add(slide);
				else
				{
					var slideDiff = new SlideDiff(slide, ChangedCourse.GetSlideById(slide.Id));
					if (!slideDiff.IsEmptyChangeset)
						SlideDiffs.Add(slideDiff);
				}
			}

			InsertedSlides.AddRange(ChangedCourse.Slides.Where(slide => !originalSlidesIds.Contains(slide.Id)));
		}

		public bool IsTitleChanged => OriginalCourse.Title != ChangedCourse.Title;

		public bool IsEmptyChangeset => !IsTitleChanged && RemovedSlides.Count + InsertedSlides.Count + SlideDiffs.Count == 0;
	}

	public class SlideDiff
	{
		public Slide OriginalSlide { get; set; }
		public Slide ChangedSlide { get; set; }

		public List<SlideBlock> RemovedBlocks { get; set; }
		public List<SlideBlock> InsertedBlocks { get; set; }
		public List<SlideBlockDiff> SlideBlockDiffs { get; set; }

		public SlideDiff(Slide original, Slide changed)
		{
			OriginalSlide = original;
			ChangedSlide = changed;

			RemovedBlocks = new List<SlideBlock>();
			InsertedBlocks = new List<SlideBlock>();
			SlideBlockDiffs = new List<SlideBlockDiff>();
			FindDifferences();
		}

		private void FindDifferences()
		{
			FindDifferencesInNonquestionBlocks();
			FindDifferencesInQuestionBlocks();
		}

		private void FindDifferencesInNonquestionBlocks()
		{
			var originalBlocks = OriginalSlide.Blocks.NotOfType<SlideBlock, AbstractQuestionBlock>().ToList();
			var changedBlocks = ChangedSlide.Blocks.NotOfType<SlideBlock, AbstractQuestionBlock>().ToList();

			foreach (var pair in originalBlocks.Zip(changedBlocks, Tuple.Create))
			{
				var isBlocksEqual = pair.Item1.XmlSerialize(true) == pair.Item2.XmlSerialize(true);
				if (!isBlocksEqual)
				{
					var slideBlockDiff = new SlideBlockDiff(pair.Item1, pair.Item2);
					SlideBlockDiffs.Add(slideBlockDiff);
				}
			}
		}

		private void FindDifferencesInQuestionBlocks()
		{
			var originalBlocks = OriginalSlide.Blocks.OfType<AbstractQuestionBlock>().ToList();
			var changedBlocks = ChangedSlide.Blocks.OfType<AbstractQuestionBlock>().ToList();

			var originalBlocksIds = originalBlocks.Select(b => b.Id).ToImmutableHashSet();
			foreach (var block in originalBlocks)
			{
				var otherBlock = changedBlocks.FirstOrDefault(b => b.Id == block.Id);
				if (otherBlock == null)
					RemovedBlocks.Add(block);
				else if (block.XmlSerialize(true) != otherBlock.XmlSerialize(true))
				{
					var slideBlockDiff = new SlideBlockDiff(block, otherBlock);
					SlideBlockDiffs.Add(slideBlockDiff);
				}
			}

			InsertedBlocks.AddRange(changedBlocks.Where(b => !originalBlocksIds.Contains(b.Id)));
		}

		public bool IsTitleChanged => OriginalSlide.Title != ChangedSlide.Title;

		public bool IsAtLeastOneBlockChanged => RemovedBlocks.Count + InsertedBlocks.Count + SlideBlockDiffs.Count != 0;

		public bool IsEmptyChangeset => !IsTitleChanged && !IsAtLeastOneBlockChanged;
	}

	public class SlideBlockDiff
	{
		public SlideBlock OriginalSlideBlock { get; set; }
		public SlideBlock ChangedSlideBlock { get; set; }

		public SlideBlockDiff(SlideBlock original, SlideBlock changed)
		{
			OriginalSlideBlock = original;
			ChangedSlideBlock = changed;
		}
	}
}