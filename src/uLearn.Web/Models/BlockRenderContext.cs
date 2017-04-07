using System;
using System.Collections.Generic;
using uLearn.Model.Blocks;

namespace uLearn.Web.Models
{
	public class BlockRenderContext
	{
		public Course Course { get; private set; }
		public Slide Slide { get; private set; }
		public string BaseUrl { get; private set; }
		public dynamic[] BlockData { get; private set; }
		public bool IsGuest { get; set; }
		public bool IsLti { get; set; }
		public AbstractManualSlideChecking ManualChecking { get; set; }
		public bool CanUserFillQuiz { get; set; }
		public bool RevealHidden { get; private set; }
		public bool Autoplay { get; private set; }
		public bool IsManualCheckingReadonly { get; private set; }

		/* GroupsIds != null if instructor filtered users by group and see their works */
		public List<string> GroupsIds { get; set; }

		/* User's version of slide, i.e. for exercises */
		public int? VersionId { get; set; }

		public dynamic GetBlockData(SlideBlock block)
		{
			var index = Array.IndexOf(Slide.Blocks, block);
			if (index < 0)
				throw new ArgumentException("No block " + block + " in slide " + Slide);
			return BlockData[index];
		}

		public BlockRenderContext(Course course, Slide slide, string baseUrl, dynamic[] blockData, bool isGuest = false, bool revealHidden = false, AbstractManualSlideChecking manualChecking = null, bool canUserFillQuiz = false, List<string> groupsIds = null, bool isLti = false, bool autoplay = false, bool isManualCheckingReadonly = false)
		{
			if (blockData.Length != slide.Blocks.Length)
				throw new ArgumentException("BlockData.Length should be slide.Blocks.Length");
			Course = course;
			Slide = slide;
			BaseUrl = baseUrl;
			BlockData = blockData;
			IsGuest = isGuest;
			RevealHidden = revealHidden;
			ManualChecking = manualChecking;
			CanUserFillQuiz = canUserFillQuiz;
			GroupsIds = groupsIds;
			IsLti = isLti;
			Autoplay = autoplay;
			IsManualCheckingReadonly = isManualCheckingReadonly;
		}
	}
}